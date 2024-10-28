using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        //Выведите платёжные ссылки для трёх разных систем платежа: 
        //pay.system1.ru/order?amount=12000RUB&hash={MD5 хеш ID заказа}
        //order.system2.ru/pay?hash={MD5 хеш ID заказа + сумма заказа}
        //system3.com/pay?amount=12000&curency=RUB&hash={SHA-1 хеш сумма заказа + ID заказа + секретный ключ от системы}

        MD5HashIdCreator MD5hashCreator = new MD5HashIdCreator();
        SHA1HashIdCreator SHA1hashCreator = new SHA1HashIdCreator();

        Console.WriteLine(MD5hashCreator.CalculateHash(45));
        Console.WriteLine(SHA1hashCreator.CalculateHash(45));
        Console.WriteLine(SHA1hashCreator.CalculateHash(45));
        Console.WriteLine(MD5hashCreator.CalculateHash(45));



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

class MD5HashIdCreator : IHashCreator
{
    public string CalculateHash(int value)
    {
        var md5 = MD5.Create();
        string hash = String.Concat(md5.ComputeHash(BitConverter.GetBytes(value))
                            .Select(x => x.ToString("x2")));

        return hash;
    }
}

class HashIdCreator
{
    public string CalculateHash(int value, HashAlgorithm hashAlgoritm)
    {
        var sha1 = SHA1.Create();
        string hash = String.Concat(sha1.ComputeHash(BitConverter.GetBytes(value))
                            .Select(x => x.ToString("x2")));

        return hash;
    }
}