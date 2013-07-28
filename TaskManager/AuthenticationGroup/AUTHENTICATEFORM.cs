/*
 * Created by SharpDevelop.
 * User: Администратор
 * Date: 6/18/2013
 * Time: 12:05 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Net;

using System.Diagnostics;

namespace TaskManager
{
	/// <summary>
	/// Description of AUTHENTICATEFORM.
	/// </summary>
	public class AuthenticateForm : Form
	{
		public WebBrowser webBrowser;
		
		public AuthenticateForm(string authenticateUri)
		{
			this.Size = new Size(600,400);
			
			webBrowser = new WebBrowser();
			webBrowser.Size = new Size(this.Size.Width, this.Size.Height);
			webBrowser.Navigated += new WebBrowserNavigatedEventHandler(webBrowser1_Navigated);
			
			webBrowser.Navigate(authenticateUri);
			
			this.Controls.Add(webBrowser);
		}
		
		private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
//			System.Windows.Forms.HtmlElementCollection r = webBrowser.Document.All;
//			for(int i = 0; i < r.Count; i++)
//			{
//				Debug.WriteLine("r.Name = " + r[i].Name + "\n" +
//				                "r.InnerText = " + r[i].InnerText + "\n" +
//				                "r.OuterText = " + r[i].OuterText + "\n" +
//								"r.ID = " + r[i].Id);
//			}
//			
//			System.Windows.Forms.HtmlElementCollection b = webBrowser.Document.All.GetElementsByName("Passwd");
//			for(int i = 0; i < b.Count; i++)
//			{
//				Debug.WriteLine("b = " + b[i].Name);
//				b[i].InnerText = "14921988";
//				
//			}
//			
//			System.Windows.Forms.HtmlElementCollection n = webBrowser.Document.All.GetElementsByName("Email");
//			for(int i = 0; i < n.Count; i++)
//			{
//				Debug.WriteLine("b = " + n[i].Name);
//				n[i].InnerText = "tasktest2013";
//			}
		}
	}
	
	
}
