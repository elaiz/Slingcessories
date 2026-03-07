# Slingcessories React Frontend

This is a React + TypeScript + Vite frontend for the Slingcessories application.

## Getting Started

1. Install dependencies:
```bash
npm install
```

2. Start the development server:
```bash
npm run dev
```

The app will be available at `http://localhost:5173`

## Features

- View all accessories
- Filter by wishlist status (All, Owned, Wishlist)
- Delete accessories
- Responsive grid layout
- TypeScript for type safety
- Vite for fast development and building

## API Configuration

The app is configured to proxy API requests to `http://localhost:5000`. Make sure your backend API is running on that port, or update the proxy configuration in `vite.config.ts`.

## Build for Production

```bash
npm run build
```

The production build will be in the `dist` folder.

## Project Structure

- `src/components/` - React components
- `src/services/` - API service layer
- `src/types.ts` - TypeScript type definitions
- `vite.config.ts` - Vite configuration
