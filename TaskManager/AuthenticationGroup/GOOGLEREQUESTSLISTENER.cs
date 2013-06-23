/*
 * Created by SharpDevelop.
 * User: Aleks
 * Date: 07.06.2013
 * Time: 2:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Web;
using System.Threading;
using System.Text;

using System.Diagnostics;
	
namespace TaskManager
{
	/// <summary>
	/// Description of CUSTOMHTTPLISTENER.
	/// </summary>
	public class GOOGLEREQUESTSLISTENER
	{
		string _code;
		Thread _listenerThread;
		HttpListener _listener;
		GoogleRequestsListenerCodeReceivedDelegate _googleRequestsListenerCodeReceivedDlg;
		
		public GOOGLEREQUESTSLISTENER(string urlToListen, GoogleRequestsListenerCodeReceivedDelegate googleRequestsListenerCodeReceivedDlg)
		{
			 _googleRequestsListenerCodeReceivedDlg = googleRequestsListenerCodeReceivedDlg;
			
			_listener = new HttpListener();
			_listener.Prefixes.Add(urlToListen);
			
			_listenerThread = new Thread(_listenerThreadMain);
			_listenerThread.Start();
		}
		
		private void _listenerThreadMain()
		{
			_listener.Start();
			
			while(true)
			{
				// Note: The GetContext method blocks while waiting for a request. 
   				HttpListenerContext context = _listener.GetContext();
    			HttpListenerRequest request = context.Request;
   				
    			Debug.WriteLine(request.Url);
    			if (HttpUtility.ParseQueryString(request.Url.Query).Get("code") != null)
    			{
    				_code = HttpUtility.ParseQueryString(request.Url.Query).Get("code");
    				Debug.WriteLine(HttpUtility.ParseQueryString(request.Url.Query).Get("code"));
    				Debug.WriteLine(request.Url);
    				_googleRequestsListenerCodeReceivedDlg(_code);
    			}
    			else
    			{
    			
    			}
			}
		}
	
	
	}
}
