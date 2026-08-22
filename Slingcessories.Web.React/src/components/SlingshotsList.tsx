import { useEffect, useState } from 'react';
import { Accessory, Slingshot } from '../types';
import { accessoriesApi, slingshotsApi } from '../services/api';
import './SlingshotsList.css';

export default function SlingshotsList() {
  const [slingshots, setSlingshots] = useState<Slingshot[]>([]);
  const [accessories, setAccessories] = useState<Accessory[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // expand/collapse accessory panels per slingshot
  const [expanded, setExpanded] = useState<Set<number>>(new Set());

  // inline edit
  const [editingId, setEditingId] = useState<number | null>(null);
  const [editYear, setEditYear] = useState(0);
  const [editModel, setEditModel] = useState('');
  const [editColor, setEditColor] = useState('');
  const [saving, setSaving] = useState(false);

  // delete confirm
  const [confirmDeleteId, setConfirmDeleteId] = useState<number | null>(null);

  // create modal
  const [showCreate, setShowCreate] = useState(false);
  const [createYear, setCreateYear] = useState(new Date().getFullYear());
  const [createModel, setCreateModel] = useState('');
  const [createColor, setCreateColor] = useState('');
  const [createError, setCreateError] = useState<string | null>(null);
  const [creating, setCreating] = useState(false);

  // add-accessories modal
  const [addForSlingshotId, setAddForSlingshotId] = useState<number | null>(null);
  const [selectedAccessoryIds, setSelectedAccessoryIds] = useState<Map<number, number>>(new Map());
  const [addingAccessories, setAddingAccessories] = useState(false);
  const [addError, setAddError] = useState<string | null>(null);

  // edit-quantity modal
  const [editQty, setEditQty] = useState<{ slingshotId: number; accessoryId: number; qty: number } | null>(null);
  const [editQtyValue, setEditQtyValue] = useState(1);
  const [updatingQty, setUpdatingQty] = useState(false);

  // remove-accessory confirm
  const [confirmRemove, setConfirmRemove] = useState<{ slingshotId: number; accessoryId: number } | null>(null);

  const userId = localStorage.getItem('userId') ?? undefined;

  useEffect(() => {
    load();
  }, []);

  const load = async () => {
    try {
      setLoading(true);
      setError(null);
      const [ss, accs] = await Promise.all([
        slingshotsApi.getAll(userId),
        accessoriesApi.getAll(),
      ]);
      setSlingshots(ss);
      setAccessories(accs);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load');
    } finally {
      setLoading(false);
    }
  };

  const toggleExpand = (id: number) => {
    setExpanded(prev => {
      const next = new Set(prev);
      next.has(id) ? next.delete(id) : next.add(id);
      return next;
    });
  };

  const toggleExpandAll = () => {
    if (expanded.size === slingshots.length) {
      setExpanded(new Set());
    } else {
      setExpanded(new Set(slingshots.map(s => s.id)));
    }
  };

  /* ── Edit ── */
  const beginEdit = (s: Slingshot) => {
    setEditingId(s.id);
    setEditYear(s.year);
    setEditModel(s.model);
    setEditColor(s.color);
    setConfirmDeleteId(null);
  };

  const cancelEdit = () => setEditingId(null);

  const saveEdit = async () => {
    if (!editingId) return;
    setSaving(true);
    try {
      const updated: Slingshot = { id: editingId, year: editYear, model: editModel.trim(), color: editColor.trim() };
      await slingshotsApi.update(editingId, updated);
      setSlingshots(prev => prev.map(s => s.id === editingId ? updated : s));
      setEditingId(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Update failed');
    } finally {
      setSaving(false);
    }
  };

  /* ── Delete ── */
  const deleteSlingshot = async (id: number) => {
    setSaving(true);
    try {
      await slingshotsApi.delete(id);
      setSlingshots(prev => prev.filter(s => s.id !== id));
      setExpanded(prev => { const n = new Set(prev); n.delete(id); return n; });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Delete failed');
    } finally {
      setSaving(false);
      setConfirmDeleteId(null);
    }
  };

  /* ── Create ── */
  const openCreate = () => {
    setCreateYear(new Date().getFullYear());
    setCreateModel('');
    setCreateColor('');
    setCreateError(null);
    setShowCreate(true);
  };

  const submitCreate = async () => {
    if (!createModel.trim() || !createColor.trim()) { setCreateError('Model and Color are required.'); return; }
    setCreating(true);
    setCreateError(null);
    try {
      const created = await slingshotsApi.create({ year: createYear, model: createModel.trim(), color: createColor.trim(), userId: userId ?? '' });
      setSlingshots(prev => [...prev, created].sort((a, b) => a.year - b.year || a.model.localeCompare(b.model)));
      setShowCreate(false);
    } catch (err) {
      setCreateError(err instanceof Error ? err.message : 'Create failed');
    } finally {
      setCreating(false);
    }
  };

  /* ── Add accessories ── */
  const openAddAccessories = (slingshotId: number) => {
    setAddForSlingshotId(slingshotId);
    setSelectedAccessoryIds(new Map());
    setAddError(null);
  };

  const toggleAccessorySelect = (accId: number, checked: boolean) => {
    setSelectedAccessoryIds(prev => {
      const n = new Map(prev);
      if (checked) n.set(accId, 1); else n.delete(accId);
      return n;
    });
  };

  const setQtyForSelected = (accId: number, val: string) => {
    const qty = Math.max(1, parseInt(val) || 1);
    setSelectedAccessoryIds(prev => { const n = new Map(prev); n.set(accId, qty); return n; });
  };

  const submitAddAccessories = async () => {
    if (!addForSlingshotId || selectedAccessoryIds.size === 0) return;
    setAddingAccessories(true);
    setAddError(null);
    try {
      for (const [accId, qty] of selectedAccessoryIds.entries()) {
        const acc = accessories.find(a => a.id === accId);
        if (!acc) continue;
        const updated = {
          ...acc,
          slingshotIds: [...(acc.slingshotIds ?? []), addForSlingshotId],
          slingshotQuantities: { ...acc.slingshotQuantities, [addForSlingshotId]: qty },
        };
        await accessoriesApi.update(accId, updated);
      }
      const accs = await accessoriesApi.getAll();
      setAccessories(accs);
      setAddForSlingshotId(null);
    } catch (err) {
      setAddError(err instanceof Error ? err.message : 'Failed to add accessories');
    } finally {
      setAddingAccessories(false);
    }
  };

  /* ── Edit quantity ── */
  const openEditQty = (slingshotId: number, accessoryId: number, currentQty: number) => {
    setEditQty({ slingshotId, accessoryId, qty: currentQty });
    setEditQtyValue(currentQty);
  };

  const submitEditQty = async () => {
    if (!editQty) return;
    setUpdatingQty(true);
    try {
      const acc = accessories.find(a => a.id === editQty.accessoryId);
      if (acc) {
        const updated = { ...acc, slingshotQuantities: { ...acc.slingshotQuantities, [editQty.slingshotId]: editQtyValue } };
        await accessoriesApi.update(acc.id, updated);
        setAccessories(prev => prev.map(a => a.id === acc.id ? updated : a));
      }
      setEditQty(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update quantity');
    } finally {
      setUpdatingQty(false);
    }
  };

  /* ── Remove accessory from slingshot ── */
  const removeAccessory = async (slingshotId: number, accessoryId: number) => {
    try {
      const acc = accessories.find(a => a.id === accessoryId);
      if (!acc) return;
      const newIds = (acc.slingshotIds ?? []).filter(id => id !== slingshotId);
      const newQtys = { ...acc.slingshotQuantities };
      delete newQtys[slingshotId];
      const updated = { ...acc, slingshotIds: newIds, slingshotQuantities: newQtys };
      await accessoriesApi.update(accessoryId, updated);
      setAccessories(prev => prev.map(a => a.id === accessoryId ? updated : a));
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to remove accessory');
    } finally {
      setConfirmRemove(null);
    }
  };

  if (loading) return <div className="status-message">Loading slingshots...</div>;

  return (
    <div className="slingshots-page">
      <h1 className="page-title">Slingshots</h1>

      {error && <div className="alert alert-error">{error}</div>}

      <div className="toolbar">
        <button className="btn-primary btn-sm" onClick={openCreate}>+ New Slingshot</button>
        <button className="btn-outline btn-sm" onClick={toggleExpandAll}>
          {expanded.size === slingshots.length ? 'Collapse All' : 'Expand All'}
        </button>
        <button className="btn-outline btn-sm" onClick={load}>Refresh</button>
      </div>

      {slingshots.length === 0 ? (
        <p className="status-message">No slingshots found.</p>
      ) : (
        <ul className="slingshot-list">
          {slingshots.map(s => {
            const slingshotAccs = accessories.filter(a => a.slingshotIds?.includes(s.id));
            const ownedTotal = slingshotAccs.filter(a => !a.wishlist).reduce((sum, a) => sum + a.price * (a.slingshotQuantities[s.id] ?? 0), 0);
            const wishTotal = slingshotAccs.filter(a => a.wishlist).reduce((sum, a) => sum + a.price * (a.slingshotQuantities[s.id] ?? 0), 0);
            const grandTotal = ownedTotal + wishTotal;
            const isExpanded = expanded.has(s.id);

            return (
              <li key={s.id} className="slingshot-item">
                {editingId === s.id ? (
                  <div className="edit-row">
                    <input type="number" value={editYear} onChange={e => setEditYear(+e.target.value)} className="input-sm" style={{ width: 80 }} />
                    <input type="text" value={editModel} onChange={e => setEditModel(e.target.value)} className="input-sm" placeholder="Model" />
                    <input type="text" value={editColor} onChange={e => setEditColor(e.target.value)} className="input-sm" placeholder="Color" />
                    <button className="btn-primary btn-sm" onClick={saveEdit} disabled={saving}>{saving ? 'Saving…' : 'Save'}</button>
                    <button className="btn-outline btn-sm" onClick={cancelEdit} disabled={saving}>Cancel</button>
                  </div>
                ) : (
                  <>
                    <div className="slingshot-row">
                      <div className="slingshot-info">
                        <strong>{s.year} {s.model}</strong>
                        <span className="text-muted"> ({s.color})</span>
                        {slingshotAccs.length > 0 && (
                          <div className="price-totals">
                            <span className="total-owned">✔ ${ownedTotal.toFixed(2)}</span>
                            <span className="total-wish">⭐ ${wishTotal.toFixed(2)}</span>
                            <span className="total-grand">💰 ${grandTotal.toFixed(2)}</span>
                          </div>
                        )}
                      </div>
                      <div className="btn-group">
                        <button className="btn-outline btn-sm" onClick={() => toggleExpand(s.id)}>
                          {isExpanded ? 'Hide' : 'Show'} Accessories
                        </button>
                        <button className="btn-outline btn-sm" onClick={() => beginEdit(s)}>Edit</button>
                        {confirmDeleteId === s.id ? (
                          <>
                            <button className="btn-danger btn-sm" onClick={() => deleteSlingshot(s.id)} disabled={saving}>Confirm</button>
                            <button className="btn-outline btn-sm" onClick={() => setConfirmDeleteId(null)} disabled={saving}>Cancel</button>
                          </>
                        ) : (
                          <button className="btn-outline-danger btn-sm" onClick={() => setConfirmDeleteId(s.id)}>Delete</button>
                        )}
                      </div>
                    </div>

                    {isExpanded && (
                      <div className="accessories-panel">
                        <div className="accessories-panel-header">
                          <span className="text-muted">Associated Accessories</span>
                          <button className="btn-primary btn-sm" onClick={() => openAddAccessories(s.id)}>+ Add Accessories</button>
                        </div>
                        {slingshotAccs.length === 0 ? (
                          <p className="text-muted small">No accessories associated yet.</p>
                        ) : (
                          <ul className="accessory-rows">
                            {slingshotAccs.map(acc => {
                              const qty = acc.slingshotQuantities[s.id] ?? 0;
                              const lineTotal = acc.price * qty;
                              const isConfirmRemove = confirmRemove?.slingshotId === s.id && confirmRemove.accessoryId === acc.id;
                              return (
                                <li key={acc.id} className="accessory-row-item">
                                  <span className="accessory-row-label">
                                    🏷 {acc.title}
                                    <span className={`qty-badge${acc.wishlist ? ' wishlist' : ''}`}>Qty: {qty}</span>
                                    <br />
                                    <small className="text-muted">${acc.price.toFixed(2)} × {qty} = <strong>${lineTotal.toFixed(2)}</strong></small>
                                  </span>
                                  <div className="btn-group">
                                    <button className="btn-outline btn-sm" onClick={() => openEditQty(s.id, acc.id, qty)}>✏</button>
                                    {isConfirmRemove ? (
                                      <>
                                        <button className="btn-danger btn-sm" onClick={() => removeAccessory(s.id, acc.id)}>Confirm</button>
                                        <button className="btn-outline btn-sm" onClick={() => setConfirmRemove(null)}>Cancel</button>
                                      </>
                                    ) : (
                                      <button className="btn-outline-danger btn-sm" onClick={() => setConfirmRemove({ slingshotId: s.id, accessoryId: acc.id })}>🗑</button>
                                    )}
                                  </div>
                                </li>
                              );
                            })}
                          </ul>
                        )}
                      </div>
                    )}
                  </>
                )}
              </li>
            );
          })}
        </ul>
      )}

      {/* Create Modal */}
      {showCreate && (
        <>
          <div className="modal-backdrop" onClick={() => !creating && setShowCreate(false)}></div>
          <div className="modal-dialog modal-sm">
            <div className="modal-content">
              <div className="modal-header">
                <h5>New Slingshot</h5>
                <button className="modal-close" onClick={() => setShowCreate(false)} disabled={creating}>×</button>
              </div>
              <div className="modal-body">
                <div className="form-group">
                  <label>Year</label>
                  <input type="number" className="input-sm" value={createYear} onChange={e => setCreateYear(+e.target.value)} />
                </div>
                <div className="form-group">
                  <label>Model</label>
                  <input type="text" className="input-sm" value={createModel} onChange={e => setCreateModel(e.target.value)} />
                </div>
                <div className="form-group">
                  <label>Color</label>
                  <input type="text" className="input-sm" value={createColor} onChange={e => setCreateColor(e.target.value)} />
                </div>
                {createError && <div className="alert alert-error">{createError}</div>}
              </div>
              <div className="modal-footer">
                <button className="btn-primary btn-sm" onClick={submitCreate} disabled={creating}>{creating ? 'Saving…' : 'OK'}</button>
                <button className="btn-outline btn-sm" onClick={() => setShowCreate(false)} disabled={creating}>Cancel</button>
              </div>
            </div>
          </div>
        </>
      )}

      {/* Add Accessories Modal */}
      {addForSlingshotId !== null && (
        <>
          <div className="modal-backdrop"></div>
          <div className="modal-dialog modal-lg">
            <div className="modal-content">
              <div className="modal-header">
                <h5>Add Accessories to Slingshot</h5>
                <button className="modal-close" onClick={() => setAddForSlingshotId(null)} disabled={addingAccessories}>×</button>
              </div>
              <div className="modal-body">
                {addError && <div className="alert alert-error">{addError}</div>}
                <div className="accessory-select-list">
                  {accessories.filter(a => !(a.slingshotIds ?? []).includes(addForSlingshotId)).length === 0 ? (
                    <p className="text-muted small">All accessories are already assigned.</p>
                  ) : accessories.filter(a => !(a.slingshotIds ?? []).includes(addForSlingshotId)).map(acc => (
                    <div key={acc.id} className="accessory-select-row">
                      <div className="accessory-select-label">
                        <input
                          type="checkbox"
                          id={`add_${acc.id}`}
                          checked={selectedAccessoryIds.has(acc.id)}
                          onChange={e => toggleAccessorySelect(acc.id, e.target.checked)}
                        />
                        <label htmlFor={`add_${acc.id}`}>
                          {acc.wishlist && <span>⭐ </span>}
                          <strong>{acc.title}</strong>
                          <span className="text-muted small"> — {acc.categoryName}</span>
                        </label>
                      </div>
                      {selectedAccessoryIds.has(acc.id) && (
                        <input
                          type="number"
                          className="input-sm"
                          min={1}
                          value={selectedAccessoryIds.get(acc.id) ?? 1}
                          onChange={e => setQtyForSelected(acc.id, e.target.value)}
                          style={{ width: 70 }}
                        />
                      )}
                    </div>
                  ))}
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn-outline btn-sm" onClick={() => setAddForSlingshotId(null)} disabled={addingAccessories}>Cancel</button>
                <button className="btn-primary btn-sm" onClick={submitAddAccessories} disabled={addingAccessories || selectedAccessoryIds.size === 0}>
                  {addingAccessories ? 'Adding…' : 'Add Selected'}
                </button>
              </div>
            </div>
          </div>
        </>
      )}

      {/* Edit Quantity Modal */}
      {editQty !== null && (
        <>
          <div className="modal-backdrop"></div>
          <div className="modal-dialog modal-sm">
            <div className="modal-content">
              <div className="modal-header">
                <h5>Edit Quantity</h5>
                <button className="modal-close" onClick={() => setEditQty(null)} disabled={updatingQty}>×</button>
              </div>
              <div className="modal-body">
                <div className="form-group">
                  <label>Quantity</label>
                  <input type="number" className="input-sm" min={1} value={editQtyValue} onChange={e => setEditQtyValue(Math.max(1, +e.target.value))} />
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn-outline btn-sm" onClick={() => setEditQty(null)} disabled={updatingQty}>Cancel</button>
                <button className="btn-primary btn-sm" onClick={submitEditQty} disabled={updatingQty || editQtyValue < 1}>
                  {updatingQty ? 'Updating…' : 'Update'}
                </button>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
