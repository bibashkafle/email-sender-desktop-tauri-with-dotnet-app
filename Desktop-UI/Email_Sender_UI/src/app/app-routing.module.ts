import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router'
import { EmailFormComponent } from './email-form/email-form.component';

const routes: Routes = [
  {path: '', redirectTo: 'email', pathMatch: 'full'},
  { path: 'email', component: EmailFormComponent },

];

@NgModule({
  declarations: [],
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
