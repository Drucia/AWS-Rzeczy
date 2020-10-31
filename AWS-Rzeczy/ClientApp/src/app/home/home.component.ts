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
    public logs = [""];

    private startsClients: Client[] = [new Client("log", "haselko", "Ola", 23), new Client("log2", "haselko2", "Maciej", 30), new Client("log3", "haselko3", "Kasia", 11)];
    private currentClients: Client[] = [];
    private bucketName: string;
    private fileName: string;
    private fileContent: string;
    private created: boolean = false;
    private uploaded: boolean = false;

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private awsService: AWSService) {
    }


    public CreateBucket() {
        this.created = true;
        let body: S3RquestBody = { fileName: this.fileName, fileContent: this.fileContent, bucketName: this.bucketName }
        this.awsService.CreateBucket(body).subscribe(result => {
            this.logs.push("CreateBucket");
            this.logs.push(result.response);
        })
    }
    public UploadToBucket() {

        this.uploaded = true;
        let body: S3RquestBody = { fileName: this.fileName, fileContent: this.fileContent, bucketName: this.bucketName }
        this.awsService.UploadToBucket(body).subscribe(result => {
            this.logs.push("UploadToBucket");
            this.logs.push(result.response);
        })
    }

    public GetList() {
        let body: S3RquestBody = { fileName: this.fileName, fileContent: this.fileContent, bucketName: this.bucketName }
        this.awsService.GetListBucket(body).subscribe(result => {
            this.logs.push("GetList");
            this.logs.push(result.response);
        })
    }

    public GetFromBucket() {
        let body: S3RquestBody = { fileName: this.fileName, fileContent: this.fileContent, bucketName: this.bucketName }
        this.awsService.GetFromBucket(body).subscribe(result => {
            this.logs.push("GetFromBucket " + this.fileName);
            this.logs.push(result.response);
        })
    }
    public DeleteFromBucket() {
        let body: S3RquestBody = { fileName: this.fileName, fileContent: this.fileContent, bucketName: this.bucketName }
        this.awsService.DeleteFromBucket(body).subscribe(result => {
            this.logs.push("DeleteFromBucket");
            this.logs.push(result.response);
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
        this.http.post<Message>(this.baseUrl + `api/clients/createclients`, this.startsClients).subscribe(
            result => {
                this.logs.push(result.msg);
            },
            error => console.error(error));
    }

    public getAllClients(withlogs)
    {
        if (withlogs) {
            this.logs = [];
            this.logs.push("-> Getting all clients...");
        }
        this.http.get<Client[]>(this.baseUrl + 'api/clients').subscribe(
            result => {
                this.currentClients = result;
                if (withlogs) {
                  this.logs.push(`Amount of clients: ${result.length}`);
                  this.currentClients.forEach(client => this.logs.push(this.toString(client)));
                }
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
        this.getAllClients(false);
        if (this.currentClients.length > 0) {
            this.logs.push("-> Updating first client...");
            var toUpdate = this.currentClients[0];
            toUpdate.age += 1;

            this.http.post<Client>(this.baseUrl + `api/clients/${toUpdate.login}`, toUpdate).subscribe(
                result => {
                  this.currentClients[0] = result;
                    this.logs.push(`Updated "${result.login}" age to ${result.age}`);
                },
              error => console.error(error));
        } else {
          this.logs.push("-> There is not enought clients...");
        }
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
                this.logs.push(result.msg);
                this.getAllClients(false);
            },
            error => console.error(error));
    }

    public deleteLastClient()
    {
        this.logs = [];
        this.getAllClients(false);
        if (this.currentClients.length > 0) {
            this.logs.push("-> Deleting last client...");
            var toDelete = this.currentClients[this.currentClients.length - 1];

            this.http.delete<Client>(this.baseUrl + `api/clients/${toDelete.login}`).subscribe(
              result => {
                this.logs.push(`Deleted client: ${this.toString(result)}`);
                this.getAllClients(false);
              },
              error => console.error(error));
        } else {
            this.logs.push("-> There is not enought clients...");
        }
    }
}


