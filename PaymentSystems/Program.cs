using System;
using System.Linq;
using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        //Выведите платёжные ссылки для трёх разных систем платежа: 
        //pay.system1.ru/order?amount=12000RUB&hash={MD5 хеш ID заказа}
        //order.system2.ru/pay?hash={MD5 хеш ID заказа + сумма заказа}
        //system3.com/pay?amount=12000&curency=RUB&hash={SHA-1 хеш сумма заказа + ID заказа + секретный ключ от системы}

        Console.WriteLine(HashCreator.GetMD5(67));
        Console.WriteLine(HashCreator.GetSHA1(67));
        Console.WriteLine(HashCreator.GetSHA1(100));
        Console.WriteLine(HashCreator.GetMD5(100));



        Console.Read();
    }
}

class Order
{
    public readonly int Id;
    public readonly int Amount;

    public Order(int id, int amount) => (Id, Amount) = (id, amount);
}

interface IPaymentSystem
{
    string GetPayingLink(Order order);
}

//abstract class PaymentSystem : IPaymentSystem
//{
//    private readonly Order _order;

//    public string GetPayingLink(Order order)
//    {
//        return null;
//    }
//}

class PaymentSystem1 : IPaymentSystem
{
    public string GetPayingLink(Order order)
    {
        return null;
    }
}

class PaymentSystem2 : IPaymentSystem
{
    public string GetPayingLink(Order order)
    {

    }
}

class PaymentSystem3 : IPaymentSystem
{
    private readonly Order _order;

    public string GetPayingLink(Order order)
    {

    }
}

static class HashCreator
{
    public static string GetMD5(int value)
    {
        return Calculate(value, MD5.Create());
    }

    public static string GetSHA1(int value)
    {
        return Calculate(value, SHA1.Create());
    }

    private static string Calculate(int value, HashAlgorithm algoritm)
    {
        return String.Concat(algoritm.ComputeHash(BitConverter.GetBytes(value))
                            .Select(x => x.ToString("x2")));
    }
}