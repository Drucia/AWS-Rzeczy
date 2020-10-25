import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Client } from './Client';
import { Message } from './Message';
import { AWSService } from '../aws.service';
import { S3RquestBody } from './S3RquestBody';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})

export class HomeComponent {
    public logs = ["nananna"];

    private clients: Client[] = [new Client("log", "haselko", "Ola", 23), new Client("log2", "haselko2", "Maciej", 30), new Client("log3", "haselko3", "Kasia", 11)];
    private bucketName: string;
    private key1: string;
    private key2: string;
    private created: boolean = false;
    private uploaded: boolean = false;

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private awsService: AWSService) {
    }


    public CreateBucket() {
        this.created = true;
        let body: S3RquestBody = { key1: this.key1, key2: this.key2, name: this.bucketName }
        this.awsService.CreateBucket(body).subscribe(result => {
            this.logs.push(result);
        })
    }
    public UploadToBucket() {

        this.uploaded = true;
        let body: S3RquestBody = { key1: this.key1, key2: this.key2, name: this.bucketName }
        this.awsService.UploadToBucket(body).subscribe(result => {
            this.logs.push(result);
        })
    }
    public GetFromBucket() {
        let body: S3RquestBody = { key1: this.key1, key2: this.key2, name: this.bucketName }
        this.awsService.GetFromBucket(body).subscribe(result => {
            this.logs.push(result);
        })
    }
    public DeleteFromBucket() {
        let body: S3RquestBody = { key1: this.key1, key2: this.key2, name: this.bucketName }
        this.awsService.DeleteFromBucket(body).subscribe(result => {
            this.logs.push(result);
        })
    }

    public deleteClientTable()
    {
        this.logs = [];
        this.logs.push("-> Deleting CLIENT table if exist...");
        this.http.delete<Message>(this.baseUrl + `api/clients/deletetable`).subscribe(
            result => {
              this.logs.push(result.msg);
            },
            error => console.error(error));
    }
    
    public createClientTable()
    {
      this.logs = [];
        this.logs.push("-> Creating CLIENT table...");
        this.http.post<Message>(this.baseUrl + `api/clients/createtable`, null).subscribe(
            result => {
                this.logs.push(`Created ${result.msg} table.`);
            },
            error => console.error(error));
    }

    public insertClientsList()
    {
        this.logs = [];
        this.logs.push("-> Adding start client list...");
        this.http.post<Message>(this.baseUrl + `api/clients/createclients`, this.clients).subscribe(
            result => {
                this.logs.push(result.msg);
            },
            error => console.error(error));
    }

    public getAllClients()
    {
        this.logs = [];
        this.logs.push("-> Getting all clients...");
        this.http.get<Client[]>(this.baseUrl + 'api/clients').subscribe(
            result => {
                this.clients = result;
                this.logs.push(`Amount of clients: ${result.length}`);
                this.clients.forEach(client => this.logs.push(this.toString(client)));
            },
            error => console.error(error));
    }

    private toString(client)
    {
        return `login: ${client.login}, password: ${client.password}, name: ${client.name}, age: ${client.age}`;
    }

    public updateFirstClient()
    {
        this.logs = [];
        this.logs.push("-> Updating first client...");
        var toUpdate = this.clients[0];
        toUpdate.age += 1;

        this.http.post<Client>(this.baseUrl + `api/clients/${toUpdate.login}`, toUpdate).subscribe(
            result => {
                this.clients[0] = result;
                this.logs.push(`Updated "${result.login}" age to ${result.age}`);
                //this.logs.push(result.msg);
            },
            error => console.error(error));
    }

    public addNewClient()
    {
        this.logs = [];
        this.logs.push("-> Adding new client...");
        let client: Client = {
            login: "newClient",
            password: "haselko",
            name: "Alabama",
            age: 20
      }
        this.http.post<Message>(this.baseUrl + 'api/clients', client).subscribe(
            result => {
            //this.clients.push(result);
            //this.logs.push(`Added new client: ${this.toString(result)}`);
                this.logs.push(result.msg);
            },
            error => console.error(error));
    }

    public deleteLastClient()
    {
        this.logs = [];
        this.logs.push("-> Deleting last client...");
        var toDelete = this.clients[this.clients.length - 1];

        this.http.delete<Client>(this.baseUrl + `api/clients/${toDelete.login}`).subscribe(
            result => {
                this.clients.pop();
                this.logs.push(`Deleted client: ${this.toString(result)}`);
            },
            error => console.error(error));
    }
}


