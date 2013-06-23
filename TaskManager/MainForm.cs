/*
 * Created by SharpDevelop.
 * User: Aleks
 * Date: 06.06.2013
 * Time: 2:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Net;

using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;

using System.Diagnostics;
using System.Linq;


namespace TaskManager
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		TASKGROUPVIEW _taskGroupView;
		TASKSVIEW _tasksView;
		Dictionary<string,CUSTOMTOOLBARBUTTON> _toolbarButtonsDict;
		
		AUTHENTICATOR _authenticator;
		
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			_taskGroupView = new TASKGROUPVIEW();
			_taskGroupView.Click += new EventHandler(onTaskGroupViewClick);
			
			_tasksView = new TASKSVIEW();
		
			_toolbarButtonsDict = new Dictionary<string, CUSTOMTOOLBARBUTTON>();
			
			this.Controls.Add(_taskGroupView);
			this.Controls.Add(_tasksView);
	
			for(int i = 0; i < 5; i++)
			{
				CUSTOMTOOLBARBUTTON toolbarButton = new CUSTOMTOOLBARBUTTON();
				toolbarButton.BackgroundImageLayout = ImageLayout.Center;
				toolbarButton.Location = new Point(5 + i * toolbarButton.Size.Width + i * 5, 5);
				this.Controls.Add(toolbarButton);
				
				if (i == 0) _toolbarButtonsDict.Add("addtask",toolbarButton);
				if (i == 1) _toolbarButtonsDict.Add("edittask",toolbarButton);
				if (i == 2) _toolbarButtonsDict.Add("removetask",toolbarButton);
				if (i == 3) _toolbarButtonsDict.Add("addtask1",toolbarButton);
				if (i == 4) _toolbarButtonsDict.Add("synchronize",toolbarButton);
			}
			
			Debug.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
			
			_toolbarButtonsDict["addtask"].BackgroundImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\icons\\plus-icon.png");
			_toolbarButtonsDict["addtask"].Click += new EventHandler(onAddTaskButtonClick);
			
			_toolbarButtonsDict["synchronize"].BackgroundImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\icons\\synchronize.png");
			_toolbarButtonsDict["synchronize"].Click += new EventHandler(onSynchronizeButtonClick);
						
			_authenticator = new AUTHENTICATOR("167957637234.apps.googleusercontent.com",
			                                  "rNm8WIonWEdVJKzWf3N76RaW",
			                                  "http://localhost:54321/",
			                                  "anystring",
			                                  "");
		}
		
		void onAddTaskButtonClick(object sender, EventArgs e)
		{

		}
		
		void onSynchronizeButtonClick(object sender, EventArgs e)
		{
			TasksService service = _authenticator.getTasksService();
			
			if (service != null)
			{
				_taskGroupView.Items.Clear();
				_taskGroupView.reloadData(service.Tasklists.List().Fetch().Items);
			}
		}
		
		void onTaskGroupViewClick(object sender, EventArgs e)
		{
			Debug.WriteLine("ListBox clicked");
			if (_taskGroupView.Items.Count > 0)
			{
				string selectedTaskTitle = (string)_taskGroupView.Items[_taskGroupView.SelectedIndex];
				string selectedTaskId = _taskGroupView.getSelectedItemId();
				Debug.WriteLine("selected id = " + selectedTaskId);
			}
		}
		
		
		
	}
}
