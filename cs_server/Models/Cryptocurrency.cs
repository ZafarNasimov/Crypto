using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;


[Table("ServerAssets")]
public class Cryptocurrency
{
	[NotMapped]
    	public string ObjectType => "Crypto";
	[NotMapped]
	public string Purpose { get; set; }

	[Key]
	public string Name { get; set; }
	public decimal Price { get; set; }
	public decimal Fee { get; set; }
	public decimal Amount { get; set; }
	
	public string Serialize()
	{
		return JsonConvert.SerializeObject(this);
	}

	public static Cryptocurrency Deserialize(string json)
	{
		return JsonConvert.DeserializeObject<Cryptocurrency>(json);
	}
	
	public static bool HandleServerAssetsRequest(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			var serverAssets = context.ServerAssets.ToList();
			message = JsonConvert.SerializeObject(serverAssets);
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
	
	public static bool HandleReceivedCryptocurrency(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{	
			Cryptocurrency c = Deserialize(data);
			var serverAssets = context.ServerAssets.ToList();
			foreach(Cryptocurrency var in serverAssets)
			{
				if(c.Name == var.Name)
				{
					var.Fee = c.Fee;
					var.Price = c.Price;
					var.Amount = c.Amount;
				}
			}
			context.SaveChanges();
			message = "Success";
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
}
