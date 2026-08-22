import { useEffect, useState } from 'react';
import { Category, Subcategory } from '../types';
import { categoriesApi, subcategoriesApi } from '../services/api';
import './CategoriesPage.css';

interface Props {
  onBack: () => void;
}

export default function CategoriesPage({ onBack }: Props) {
  const [categories, setCategories] = useState<Category[]>([]);
  const [subcategories, setSubcategories] = useState<Subcategory[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [expanded, setExpanded] = useState<Set<number>>(new Set());

  // Category inline edit
  const [editingCatId, setEditingCatId] = useState<number | null>(null);
  const [editCatName, setEditCatName] = useState('');
  const [confirmDeleteCatId, setConfirmDeleteCatId] = useState<number | null>(null);
  const [saving, setSaving] = useState(false);

  // Subcategory inline edit
  const [editingSubId, setEditingSubId] = useState<number | null>(null);
  const [editSubName, setEditSubName] = useState('');
  const [confirmDeleteSubId, setConfirmDeleteSubId] = useState<number | null>(null);

  // Create category modal
  const [showCreateCat, setShowCreateCat] = useState(false);
  const [newCatName, setNewCatName] = useState('');
  const [createCatError, setCreateCatError] = useState<string | null>(null);
  const [creatingCat, setCreatingCat] = useState(false);

  // Create subcategory modal
  const [createSubForCatId, setCreateSubForCatId] = useState<number | null>(null);
  const [newSubName, setNewSubName] = useState('');
  const [createSubError, setCreateSubError] = useState<string | null>(null);
  const [creatingSub, setCreatingSub] = useState(false);

  useEffect(() => { load(); }, []);

  const load = async () => {
    try {
      setLoading(true);
      setError(null);
      const [cats, subs] = await Promise.all([categoriesApi.getAll(), subcategoriesApi.getAll()]);
      setCategories(cats);
      setSubcategories(subs);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load');
    } finally {
      setLoading(false);
    }
  };

  const toggleExpand = (id: number) => {
    setExpanded(prev => { const n = new Set(prev); n.has(id) ? n.delete(id) : n.add(id); return n; });
  };

  const toggleExpandAll = () => {
    setExpanded(expanded.size === categories.length ? new Set() : new Set(categories.map(c => c.id)));
  };

  /* ── Category CRUD ── */
  const beginEditCat = (c: Category) => { setEditingCatId(c.id); setEditCatName(c.name); setConfirmDeleteCatId(null); };
  const cancelEditCat = () => setEditingCatId(null);

  const saveEditCat = async () => {
    if (!editingCatId || !editCatName.trim()) return;
    setSaving(true);
    try {
      await categoriesApi.update(editingCatId, editCatName.trim());
      setCategories(prev => prev.map(c => c.id === editingCatId ? { ...c, name: editCatName.trim() } : c));
      setEditingCatId(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Update failed');
    } finally { setSaving(false); }
  };

  const deleteCat = async (id: number) => {
    setSaving(true);
    try {
      await categoriesApi.delete(id);
      setCategories(prev => prev.filter(c => c.id !== id));
      setSubcategories(prev => prev.filter(s => s.categoryId !== id));
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Delete failed');
    } finally { setSaving(false); setConfirmDeleteCatId(null); }
  };

  const submitCreateCat = async () => {
    if (!newCatName.trim()) { setCreateCatError('Name is required'); return; }
    setCreatingCat(true); setCreateCatError(null);
    try {
      const cat = await categoriesApi.create(newCatName.trim());
      setCategories(prev => [...prev, cat].sort((a, b) => a.name.localeCompare(b.name)));
      setShowCreateCat(false);
    } catch (err) {
      setCreateCatError(err instanceof Error ? err.message : 'Create failed');
    } finally { setCreatingCat(false); }
  };

  /* ── Subcategory CRUD ── */
  const beginEditSub = (s: Subcategory) => { setEditingSubId(s.id); setEditSubName(s.name); setConfirmDeleteSubId(null); };
  const cancelEditSub = () => setEditingSubId(null);

  const saveEditSub = async () => {
    if (!editingSubId || !editSubName.trim()) return;
    setSaving(true);
    try {
      const sub = subcategories.find(s => s.id === editingSubId)!;
      await subcategoriesApi.update(editingSubId, editSubName.trim(), sub.categoryId);
      setSubcategories(prev => prev.map(s => s.id === editingSubId ? { ...s, name: editSubName.trim() } : s));
      setEditingSubId(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Update failed');
    } finally { setSaving(false); }
  };

  const deleteSub = async (id: number) => {
    setSaving(true);
    try {
      await subcategoriesApi.delete(id);
      setSubcategories(prev => prev.filter(s => s.id !== id));
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Delete failed');
    } finally { setSaving(false); setConfirmDeleteSubId(null); }
  };

  const submitCreateSub = async () => {
    if (!createSubForCatId || !newSubName.trim()) { setCreateSubError('Name is required'); return; }
    setCreatingSub(true); setCreateSubError(null);
    try {
      const sub = await subcategoriesApi.create(newSubName.trim(), createSubForCatId);
      setSubcategories(prev => [...prev, sub].sort((a, b) => a.name.localeCompare(b.name)));
      setCreateSubForCatId(null);
    } catch (err) {
      setCreateSubError(err instanceof Error ? err.message : 'Create failed');
    } finally { setCreatingSub(false); }
  };

  if (loading) return <div className="status-message">Loading categories...</div>;

  return (
    <div className="categories-page">
      <button className="btn-link back-link" onClick={onBack}>← Back to Settings</button>
      <h1 className="page-title">Categories &amp; Subcategories</h1>

      {error && <div className="alert alert-error">{error}</div>}

      <div className="toolbar">
        <button className="btn-primary btn-sm" onClick={() => { setNewCatName(''); setCreateCatError(null); setShowCreateCat(true); }}>+ New Category</button>
        <button className="btn-outline btn-sm" onClick={toggleExpandAll}>{expanded.size === categories.length ? 'Collapse All' : 'Expand All'}</button>
        <button className="btn-outline btn-sm" onClick={load}>Refresh</button>
      </div>

      {categories.length === 0 ? (
        <p className="status-message">No categories found.</p>
      ) : (
        <ul className="cat-list">
          {categories.map(c => {
            const subs = subcategories.filter(s => s.categoryId === c.id);
            const isExp = expanded.has(c.id);
            return (
              <li key={c.id} className="cat-item">
                {editingCatId === c.id ? (
                  <div className="edit-row">
                    <input type="text" className="input-sm" value={editCatName} onChange={e => setEditCatName(e.target.value)} />
                    <button className="btn-primary btn-sm" onClick={saveEditCat} disabled={saving}>{saving ? 'Saving…' : 'Save'}</button>
                    <button className="btn-outline btn-sm" onClick={cancelEditCat} disabled={saving}>Cancel</button>
                  </div>
                ) : (
                  <>
                    <div className="cat-row">
                      <div className="cat-label">
                        <button className="expand-btn" onClick={() => toggleExpand(c.id)}>
                          {isExp ? '▾' : '▸'}
                        </button>
                        <strong>{c.name}</strong>
                        <span className="badge">{subs.length}</span>
                      </div>
                      <div className="btn-group">
                        <button className="btn-outline btn-sm" onClick={() => { setCreateSubForCatId(c.id); setNewSubName(''); setCreateSubError(null); setExpanded(prev => new Set([...prev, c.id])); }}>+ Add Subcategory</button>
                        <button className="btn-outline btn-sm" onClick={() => beginEditCat(c)}>Edit</button>
                        {confirmDeleteCatId === c.id ? (
                          <>
                            <button className="btn-danger btn-sm" onClick={() => deleteCat(c.id)} disabled={saving}>Confirm</button>
                            <button className="btn-outline btn-sm" onClick={() => setConfirmDeleteCatId(null)} disabled={saving}>Cancel</button>
                          </>
                        ) : (
                          <button className="btn-outline-danger btn-sm" onClick={() => setConfirmDeleteCatId(c.id)}>Delete</button>
                        )}
                      </div>
                    </div>

                    {isExp && (
                      <ul className="sub-list">
                        {subs.length === 0 ? (
                          <li className="text-muted small" style={{ padding: '0.4rem 0.5rem' }}>No subcategories yet.</li>
                        ) : subs.map(s => (
                          <li key={s.id} className="sub-item">
                            {editingSubId === s.id ? (
                              <div className="edit-row">
                                <input type="text" className="input-sm" value={editSubName} onChange={e => setEditSubName(e.target.value)} />
                                <button className="btn-primary btn-sm" onClick={saveEditSub} disabled={saving}>{saving ? 'Saving…' : 'Save'}</button>
                                <button className="btn-outline btn-sm" onClick={cancelEditSub} disabled={saving}>Cancel</button>
                              </div>
                            ) : (
                              <div className="sub-row">
                                <span>{s.name}</span>
                                <div className="btn-group">
                                  <button className="btn-outline btn-sm" onClick={() => beginEditSub(s)}>Edit</button>
                                  {confirmDeleteSubId === s.id ? (
                                    <>
                                      <button className="btn-danger btn-sm" onClick={() => deleteSub(s.id)} disabled={saving}>Confirm</button>
                                      <button className="btn-outline btn-sm" onClick={() => setConfirmDeleteSubId(null)} disabled={saving}>Cancel</button>
                                    </>
                                  ) : (
                                    <button className="btn-outline-danger btn-sm" onClick={() => setConfirmDeleteSubId(s.id)}>Delete</button>
                                  )}
                                </div>
                              </div>
                            )}
                          </li>
                        ))}
                      </ul>
                    )}
                  </>
                )}
              </li>
            );
          })}
        </ul>
      )}

      {/* Create Category Modal */}
      {showCreateCat && (
        <>
          <div className="modal-backdrop" onClick={() => !creatingCat && setShowCreateCat(false)}></div>
          <div className="modal-dialog modal-sm">
            <div className="modal-content">
              <div className="modal-header">
                <h5>New Category</h5>
                <button className="modal-close" onClick={() => setShowCreateCat(false)} disabled={creatingCat}>×</button>
              </div>
              <div className="modal-body">
                <div className="form-group">
                  <label>Name</label>
                  <input type="text" className="input-sm" value={newCatName} onChange={e => setNewCatName(e.target.value)} autoFocus />
                </div>
                {createCatError && <div className="alert alert-error">{createCatError}</div>}
              </div>
              <div className="modal-footer">
                <button className="btn-primary btn-sm" onClick={submitCreateCat} disabled={creatingCat}>{creatingCat ? 'Saving…' : 'OK'}</button>
                <button className="btn-outline btn-sm" onClick={() => setShowCreateCat(false)} disabled={creatingCat}>Cancel</button>
              </div>
            </div>
          </div>
        </>
      )}

      {/* Create Subcategory Modal */}
      {createSubForCatId !== null && (
        <>
          <div className="modal-backdrop" onClick={() => !creatingSub && setCreateSubForCatId(null)}></div>
          <div className="modal-dialog modal-sm">
            <div className="modal-content">
              <div className="modal-header">
                <h5>New Subcategory</h5>
                <button className="modal-close" onClick={() => setCreateSubForCatId(null)} disabled={creatingSub}>×</button>
              </div>
              <div className="modal-body">
                <div className="form-group">
                  <label>Category</label>
                  <input type="text" className="input-sm" value={categories.find(c => c.id === createSubForCatId)?.name ?? ''} disabled />
                </div>
                <div className="form-group">
                  <label>Name</label>
                  <input type="text" className="input-sm" value={newSubName} onChange={e => setNewSubName(e.target.value)} autoFocus />
                </div>
                {createSubError && <div className="alert alert-error">{createSubError}</div>}
              </div>
              <div className="modal-footer">
                <button className="btn-primary btn-sm" onClick={submitCreateSub} disabled={creatingSub}>{creatingSub ? 'Saving…' : 'OK'}</button>
                <button className="btn-outline btn-sm" onClick={() => setCreateSubForCatId(null)} disabled={creatingSub}>Cancel</button>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
