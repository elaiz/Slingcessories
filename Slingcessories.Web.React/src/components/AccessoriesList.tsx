import { useEffect, useState } from 'react';
import { Accessory, Category, CreateAccessory, Subcategory } from '../types';
import { accessoriesApi, categoriesApi, slingshotsApi, subcategoriesApi } from '../services/api';
import './AccessoriesList.css';

interface Props {
  filterWishlist?: boolean;
}

interface AccessoryForm {
  title: string;
  pictureUrl: string;
  price: string;
  url: string;
  wishlist: boolean;
  categoryId: number | '';
  subcategoryId: number | '';
}

const emptyForm = (): AccessoryForm => ({
  title: '', pictureUrl: '', price: '', url: '',
  wishlist: false, categoryId: '', subcategoryId: '',
});

export default function AccessoriesList({ filterWishlist }: Props) {
  const [accessories, setAccessories] = useState<Accessory[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // categories/subcategories/slingshots for the form
  const [categories, setCategories] = useState<Category[]>([]);
  const [subcategories, setSubcategories] = useState<Subcategory[]>([]);
  const [, setSlingshots] = useState<unknown[]>([]);

  // create/edit modal
  const [showForm, setShowForm] = useState(false);
  const [editingAccessory, setEditingAccessory] = useState<Accessory | null>(null);
  const [form, setForm] = useState<AccessoryForm>(emptyForm());
  const [formError, setFormError] = useState<string | null>(null);
  const [formBusy, setFormBusy] = useState(false);

  // view mode
  const [viewMode, setViewMode] = useState<'cards' | 'grid'>('cards');

  // delete confirm
  const [confirmDeleteId, setConfirmDeleteId] = useState<number | null>(null);

  const userId = localStorage.getItem('userId') ?? undefined;

  useEffect(() => {
    loadAccessories();
  }, [filterWishlist]);

  // Load categories etc. lazily when the form is first opened
  const ensureFormData = async () => {
    if (categories.length === 0) {
      const [cats, subs, ss] = await Promise.all([
        categoriesApi.getAll(),
        subcategoriesApi.getAll(),
        slingshotsApi.getAll(userId),
      ]);
      setCategories(cats);
      setSubcategories(subs);
      setSlingshots(ss);
    }
  };

  const loadAccessories = async () => {
    try {
      setLoading(true);
      const data = await accessoriesApi.getAll(filterWishlist);
      setAccessories(data);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const openCreate = async () => {
    await ensureFormData();
    setEditingAccessory(null);
    setForm({ ...emptyForm(), wishlist: filterWishlist ?? false });
    setFormError(null);
    setShowForm(true);
  };

  const openEdit = async (acc: Accessory) => {
    await ensureFormData();
    setEditingAccessory(acc);
    setForm({
      title: acc.title,
      pictureUrl: acc.pictureUrl ?? '',
      price: acc.price?.toString() ?? '',
      url: acc.url ?? '',
      wishlist: acc.wishlist,
      categoryId: acc.categoryId,
      subcategoryId: acc.subcategoryId ?? '',
    });
    setFormError(null);
    setShowForm(true);
  };

  const submitForm = async () => {
    if (!form.title.trim()) { setFormError('Title is required'); return; }
    if (!form.categoryId) { setFormError('Category is required'); return; }
    setFormBusy(true); setFormError(null);
    try {
      const payload: CreateAccessory = {
        title: form.title.trim(),
        pictureUrl: form.pictureUrl.trim() || null,
        price: parseFloat(form.price) || 0,
        url: form.url.trim() || null,
        wishlist: form.wishlist,
        categoryId: form.categoryId as number,
        subcategoryId: form.subcategoryId !== '' ? form.subcategoryId as number : null,
        slingshotQuantities: editingAccessory?.slingshotQuantities ?? {},
      };
      if (editingAccessory) {
        await accessoriesApi.update(editingAccessory.id, { ...editingAccessory, ...payload });
      } else {
        await accessoriesApi.create(payload);
      }
      await loadAccessories();
      setShowForm(false);
    } catch (err) {
      setFormError(err instanceof Error ? err.message : 'Save failed');
    } finally {
      setFormBusy(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await accessoriesApi.delete(id);
      setAccessories(prev => prev.filter(a => a.id !== id));
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete accessory');
    } finally {
      setConfirmDeleteId(null);
    }
  };

  const filteredSubs = subcategories.filter(s => s.categoryId === (form.categoryId as number));

  if (loading) {
    return <div className="status-message">Loading accessories...</div>;
  }

  if (error) {
    return <div className="status-message status-error">Error: {error}</div>;
  }

  const pageTitle =
    filterWishlist === true ? 'Wishlist' :
    filterWishlist === false ? 'Accessories' :
    'Slingshot Accessories';

  return (
    <div className="accessories-list">
      <div className="accessories-header">
        <h1 className="page-title">{pageTitle}</h1>
        <div className="header-actions">
          <button className="btn-primary btn-sm" onClick={openCreate}>+ New</button>
          <div className="view-toggle">
            <button
              className={`view-toggle-btn${viewMode === 'cards' ? ' active' : ''}`}
              onClick={() => setViewMode('cards')}
              title="Cards view"
            >
              ⊞ Cards
            </button>
            <button
              className={`view-toggle-btn${viewMode === 'grid' ? ' active' : ''}`}
              onClick={() => setViewMode('grid')}
              title="Grid view"
            >
              ☰ Grid
            </button>
          </div>
        </div>
      </div>

      {accessories.length === 0 && (
        <div className="status-message">No accessories found</div>
      )}

      {viewMode === 'cards' ? (
        <div className="accessories-grid">
          {accessories.map((accessory) => (
            <div key={accessory.id} className="accessory-card">
              {accessory.pictureUrl && (
                <img src={accessory.pictureUrl} alt={accessory.title} />
              )}
              <div className="accessory-content">
                <h3>{accessory.title}</h3>
                <p className="category">
                  {accessory.categoryName}
                  {accessory.subcategoryName && ` › ${accessory.subcategoryName}`}
                </p>

                {accessory.price != null && (
                  <p className="price">${accessory.price.toFixed(2)}</p>
                )}

                {accessory.slingshotDescriptions && accessory.slingshotDescriptions.length > 0 && (
                  <div className="slingshots">
                    <strong>For:</strong>
                    <ul>
                      {accessory.slingshotDescriptions.map((desc, idx) => {
                        const slingshotId = accessory.slingshotIds?.[idx];
                        const quantity = slingshotId ? accessory.slingshotQuantities[slingshotId] : undefined;
                        return (
                          <li key={idx}>
                            {desc}
                            {quantity && ` (Qty: ${quantity})`}
                          </li>
                        );
                      })}
                    </ul>
                  </div>
                )}

                <div className="actions">
                  {accessory.url && (
                    <a href={accessory.url} target="_blank" rel="noopener noreferrer" className="btn-view">
                      Open Link
                    </a>
                  )}
                  <button onClick={() => openEdit(accessory)} className="btn-edit">Edit</button>
                  {confirmDeleteId === accessory.id ? (
                    <>
                      <button onClick={() => handleDelete(accessory.id)} className="btn-delete-confirm">Confirm</button>
                      <button onClick={() => setConfirmDeleteId(null)} className="btn-cancel">Cancel</button>
                    </>
                  ) : (
                    <button onClick={() => setConfirmDeleteId(accessory.id)} className="btn-delete">Delete</button>
                  )}
                </div>

                {accessory.wishlist && (
                  <span className="wishlist-badge">Wishlist</span>
                )}
              </div>
            </div>
          ))}
        </div>
      ) : (
        <table className="accessories-table">
          <thead>
            <tr>
              <th>Title</th>
              <th>Category</th>
              <th>Subcategory</th>
              <th>Price</th>
              <th>Slingshots</th>
              <th>Wishlist</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {accessories.map((accessory) => (
              <tr key={accessory.id}>
                <td>
                  {accessory.pictureUrl
                    ? <a href={accessory.url ?? '#'} target="_blank" rel="noopener noreferrer">{accessory.title}</a>
                    : accessory.title}
                </td>
                <td>{accessory.categoryName}</td>
                <td>{accessory.subcategoryName ?? '—'}</td>
                <td>{accessory.price != null ? `$${accessory.price.toFixed(2)}` : '—'}</td>
                <td>{accessory.slingshotDescriptions?.join(', ') || '—'}</td>
                <td>{accessory.wishlist ? '★' : ''}</td>
                <td className="actions">
                  {accessory.url && (
                    <a href={accessory.url} target="_blank" rel="noopener noreferrer" className="btn-view">Link</a>
                  )}
                  <button onClick={() => openEdit(accessory)} className="btn-edit">Edit</button>
                  {confirmDeleteId === accessory.id ? (
                    <>
                      <button onClick={() => handleDelete(accessory.id)} className="btn-delete-confirm">Confirm</button>
                      <button onClick={() => setConfirmDeleteId(null)} className="btn-cancel">Cancel</button>
                    </>
                  ) : (
                    <button onClick={() => setConfirmDeleteId(accessory.id)} className="btn-delete">Delete</button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {/* Create / Edit Modal */}
      {showForm && (
        <>
          <div className="modal-backdrop" onClick={() => !formBusy && setShowForm(false)}></div>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5>{editingAccessory ? 'Edit Accessory' : 'New Accessory'}</h5>
                <button className="modal-close" onClick={() => setShowForm(false)} disabled={formBusy}>×</button>
              </div>
              <div className="modal-body">
                {formError && <div className="form-alert">{formError}</div>}
                <div className="form-group">
                  <label>Title *</label>
                  <input type="text" className="form-input" value={form.title} onChange={e => setForm(f => ({ ...f, title: e.target.value }))} />
                </div>
                <div className="form-row">
                  <div className="form-group">
                    <label>Category *</label>
                    <select className="form-input" value={form.categoryId} onChange={e => setForm(f => ({ ...f, categoryId: e.target.value ? +e.target.value : '', subcategoryId: '' }))}>
                      <option value="">Select…</option>
                      {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                    </select>
                  </div>
                  <div className="form-group">
                    <label>Subcategory</label>
                    <select className="form-input" value={form.subcategoryId} onChange={e => setForm(f => ({ ...f, subcategoryId: e.target.value ? +e.target.value : '' }))} disabled={!form.categoryId}>
                      <option value="">None</option>
                      {filteredSubs.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
                    </select>
                  </div>
                </div>
                <div className="form-row">
                  <div className="form-group">
                    <label>Price</label>
                    <input type="number" step="0.01" min="0" className="form-input" value={form.price} onChange={e => setForm(f => ({ ...f, price: e.target.value }))} />
                  </div>
                  <div className="form-group form-check-group">
                    <label>
                      <input type="checkbox" checked={form.wishlist} onChange={e => setForm(f => ({ ...f, wishlist: e.target.checked }))} />
                      {' '}Wishlist
                    </label>
                  </div>
                </div>
                <div className="form-group">
                  <label>Product URL</label>
                  <input type="url" className="form-input" value={form.url} onChange={e => setForm(f => ({ ...f, url: e.target.value }))} />
                </div>
                <div className="form-group">
                  <label>Picture URL</label>
                  <input type="url" className="form-input" value={form.pictureUrl} onChange={e => setForm(f => ({ ...f, pictureUrl: e.target.value }))} />
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn-primary btn-sm" onClick={submitForm} disabled={formBusy}>{formBusy ? 'Saving…' : 'Save'}</button>
                <button className="btn-outline btn-sm" onClick={() => setShowForm(false)} disabled={formBusy}>Cancel</button>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
