using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;

[Table("Transactions")]
public class Transaction
{
    [NotMapped]
    public string ObjectType => "Transaction";
    [NotMapped]
    public string Purpose {get; set;}
    [NotMapped]
    public int MinerId {get; set;}
    [NotMapped]
    public decimal TransactionFee {get; set;}
    
    [Key]
    public int TransactionId { get; set; }
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public decimal CryptoValue { get; set; }
    public decimal CashValue { get; set; }
    public string CryptocurrencyName { get; set; }
    public DateTime DateTime { get; set; }
    public string TransactionsHash { get; set; }
    public string ValidationStatus { get; set; }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static Transaction Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<Transaction>(json);
    }
    
    public static bool HandleReceivedTransaction(string data, out string message)
{
    // Deserialize the Transaction object
    Transaction receivedTransaction = JsonConvert.DeserializeObject<Transaction>(data);
    	if(!receivedTransaction.InsertTransaction(out message)){
    	return false;
    	}

    // Process the received transaction object as needed
    return true;
}

   public bool InsertTransaction(out string message)
    {
        try
        {
            using (var context = new AppDbContext()) // Replace YourDbContext with your actual DbContext class
            {
                this.TransactionsHash = HashTransaction();
                this.ValidationStatus = "Pending";
                this.DateTime = DateTime.Now;
                context.Transactions.Add(this);
                context.SaveChanges();
                message = "Success";
            }
        }
        catch (Exception ex)
        {
            message = "Error: {ex.Message}";
            Console.WriteLine(ex.Message);
            return false;          
        }
        return true;
    }
    
    public string HashTransaction()
    {
            // Concatenate the values of the properties
            var concatenatedString = $"{FromAddress}{ToAddress}{CryptoValue}{CashValue}{CryptocurrencyName}{DateTime}";

            // Hash the concatenated string (you can use any hash function you prefer)
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
    }
    
    public static bool HandleTransactionListRequest(string data, out string message)
	{
	    Wallet receivedWallet = JsonConvert.DeserializeObject<Wallet>(data);
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			var transactionList = context.Transactions.Where(t => t.FromAddress == receivedWallet.WalletAddress ||  t.ToAddress == receivedWallet.WalletAddress).ToList();
			message = JsonConvert.SerializeObject(transactionList);
		}
	    }
	    catch (Exception ex)
	    {
	    	Console.WriteLine(ex.Message);
		message = "Failure to send Server Assets";
		return false;
	    }
	    return true;
	}
	
	public static bool HandleFullTransactionListRequest(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			var transactionList = context.Transactions
                            .Where(t => t.TransactionId > 500)
                            .ToList();

			message = JsonConvert.SerializeObject(transactionList);
		}
	    }
	    catch (Exception ex)
	    {
	    	Console.WriteLine(ex.Message);
		message = "Failure to send Server Assets";
		return false;
	    }
	    return true;
	}
    
    
}
