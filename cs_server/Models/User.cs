using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using static Authentication; 

[Table("Users")]
public class User
{
    [NotMapped]
    public string ObjectType => "User";
    [NotMapped]
    public string Purpose {get; set;}
    
    [Key]
    public int UserId {get; set;}
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Nationality { get; set; }
    public DateTime JoinDate { get; set; }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
    
    public static User Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<User>(json);
    }
    
    public bool InsertUser(out string message)
    {
        try
        {
            using (var context = new AppDbContext()) // Replace YourDbContext with your actual DbContext class
            {
            	string emailVar = context.Users
                	.Where(user => user.Email == this.Email)
                	.Select(user => user.Email)
                	.FirstOrDefault();
                	
                if(emailVar==null){
                this.PasswordHash = HashPassword(this.PasswordHash);
                this.JoinDate = DateTime.Now;
                context.Users.Add(this);
                Console.WriteLine("User added!");
                context.SaveChanges();
                message = "Success";
                }
                else{
                message = "Email is already used !";
                return false;
                }
            
                
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;          
        }
        return true;
    }
    
    public static bool HandleRegisteredUser(string data, out string message)
    {
    	// Deserialize the User object
    	User receivedUser = JsonConvert.DeserializeObject<User>(data);
    	if(!receivedUser.InsertUser(out message)){
    	return false;
    	}
    	
    	Wallet wallet = new();
    	WalletUtil w = new();
    	wallet.WalletAddress = w.GenerateAddress();
    	wallet.PrivateKey = w.GetPrivateKey();
    	wallet.PublicKey = w.GetPublicKey(); 	
    	wallet.UserId = GetUserIdFromEmail(receivedUser.Email);
    	if(!wallet.InsertWallet(out message)){
    		return false;
    	}
	message = "Success";
    	return true;
    }
    
    public static bool HandleUpdateUser(string data, out string message)
	{
	    // Deserialize the User object
	    User receivedUser = JsonConvert.DeserializeObject<User>(data);

	    try
	    {
		using (var context = new AppDbContext())
		{
		    // Find the user to update
		    var updateUser = context.Users
		        .Where(user => user.UserId == receivedUser.UserId)
		        .FirstOrDefault();

		    if (updateUser != null)
		    {
		        // Update the properties with new values
		        if(updateUser.Email != receivedUser.Email)
		        {
		        	updateUser.Email = receivedUser.Email;
		        }
		        else
		        {
		        	
		        	updateUser.PasswordHash = HashPassword(receivedUser.PasswordHash);
		        	Console.WriteLine(updateUser.PasswordHash);
		        }
		        
		        

		        // Save changes to the database
		        context.SaveChanges();

		        message = "Success";
		    }
		    else
		    {
		        message = "User not found for updating.";
		    }
		}
	    }
	    catch (Exception ex)
	    {
		message = $"Failure: {ex.Message}";
		Console.WriteLine(ex);
		return false;
	    }

	    return true;
	}

public static bool HandleDeleteUser(string data, out string message)
	{
	    // Deserialize the Transaction object
	    User receivedUser = JsonConvert.DeserializeObject<User>(data);
	    
	    
	    using (var context = new AppDbContext()) 
            {
            	    var user = context.Users.FirstOrDefault(uf => uf.UserId == receivedUser.UserId);
            	    var userPort = context.AccPortfolio.FirstOrDefault(up => up.UserId == receivedUser.UserId);
            	    var wallet = context.Wallets.FirstOrDefault(w => w.UserId == receivedUser.UserId);
            	    
            	    
		    if (user != null)
			{
			    context.AccPortfolio.Remove(userPort);
			    context.SaveChanges();
			    context.Wallets.Remove(wallet);
			    context.SaveChanges();
			    context.Users.Remove(user);
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

    
    public static bool HandleFullUserListRequest(string data, out string message)
	{
	    try
	    {
	    	using (var context = new AppDbContext())
		{
			var users = context.Users.ToList();
			message = JsonConvert.SerializeObject(users);
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
    
    public static bool HandleLoggedUser(string data, out string message)
    {
    	// Deserialize the User object
    	User receivedUser = JsonConvert.DeserializeObject<User>(data);
    	
    	using (var context = new AppDbContext()) // Replace YourDbContext with your actual DbContext class
        {
    	User user = context.Users.FirstOrDefault(u => u.Email == receivedUser.Email);
    	Admin admin = context.Admins.FirstOrDefault(u => u.Email == receivedUser.Email);
    	
    	if(admin is not null && VerifyPassword(receivedUser.PasswordHash, admin.PasswordHash))
    	{
    		message = "Admin approved";
    		return true;
    	}
    	
        
        if(user is not null && VerifyPassword(receivedUser.PasswordHash, user.PasswordHash)){
        	Wallet wallet = context.Wallets
            .FirstOrDefault(w => w.UserId == user.UserId);
        	message = wallet.Serialize();
        	return true;
        }
        message = "Password or Email not verified ! ";
        return false;
    	}
    }
    
    public static int GetUserIdFromEmail(string userEmail)
    {
        try
        {
            using (var context = new AppDbContext()) // Replace YourDbContext with your actual DbContext class
            {
            	int userId = context.Users
                	.Where(user => user.Email == userEmail)
                	.Select(user => user.UserId)
                	.FirstOrDefault();

            if (userId != 0)
            {
                return userId;
            }
            else
            {
                // Handle the case where the email is not associated with any user
                Console.WriteLine("User with the given email not found.");
                return -1; // Or some other value indicating that the email is not found
            }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"Error: {ex.Message}");
            return -1; // Or some other value indicating an error
        }
    }
}
