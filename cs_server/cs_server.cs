using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using static User;
using static Transaction;
using static Cryptocurrency;
using static UserPortfolio;
using static UserOffer;
using static Miner;
using static MinerUtil;
using static Wallet;
using static Block;

class CSharpServer
{
    static void Main()
    {
        TcpListener server = null;

        try
        {
            int port = 8887;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = StartServer(localAddr, port);

	    while(true){
            	using (TcpClient client = AcceptClient(server))
            	using (NetworkStream stream = client.GetStream())
            	{
                	HandleClientCommunication(stream);
            	}	
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            server?.Stop();
        }

        Console.WriteLine("Server closing...");
    }

    static TcpListener StartServer(IPAddress localAddr, int port)
    {
        TcpListener server = new TcpListener(localAddr, port);
        server.Start();
        Console.WriteLine($"Server started on {localAddr}:{port}");
        return server;
    }

    static TcpClient AcceptClient(TcpListener server)
    {
        Console.WriteLine("Waiting for a connection...");
        return server.AcceptTcpClient();
    }

    static void HandleClientCommunication(NetworkStream stream)
{
    try
    {
        byte[] bytes = new byte[4194304];

        while (true)
        {
            int bytesRead = stream.Read(bytes, 0, bytes.Length);

            if (bytesRead == 0)
            {
                Console.WriteLine("Connection closed by client.");
                break;
            }

            string data = Encoding.ASCII.GetString(bytes, 0, bytesRead);
            Console.WriteLine($"Received from client: {data}");
            
            string message = "";

            switch (data)
	    {
    		case string _ when data.Contains("\"ObjectType\":\"User\"") && data.Contains("\"Purpose\":\"Register\""):
        		if (HandleRegisteredUser(data, out message))
        		{
            			SendMessageToClient(stream, message);
       	 		}	
        		else
        		{		
            			SendMessageToClient(stream, message);
        		}
        	break;
        	
        	case string _ when data.Contains("\"ObjectType\":\"User\"") && data.Contains("\"Purpose\":\"GetWallet\""):
        		if (HandleLoggedUser(data, out message))
        		{
            			SendMessageToClient(stream, message);
       	 		}	
        		else
        		{		
            			SendMessageToClient(stream, message);
        		}
        	break;

    		case string _ when data.Contains("\"ObjectType\":\"Transaction\"") && data.Contains("\"Purpose\":\"Publish\"") :
        		if (HandleReceivedTransaction(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("GetServerAssetsList") :
        		if (HandleServerAssetsRequest(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        	
        	case string _ when data.Contains("\"ObjectType\":\"User\"") && data.Contains("\"Purpose\":\"GetPortfolio\"") :
        		if (HandleUserPortfolioRequest(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("\"ObjectType\":\"Wallet\"") && data.Contains("\"Purpose\":\"GetTransactionList\"") :
        		if (HandleTransactionListRequest(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("\"ObjectType\":\"UserOffer\"") && data.Contains("\"Purpose\":\"Publish\"") :
        		if (HandleReceivedUserOffer(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("\"ObjectType\":\"UserOffer\"") && data.Contains("\"Purpose\":\"DeleteUserOffer\"") :
        		if (HandleDeletionUserOffer(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("GetUserOffers") :
        		if (HandleUserOffersRequest(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        	
        	case string _ when data.Contains("\"ObjectType\":\"User\"") && data.Contains("\"Purpose\":\"MinerCheck\""):
        		if (CheckMiningUser(data, out message))
        		{
            			SendMessageToClient(stream, message);
       	 		}	
        		else
        		{		
            			SendMessageToClient(stream, message);
        		}
        	break;
        		
        	case string _ when data.Contains("\"ObjectType\":\"User\"") && data.Contains("\"Purpose\":\"MinerRegister\""):
        		if (HandleMiningUser(data, out message))
        		{
            			SendMessageToClient(stream, message);
       	 		}	
        		else
        		{		
            			SendMessageToClient(stream, message);
        		}
        	break;
        	
        	case string _ when data.Contains("\"ObjectType\":\"User\"") && data.Contains("\"Purpose\":\"UpdateUser\""):
        		if (HandleUpdateUser(data, out message))
        		{
            			SendMessageToClient(stream, message);
       	 		}	
        		else
        		{		
            			SendMessageToClient(stream, message);
        		}
        	break;
        	
        	case string _ when data.Contains("\"ObjectType\":\"User\"") && data.Contains("\"Purpose\":\"DeleteUser\""):
        		if (HandleDeleteUser(data, out message))
        		{
            			SendMessageToClient(stream, message);
       	 		}	
        		else
        		{		
            			SendMessageToClient(stream, message);
        		}
        	break;
        	 
        	case string _ when data.Contains("TransactionValidationStatusForMiner") :
        		if (HandleValidationStatus(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("GetTransactionForValidation") :
        		if (HandleTransactionForValidation(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("\"ObjectType\":\"Wallet\"") && data.Contains("\"Purpose\":\"GetWallet\"") :
        		if (HandleWalletRequest(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("\"ObjectType\":\"Transaction\"") && (data.Contains("\"Purpose\":\"Valid\"") || data.Contains("\"Purpose\":\"NotValid\"")):
        		if (HandleValidationOfTransaction(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("\"ObjectType\":\"UserPortfolio\"") :
        		if (HandleUpdateUserPortfolio(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("\"ObjectType\":\"Wallet\"") && data.Contains("\"Purpose\":\"UpdateWallet\"") :
        		if (HandleWalletUpdate(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        	
        	case string _ when data.Contains("BlockValidationStatusForMiner") :
        		if (HandleBlockValidationStatus(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("GetTransactionsForBlockValidation") :
        		if (HandleBlockForValidation(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("\"ObjectType\":\"Block\"") && data.Contains("\"Purpose\":\"InsertBlock\"") :
        		if (HandleBlockInsertionRequest(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("GetBlockchain") :
        		if (HandleBlockchainRequest(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("GetAdminTable") :
        		if (HandleAdminTableRequest(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("GetMinerTable") :
        		if (HandleMinerTableRequest(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("GetWalletTable") :
        		if (HandleWalletTableRequest(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("\"ObjectType\":\"Crypto\"") && data.Contains("\"Purpose\":\"UpdateInServerAssets\"") :
        		if (HandleReceivedCryptocurrency(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        		
        	case string _ when data.Contains("GetFullTransactionList") :
        		if (HandleFullTransactionListRequest(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;
        	
        	case string _ when data.Contains("GetUserList") :
        		if (HandleFullUserListRequest(data, out message))
        		{
        	    		SendMessageToClient(stream, message);
        		}
        		else
        		{
            			SendMessageToClient(stream, message);
        		}
        		break;

    		default:
        		Console.WriteLine("Unknown object type or missing ObjectType property.");
        		break;
	     }

        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error in handling client communication: {e}");
    }
}

    static void SendMessageToClient(NetworkStream stream, string message)
    {
        byte[] messageData = Encoding.ASCII.GetBytes(message);
        stream.Write(messageData, 0, messageData.Length);
    }
}

