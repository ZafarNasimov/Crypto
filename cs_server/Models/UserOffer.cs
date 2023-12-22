using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("UserOffer")]
public class UserOffer
{
    [NotMapped]
    public string ObjectType => "UserOffer";
    [NotMapped]
    public string Purpose {get; set;}
    
    [Key]
    public int OfferId { get; set;  }
    
    public string FromAddress { get; set;  }
    public decimal CryptoValue { get; set;  }
    public decimal CashValue { get; set;  }
    public string CryptocurrencyName { get; set;  }
    public DateTime DateTime { get; set; }
    public bool Available { get; set; }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static UserOffer Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<UserOffer>(json);
    }
    
    public static bool HandleUserOffersRequest(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			var userOffers = context.UserOffers.ToList();
			message = JsonConvert.SerializeObject(userOffers);
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
    
    public bool InsertUserOffer(out string message)
    {
        try
        {
            using (var context = new AppDbContext()) 
            {
                this.Available = true;
                this.DateTime = DateTime.Now;
                context.UserOffers.Add(this);
                context.SaveChanges();
                message = "Success";
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;          
        }
        return true;
    }
    
        public static bool HandleDeletionUserOffer(string data, out string message)
	{
	    // Deserialize the Transaction object
	    UserOffer receivedUserOffer = JsonConvert.DeserializeObject<UserOffer>(data);
	    
	    
	    using (var context = new AppDbContext()) 
            {
            	    var userOfferToRemove = context.UserOffers.FirstOrDefault(uf => uf.OfferId == receivedUserOffer.OfferId);
		    if (userOfferToRemove != null)
			{
			    context.UserOffers.Remove(userOfferToRemove);
			    context.SaveChanges();
			    message = "Success";
			}
			else
			{
			    message = "No matching UserOffer found to remove.";
			}
	    }
	    message = "Success";
	    
	    return true;
	}
	
	public static bool HandleReceivedUserOffer(string data, out string message)
	{
	    // Deserialize the Transaction object
	    UserOffer receivedUserOffer = JsonConvert.DeserializeObject<UserOffer>(data);
	    	if(!receivedUserOffer.InsertUserOffer(out message)){
	    	return false;
	    	}
	    return true;
	}
}
