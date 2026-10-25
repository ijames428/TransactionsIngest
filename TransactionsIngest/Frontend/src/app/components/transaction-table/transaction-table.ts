import { Component, OnInit } from '@angular/core';
import { AsyncPipe, CommonModule } from '@angular/common';
import { TransactionService } from '../../services/transaction';
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
    private sub!: Subscription;
    
    transactions$ = this.refresh$.pipe(
        switchMap(() => this.transactionService.getTransactions())
    );

    constructor(private transactionService: TransactionService) {}

    ngOnInit(): void {
        this.fetchTransactionsTableData();
        
        this.sub! = this.transactionService.currentData$.subscribe(savedTransaction => {
            this.fetchTransactionsTableData();
        });
    }

    public fetchTransactionsTableData(): void {
        this.refresh$.next();
    }
}