using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


[Table("Miners")]
public class Miner {
	[Key]
	public int UserId { get; set; }
	public DateTime JoinDate { get; set; }
	
     public bool InsertMiner(out string message)
    {
        try
        {
            using (var context = new AppDbContext()) // Replace YourDbContext with your actual DbContext class
            {   	
                this.JoinDate = DateTime.Now;
                context.Miners.Add(this);
                context.SaveChanges();
                message = "Success"; 
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException.Message);
            message = ex.Message;
            return false;          
        }
        return true;
    }
    
    public static bool HandleMiningUser(string data, out string message)
    {
    	// Deserialize the User object
    	User receivedUser = JsonConvert.DeserializeObject<User>(data);
    	Miner miner = new();
    	miner.UserId = receivedUser.UserId;
    	if(!miner.InsertMiner(out message)){
    	return false;
    	}
    	
	message = "Success";
    	return true;
    }
    
    public static bool CheckMiningUser(string data, out string message)
    {
    	// Deserialize the User object
    	User receivedUser = JsonConvert.DeserializeObject<User>(data);
    	
    	
    	using (var context = new AppDbContext())
            {   	
                Miner miner = context.Miners.Where(m => m.UserId == receivedUser.UserId).SingleOrDefault();
                
                if(miner is null){
                	message = "Fail";
                	return false; 
                }         
            }
    	
	message = "Success";
    	return true;
    }

}
