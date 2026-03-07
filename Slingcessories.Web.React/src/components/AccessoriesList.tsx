import { useEffect, useState } from 'react';
import { Accessory } from '../types';
import { accessoriesApi } from '../services/api';
import './AccessoriesList.css';

export default function AccessoriesList() {
  const [accessories, setAccessories] = useState<Accessory[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filterWishlist, setFilterWishlist] = useState<boolean | undefined>(undefined);

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
    return <div className="loading">Loading accessories...</div>;
  }

  if (error) {
    return <div className="error">Error: {error}</div>;
  }

  return (
    <div className="accessories-list">
      <div className="header">
        <h1>Slingshot Accessories</h1>
        <div className="filters">
          <button 
            className={filterWishlist === undefined ? 'active' : ''}
            onClick={() => setFilterWishlist(undefined)}
          >
            All
          </button>
          <button 
            className={filterWishlist === false ? 'active' : ''}
            onClick={() => setFilterWishlist(false)}
          >
            Owned
          </button>
          <button 
            className={filterWishlist === true ? 'active' : ''}
            onClick={() => setFilterWishlist(true)}
          >
            Wishlist
          </button>
        </div>
      </div>

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
                {accessory.subcategoryName && ` > ${accessory.subcategoryName}`}
              </p>
              
              {accessory.price && (
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
                  <a href={accessory.url} target="_blank" rel="noopener noreferrer">
                    View Product
                  </a>
                )}
                <button onClick={() => handleDelete(accessory.id)} className="delete-btn">
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
        <div className="no-results">No accessories found</div>
      )}
    </div>
  );
}
