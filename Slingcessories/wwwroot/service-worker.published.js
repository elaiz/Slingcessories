// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/ ];
const offlineAssetsExclude = [ /^service-worker\.js$/ ];

// Replace with your base path if you are hosting on a subfolder. Ensure there is a trailing '/'.
const base = "/";
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));

    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    let cachedResponse = null;
    
    // For API requests, try network first, fall back to cache
    if (event.request.url.includes('/api/')) {
        try {
            // Try network first
            const response = await fetch(event.request);
            
            // Cache successful GET responses
            if (event.request.method === 'GET' && response.ok) {
                const cache = await caches.open(cacheName);
                cache.put(event.request, response.clone());
            }
            
            return response;
        } catch (error) {
            // Network failed, try cache
            cachedResponse = await caches.match(event.request);
            if (cachedResponse) {
                console.log('Service worker: Serving API from cache:', event.request.url);
                return cachedResponse;
            }
            
            // Return offline response for failed requests
            return new Response(JSON.stringify({ 
                error: 'offline', 
                message: 'You are offline and this data is not cached' 
            }), {
                status: 503,
                statusText: 'Service Unavailable',
                headers: new Headers({ 'Content-Type': 'application/json' })
            });
        }
    }

    // For static assets, use cache-first strategy
    if (event.request.method === 'GET') {
        // Try cache first
        cachedResponse = await caches.match(event.request);
        
        if (cachedResponse) {
            return cachedResponse;
        }
        
        // Not in cache, try network
        try {
            const response = await fetch(event.request);
            
            // Cache the response for future use
            if (response.ok) {
                const cache = await caches.open(cacheName);
                cache.put(event.request, response.clone());
            }
            
            return response;
        } catch (error) {
            // Network failed and not in cache
            console.error('Service worker: Fetch failed for:', event.request.url);
            
            // Return offline page for navigation requests
            if (event.request.mode === 'navigate') {
                cachedResponse = await caches.match('/');
                if (cachedResponse) {
                    return cachedResponse;
                }
            }
            
            throw error;
        }
    }
    
    // For non-GET requests, always use network
    return fetch(event.request);
}
