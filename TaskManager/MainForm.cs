﻿/*
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

using System.Text;

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
		CustomTreeView _customTreeView;
		
		TextBox _taskTitleTextBox;
		TextBox _taskNotesTextBox;
		DateTimeLabel _taskUpdatedDateLabel;
		DateTimeLabel _taskDueDateLabel;
		DateTimeLabel _taskCurrentDateLabel;
		Timer _timer;
		
		CompletedLabel _taskCompletedLabel;
		Button _taskSaveChangesButton;
		
		Dictionary<string,CustomToolbarButton> _toolbarButtonsDict;
		
		
		Authenticator _authenticator;
		
		DBManager _dbManager;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			_customTreeView = new CustomTreeView();
			_customTreeView.Location = new Point(5,60);
			_customTreeView.Size = new Size(230,440);
			_customTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(_onCustomTreeNodeClick);
			_customTreeView.NodeDrop += new CustomTreeView.NodeDropHandler(_onCustomTreeNodeDrop);
			_customTreeView.AfterSelect += new TreeViewEventHandler(_onCustomTreeViewAfterSelect);
			_customTreeView.KeyUp += new KeyEventHandler(_onCustomTreeViewKeyUp);
			this.Controls.Add(_customTreeView);
			
			_taskTitleTextBox = new TextBox();
			_taskTitleTextBox.Location = new Point(243,60);
			_taskTitleTextBox.Size = new Size(240,40);
			_taskTitleTextBox.BackColor = Color.White;
			_taskTitleTextBox.TextAlign = HorizontalAlignment.Center;
			_taskTitleTextBox.Font = new Font("Arial",12);
			this.Controls.Add(_taskTitleTextBox);
			
			_taskNotesTextBox = new TextBox();
			_taskNotesTextBox.Location = new Point(243,95);
			_taskNotesTextBox.Size = new Size(240,300);
			_taskNotesTextBox.BackColor = Color.White;
			_taskNotesTextBox.Multiline = true;
			_taskNotesTextBox.Font = new Font("Arial",12);
			this.Controls.Add(_taskNotesTextBox);
			
			_taskUpdatedDateLabel = new DateTimeLabel("Updated:");
			_taskUpdatedDateLabel.Location = new Point(243,405);
			_taskUpdatedDateLabel.Enabled = false;
			this.Controls.Add(_taskUpdatedDateLabel);
			
			_taskDueDateLabel = new DateTimeLabel("Due:");
			_taskDueDateLabel.Location = new Point(243,440);
			this.Controls.Add(_taskDueDateLabel);

			_taskCurrentDateLabel = new DateTimeLabel("Now:");
			_taskCurrentDateLabel.Location = new Point(243,475);
			_taskCurrentDateLabel.Enabled = false;
			this.Controls.Add(_taskCurrentDateLabel);
			
			_timer = new Timer();
			_timer.Interval = 1000;
			_timer.Tick += new EventHandler(_onTimerTick);
			_timer.Start();
			
			_taskCompletedLabel = new CompletedLabel("Completed:");
			_taskCompletedLabel.Location = new Point(243,505);
			this.Controls.Add(_taskCompletedLabel);
			
			_taskSaveChangesButton = new Button();
			_taskSaveChangesButton.Font = new Font("Arial", 14);
			_taskSaveChangesButton.Text = "Save Task Changes";		
			_taskSaveChangesButton.Location = new Point(243,540);
			_taskSaveChangesButton.Size = new Size(240,35);
			_taskSaveChangesButton.Click += new EventHandler(_onSaveTaskChangesButtonClick);
			this.Controls.Add(_taskSaveChangesButton);
			
			_enableTaskControls(false);
				
			_toolbarButtonsDict = new Dictionary<string,CustomToolbarButton>();
			
			
			for(int i = 0; i < 5; i++)
			{
				CustomToolbarButton toolbarButton = new CustomToolbarButton();
				toolbarButton.BackgroundImageLayout = ImageLayout.Center;
				toolbarButton.Location = new Point(5 + i * toolbarButton.Size.Width + i * 5, 5);
				this.Controls.Add(toolbarButton);
				
				if (i == 0) _toolbarButtonsDict.Add("addtasklist",toolbarButton);
				if (i == 1) _toolbarButtonsDict.Add("addtask",toolbarButton);
				if (i == 2) _toolbarButtonsDict.Add("removetask",toolbarButton);
				if (i == 3) _toolbarButtonsDict.Add("addtask1",toolbarButton);
				if (i == 4) _toolbarButtonsDict.Add("synchronize",toolbarButton);
			}
			
			Debug.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
			
			_toolbarButtonsDict["addtasklist"].BackgroundImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\icons\\addtasklist.jpeg");
			_toolbarButtonsDict["addtasklist"].Click += new EventHandler(_onAddTaskListButtonClick);
			
			_toolbarButtonsDict["addtask"].BackgroundImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\icons\\addtask.png");
			_toolbarButtonsDict["addtask"].Click += new EventHandler(_onAddTaskButtonClick);
			
			_toolbarButtonsDict["synchronize"].BackgroundImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\icons\\synchronize.png");
			_toolbarButtonsDict["synchronize"].Click += new EventHandler(_onSynchronizeButtonClick);
			
			_toolbarButtonsDict["addtask1"].Click += new EventHandler(_onDBButtonClick);
			
			_authenticator = new Authenticator("167957637234.apps.googleusercontent.com",
			                                  "rNm8WIonWEdVJKzWf3N76RaW",
			                                  "http://localhost:54321/",
			                                  "anystring",
			                                  "");
			
			//Create DB
			_dbManager = new DBManager("C:\\dbtaskmanager2.mdf");
			if (!_dbManager.DatabaseExists())
			{
				_dbManager.CreateDatabase();
			}
		}
		
		
		
		//TaskControls Enabling/Disabling Section
		private void _enableTaskListControls(bool val)
		{
			if (val == true)
			{
				_taskTitleTextBox.Enabled = true;
				_taskNotesTextBox.Enabled = false;
				_taskDueDateLabel.Enabled = false;
				_taskCompletedLabel.Enabled = false;
				_taskSaveChangesButton.Enabled = true;
			}
			else
			{
				_taskTitleTextBox.Enabled = false;
				_taskNotesTextBox.Enabled = false;
				_taskDueDateLabel.Enabled = false;
				_taskCompletedLabel.Enabled = false;
				_taskSaveChangesButton.Enabled = false;
			}
		}
				
		private void _enableTaskControls(bool val)
		{
			if (val == true)
			{
				_taskTitleTextBox.Enabled = true;
				_taskNotesTextBox.Enabled = true;
				_taskDueDateLabel.Enabled = true;
				_taskCompletedLabel.Enabled = true;
				_taskSaveChangesButton.Enabled = true;
			}
			else
			{
				_taskTitleTextBox.Enabled = false;
				_taskNotesTextBox.Enabled = false;
				_taskDueDateLabel.Enabled = false;
				_taskCompletedLabel.Enabled = false;
				_taskSaveChangesButton.Enabled = false;
			}
		}
		
		
		//CustomTreeView Section
		private void reloadCustomTreeViewData()
		{
			IQueryable<DBTaskList> selectedDBTaskLists = from selectedDBTaskList in _dbManager.tasklists select selectedDBTaskList;
			IQueryable<DBTask> selectedDBTasks = from selectedDBTask in _dbManager.tasks select selectedDBTask;
			_customTreeView.reloadData(selectedDBTaskLists, selectedDBTasks);
		}
		
		private void _onCustomTreeNodeClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			TreeNodeWithId clickedNode = (TreeNodeWithId)e.Node;
			if (e.Button == MouseButtons.Left)
			{
				_reloadTaskControlsData(clickedNode);
			}
		}
		
		private void _onCustomTreeNodeDrop(object sender, CustomTreeViewDropEventArgs e)
		{
			IQueryable<DBTask> selectedDBTasks = from selectedDBTask in _dbManager.tasks where selectedDBTask.id.Equals(e.nodeId) select selectedDBTask;
			foreach(DBTask dbTask in selectedDBTasks)
			{
				dbTask.taskListId = e.parentNodeId;
			}			
			_dbManager.SubmitChanges();
		}
		
		private void _onCustomTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNodeWithId clickedNode = (TreeNodeWithId)e.Node;
			_reloadTaskControlsData(clickedNode);
		}
		
		private void _reloadTaskControlsData(TreeNodeWithId clickedNode)
		{
				if (_customTreeView.isTaskNode(clickedNode))
				{
					_enableTaskControls(true);
					
					IQueryable<DBTask> selectedDBTasks = from selectedDBTask in _dbManager.tasks where selectedDBTask.id.Equals(clickedNode.id) select selectedDBTask;
					foreach(DBTask dbTask in selectedDBTasks)
					{
						_taskTitleTextBox.Text = dbTask.title;
						_taskNotesTextBox.Text = dbTask.notes;
						_taskUpdatedDateLabel.setDate(dbTask.updated);
						_taskDueDateLabel.setDate(dbTask.due);
					}
				}
				else
					_enableTaskControls(false);
				
				if (_customTreeView.isTaskListNode(clickedNode))
				{
					_enableTaskListControls(true);
					
					IQueryable<DBTaskList> selectedDBTaskLists = from selectedDBTaskList in _dbManager.tasklists where selectedDBTaskList.id.Equals(clickedNode.id) select selectedDBTaskList;
					foreach(DBTaskList dbTaskList in selectedDBTaskLists)
					{
						_taskTitleTextBox.Text = dbTaskList.title;
						_taskNotesTextBox.Text = "";
						_taskUpdatedDateLabel.setDate(dbTaskList.updated);
						_taskDueDateLabel.setDate(DateTime.MaxValue);
					}
				}
		}
		
		private void _onCustomTreeViewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				TreeNodeWithId selectedNodeInTreeView = (TreeNodeWithId)_customTreeView.SelectedNode;	
				
				if (_customTreeView.isTaskListNode(selectedNodeInTreeView))
				{
					IQueryable<DBTask> dbTasksToDelete = from selectedDBTask in _dbManager.tasks where selectedDBTask.taskListId.Equals(selectedNodeInTreeView.id) select selectedDBTask;
					foreach(DBTask dbTask in dbTasksToDelete)
					{
						_dbManager.tasks.DeleteOnSubmit(dbTask);
					
						//DELETE MUST BE CALLED WHEN "SYNCHRONIZE" BUTTON PRESSED
						//service.Tasks.Delete(dbTask.taskListId, dbTask.id).Fetch();
						Debug.WriteLine("delete request started");
					}
					_dbManager.SubmitChanges();
					
					IQueryable<DBTaskList> dbTaskListsToDelete = from selectedDBTaskList in _dbManager.tasklists where selectedDBTaskList.id.Equals(selectedNodeInTreeView.id) select selectedDBTaskList;
					foreach(DBTaskList dbTaskList in dbTaskListsToDelete)
					{
						_dbManager.tasklists.DeleteOnSubmit(dbTaskList);
						
						//DELETE MUST BE CALLED WHEN "SYNCHRONIZE" BUTTON PRESSED
						//service.Tasklists.Delete(dbTaskList.id).Fetch();
						Debug.WriteLine("delete request started");
					}
					_dbManager.SubmitChanges();
				}
				
				if (_customTreeView.isTaskNode(selectedNodeInTreeView))
				{
			    	IQueryable<DBTask> dbTasksToDelete = from selectedDBTask in _dbManager.tasks where selectedDBTask.id.Equals(selectedNodeInTreeView.id) select selectedDBTask;
					foreach(DBTask dbTask in dbTasksToDelete)
					{
						_dbManager.tasks.DeleteOnSubmit(dbTask);
					
						//DELETE MUST BE CALLED WHEN "SYNCHRONIZE" BUTTON PRESSED
						//service.Tasks.Delete(dbTask.taskListId, dbTask.id).Fetch();
						Debug.WriteLine("delete request started");
					}
				}
				
				_dbManager.SubmitChanges();
				reloadCustomTreeViewData();
			}
		}
		//End of CustomTreeView Section
				
		//Timer Section
		private void _onTimerTick(object sender, EventArgs e)
		{
			_taskCurrentDateLabel.setDate(DateTime.Now);
		}
		
		
		private void _onSaveTaskChangesButtonClick(object sender, EventArgs e)
		{
			TreeNodeWithId selectedNodeInTreeView = (TreeNodeWithId)_customTreeView.SelectedNode;
			
			if (_customTreeView.isTaskListNode(selectedNodeInTreeView))
			{
				IQueryable<DBTaskList> selectedDBTaskLists = from selectedDBTaskList in _dbManager.tasklists where selectedDBTaskList.id.Equals(selectedNodeInTreeView.id) select selectedDBTaskList;
				foreach(DBTaskList dbTaskList in selectedDBTaskLists)
				{
					dbTaskList.title = _taskTitleTextBox.Text;        
				}
			}
						
			if (_customTreeView.isTaskNode(selectedNodeInTreeView))
			{
				IQueryable<DBTask> selectedDBTasks = from selectedDBTask in _dbManager.tasks where selectedDBTask.id.Equals(selectedNodeInTreeView.id) select selectedDBTask;
			
				foreach(DBTask dbTask in selectedDBTasks)
					{
						dbTask.title = _taskTitleTextBox.Text;
						dbTask.notes = _taskNotesTextBox.Text;
						dbTask.due = _taskDueDateLabel.getDate();
						if (_taskCompletedLabel.checkBoxChecked())
							dbTask.status = "completed";
						else dbTask.status = "needsAction";
							
						//UPDATE MUST BE CALLED WHEN "SYNCHRONIZE" BUTTON PRESSED
						//Task gglTask = dbTask.toGGLTask();
						//	service.Tasks.Update(gglTask, dbTask.taskListId, gglTask.Id).Fetch();                      
						Debug.WriteLine("update request started");
					}
			}
			
			_dbManager.SubmitChanges();
			reloadCustomTreeViewData();
		}
		
		
		//ToolBar Buttons Section
		void _onAddTaskListButtonClick(object sender, EventArgs e)
		{
			TasksService service = _authenticator.getTasksService();
			if (service != null)
			{
				//INSERT MUST BE CALLED WHEN "SYNCHRONIZE" BUTTON PRESSED
				TaskList taskListToInsert = new TaskList();
				taskListToInsert.Title = "New TaskList Title";
				service.Tasklists.Insert(taskListToInsert).Fetch();
	
			}
		}
		
		void _onAddTaskButtonClick(object sender, EventArgs e)
		{
			TasksService service = _authenticator.getTasksService();
			if (service != null)
			{
				//INSERT MUST BE CALLED WHEN "SYNCHRONIZE" BUTTON PRESSED
				//Task taskToInsert = new Task();
				//taskToInsert.Title = "New Task Title 2";
				//service.Tasks.Insert(taskToInsert, _dbManager.tasklists.ToList()[0].id).Fetch();
			}
		}
		
		
		void _onDBButtonClick(object sender, EventArgs e)
		{
			//Read from DB
			IQueryable<DBTaskList> selectedDBTaskLists = from selectedDBTaskList in _dbManager.tasklists select selectedDBTaskList;
			IQueryable<DBTask> selectedDBTasks = from selectedDBTask in _dbManager.tasks select selectedDBTask;
			foreach(DBTask dbTask in selectedDBTasks)
			{
				Debug.WriteLine("selectedDBTask = " + dbTask.title);
			}
			
			_customTreeView.reloadData(selectedDBTaskLists, selectedDBTasks);
		}
		
		void _onSynchronizeButtonClick(object sender, EventArgs e)
		{
			TasksService service = _authenticator.getTasksService();
			
			if (service != null)
			{
				
				
				//Write to DB
				foreach(DBTaskList dbTaskList in (from selectedDBTaskList in _dbManager.tasklists select selectedDBTaskList))
				{
					_dbManager.tasklists.DeleteOnSubmit(dbTaskList);
					_dbManager.SubmitChanges();
				}
				foreach(DBTask dbTask in (from selectedDBTask in _dbManager.tasks select selectedDBTask))
				{
					_dbManager.tasks.DeleteOnSubmit(dbTask);
					_dbManager.SubmitChanges();
				}
				_dbManager.SubmitChanges();
				

				foreach(TaskList item in service.Tasklists.List().Fetch().Items)
				{
					//
					
					DBTaskList dbTaskList = new DBTaskList(item);
					if (!_dbManager.tasklists.Contains(dbTaskList))
					{
						_dbManager.tasklists.InsertOnSubmit(dbTaskList);
						_dbManager.SubmitChanges();
						
						foreach(Task gglTask in service.Tasks.List(dbTaskList.id).Fetch().Items)
						{
							DBTask dbTask = new DBTask(gglTask, dbTaskList.id);
							if (!_dbManager.tasks.Contains(dbTask))
							{
								_dbManager.tasks.InsertOnSubmit(dbTask);
								_dbManager.SubmitChanges();
							}
						}
					}
				}
				
				
				
				//MDE2ODMxMzk1MTAzNzcwMTExMDk6MzU1MTU2NDgwOjA
				//	MDE2ODMxMzk1MTAzNzcwMTExMDk6OTUxOTcxMjE6MA
				IEnumerable<Task> tasks = service.Tasks.List("MDE2ODMxMzk1MTAzNzcwMTExMDk6MzU1MTU2NDgwOjA").Fetch().Items;
				foreach(Task task in tasks)
				{
					Debug.WriteLine("task.Id = " + task.Id);
					Debug.WriteLine("task.Title = " + task.Title);
					Debug.WriteLine("task.Position = " + task.Position);
					Debug.WriteLine("task.Parent = " + task.Parent);
					Debug.WriteLine("task.Status = " + task.Status);
					Debug.WriteLine("task.SelfLink = " + task.SelfLink);
					Debug.WriteLine("task.Completed = " + task.Completed);
					Debug.WriteLine("task.Deleted = " + task.Deleted);
					Debug.WriteLine("task.Due = " + task.Due);
					Debug.WriteLine("task.Updated = " + task.Updated);
					Debug.WriteLine("task.Notes = " + task.Notes);
				}

			}
		}
		
		
		
		
		
		
		
		
	}
}
