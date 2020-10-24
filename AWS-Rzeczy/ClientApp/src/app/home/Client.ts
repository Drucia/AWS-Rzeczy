
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
