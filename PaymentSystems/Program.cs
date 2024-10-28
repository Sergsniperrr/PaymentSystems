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

        int id = 12345;
        int amount = 5500;

        Order order = new Order(id, amount);

        IPaymentSystem payment1 = new PaymentSystem1();
        IPaymentSystem payment2 = new PaymentSystem2();
        IPaymentSystem payment3 = new PaymentSystem3();

        Console.WriteLine("Платежная система 1:");
        Console.WriteLine(payment1.GetPayingLink(order));
        Console.WriteLine();

        Console.WriteLine("Платежная система 2:");
        Console.WriteLine(payment2.GetPayingLink(order));
        Console.WriteLine();

        Console.WriteLine("Платежная система 3:");
        Console.WriteLine(payment3.GetPayingLink(order));

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

abstract class PaymentSystem : IPaymentSystem
{
    protected string AmountText(int amount) => $"amount={amount}";
    protected string MD5HashIdText(int id) => $"hash={HashCreator.GetMD5(id)}";

    public string GetPayingLink(Order order)
    {
        return CreateLink(order);
    }

    protected virtual string CreateLink(Order order)
    {
        return null;
    }
}

class PaymentSystem1 : PaymentSystem
{
    protected override string CreateLink(Order order)
    {
        string mainLink = "pay.system1.ru/order";

        return $"{mainLink}?{AmountText(order.Amount)}RUB&{MD5HashIdText(order.Id)}";
    }
}

class PaymentSystem2 : PaymentSystem
{
    protected override string CreateLink(Order order)
    {
        string mainLink = "order.system2.ru/pay";

        return $"{mainLink}?{MD5HashIdText(order.Id)}+{order.Amount}";
    }
}

class PaymentSystem3 : PaymentSystem
{
    protected override string CreateLink(Order order)
    {
        string mainLink = "system3.com/pay";

        return $"{mainLink}?{AmountText(order.Amount)}&curency=RUB&hash={HashCreator.GetSHA1(order.Amount)}" +
               $"+{order.Id}+{KeyGenerator.Create()}";
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

static class KeyGenerator
{
    public static string Create()
    {
        int keySize = 1024;

        return Convert.ToBase64String(new RSACryptoServiceProvider(keySize).ExportCspBlob(true));
    }
}