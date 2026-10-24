import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TransactionFormComponent } from './components/transaction-form/transaction-form';
import { TransactionTableComponent } from './components/transaction-table/transaction-table';

@Component({
  imports: [RouterOutlet, TransactionFormComponent, TransactionTableComponent],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html'
})

export class App {
  title = 'Transactions Ingestor';
}
