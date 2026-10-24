import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

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

  constructor(private http: HttpClient) {}

  createTransaction(transaction: Transaction): Observable<Transaction> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    
    var ObservableTransaction = this.http.post<Transaction>(this.apiUrl + this.apiImportExtension, transaction, { headers });

    return ObservableTransaction;
  }
}