# ValorDolarHoy - ClientApp

Esta aplicación React ha sido actualizada para usar las últimas versiones compatibles con Node 22.

## Cambios Realizados

### Actualizaciones Principales

1. **React 16 → React 18.3.1**
   - Migrado a la nueva API `createRoot`
   - Convertidos todos los componentes de clase a componentes funcionales con hooks
   - Eliminado `React.PureComponent` en favor de componentes funcionales

2. **React Router 5 → React Router 6.28.0**
   - Eliminado `connected-react-router` (incompatible con React Router v6)
   - Actualizado a `BrowserRouter` directo
   - Cambiado de `<Route component={...}>` a `<Route element={<.../>}>`
   - Eliminado prop `exact` (ya no es necesario en v6)

3. **Redux 4 → Redux 5.0.1**
   - Actualizado `redux-thunk` a v3.1.0
   - Actualizado `react-redux` a v9.1.2
   - Simplificado el store sin router middleware

4. **react-scripts → Vite 6.0.1**
   - Migrado de Create React App a Vite para mejor rendimiento
   - Build mucho más rápido
   - Hot Module Replacement (HMR) mejorado
   - Ya no se necesita `--openssl-legacy-provider`

5. **Bootstrap 4 → Bootstrap 5.3.3**
   - Actualizado Reactstrap a v9.2.3
   - Migrado de `popper.js` a `@popperjs/core`

6. **TypeScript 3.9 → TypeScript 5.7.2**
   - Configuración moderna de TypeScript
   - Mejor soporte para módulos ESM

## Estructura de Archivos Actualizada

```
ClientApp/
├── index.html              # Nuevo: HTML principal (movido de public/)
├── vite.config.ts          # Nuevo: Configuración de Vite
├── tsconfig.json           # Actualizado: TypeScript config moderna
├── tsconfig.node.json      # Nuevo: Config para Vite
├── package.json            # Actualizado: Todas las dependencias
├── .env                    # Nuevo: Variables de entorno
├── public/
│   ├── favicon.ico
│   └── manifest.json
└── src/
    ├── index.tsx           # Actualizado: usa createRoot
    ├── App.tsx             # Actualizado: React Router v6
    ├── components/
    │   ├── Layout.tsx      # Convertido a functional component
    │   ├── nav-menu/
    │   │   └── NavMenu.tsx # Convertido a functional component
    │   └── home/
    │       └── Home.tsx    # Convertido a functional component con hooks
    └── store/
        ├── configureStore.ts # Actualizado: sin router middleware
        └── index.ts
```

## Requisitos

- **Node.js**: v22.x o superior
- **npm**: v10.x o superior

## Instalación

```bash
cd ValorDolarHoy/ClientApp
npm install
```

## Scripts Disponibles

### Desarrollo
```bash
npm start
# o
npm run dev
```
Inicia el servidor de desarrollo en http://localhost:3000

### Producción
```bash
npm run build
```
Compila la aplicación para producción en la carpeta `build/`

### Preview
```bash
npm run preview
```
Previsualiza la build de producción localmente

### Linting
```bash
npm run lint
```
Ejecuta ESLint en todos los archivos TypeScript/TSX

## Configuración de Vite

El archivo `vite.config.ts` está configurado para:
- Usar el puerto 3000
- Proxy de `/api` hacia `http://localhost:5000` (backend ASP.NET Core)
- Build output en carpeta `build/` (compatible con configuración existente)
- Sourcemaps habilitados

## Proxy de API

Las llamadas a `/api/*` son automáticamente redirigidas al backend en `http://localhost:5000`. 

Por ejemplo:
```typescript
// En Home.tsx
const response = await httpClient.get<CurrencyDto>(`/api/Currency`);
// Se redirige a: http://localhost:5000/api/Currency
```

## Variables de Entorno

Archivo `.env`:
```
VITE_API_URL=http://localhost:5000
```

Para usar en el código:
```typescript
const apiUrl = import.meta.env.VITE_API_URL;
```

## Componentes Actualizados

### index.tsx
- Usa `createRoot` de React 18
- Envuelve con `<React.StrictMode>`
- Usa `<BrowserRouter>` directamente

### App.tsx
- Usa `<Routes>` y `<Route>` de React Router v6
- Sintaxis moderna con `element` prop

### Layout.tsx
- Convertido a componente funcional
- Usa Fragment corto `<>...</>`

### NavMenu.tsx
- Convertido a componente funcional con `useState`
- Sintaxis moderna de React

### Home.tsx
- Convertido a componente funcional con `useEffect` y `useState`
- Eliminada dependencia de Redux (no era necesaria aquí)
- Mejor manejo de errores

## Solución de Problemas

### Error: "Cannot find module 'react-dom/client'"
Ejecutar: `npm install`

### Error con tipos de TypeScript
Ejecutar: `npm install --save-dev @types/react @types/react-dom`

### Puerto 3000 ocupado
Cambiar el puerto en `vite.config.ts`:
```typescript
server: {
  port: 3001, // Cambiar aquí
  strictPort: true,
  // ...
}
```

### El backend no responde
Asegúrate de que el backend ASP.NET Core esté corriendo en el puerto 5000.

## Migración del Backend

Si necesitas actualizar la configuración del backend para servir esta nueva build:

En `Program.cs` o `Startup.cs`, asegúrate de que la configuración de SPA apunte a la carpeta `build/`:

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

## Diferencias Clave con la Versión Anterior

| Aspecto | Antes | Ahora |
|---------|-------|-------|
| Bundler | react-scripts (Webpack) | Vite |
| React | 16.11.0 | 18.3.1 |
| React Router | 5.1.2 | 6.28.0 |
| Redux | 4.0.4 | 5.0.1 |
| TypeScript | 3.9.10 | 5.7.2 |
| Bootstrap | 4.3.1 | 5.3.3 |
| Build time | ~30-60s | ~1-3s |
| Dev server start | ~10-20s | ~1-2s |

## Beneficios de Vite

1. **Inicio instantáneo**: El dev server inicia en segundos
2. **HMR ultrarrápido**: Los cambios se reflejan instantáneamente
3. **Build optimizado**: Usa Rollup para builds de producción más pequeñas
4. **ESM nativo**: Aprovecha los módulos ES nativos del navegador
5. **Sin configuración compleja**: Funciona out-of-the-box

## Siguientes Pasos Recomendados

1. **Testing**: Configurar Vitest para pruebas unitarias
   ```bash
   npm install -D vitest @testing-library/react @testing-library/jest-dom
   ```

2. **PWA**: Agregar soporte para Progressive Web App
   ```bash
   npm install -D vite-plugin-pwa
   ```

3. **TypeScript estricto**: Habilitar más opciones en tsconfig.json
   ```json
   "noUncheckedIndexedAccess": true,
   "noImplicitReturns": true
   ```

## Soporte

Si encuentras problemas, verifica:
1. Versión de Node.js: `node --version` (debe ser v22+)
2. Logs de la consola del navegador
3. Logs del terminal donde corre `npm start`
4. Estado del backend en http://localhost:5000

---

**Actualizado**: Diciembre 2024 para Node 22 y últimas versiones de todas las dependencias.

