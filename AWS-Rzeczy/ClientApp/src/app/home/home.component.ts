import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {
  public logs = ["nananna"];

  private clients: Client[] = [new Client("log", "haselko", "Ola", 23), new Client("log2", "haselko2", "Maciej", 30), new Client("log3", "haselko3", "Kasia", 11)];

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  public runScriptWithCRUDDB() {
    this.logs = [];
    this.deleteClientTableIfExist();
    //this.createClientTable();
    //this.createClientList();
    //this.getAllClients();

    //this.updateFirstClient();
    //this.getAllClients();
    //this.addClient()
    //this.getAllClients();
    //this.deleteLastClient();
    //this.getAllClients();
  }

  deleteClientTableIfExist() {
    this.logs.push("-> Deleting CLIENT table if exist...");
    this.http.delete<Message>(this.baseUrl + `api/clients/deletetable`).subscribe(result => {
      this.logs.push(result.msg);
      this.createClientTable();
    },
      error => console.error(error));
  }

  createClientTable() {
    this.logs.push("-> Creating CLIENT table...");
    this.http.post<Message>(this.baseUrl + `api/clients/createtable`, null).subscribe(result => {
      this.logs.push(`Created ${result.msg} table.`);
      this.createClientList();
      }, error => console.error(error));
  }

  createClientList() {
    this.logs.push("-> Adding start client list...");
    this.http.post<Message>(this.baseUrl + `api/clients/createclients`, this.clients).subscribe(result => {
      this.logs.push(result.msg);
      this.getAllClients(() => this.updateFirstClient());
    }, error => console.error(error));
  }

  getAllClients(nextFunction: () => void) {
    this.logs.push("-> Getting all clients...");
    this.http.get<Client[]>(this.baseUrl + 'api/clients').subscribe((result) => {
      this.clients = result;
      this.logs.push(`Amount of clients: ${result.length}`);
      this.clients.forEach(client => this.logs.push(this.toString(client)));
      nextFunction();
    }, error => console.error(error));
  }

  toString(client) {
    return `login: ${client.login}, password: ${client.password}, name: ${client.name}, age: ${client.age}`;
  }

  updateFirstClient() {
    this.logs.push("-> Updating first client...");
    var toUpdate = this.clients[0];
    toUpdate.age += 1;
    this.http.post<Message>(this.baseUrl + `api/clients/${toUpdate.login}`, toUpdate).subscribe(result => {
      //this.clients[0] = result;
      //this.logs.push(`Updated "${result.login}" age to ${result.age}`);
      this.logs.push(result.msg);
      this.getAllClients(() => this.addClient());
    }, error => console.error(error));
  }

  addClient() {
    this.logs.push("-> Adding new client...");
    let client: Client = {
      login: "newClient",
      password: "haselko",
      name: "Alabama",
      age: 20
    }
    this.http.post<Message>(this.baseUrl + 'api/clients', client).subscribe(result => {
      //this.clients.push(result);
      //this.logs.push(`Added new client: ${result.toString()}`);
      this.logs.push(result.msg);
      this.getAllClients(() => this.deleteLastClient());
    }, error => console.error(error));
  }

  deleteLastClient() {
    this.logs.push("-> Deleting last client...");
    var toDelete = this.clients[this.clients.length - 1];
    this.http.delete<Message>(this.baseUrl + `api/clients/${toDelete.login}`).subscribe(result => {
      //this.clients.pop();
      //this.logs.push(result.msg);
      this.logs.push(result.msg);
      this.getAllClients(() => this.logs.push("---KONIEC---"));
    }, error => console.error(error));
  }
}

export class Client {
  public login: string;
  public password: string;
  public name: string;
  public age: number;

  constructor(log, pass, name, age) {
    this.login = log;
    this.password = pass;
    this.name = name;
    this.age = age;
  }
}

export class Message {
  public msg: string;
}

