
import endpoints from '../configuration/config';
import Order from '../models/order';

export default async function postOrder(order: Order, 
                                        handleSuccess: (body: any) => any, 
                                        handleError: (error: unknown) => any)
{ 
    try 
    {
        const response = await fetch(endpoints.postOrder,
            {
                method: 'POST',
                headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(order)
            });

        if(!response.ok)
            throw new Error(response.statusText);
        else
            handleSuccess(await response.json());
    } 
    catch (error) 
    {
        handleError(error);
    }
}