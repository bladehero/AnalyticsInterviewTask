﻿using System.Diagnostics.CodeAnalysis;

namespace Sandbox;

// Tasks
// 1. Review code & explain what it does
// 2. Identify issues
// 3. Propose improvements
// 4. Agree an improvements with interviewer and implement those

public class User(string name, double cashBalance, double creditCardBalance)
{
    // TODO: Violation of coding style convention: use property instead of field
    public string Name = name;
    public double CashBalance = cashBalance;
    public double CreditCardBalance = creditCardBalance;

    public override string ToString() =>
        $"Name: {Name}; CashBalance: {CashBalance:N2}; CreditCardBalance: {CreditCardBalance:N2}";
}

public class Product(string name, double price)
{
    // TODO: Violation of coding style convention: use property instead of field
    public string Name = name;
    // TODO: Violation of coding style convention: use property instead of field
    public double Price = price;

    public override string ToString() =>
        $"Name: {Name}; Price: {Price:N2}";
}

public abstract class OrderProcessorBase
{
    protected abstract void ProcessPayment(User user, IEnumerable<Product> items);

    protected abstract Task GenerateReceiptAsync(User user, IEnumerable<Product> items);
    
    public Task ProcessAsync(User user, IEnumerable<Product> items)
    {
        try
        {
            // TODO: Enumerate to stable collection items here, and use for next steps
            
            // TODO: Logging-like nodes can be extracted to a private method-helper to un-blur the perception
            Console.WriteLine($"{GetType().Name}: Processing payment");

            // TODO: Possible multiple enumeration that will lead to re-iterating the same items multiple times
            // Sometimes might lead to unexpected behaviors & different results
            ProcessPayment(user, items);

            // TODO: Logging-like nodes can be extracted to a private method-helper to un-blur the perception
            Console.WriteLine($"{GetType().Name}: Generating Receipt");

            // TODO: Possible multiple enumeration that will lead to re-iterating the same items multiple times
            // Sometimes might lead to unexpected behaviors & different results
            return GenerateReceiptAsync(user, items);
        }
        catch (Exception e)
        {
            // TODO: Logging-like nodes can be extracted to a private method-helper to un-blur the perception
            Console.WriteLine($"{GetType().Name}: Error processing order: {e.Message}");
            // TODO: Straight re-throw exception it will lead to cutting off gathered information from the existing stack
            throw e;
        }
        finally
        {
            // TODO: Logging-like nodes can be extracted to a private method-helper to un-blur the perception
            Console.WriteLine($"{GetType().Name}: Order Processed");
        }
    }
}

public class CashOrderProcessor : OrderProcessorBase
{
    protected override void ProcessPayment(User user, IEnumerable<Product> items)
    {
        // TODO: It might a be good idea also to extract items' sum calculation into specific Calculator
        // It may help in separating responsibilities & removing duplicating code
        var sum = items.Sum(p => p.Price);
        if (user.CashBalance < sum) throw new ApplicationException("Insufficient Cash funds");

        user.CashBalance -= sum;
    }

    // TODO: Async keyword is not needed for methods where no await pair inside.
    // It may be replaced with a returning Task.CompletedTask
    protected override async Task GenerateReceiptAsync(User user, IEnumerable<Product> items)
    {
        Console.WriteLine($"Receipt for {user.Name}:\n{string.Join("\n", items)}\n");
    }
}

public class CreditCardOrderProcessor : OrderProcessorBase
{
    protected override void ProcessPayment(User user, IEnumerable<Product> items)
    {
        // TODO: It might be a good idea also to extract items' sum calculation into specific Calculator
        // It may help in separating responsibilities & removing duplicating code
        user.CreditCardBalance -= items.Sum(p => p.Price);
    }

    protected override async Task GenerateReceiptAsync(User user, IEnumerable<Product> items)
    {
        await Task.Delay(1000);
        throw new ApplicationException("Error generating receipt");
    }
}

/// <summary>
/// I assume this part of the code is not allowed to change. <br />
/// *Considering it is client-side code we have no power to change
/// </summary>
public static class Program
{
    public static IEnumerable<Product> GetCartProducts()
    {
        yield return new("Laptop", 800);
        throw new ApplicationException("Product is out of stock");
#pragma warning disable CS0162 // Unreachable code detected
        // ReSharper disable once HeuristicUnreachableCode
        yield return new("Smartphone", 700);
#pragma warning restore CS0162 // Unreachable code detected
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public static async Task Main()
    {
        Console.WriteLine("Starting Application");
        User user = new("John Smith", 1000, 1000);
        IEnumerable<Product> items = GetCartProducts();

        Console.WriteLine($"Processing order for [{user}] using Cash");
        await new CashOrderProcessor().ProcessAsync(user, items);

        Console.WriteLine($"Processing order for [{user}] using Credit Card");
        await new CreditCardOrderProcessor().ProcessAsync(user, items);
    }
}