import endpoints from '../../../configuration/config';
import Order from '../models/order';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export default class OrderService {
  async postOrder(
    order: Order,
    handleSuccess: () => any,
    handleError: (error: unknown) => any
  ) {
    try {
      const response = await fetch(endpoints.postOrder, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(order),
      });

      if (!response.ok) throw new Error(response.statusText);
      else handleSuccess();
    } catch (error) {
      handleError(error);
    }
  }
}
