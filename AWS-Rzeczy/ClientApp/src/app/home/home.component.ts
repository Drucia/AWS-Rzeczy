import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {
  public logs = ["nananna"];
  private tableName: string = "client"

  private clients: Client[] = [new Client("log", "haselko", "Ola", 23), new Client("log2", "haselko2", "Maciej", 30), new Client("log3", "haselko3", "Kasia", 11)];

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  public runScriptWithCRUDDB() {
    this.logs = [];
    this.deleteClientTableIfExist();
    this.createClientTable();
    this.createClientList();
    this.getAllClients();
    this.updateFirstClient();
    this.getAllClients();
    this.addClient()
    this.getAllClients();
    this.deleteLastClient();
    this.getAllClients();
  }

  deleteClientTableIfExist() {
    this.http.delete<Message>(this.baseUrl + `api/deletetable/${this.tableName}`).subscribe(result => {
      this.logs.push(result.msg);
    },
      error => console.error(error));
  }

  createClientTable() {
    this.http.post<Message>(this.baseUrl + `api/createtable/${this.tableName}`, null).subscribe(result => {
      this.logs.push(result.msg);
      }, error => console.error(error));
  }

  createClientList() {
    this.http.post<Message>(this.baseUrl + `api/createclients`, this.clients).subscribe(result => {
      this.logs.push(result.msg);
    }, error => console.error(error));
  }

  getAllClients() {
    this.http.get<Client[]>(this.baseUrl + 'api/clients').subscribe(result => {
      this.clients = result;
      result.forEach(function (client) { this.logs += client.toString(); });
    }, error => console.error(error));
  }

  updateFirstClient() {
    var toUpdate = this.clients[0];
    toUpdate.age += 1;
    this.http.post<Client>(this.baseUrl + `api/clients/${toUpdate.login}`, toUpdate).subscribe(result => {
      this.clients[0] = result;
      this.logs.push(`Updated "${result.login}" age to ${result.age}`);
    }, error => console.error(error));
  }

  addClient() {
    let client: Client = {
      login: "newClient",
      password: "haselko",
      name: "Alabama",
      age: 20
    }
    this.http.post<Client>(this.baseUrl + 'api/clients', client).subscribe(result => {
      this.clients.push(result);
      this.logs.push(`Added new client: ${result.toString()}`);
    }, error => console.error(error));
  }

  deleteLastClient() {
    var toDelete = this.clients[this.clients.length - 1];
    this.http.delete<Message>(this.baseUrl + `api/clients/${toDelete.login}`).subscribe(result => {
      this.clients.pop();
      this.logs.push(result.msg);
    }, error => console.error(error));
  }
}

export class Client {
  login: string;
  password: string;
  name: string;
  age: number;

  constructor(log, pass, name, age) {
    this.login = log;
    this.password = pass;
    this.name = name;
    this.age = age;
  }

  public toString()
  {
    return `login: ${this.login}, password: ${this.password}, name: ${this.name}, age: ${this.age}`;
  }
}

export class Message {
  public msg: string;
}

