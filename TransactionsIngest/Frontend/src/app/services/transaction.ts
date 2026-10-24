import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';

export interface Transaction
{
  sourceSystem: string;
  transactionId: number;
  cardNumber: string;
  locationCode: string;
  productName: string;
  amount: number;
  transactionTime: string;
}

@Injectable({
  providedIn: 'root'
})

export class TransactionService {
    private apiUrl = 'https://localhost:7037/api/';
    private apiImportExtension = 'import';
    private apiTransactionsExtension = 'transactions';

    private dataStream$ = new Subject<any>();
    public currentData$ = this.dataStream$.asObservable();

    constructor(private http: HttpClient) {}

    createTransaction(transaction: Transaction): Observable<Transaction> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        
        var ObservableTransaction = this.http.post<Transaction>(this.apiUrl + this.apiImportExtension, transaction, { headers });

        return ObservableTransaction;
    }
    
    getTransactions(): Observable<Transaction[]> {
        return this.http.get<Transaction[]>(this.apiUrl + this.apiTransactionsExtension);
    }

    public triggerUpdate(savedTransaction: Transaction) {
        this.dataStream$.next(savedTransaction); // Emits the data to all subscribers
    }
}