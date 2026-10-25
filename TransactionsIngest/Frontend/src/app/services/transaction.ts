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
  transactionStatus: string;
}

export interface HistoryEntry
{
  entryId: number;
  sourceSystem: string;
  transactionId: number;
  updatedField: string;
  newString: string;
  oldString: string;
  newDecimal: number;
  oldDecimal: number;
  updateTime: string;
}

export interface SourceSystemImport
{
  sourceSystem: string;
  transactions: Transaction[];
}

@Injectable({
  providedIn: 'root'
})

export class TransactionService {
    private apiUrl = 'https://localhost:7037/api/';
    private apiImportExtension = 'import';
    private apiManualImportExtension = 'manual_import';
    private apiTransactionsExtension = 'transactions';
    private apiHistoryExtension = 'history';
    private apiReset = 'reset';

    private dataStream$ = new Subject<any>();
    public currentData$ = this.dataStream$.asObservable();

    constructor(private http: HttpClient) {}

    runImport(sourceSystem: string, transactions: Transaction[]): Observable<Transaction> {
        const sourceSystemImport: SourceSystemImport = {
				sourceSystem: sourceSystem,
				transactions: transactions
			};

        const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        
        var ObservableTransaction = this.http.post<Transaction>(this.apiUrl + this.apiImportExtension, sourceSystemImport, { headers });

        return ObservableTransaction;
    }

    runManualImport(transaction: Transaction): Observable<Transaction> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        
        var ObservableTransaction = this.http.post<Transaction>(this.apiUrl + this.apiManualImportExtension, transaction, { headers });

        return ObservableTransaction;
    }
    
    getTransactions(): Observable<Transaction[]> {
        return this.http.get<Transaction[]>(this.apiUrl + this.apiTransactionsExtension);
    }
    
    getHistory(): Observable<HistoryEntry[]> {
        return this.http.get<HistoryEntry[]>(this.apiUrl + this.apiHistoryExtension);
    }

    public triggerUpdateWithTransaction(savedTransaction: Transaction) {
        this.dataStream$.next(savedTransaction); // Emits the data to all subscribers
    }

    public triggerUpdate() {
        this.dataStream$.next(null); // Emits the data to all subscribers
    }
    
    resetDatabases(): Observable<any> {
        return this.http.post<Transaction>(this.apiUrl + this.apiReset, {});
    }
}