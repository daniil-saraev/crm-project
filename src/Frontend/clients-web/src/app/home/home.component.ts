import { Component, OnInit } from '@angular/core';
import Order from 'src/app/core/models/order';
import OrderService from '../core/services/order.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  constructor(
    private readonly orderService: OrderService,
    private readonly messageService: MessageService
  ) {}

  private _orderForm: FormGroup | null = null;

  get orderForm(): FormGroup {
    return this._orderForm!;
  }

  ngOnInit(): void {
    this._orderForm = new FormGroup({
      name: new FormControl('', [Validators.required, Validators.minLength(1)]),
      phoneNumber: new FormControl('', [
        Validators.required,
        Validators.pattern('[0-9]{11}'),
      ]),
      email: new FormControl('', [Validators.required, Validators.email]),
      description: new FormControl('', [
        Validators.required,
        Validators.minLength(1),
      ]),
    });
  }

  get name() {
    return this._orderForm?.get('name')!;
  }
  get phone() {
    return this._orderForm?.get('phoneNumber')!;
  }
  get email() {
    return this._orderForm?.get('email')!;
  }
  get description() {
    return this._orderForm?.get('description')!;
  }

  showSuccess() {
    this.messageService.add({
      severity: 'success',
      summary: 'Отлично!',
      detail: 'Ваша заявка успешно отправлена',
    });
  }

  showError() {
    this.messageService.add({
      severity: 'error',
      summary: 'Какие-то неполадки',
      detail: 'Попробуйте отправить заявку еще раз через минуту',
    });
  }

  async postOrder() {
    const order = new Order(
      this.name.getRawValue(),
      this.phone.getRawValue(),
      this.email.getRawValue(),
      this.description.getRawValue()
    );
    await this.orderService.postOrder(order, this.showSuccess, this.showError);
  }
}
