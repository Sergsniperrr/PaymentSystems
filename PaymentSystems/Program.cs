using System;
using System.Text;
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

        IPaymentSystem payment1 = new PaymentSystem1(new MD5HashGenerator());
        IPaymentSystem payment2 = new PaymentSystem2(new MD5HashGenerator());
        IPaymentSystem payment3 = new PaymentSystem3(new SHA1HashGenerator());

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
    public Order(int id, int amount)
    {
        Id = id;
        Amount = amount;
    }

    public int Id { get; }
    public int Amount { get; }

}

interface IPaymentSystem
{
    string GetPayingLink(Order order);
}

class PaymentSystem1 : IPaymentSystem
{
    private readonly HashGenerator _hashGenerator;

    public PaymentSystem1(HashGenerator hashGenerator)
    {
        _hashGenerator = hashGenerator;
    }

    public string GetPayingLink(Order order)
    {
        string mainLink = "pay.system1.ru/order";

        return $"{mainLink}?amount={order.Amount}RUB&hash={_hashGenerator.Compute(order.Id.ToString())}";
    }
}

class PaymentSystem2 : IPaymentSystem
{
    private readonly HashGenerator _hashGenerator;

    public PaymentSystem2(HashGenerator hashGenerator)
    {
        _hashGenerator = hashGenerator;
    }

    public string GetPayingLink(Order order)
    {
        string mainLink = "order.system2.ru/pay";
        string dataForHashing = String.Concat(order.Id, order.Amount);

        return $"{mainLink}?hash={_hashGenerator.Compute(dataForHashing)}";
    }
}

class PaymentSystem3 : IPaymentSystem
{
    private readonly HashGenerator _hashGenerator;

    public PaymentSystem3(HashGenerator hashGenerator)
    {
        _hashGenerator = hashGenerator;
    }

    public string GetPayingLink(Order order)
    {
        string mainLink = "system3.com/pay";
        string dataForHashing = String.Concat(order.Amount, order.Id, KeyGenerator.Generate());

        return $"{mainLink}?amount={order.Amount}&curency=RUB&hash={_hashGenerator.Compute(dataForHashing)}";
    }
}

abstract class HashGenerator
{
    private readonly HashAlgorithm _algorithm;

    protected HashGenerator(HashAlgorithm algorithm)
    {
        _algorithm = algorithm;
    }

    public string Compute(string text)
    {
        StringBuilder result = new StringBuilder();

        byte[] input = Encoding.ASCII.GetBytes(text);
        byte[] hash = _algorithm.ComputeHash(input);

        for (int i = 0; i < hash.Length; i++)
            result.Append(hash[i].ToString("X2"));

        return result.ToString();
    }
}

class MD5HashGenerator : HashGenerator
{
    public MD5HashGenerator() : base(MD5.Create()) { }
}

class SHA1HashGenerator : HashGenerator
{
    public SHA1HashGenerator() : base(SHA1.Create()) { }
}

static class KeyGenerator
{
    public static string Generate()
    {
        int keySize = 1024;

        return Convert.ToBase64String(new RSACryptoServiceProvider(keySize).ExportCspBlob(true));
    }
}