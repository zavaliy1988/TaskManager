/*
 * Created by SharpDevelop.
 * User: Aleks
 * Date: 08.06.2013
 * Time: 0:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Web;

namespace TaskManager
{
	/// <summary>
	/// Description of AUTHENTICATEFORM.
	/// </summary>
	public class AUTHENTICATEFORM : Form
	{
		WebBrowser _browser;
		
		public AUTHENTICATEFORM(string urlToShow)
		{
			this.Text = "Authenticate";
			this.Size = new Size(600,400);
			this.MinimumSize = new Size(500,370);
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			
			_browser = new WebBrowser();
			_browser.Size = new Size(this.Size.Width-8, this.Size.Height);
			_browser.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			_browser.Navigate(urlToShow);
			
			this.Controls.Add(_browser);
			
		}
		
		public void Navigate(string urlToShow)
		{
			_browser.Navigate(urlToShow);
		}
	}
}
