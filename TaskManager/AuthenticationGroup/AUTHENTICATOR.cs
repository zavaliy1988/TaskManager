/*
 * Created by SharpDevelop.
 * User: Aleks
 * Date: 07.06.2013
 * Time: 21:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Net;
using System.Web;
using System.Text;

using System.Web.UI;
using System.Web.UI.WebControls;

using System.Collections.Generic;
using System.Collections.Specialized;

using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using Google.Apis.Util;
using Google.Apis.Samples.Helper;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

using System.Diagnostics;

namespace TaskManager
{
	/// <summary>
	/// Description of AUTHENTICATOR.
	/// </summary>
	/// 
	public delegate void GoogleRequestsListenerCodeReceivedDelegate(string code);

	public class AUTHENTICATOR
	{
		string _clientId;
		string _clientSecret;
		string _redirectUri;
		string _state;
		string _loginHint;

		string _code;
		string _accessToken;
		string _tokenType;
		string _expiresIn;
		string _refreshToken;	

		GOOGLEREQUESTSLISTENER _googleRequestsListener;
		AUTHENTICATEFORM _authenticateForm;

		GoogleRequestsListenerCodeReceivedDelegate _googleRequestsListenerCodeReceivedDlg;


		public AUTHENTICATOR(string clientId, string clientSecret, string redirectUri, string state, string loginHint)
		{
			_clientId = clientId;
			_clientSecret = clientSecret;
			_redirectUri = redirectUri;
			_state = state;
			_loginHint = loginHint;

			_googleRequestsListenerCodeReceivedDlg = _googleRequestsListenerCodeReceived;
			_googleRequestsListener = new GOOGLEREQUESTSLISTENER(_redirectUri, _googleRequestsListenerCodeReceivedDlg);

		}

		public string getAuthenticateUrl()
		{
			string authenticateUrl = 
			"https://accounts.google.com/o/oauth2/auth?" +
			"client_id=" +_clientId + "&" + 
			"response_type=code&" +
			"scope=" + "https://www.googleapis.com/auth/tasks" + "&" +
			"redirect_uri=" +_redirectUri + "&" + 
			"state=" + _state + "&" + 
			"login_hint=" + _loginHint;

			return authenticateUrl;
		}

		public TasksService getTasksService()
		{
			if (_accessToken == null) 
			{
				AuthenticateApplication();
				return null;
			}
			else
				try
				{
					var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description)
                	{
                	    ClientIdentifier = _clientId,
                	    ClientSecret = _clientSecret
                	};
					var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, _createAuthorizationState);
					var service = new TasksService(new BaseClientService.Initializer() { Authenticator = auth });
					return service;				
				}
				catch (Exception ex)
				{
					Debug.WriteLine("exception in AUTHENTICATOR.getTaskService" + ex);
					return null;
				}
		}

		private void AuthenticateApplication()
		{
			string authenticateURL = getAuthenticateUrl();
			_authenticateForm = new AUTHENTICATEFORM(authenticateURL);
			_authenticateForm.Show();
			
			//HtmlDocument d = _authenticateForm.webBrowser.Document;
			
			Debug.WriteLine("d = " + _authenticateForm.webBrowser.DocumentText);
			//System.Windows.Forms.control c = _authenticateForm.webBrowser.Controls;
			//for(int i = 0; i < c.Count; i++)
			//{
			//	Debug.WriteLine("c = " + c[i].ID);
			//}
			//System.Windows.Forms.HtmlElementCollection r = _authenticateForm.webBrowser.Document.All;
		}

		private void _googleRequestsListenerCodeReceived(string code)
		{
			if (_authenticateForm.InvokeRequired)
			{
			//	HtmlElementCollection d = _authenticateForm.webBrowser.Document.All;
			//	Debug.WriteLine("d = " + d.Count);
				//Debug.WriteLine("doc title = " + d.Title);
				//Debug.WriteLine("doc body = " + d.Body);
				
				
				_authenticateForm.Invoke(new MethodInvoker(delegate
				                                           {
				                                           	_authenticateForm.Hide();
				                                           	//_authenticateForm.Dispose();
				                                           	}));
				_code = code;
				_requestTokensBegin(_code);
			}
		}

		private void _requestTokensBegin(string code)
		{
			using (var webClient = new WebClient())
			{
				var data = new NameValueCollection();
    			data["code"] = _code;
    			data["client_id"] = _clientId;
    			data["client_secret"] = _clientSecret;
    			data["redirect_uri"] = _redirectUri;
    			data["grant_type"]="authorization_code";

				webClient.UploadValuesAsync(new Uri("https://accounts.google.com/o/oauth2/token"), "POST", data);
				webClient.UploadValuesCompleted += new UploadValuesCompletedEventHandler(_receiveTokensCompleted);
			}
		}

		private void _receiveTokensCompleted(object sender, UploadValuesCompletedEventArgs e)
		{
			JToken requstTokensResult = JObject.Parse(Encoding.ASCII.GetString(e.Result));
			_accessToken = requstTokensResult.SelectToken("access_token").ToString();
			_tokenType = requstTokensResult.SelectToken("token_type").ToString();
			_expiresIn = requstTokensResult.SelectToken("expires_in").ToString();
			_refreshToken = requstTokensResult.SelectToken("refresh_token").ToString();

			Debug.WriteLine("Access_token = " + _accessToken);

			var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description)
                {
                    ClientIdentifier = _clientId,
                    ClientSecret = _clientSecret
                };
			var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, _createAuthorizationState);
			var service = new TasksService(new BaseClientService.Initializer() { Authenticator = auth });		
		}

		private IAuthorizationState _createAuthorizationState(NativeApplicationClient arg)
  		{
      		IAuthorizationState state = new AuthorizationState(new[] { TasksService.Scopes.Tasks.GetStringValue() });
      		state.Callback = new Uri(_redirectUri);
     		state.AccessToken = _accessToken;
	   		return state;
  		}


	}
}