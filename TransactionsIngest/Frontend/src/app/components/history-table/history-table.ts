import { Component, OnInit } from '@angular/core';
import { AsyncPipe, CommonModule } from '@angular/common';
import { TransactionService } from '../../services/transaction';
import { switchMap, Subscription, BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-history-table',
  standalone: true,
  imports: [CommonModule, AsyncPipe],
  templateUrl: './history-table.html',
  styleUrls: ['./history-table.css'],
})

export class HistoryTableComponent implements OnInit
{
    private refresh$ = new BehaviorSubject<void>(undefined);
    private sub!: Subscription;
    
    history_entries$ = this.refresh$.pipe(
        switchMap(() => this.transactionService.getHistory())
    );

    constructor(private transactionService: TransactionService) {}

    ngOnInit(): void {
        this.fetchHistoryTableData();
        
        this.sub! = this.transactionService.currentData$.subscribe(savedTransaction => {
            this.fetchHistoryTableData();
        });
    }

    public fetchHistoryTableData(): void {
        this.refresh$.next();
    }
}