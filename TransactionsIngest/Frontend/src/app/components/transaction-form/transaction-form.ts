import { Component } from '@angular/core';
import { TransactionService, Transaction } from '../../services/transaction';

@Component({
  selector: 'app-transaction-form',
  templateUrl: './transaction-form.html',
  styleUrls: ['./transaction-form.css']
})
export class TransactionFormComponent {

  // 1. Inject the transactionService through the constructor
  constructor(private transactionService: TransactionService) {}

  // 2. Trigger this method when a user submits a form or clicks a button
  onSubmitTransaction(_source_system : string,
                      _transaction_id: number,
                      _card_number: string,
                      _location_code: string,
                      _product_name: string,
                      _amount: number) 
  {
    const now: Date = new Date();

    const newTransaction: Transaction = {
      sourceSystem: _source_system,
      transactionId: _transaction_id,
      cardNumber: _card_number,
      locationCode: _location_code,
      productName: _product_name,
      amount: _amount,
      transactionTime: now.toISOString()
    };

    // 3. Call the service method and subscribe to the network response observable
    this.transactionService.createTransaction(newTransaction).subscribe({
      next: (savedTransaction) => {
        console.log('Successfully saved via MapPost:', savedTransaction);
        alert(`Success! Product created with Database ID: ${savedTransaction.transactionId}`);
      },
      error: (error) => {
        console.error('Failed to send data to MapPost:', error);
      }
    });
  }
}