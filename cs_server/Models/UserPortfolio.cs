using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;

[Table("UserPortfolio")]
public class UserPortfolio
{
    [NotMapped]
    public string ObjectType => "UserPortfolio";

    [Key]
    public int UserId { get; set; }
    
    public decimal Bitcoin { get; set; }
    public decimal Ethereum { get; set; }
    public decimal Ripple { get; set; }
    public decimal Litecoin { get; set; }
    public decimal Cardano { get; set; }
    public decimal Polkadot { get; set; }
    public decimal BinanceCoin { get; set; }
    public decimal Chainlink { get; set; }
    public decimal Stellar { get; set; }
    public decimal BitcoinCash { get; set; }
    public decimal Dogecoin { get; set; }
    public decimal USD_Coin { get; set; }
    public decimal Aave { get; set; }
    public decimal Cosmos { get; set; }
    public decimal Monero { get; set; }
    public decimal Neo { get; set; }
    public decimal Tezos { get; set; }
    public decimal Maker { get; set; }
    public decimal EOS { get; set; }
    public decimal TRON { get; set; }
    public decimal VeChain { get; set; }
    public decimal Solana { get; set; }
    public decimal Theta { get; set; }
    public decimal Dash { get; set; }
    public decimal Uniswap { get; set; }
    public decimal Compound { get; set; }
    
    public static bool HandleUserPortfolioRequest(string data, out string message)
	{ 
            User user = JsonConvert.DeserializeObject<User>(data);
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			var userPortfolio = context.AccPortfolio.FirstOrDefault(p => p.UserId == user.UserId);
			message = JsonConvert.SerializeObject(userPortfolio);
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
	
	public static bool HandleUserPortfolioListRequest(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			var userPortList = context.AccPortfolio.ToList();
			message = JsonConvert.SerializeObject(userPortList);
		}
	    }
	    catch (Exception ex)
	    {
	    	Console.WriteLine(ex.Message);
		message = "Failure to send Wallet List";
		return false;
	    }
	    return true;
	}
	
	public static bool HandleUpdateUserPortfolio(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			UserPortfolio user = JsonConvert.DeserializeObject<UserPortfolio>(data);
			var userPort = context.AccPortfolio.FirstOrDefault(p => p.UserId == user.UserId);
			
			if(userPort is not null)
			{
				CopyProperties(user, userPort);
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
		message = "Failure";
		return false;
	    }
	    return true;
	}
	
	private static void CopyProperties(object source, object destination)
	{
	    var sourceType = source.GetType();
	    var destinationType = destination.GetType();

	    // Get all properties from the source type
	    var sourceProperties = sourceType.GetProperties();

	    foreach (var sourceProperty in sourceProperties)
	    {
		// Find the corresponding property in the destination type
		var destinationProperty = destinationType.GetProperty(sourceProperty.Name);

		// Check if the property exists in both types
		if (destinationProperty != null && destinationProperty.CanWrite)
		{
		    // Get the value from the source property
		    var value = sourceProperty.GetValue(source, null);

		    // Set the value to the destination property
		    destinationProperty.SetValue(destination, value, null);
		}
	    }
	}
 
}
