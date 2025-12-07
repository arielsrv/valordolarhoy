# ValorDolarHoy - ClientApp

This React application has been updated to use the latest versions compatible with Node 22.

## Changes Made

### Main Updates

1. **React 16 → React 18.3.1**
   - Migrated to the new `createRoot` API
   - Converted all class components to functional components with hooks
   - Removed `React.PureComponent` in favor of functional components

2. **React Router 5 → React Router 6.28.0**
   - Removed `connected-react-router` (incompatible with React Router v6)
   - Updated to direct `BrowserRouter`
   - Changed from `<Route component={...}>` to `<Route element={<.../>}>`
   - Removed `exact` prop (no longer needed in v6)

3. **Redux 4 → Redux 5.0.1**
   - Updated `redux-thunk` to v3.1.0
   - Updated `react-redux` to v9.1.2
   - Simplified store without router middleware

4. **react-scripts → Vite 6.0.1**
   - Migrated from Create React App to Vite for better performance
   - Much faster builds
   - Improved Hot Module Replacement (HMR)
   - No longer needs `--openssl-legacy-provider`

5. **Bootstrap 4 → Bootstrap 5.3.3**
   - Updated Reactstrap to v9.2.3
   - Migrated from `popper.js` to `@popperjs/core`

6. **TypeScript 3.9 → TypeScript 5.7.2**
   - Modern TypeScript configuration
   - Better support for ESM modules

## Updated File Structure

```
ClientApp/
├── index.html              # New: Main HTML (moved from public/)
├── vite.config.ts          # New: Vite configuration
├── tsconfig.json           # Updated: Modern TypeScript config
├── tsconfig.node.json      # New: Config for Vite
├── package.json            # Updated: All dependencies
├── .env                    # New: Environment variables
├── public/
│   ├── favicon.ico
│   └── manifest.json
└── src/
    ├── index.tsx           # Updated: uses createRoot
    ├── App.tsx             # Updated: React Router v6
    ├── components/
    │   ├── Layout.tsx      # Converted to functional component
    │   ├── nav-menu/
    │   │   └── NavMenu.tsx # Converted to functional component
    │   └── home/
    │       └── Home.tsx    # Converted to functional component with hooks
    └── store/
        ├── configureStore.ts # Updated: without router middleware
        └── index.ts
```

## Requirements

- **Node.js**: v22.x or higher
- **npm**: v10.x or higher

## Installation

```bash
cd ValorDolarHoy/ClientApp
npm install
```

## Available Scripts

### Development
```bash
npm start
# or
npm run dev
```
Starts the development server at http://localhost:3000

### Production
```bash
npm run build
```
Compiles the application for production in the `build/` folder

### Preview
```bash
npm run preview
```
Previews the production build locally

### Linting
```bash
npm run lint
```
Runs ESLint on all TypeScript/TSX files

## Vite Configuration

The `vite.config.ts` file is configured to:
- Use port 3000
- Proxy `/api` to `http://localhost:5000` (ASP.NET Core backend)
- Build output in `build/` folder (compatible with existing configuration)
- Sourcemaps enabled

## API Proxy

Calls to `/api/*` are automatically redirected to the backend at `http://localhost:5000`. 

For example:
```typescript
// In Home.tsx
const response = await httpClient.get<CurrencyDto>(`/api/Currency`);
// Redirects to: http://localhost:5000/api/Currency
```

## Environment Variables

`.env` file:
```
VITE_API_URL=http://localhost:5000
```

To use in code:
```typescript
const apiUrl = import.meta.env.VITE_API_URL;
```

## Updated Components

### index.tsx
- Uses React 18's `createRoot`
- Wraps with `<React.StrictMode>`
- Uses `<BrowserRouter>` directly

### App.tsx
- Uses `<Routes>` and `<Route>` from React Router v6
- Modern syntax with `element` prop

### Layout.tsx
- Converted to functional component
- Uses short Fragment `<>...</>`

### NavMenu.tsx
- Converted to functional component with `useState`
- Modern React syntax

### Home.tsx
- Converted to functional component with `useEffect` and `useState`
- Removed Redux dependency (not needed here)
- Better error handling

## Troubleshooting

### Error: "Cannot find module 'react-dom/client'"
Run: `npm install`

### TypeScript type errors
Run: `npm install --save-dev @types/react @types/react-dom`

### Port 3000 is busy
Change the port in `vite.config.ts`:
```typescript
server: {
  port: 3001, // Change here
  strictPort: true,
  // ...
}
```

### Backend not responding
Make sure the ASP.NET Core backend is running on port 5000.

## Backend Migration

If you need to update the backend configuration to serve this new build:

In `Program.cs` or `Startup.cs`, make sure the SPA configuration points to the `build/` folder:

```csharp
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";
    
    if (env.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
    }
});
```

## Key Differences from Previous Version

| Aspect | Before | Now |
|---------|-------|-------|
| Bundler | react-scripts (Webpack) | Vite |
| React | 16.11.0 | 18.3.1 |
| React Router | 5.1.2 | 6.28.0 |
| Redux | 4.0.4 | 5.0.1 |
| TypeScript | 3.9.10 | 5.7.2 |
| Bootstrap | 4.3.1 | 5.3.3 |
| Build time | ~30-60s | ~1-3s |
| Dev server start | ~10-20s | ~1-2s |

## Vite Benefits

1. **Instant startup**: Dev server starts in seconds
2. **Lightning-fast HMR**: Changes reflect instantly
3. **Optimized builds**: Uses Rollup for smaller production builds
4. **Native ESM**: Leverages browser's native ES modules
5. **No complex configuration**: Works out-of-the-box

## Recommended Next Steps

1. **Testing**: Configure Vitest for unit tests
   ```bash
   npm install -D vitest @testing-library/react @testing-library/jest-dom
   ```

2. **PWA**: Add Progressive Web App support
   ```bash
   npm install -D vite-plugin-pwa
   ```

3. **Strict TypeScript**: Enable more options in tsconfig.json
   ```json
   "noUncheckedIndexedAccess": true,
   "noImplicitReturns": true
   ```

## Support

If you encounter issues, check:
1. Node.js version: `node --version` (must be v22+)
2. Browser console logs
3. Terminal logs where `npm start` is running
4. Backend status at http://localhost:5000

---

**Updated**: December 2024 for Node 22 and latest versions of all dependencies.

