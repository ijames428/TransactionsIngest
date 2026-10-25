import { Component } from '@angular/core';
import { TransactionService, Transaction } from '../../services/transaction';

const SOURCE_SYSTEM_STORE_UPTOWN = "Store Uptown";
const SOURCE_SYSTEM_STORE_DOWNTOWN = "Store Downtown";
const SOURCE_SYSTEM_ONLINE = "Online";
const LOCATION_CODE_STORE_UPTOWN = "UPTOWN";
const LOCATION_CODE_STORE_DOWNTOWN = "DOWNTOWN";
const LOCATION_CODE_ONLINE = "ONLINE";

@Component({
	selector: 'app-transaction-form',
	templateUrl: './transaction-form.html',
	styleUrls: ['./transaction-form.css']
})

export class TransactionFormComponent {
	isIdErrorVisible = false;
	isCardNumberErrorVisible = false;
	isProductNameErrorVisible = false;
	isPriceErrorVisible = false;

	private ImportDatas_Uptown = [
		[SOURCE_SYSTEM_STORE_UPTOWN, 100002, '***1111', 'Wireless Mouse', 24.99, '2026-10-25T08:15:30Z'],
		[SOURCE_SYSTEM_STORE_UPTOWN, 100003, '***1111', 'Keyboard', 34.99, '2026-10-25T08:20:30Z']
	];
	private ImportDatas_Downtown = [
		[SOURCE_SYSTEM_STORE_DOWNTOWN, 100002, '***0002', 'Mechanical Keyboard', 89.99, '2026-10-25T09:45:12Z'],
		[SOURCE_SYSTEM_STORE_DOWNTOWN, 100003, '***0002', 'Mousepad', 89.99, '2026-10-25T09:50:12Z']
	];
	private ImportDatas_Online = [
		[SOURCE_SYSTEM_ONLINE, 100002, '***4444', 'USB-C Docking Station', 129.95, '2026-10-25T11:20:05Z'],
		[SOURCE_SYSTEM_ONLINE, 100003, '***4444', 'Charging Dock', 49.99, '2026-10-25T11:25:05Z']
	];

	private ImportData = [
		this.ImportDatas_Uptown,
		this.ImportDatas_Downtown,
		this.ImportDatas_Online
	];

	constructor(private transactionService: TransactionService) { }

	onSubmitTransaction(_source_system: string,
						_transaction_id: number,
						_card_number: string,
						_product_name: string,
						_amount: number) {
		let wasErrorFound = false;
		
		if (Number.isNaN(_transaction_id)) {
			wasErrorFound = true;
			this.isIdErrorVisible = true;
		} else {
			this.isIdErrorVisible = false;
		}
		
		if (_card_number == "") {
			wasErrorFound = true;
			this.isCardNumberErrorVisible = true;
		} else {
			this.isCardNumberErrorVisible = false;
		}
		
		if (_product_name == "") {
			wasErrorFound = true;
			this.isProductNameErrorVisible = true;
		} else {
			this.isProductNameErrorVisible = false;
		}
		
		if (Number.isNaN(_amount)) {
			wasErrorFound = true;
			this.isPriceErrorVisible = true;
		} else {
			this.isPriceErrorVisible = false;
		}

		if (wasErrorFound)
		{
			return;
		}

		const now: Date = new Date();

		const newTransaction: Transaction = {
			sourceSystem: _source_system,
			transactionId: _transaction_id,
			cardNumber: _card_number,
			locationCode: this.getLocationCode(_source_system),
			productName: _product_name,
			amount: _amount,
			transactionTime: now.toISOString(),
			transactionStatus: ''
		};

		this.transactionService.runManualImport(newTransaction).subscribe({
			next: (savedTransaction) => {
				this.transactionService.triggerUpdateWithTransaction(savedTransaction);
			},
			error: (error) => {
				console.error('Failed to send data to MapPost:', error);
			}
		});
	}

	async onRunImports() {
		await this.runImport(0, SOURCE_SYSTEM_STORE_UPTOWN);
		await this.runImport(1, SOURCE_SYSTEM_STORE_DOWNTOWN);
		await this.runImport(2, SOURCE_SYSTEM_ONLINE);
	}

	getLocationCode(sourceSystem: string)
	{
		if (sourceSystem == SOURCE_SYSTEM_STORE_DOWNTOWN) {
			return LOCATION_CODE_STORE_DOWNTOWN;
		}
		
		if (sourceSystem == SOURCE_SYSTEM_ONLINE) {
			return LOCATION_CODE_ONLINE;
		}

		return LOCATION_CODE_STORE_UPTOWN;
	}

	async runImport(import_index: number, sourceSystem: string) {
		if (this.ImportData.length <= import_index) {
			return;
		}

		const import_datas: any[] = this.ImportData[import_index];
		if (import_datas.length == 0) {
			return;
		}

		let import_transactions: Transaction[] = [];

		import_datas.forEach((import_data) => {
			const newImportTransaction: Transaction = {
				sourceSystem: import_data[0],
				transactionId: import_data[1],
				cardNumber: import_data[2],
				locationCode: this.getLocationCode(sourceSystem),
				productName: import_data[3],
				amount: import_data[4],
				transactionTime: import_data[5],
				transactionStatus: ''
			};

			import_transactions.push(newImportTransaction);
		})

		this.transactionService.runImport(sourceSystem, import_transactions).subscribe({
			next: () => {
				this.transactionService.triggerUpdate();
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

	async onResetDatabases() {
		this.transactionService.resetDatabases().subscribe({
			next: (response) => {
				this.transactionService.triggerUpdate();
			},
			error: (err) => {
				console.error('Failed to clear database', err);
			}
		});
	}
}