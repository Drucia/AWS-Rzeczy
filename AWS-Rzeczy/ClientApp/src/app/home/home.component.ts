import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {
  public logs = "nananna";

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  public runScriptWithCRUDDB() {
    this.logs = "";
    this.createClientTable();
  }
    createClientTable() {
      this.http.get<Client[]>(this.baseUrl + 'api/createtable').subscribe(result => {
        result.forEach(function (client) { this.logs += client.toString() + "/n"; });
      }, error => console.error(error));
    }
}

export class Client {
  login: string;
  password: string;
  name: string;
  age: number;

  public toString()
  {
    return `login: ${this.login}, password: ${this.password}, name: ${this.name}, age: ${this.age}`;
  }
}

