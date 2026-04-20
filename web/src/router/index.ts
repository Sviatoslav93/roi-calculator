import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/roi-forms',
    },
    {
      path: '/roi-forms',
      name: 'roi-forms',
      component: () => import('../views/RoiFormsView.vue'),
    },
    {
      path: '/roi-forms/:id',
      name: 'roi-form-detail',
      component: () => import('../views/RoiFormDetailView.vue'),
    },
  ],
})

export default router
