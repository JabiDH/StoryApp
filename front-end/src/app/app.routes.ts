import { Routes } from '@angular/router';
import { StoriesComponent } from '../features/stories/stories.component';

export const routes: Routes = [
  { path: '', redirectTo: '/stories', pathMatch: 'full' },
  { path: 'stories', component: StoriesComponent },
];
