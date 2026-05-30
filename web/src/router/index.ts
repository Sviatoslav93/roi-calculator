import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/form-templates',
    },
    {
      path: '/form-templates',
      name: 'form-templates',
      component: () => import('../views/FormTemplatesView.vue'),
    },
    {
      path: '/form-templates/:id',
      name: 'form-template-detail',
      component: () => import('../views/FormTemplateDetailView.vue'),
    },
  ],
})

export default router
