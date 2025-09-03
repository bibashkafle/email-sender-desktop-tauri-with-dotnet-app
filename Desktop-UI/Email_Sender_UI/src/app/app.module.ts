import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { AppComponent } from "./app.component";
import { AppRoutingModule } from "./app-routing.module";
import { MultiSelectModule } from "primeng/multiselect";
import { DropdownModule } from "primeng/dropdown";
import { ToastModule } from "primeng/toast";
import { SidebarModule } from "primeng/sidebar";
import { ConfirmationService, MessageService } from "primeng/api";
import { RippleModule } from "primeng/ripple";
import { MessagesModule } from "primeng/messages";
import { ConfirmDialogModule } from "primeng/confirmdialog";
import { ConfirmPopupModule } from "primeng/confirmpopup";
import { DialogModule } from "primeng/dialog";
import { ButtonModule } from "primeng/button";
import { AutomationClientService } from "./automation-client.service";
import { MenubarComponent } from "./menubar/menubar.component";
import { PanelModule } from "primeng/panel";
import { TableModule } from "primeng/table";
import { InputTextareaModule } from "primeng/inputtextarea";
import { InputTextModule } from "primeng/inputtext";
import { EditorModule } from "primeng/editor";
import { EmailFormComponent } from "./email-form/email-form.component";

@NgModule({
  declarations: [AppComponent, MenubarComponent, EmailFormComponent],
  imports: [
    BrowserModule,
    CommonModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    RippleModule,
    DialogModule,
    ButtonModule,
    PanelModule,
    EditorModule
  ],
  providers: [ConfirmationService, MessageService, AutomationClientService],
  bootstrap: [AppComponent],
})
export class AppModule {}
