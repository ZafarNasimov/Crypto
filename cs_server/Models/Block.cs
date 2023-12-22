using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

[Table("Block")]
public class Block
{
    [NotMapped]
    public string ObjectType => "Block";
    
    [NotMapped]
    public string Purpose {get; set;}
    
    [NotMapped]
    public List<Transaction> BlockTransactions = new();

    [Key]
    public int BlockNumber { get; set; }

    public string RootHash { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime Timestamp { get; set; }

    public int TotalTransactions { get; set; }


    public string? PreviousHash { get; set; }

    public string Hash { get; set; }
    
    public static bool HandleBlockInsertionRequest(string data, out string message)
	{
    	// Deserialize the Transaction object
    	Block receivedBlock = JsonConvert.DeserializeObject<Block>(data);
    	if(!receivedBlock.InsertBlock(out message))
    	{
    		return false;
    	}

    	// Process the received transaction object as needed
    	return true;
	}

   public bool InsertBlock(out string message)
    {
        try
        {
            using (var context = new AppDbContext()) // Replace YourDbContext with your actual DbContext class
            {
                context.Blocks.Add(this);
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
    
    public static bool HandleBlockchainRequest(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			var blockchain = context.Blocks.ToList();
			message = JsonConvert.SerializeObject(blockchain);
		}
	    }
	    catch (Exception ex)
	    {
	    	Console.WriteLine(ex.Message);
		message = "Failure to send Blockchain";
		return false;
	    }
	    return true;
	}
	
	public static bool HandleAdminTableRequest(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			var admins = context.Admins.ToList();
			message = JsonConvert.SerializeObject(admins);
		}
	    }
	    catch (Exception ex)
	    {
	    	Console.WriteLine(ex.Message);
		message = "Failure to send Blockchain";
		return false;
	    }
	    return true;
	}
	
	public static bool HandleMinerTableRequest(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			var miners = context.Miners.ToList();
			message = JsonConvert.SerializeObject(miners);
		}
	    }
	    catch (Exception ex)
	    {
	    	Console.WriteLine(ex.Message);
		message = "Failure to send Blockchain";
		return false;
	    }
	    return true;
	}
	
	public static bool HandleUserPortfolioTableRequest(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			var uPort = context.AccPortfolio.ToList();
			message = JsonConvert.SerializeObject(uPort);
		}
	    }
	    catch (Exception ex)
	    {
	    	Console.WriteLine(ex.Message);
		message = "Failure to send Blockchain";
		return false;
	    }
	    return true;
	}
	
	public static bool HandleWalletTableRequest(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			var wallets = context.Wallets.ToList();
			message = JsonConvert.SerializeObject(wallets);
		}
	    }
	    catch (Exception ex)
	    {
	    	Console.WriteLine(ex.Message);
		message = "Failure to send Blockchain";
		return false;
	    }
	    return true;
	}
}

