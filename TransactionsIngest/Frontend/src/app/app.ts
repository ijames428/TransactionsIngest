import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TransactionFormComponent } from './components/transaction-form/transaction-form';

@Component({
  imports: [RouterOutlet, TransactionFormComponent],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html'
})

export class App {
  title = 'Transactions Ingestor';
}
