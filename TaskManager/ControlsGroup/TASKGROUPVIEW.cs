/*
 * Created by SharpDevelop.
 * User: Aleks
 * Date: 06.06.2013
 * Time: 2:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;


using System.Collections.Generic;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;


namespace TaskManager
{
	/// <summary>
	/// Description of TASKGROUPVIEW.
	/// </summary>
	public class TASKGROUPVIEW : ListBox
	{
		IList<TaskList> _data; 
			
		public TASKGROUPVIEW()
		{
			this.Location = new Point(5,60);
			this.Size = new Size(200,200);
		}
		
		public void reloadData(IList<TaskList> data)
		{
			_data = data;
			foreach(TaskList item in _data)
			{
				this.Items.Add(item.Title);
			}
		}
		
		public string getSelectedItemId()
		{
			if (this.SelectedItems.Count > 0)
			{
				return _data[this.SelectedIndex].Id;
			}
			else 
				return null;
		}
	}
}
