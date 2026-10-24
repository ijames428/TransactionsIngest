import { Component } from '@angular/core';
import { TransactionService, Transaction } from '../../services/transaction';

@Component({
  selector: 'app-transaction-form',
  templateUrl: './transaction-form.html',
  styleUrls: ['./transaction-form.css']
})

export class TransactionFormComponent 
{
  constructor(private transactionService: TransactionService) {}

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

    this.transactionService.createTransaction(newTransaction).subscribe({
      next: (savedTransaction) => {
        this.transactionService.triggerUpdate(savedTransaction);
      },
      error: (error) => {
        console.error('Failed to send data to MapPost:', error);
      }
    });
  }
}