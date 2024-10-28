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

        Console.WriteLine(HashCreator.Calculate(45, MD5.Create()));
        Console.WriteLine(HashCreator.Calculate(45, SHA1.Create()));
        Console.WriteLine(HashCreator.Calculate(45, SHA1.Create()));
        Console.WriteLine(HashCreator.Calculate(45, MD5.Create()));

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

interface IHashCreator
{
    string CalculateHash(int value);
}

static class HashCreator
{
    public static string Calculate(int value, HashAlgorithm algoritm)
    {
        return String.Concat(algoritm.ComputeHash(BitConverter.GetBytes(value))
                            .Select(x => x.ToString("x2")));
    }
}