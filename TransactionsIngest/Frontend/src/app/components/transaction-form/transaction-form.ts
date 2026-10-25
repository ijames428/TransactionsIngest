import { Component } from '@angular/core';
import { TransactionService, Transaction } from '../../services/transaction';

@Component({
  selector: 'app-transaction-form',
  templateUrl: './transaction-form.html',
  styleUrls: ['./transaction-form.css']
})

export class TransactionFormComponent 
{
  private TestDatas = [
    ['Store Uptown', 100002, '***1111', 'UPTOWN', 'Wireless Mouse', 24.99, '2026-02-27T08:15:30Z', 'imported'],
    ['Store Downtown', 100002, '***0002', 'DOWNTOWN', 'Mechanical Keyboard', 89.99, '2026-02-27T09:45:12Z', 'imported'],
    ['Online', 100002, '***4444', 'ONLINE', 'USB-C Docking Station', 129.95, '2026-02-27T11:20:05Z', 'imported']
  ];

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
      transactionTime: now.toISOString(),
      transactionStatus: 'imported'
    };

    this.transactionService.createTransaction(newTransaction).subscribe({
      next: (savedTransaction) => {
        this.transactionService.triggerUpdateWithTransaction(savedTransaction);
      },
      error: (error) => {
        console.error('Failed to send data to MapPost:', error);
      }
    });
  }

  async onRunTests() 
  {
    let x = this.TestDatas.length;
    let num_tests = this.TestDatas.length;

    await this.runTest(num_tests - x--);
    await this.runTest(num_tests - x--);
    await this.runTest(num_tests - x--);
  }

  async runTest(test_index : number)
  {
    if (this.TestDatas.length <= test_index)
    {
      return;
    }

    console.log(test_index)
    const test_data: any[] = this.TestDatas[test_index];
    const now: Date = new Date();
    
    const newTestTransaction: Transaction = {
      sourceSystem: test_data[0],
      transactionId: test_data[1],
      cardNumber: test_data[2],
      locationCode: test_data[3],
      productName: test_data[4],
      amount: test_data[5],
      transactionTime: test_data[6],
      transactionStatus: ''
    };

    this.transactionService.createTransaction(newTestTransaction).subscribe({
      next: (savedTransaction) => {
        this.transactionService.triggerUpdateWithTransaction(savedTransaction);
      },
      error: (error) => {
        console.error('Failed to send data to MapPost:', error);
      }
    });

    await this.delay(100);
  }
  
  delay = (ms: number): Promise<void> => {
    return new Promise(resolve => setTimeout(resolve, ms));
  };

  async onResetDatabases() 
  {
    this.transactionService.resetDatabases().subscribe({
      next: (response) => {
        this.transactionService.triggerUpdate();
        // Proactively clear your local data state or reload the page here
        // e.g., this.myTableData$.next([]);
      },
      error: (err) => {
        console.error('Failed to clear database', err);
      }
    });
  }
}