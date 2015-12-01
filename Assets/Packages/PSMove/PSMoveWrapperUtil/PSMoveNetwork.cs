using System.Net;
using System.Threading;
using System;

namespace PSMoveSharp {

	public static class PSMoveNetwork {
		
		public static PSMoveClientThreadedRead moveClient {get; private set;}
		
		
		public static bool client_connect(string server_address, int server_port)
	    {
	        moveClient = new PSMoveClientThreadedRead();
	
	        try
	        {
	            moveClient.Connect(Dns.GetHostAddresses(server_address)[0].ToString(), server_port);
	            moveClient.StartThread();
	        }
	        catch
	        {
	            
	            return false;
	        }
			
			
			return true;
	
	    }
		
		public static void client_disconnect()
	    {
	        try
	        {
	
	            moveClient.StopThread();
	            moveClient.Close();
	        }
	        catch
	        {
	            return;
	        }
			
	    }
		
	}
	
}