import { Component, OnInit } from '@angular/core';
import { AutomationClientService } from '../automation-client.service';
import { open } from '@tauri-apps/api/dialog';
import { appLocalDataDir} from '@tauri-apps/api/path';
import { writeTextFile, BaseDirectory,createDir } from '@tauri-apps/api/fs';
import { Command } from '@tauri-apps/api/shell'
import { invoke } from "@tauri-apps/api/tauri";
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-email-form',
  templateUrl: './email-form.component.html',
  styleUrls: ['./email-form.component.css']
})
export class EmailFormComponent implements OnInit {

  displayName = "";
  thisPcusername= "";
  userName = ""
  userPassword = "";
  showPasswordDialogBox = false;
  sideCarOutput = "";
  showOutPutDialog = false
  constructor(private service: AutomationClientService){}

  emailForm = new FormGroup({
    subject: new FormControl('', [Validators.required]),
    emailTo: new FormControl('', [Validators.required]),
    attachments: new FormControl(''),
    emailBody: new FormControl('', [Validators.required]),
  });
  
  ngOnInit(): void {
    this.service.getUserName().then((text) => {
      this.displayName = text;
      this.thisPcusername = text;
    });
  }

  async selectReceiverListFile(){
    const selected = await open({
      multiple: false,
      filters: [{
        name: 'Excel/csv',
        extensions: ['xlsx', 'csv']
      }]
    });

    if (selected) {
      this.emailForm.get("emailTo")?.setValue(selected as string);
    }
  }

  async selectAttachementFiles(){
    const selected = await open({
      multiple: true,
    });

    if (selected) {
      this.emailForm.get("attachments")?.setValue(selected.toString());
    }
  }

  askUserCredential() :boolean{
    if(this.emailForm.get("subject")?.valid && this.emailForm.get("emailTo")?.valid && this.emailForm.get("emailBody")?.valid){
      return true;
    }
    return false;
  }

  allowToSend() :boolean{
   return this.userName && this.userPassword? true: false;
  }


  onFormsubmit(){
    if(!this.emailForm.valid){
      this.emailForm.markAsTouched();
      return;
    }
    this.showPasswordDialogBox = true;
  }

  getCurrentURL(){
    return window.location.href;
  }

  async sendEmail(){
    this.showPasswordDialogBox = false;
    this.showOutPutDialog = true;
    this.sideCarOutput = "Email Sending......... please wait";

    try{
      const appLocalDataDirPath = await appLocalDataDir();
      var emailFormData  = {
        DisplayName: this.displayName?this.displayName:this.thisPcusername,
        UserName: this.userName,
        Password: this.userPassword,
        Subject: this.emailForm.get("subject")?.value as any,
        Body: this.emailForm.get("emailBody")?.value as any,
        ReceiverAndAttachments: [this.emailForm.get("emailTo")?.value]
      }
      
      if(this.emailForm.get("attachments")?.value){
        const temp = this.emailForm.get("attachments")?.value?.split(",");
        temp?.forEach(file=>{
          emailFormData.ReceiverAndAttachments.push(file);
        });    
      }
  
      var emailFormJsonData = JSON.stringify(emailFormData);
      await createDir('EmailData', { dir: BaseDirectory.AppLocalData, recursive: true });
      const emailFormFile = `${appLocalDataDirPath}\\EmailData\\emailForm.json`;
      await writeTextFile(emailFormFile, emailFormJsonData);
      const sentEmailLog = `${appLocalDataDirPath}\\EmailData\\log.txt`;
      const command = Command.sidecar('banaries/Email-Sender',[emailFormFile,sentEmailLog]);
      command.on('error', error => this.sideCarOutput = `${error}` );
      command.stdout.on('data', line => this.sideCarOutput = `${line}`);
      command.stderr.on('data', line => this.sideCarOutput = `${line}`);
      await command.execute();
    }catch(e){
      this.sideCarOutput = (e as Error)?.message;
    }

  }
}