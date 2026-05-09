<script setup>
import { ref } from 'vue'

const drawer = ref(null)
const navLinks = [
  { label: 'Orders', to: '/orders', icon: 'shopping_cart' },
  { label: 'Reports', to: '/reports', icon: 'insights' },
]

const toggleDrawer = () => {
  drawer.value?.toggle()
}
</script>

<template>
  <q-layout view="lHh Lpr lFf" class="app-shell">
    <q-header elevated class="bg-primary text-white">
      <q-toolbar>
        <q-btn flat dense round icon="menu" class="q-mr-sm" @click="toggleDrawer" />
        <q-toolbar-title>Northwind Traders</q-toolbar-title>
        <q-space />
        <q-btn flat dense icon="help" href="/docs" target="_blank" aria-label="Docs" />
      </q-toolbar>
    </q-header>

    <q-drawer ref="drawer" show-if-above bordered class="bg-grey-1" aria-label="Main navigation">
      <q-list padding>
        <q-item-label header class="text-grey-7">Operations</q-item-label>
        <q-item v-for="link in navLinks" :key="link.to" clickable :to="link.to">
          <q-item-section avatar>
            <q-icon :name="link.icon" />
          </q-item-section>
          <q-item-section>{{ link.label }}</q-item-section>
        </q-item>
      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
