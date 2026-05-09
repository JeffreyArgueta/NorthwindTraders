import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    component: () => import('../layouts/MainLayout.vue'),
    children: [
      { path: '', redirect: '/orders' },
      { path: 'orders', component: () => import('../pages/OrdersPage.vue') },
      { path: 'reports', component: () => import('../pages/ReportsPage.vue') },
      { path: 'docs', component: () => import('../pages/DocsPage.vue') },
    ],
  },
]

export default createRouter({
  history: createWebHistory(),
  routes,
})
