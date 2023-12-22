using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("Admin")]
public class Admin
{
	[Key]
	public int id {get; set; }

	public string Email { get; set; }
    	public string PasswordHash { get; set; }
}
