import { useEffect, useState } from 'react';
import { Accessory } from '../types';
import { accessoriesApi } from '../services/api';
import './AccessoriesList.css';

interface Props {
  filterWishlist?: boolean;
}

export default function AccessoriesList({ filterWishlist }: Props) {
  const [accessories, setAccessories] = useState<Accessory[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadAccessories();
  }, [filterWishlist]);

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

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this accessory?')) {
      return;
    }

    try {
      await accessoriesApi.delete(id);
      await loadAccessories();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Failed to delete accessory');
    }
  };

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
      <h1 className="page-title">{pageTitle}</h1>

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
                    View Product
                  </a>
                )}
                <button onClick={() => handleDelete(accessory.id)} className="btn-delete">
                  Delete
                </button>
              </div>

              {accessory.wishlist && (
                <span className="wishlist-badge">Wishlist</span>
              )}
            </div>
          </div>
        ))}
      </div>

      {accessories.length === 0 && (
        <div className="status-message">No accessories found</div>
      )}
    </div>
  );
}
