# Northwind Traders Frontend

Minimalistic Quasar-powered frontend for order management, Google Maps address validation, and reporting.

## Recommended IDE Setup

[VS Code](https://code.visualstudio.com/) + [Vue (Official)](https://marketplace.visualstudio.com/items?itemName=Vue.volar) (and disable Vetur).

## Recommended Browser Setup

- Chromium-based browsers (Chrome, Edge, Brave, etc.):
  - [Vue.js devtools](https://chromewebstore.google.com/detail/vuejs-devtools/nhdogjmejiglipccpnnnanhbledajbpd)
  - [Turn on Custom Object Formatter in Chrome DevTools](http://bit.ly/object-formatters)
- Firefox:
  - [Vue.js devtools](https://addons.mozilla.org/en-US/firefox/addon/vue-js-devtools/)
  - [Turn on Custom Object Formatter in Firefox DevTools](https://fxdx.dev/firefox-devtools-custom-object-formatters/)

## Customize configuration

See [Vite Configuration Reference](https://vite.dev/config/).

## Project Setup

```sh
npm install
```

## Environment Variables

Create a `.env` file in `frontend/` with:

```sh
VITE_API_BASE_URL=http://localhost:5000
VITE_GOOGLE_MAPS_API_KEY=YOUR_GOOGLE_MAPS_KEY
```

## Google Maps + Address Validation

- Enable **Address Validation API** and **Maps JavaScript API** in Google Cloud.
- Restrict the API key to your frontend origin(s).
- Store the key in `.env` as `VITE_GOOGLE_MAPS_API_KEY`.

## PDF Export

Order reports are generated client-side using `jspdf` + `jspdf-autotable`.
If you customize branding, update `src/utils/pdf.js`.

## Database Configuration

Update the API `DefaultConnection` string in `src/NorthwindTraders.Api/appsettings.Development.json`
or via environment variable for production environments.

### Compile and Hot-Reload for Development

```sh
npm run dev
```

### Compile and Minify for Production

```sh
npm run build
```

### Run Unit Tests with [Vitest](https://vitest.dev/)

```sh
npm run test:unit
```

### Lint with [ESLint](https://eslint.org/)

```sh
npm run lint
```
