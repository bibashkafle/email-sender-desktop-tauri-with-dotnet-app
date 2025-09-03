import { Injectable } from '@angular/core';
import { invoke } from "@tauri-apps/api/tauri";
@Injectable({
  providedIn: 'root'
})
export class AutomationClientService {

  constructor() { }

  public greeting(name:string) : Promise<string>{
    return invoke<string>("greet", { name });
  }
  
  public getUserName(): Promise<string>{
    return invoke<string>("get_username");
  }


}
