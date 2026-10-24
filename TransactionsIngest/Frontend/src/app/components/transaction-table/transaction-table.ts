import { Component, OnInit } from '@angular/core';
import { AsyncPipe, CommonModule } from '@angular/common';
import { TransactionService, Transaction } from '../../services/transaction';
import { switchMap, Subscription, BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-transaction-table',
  standalone: true,
  imports: [CommonModule, AsyncPipe],
  templateUrl: './transaction-table.html',
  styleUrls: ['./transaction-table.css'],
})

export class TransactionTableComponent implements OnInit
{
    private refresh$ = new BehaviorSubject<void>(undefined);
    // transactions$!: Observable<Transaction[]>;
    private sub!: Subscription;
    
    transactions$ = this.refresh$.pipe(
        switchMap(() => this.transactionService.getTransactions())
    );

    constructor(private transactionService: TransactionService) {}

    ngOnInit(): void {
        this.fetchTableData();
        
        this.sub! = this.transactionService.currentData$.subscribe(savedTransaction => {
            this.fetchTableData();
        });
    }

    public fetchTableData(): void {
        this.refresh$.next();
    }
}