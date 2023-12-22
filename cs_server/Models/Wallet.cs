using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;

[Table("Wallets")]
public class Wallet 
{
    [NotMapped]
    public string ObjectType => "Wallet";
    [NotMapped]
    public string Purpose {get; set;}
    
    [Key]
    public int WalletId { get; set; }

    public string WalletAddress { get; set; }

    public int UserId { get; set; }

    public decimal Balance { get; set; }
    
    public string PrivateKey { get; set; }
    
    public string PublicKey { get; set; }
    
    public bool InsertWallet(out string message)
{
    try
    {
        using (var context = new AppDbContext()) // Replace YourDbContext with your actual DbContext class
        {
            Console.WriteLine("Adding wallet!");
            context.Wallets.Add(this);
            Console.WriteLine("Wallet added!");
            context.SaveChanges();
            message = "Success";
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);

        // Print inner exception details
        if (ex.InnerException != null)
        {
            Console.WriteLine("Inner Exception:");
            Console.WriteLine(ex.InnerException.Message);
            Console.WriteLine(ex.InnerException.StackTrace);
            // Add more properties if needed
        }

        message = ex.Message;
        return false;
    }

    return true;
}

    
    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
    
    public static Wallet Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<Wallet>(json);
    }
    
    public static bool HandleWalletRequest(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			Wallet w = Deserialize(data);
			Wallet wallet = context.Wallets.Where(wal => wal.WalletAddress == w.WalletAddress).FirstOrDefault();
			message = wallet.Serialize();
		}
	    }
	    catch (Exception ex)
	    {
	    	Console.WriteLine(ex.Message);
		message = "Failure. Wallet address does not exist !";
		return false;
	    }
	    return true;
	}
	
	public static bool HandleWalletUpdate(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			Wallet w = Deserialize(data);
			Wallet wallet = context.Wallets.Where(wal => wal.WalletAddress == w.WalletAddress).FirstOrDefault();
			if(wallet is not null)
			{
				wallet.Balance = w.Balance;
				context.SaveChanges();
				message = "Success";
			}
			else
			{
				message = "Failure";
				return false;
			}
		}
	    }
	    catch (Exception ex)
	    {
	    	Console.WriteLine(ex.Message);
		message = "Failure. Wallet address does not exist !";
		return false;
	    }
	    return true;
	}
}

