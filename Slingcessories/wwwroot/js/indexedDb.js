(function(){
  const DB_NAME = 'slingcessories';
  const DB_VERSION = 2; // Increment version for new features
  const STORE = 'kv';
  const PENDING_CHANGES_STORE = 'pendingChanges';
  let dbPromise = null;
  let dotNetHelper = null;

  function openDb(){
    if (dbPromise) return dbPromise;
    dbPromise = new Promise((resolve, reject) => {
      const req = indexedDB.open(DB_NAME, DB_VERSION);
      req.onupgradeneeded = () => {
        const db = req.result;
        if (!db.objectStoreNames.contains(STORE)){
          db.createObjectStore(STORE, { keyPath: 'key' });
        }
        if (!db.objectStoreNames.contains(PENDING_CHANGES_STORE)){
          db.createObjectStore(PENDING_CHANGES_STORE, { keyPath: 'id', autoIncrement: true });
        }
      };
      req.onsuccess = () => resolve(req.result);
      req.onerror = () => reject(req.error);
    });
    return dbPromise;
  }

  async function setItem(key, value){
    const db = await openDb();
    return new Promise((resolve, reject) => {
      const tx = db.transaction(STORE, 'readwrite');
      const store = tx.objectStore(STORE);
      const req = store.put({ key, value, timestamp: Date.now() });
      req.onsuccess = () => resolve(true);
      req.onerror = () => reject(req.error);
    });
  }

  async function getItem(key){
    const db = await openDb();
    return new Promise((resolve, reject) => {
      const tx = db.transaction(STORE, 'readonly');
      const store = tx.objectStore(STORE);
      const req = store.get(key);
      req.onsuccess = () => resolve(req.result ? req.result.value : null);
      req.onerror = () => reject(req.error);
    });
  }

  async function removeItem(key){
    const db = await openDb();
    return new Promise((resolve, reject) => {
      const tx = db.transaction(STORE, 'readwrite');
      const store = tx.objectStore(STORE);
      const req = store.delete(key);
      req.onsuccess = () => resolve(true);
      req.onerror = () => reject(req.error);
    });
  }

  async function clear(){
    const db = await openDb();
    return new Promise((resolve, reject) => {
      const tx = db.transaction(STORE, 'readwrite');
      const store = tx.objectStore(STORE);
      const req = store.clear();
      req.onsuccess = () => resolve(true);
      req.onerror = () => reject(req.error);
    });
  }

  async function getAllKeys(){
    const db = await openDb();
    return new Promise((resolve, reject) => {
      const tx = db.transaction(STORE, 'readonly');
      const store = tx.objectStore(STORE);
      const req = store.getAllKeys();
      req.onsuccess = () => resolve(req.result);
      req.onerror = () => reject(req.error);
    });
  }

  // Pending changes for offline sync
  async function addPendingChange(changeType, entityType, entityId, data) {
    const db = await openDb();
    return new Promise((resolve, reject) => {
      const tx = db.transaction(PENDING_CHANGES_STORE, 'readwrite');
      const store = tx.objectStore(PENDING_CHANGES_STORE);
      const req = store.add({
        changeType, // 'create', 'update', 'delete'
        entityType, // 'accessory', 'slingshot', 'category', etc.
        entityId,
        data,
        timestamp: Date.now()
      });
      req.onsuccess = () => resolve(req.result);
      req.onerror = () => reject(req.error);
    });
  }

  async function getPendingChanges() {
    const db = await openDb();
    return new Promise((resolve, reject) => {
      const tx = db.transaction(PENDING_CHANGES_STORE, 'readonly');
      const store = tx.objectStore(PENDING_CHANGES_STORE);
      const req = store.getAll();
      req.onsuccess = () => resolve(req.result);
      req.onerror = () => reject(req.error);
    });
  }

  async function clearPendingChanges() {
    const db = await openDb();
    return new Promise((resolve, reject) => {
      const tx = db.transaction(PENDING_CHANGES_STORE, 'readwrite');
      const store = tx.objectStore(PENDING_CHANGES_STORE);
      const req = store.clear();
      req.onsuccess = () => resolve(true);
      req.onerror = () => reject(req.error);
    });
  }

  // Network status detection with proper .NET callback handling
  function setupNetworkListeners() {
    window.addEventListener('online', () => {
      console.log('Network status: Online');
      if (dotNetHelper) {
        dotNetHelper.invokeMethodAsync('HandleOnline');
      }
    });
    
    window.addEventListener('offline', () => {
      console.log('Network status: Offline');
      if (dotNetHelper) {
        dotNetHelper.invokeMethodAsync('HandleOffline');
      }
    });
  }

  // Initialize network listeners when page loads
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupNetworkListeners);
  } else {
    setupNetworkListeners();
  }

  window.indexedDbHelper = {
    init: () => openDb().then(() => true),
    set: setItem,
    get: getItem,
    remove: removeItem,
    clear: clear,
    getAllKeys: getAllKeys,
    isOnline: () => navigator.onLine,
    
    // Pending changes for offline sync
    addPendingChange: addPendingChange,
    getPendingChanges: getPendingChanges,
    clearPendingChanges: clearPendingChanges,
    
    // Store the .NET helper reference for callbacks
    setDotNetHelper: (helper) => {
      dotNetHelper = helper;
    }
  };
})();
