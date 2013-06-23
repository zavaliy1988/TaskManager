/*
 * Created by SharpDevelop.
 * User: Aleks
 * Date: 06.06.2013
 * Time: 2:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;

namespace TaskManager
{
	/// <summary>
	/// Description of TASKSVIEW.
	/// </summary>
	public class TASKSVIEW : ListBox
	{
		public TASKSVIEW()
		{
			this.Location = new Point(300,60);
			this.Size = new Size(200,200);
		}
	}
}
