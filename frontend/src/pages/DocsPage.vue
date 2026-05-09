<template>
  <q-page class="q-pa-lg app-page">
    <q-card class="card-surface">
      <q-card-section>
        <div class="text-h5 section-title">Setup Guide</div>
        <div class="text-body2 subtle-text q-mt-sm">
          Configure API keys, database connectivity, and environment variables.
        </div>
      </q-card-section>
      <q-separator />
      <q-card-section>
        <div class="text-subtitle1 section-title">Environment Variables</div>
        <q-list bordered class="rounded-borders q-mt-sm">
          <q-item>
            <q-item-section>
              <q-item-label>VITE_API_BASE_URL</q-item-label>
              <q-item-label caption class="subtle-text">Base URL for the Northwind Traders API.</q-item-label>
            </q-item-section>
          </q-item>
          <q-item>
            <q-item-section>
              <q-item-label>VITE_GOOGLE_MAPS_API_KEY</q-item-label>
              <q-item-label caption class="subtle-text">Google Maps + Address Validation API key.</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>

        <div class="text-subtitle1 section-title q-mt-lg">Database Configuration</div>
        <div class="text-body2 subtle-text q-mt-sm">
          Update the API project's <code>DefaultConnection</code> string in
          <code>src/NorthwindTraders.Api/appsettings.Development.json</code> or via environment
          variable.
        </div>

        <div class="text-subtitle1 section-title q-mt-lg">Google Maps Setup</div>
        <ul class="q-ml-md">
          <li>Enable Address Validation API and Maps JavaScript API.</li>
          <li>Restrict the API key to your frontend origin.</li>
          <li>Store the key in <code>.env</code> as <code>VITE_GOOGLE_MAPS_API_KEY</code>.</li>
        </ul>
      </q-card-section>
    </q-card>
  </q-page>
</template>
