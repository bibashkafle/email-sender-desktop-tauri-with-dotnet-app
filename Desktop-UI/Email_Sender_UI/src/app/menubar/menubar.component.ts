import { Component, OnInit } from '@angular/core';
import { AutomationClientService } from '../automation-client.service';

@Component({
  selector: 'app-menubar',
  templateUrl: './menubar.component.html',
  styleUrls: ['./menubar.component.css']
})
export class MenubarComponent implements OnInit {
  userName: string = "";

  constructor(private service: AutomationClientService){}

  ngOnInit(): void {
    this.service.getUserName().then((text) => {
      this.userName = text;
    });
  }

}
