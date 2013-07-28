/*
 * Created by SharpDevelop.
 * User: Администратор
 * Date: 6/18/2013
 * Time: 10:41 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using System.Collections.Generic;

using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;

using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;

using System.Diagnostics;

namespace TaskManager
{
	/// <summary>
	/// Description of DBTEST.
	/// </summary>
	public class DBManager : DataContext
	{
		public Table<DBTaskList> tasklists;
		public Table<DBTask> tasks;
		
		public DBManager(string connection) : base(connection)
		{
			
		}
		
		public void addNewDBTaskList()
		{
			TaskList gglTaskListToInsert = new TaskList();
			
			while(true)
			{
				gglTaskListToInsert.Id = TaskManagerHelper.createRandomTaskListId();
				if (!(from dbTaskList in this.tasklists select dbTaskList.id).Contains(gglTaskListToInsert.Id)) break;
			}
			gglTaskListToInsert.Title = "This is new TaskList";
			
			DBTaskList dbTaskListToInsert = new DBTaskList(gglTaskListToInsert);
			dbTaskListToInsert.updated = System.TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
			dbTaskListToInsert.mustBeCreatedOnGoogle = true;
			
			this.tasklists.InsertOnSubmit(dbTaskListToInsert);
			this.SubmitChanges();
		}
		
		public void addNewDBTask(string dbTaskListId)
		{
			Task gglTaskToInsert = new Task();
			while(true)
			{
				gglTaskToInsert.Id = TaskManagerHelper.createRandomTaskId();
				if (!(from dbTask in this.tasks select dbTask.id).Contains(gglTaskToInsert.Id)) break;
			}
			gglTaskToInsert.Title = "This is new Task!";
			
			DBTask dbTaskToInsert = new DBTask(gglTaskToInsert,dbTaskListId);
			gglTaskToInsert.Notes = "";
			dbTaskToInsert.status = "needsAction";
			dbTaskToInsert.due = DateTime.MaxValue;
			dbTaskToInsert.deleted = false;
			dbTaskToInsert.updated = System.TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
			dbTaskToInsert.completed = DateTime.MaxValue;
			dbTaskToInsert.mustBeCreatedOnGoogle = true;
			
			this.tasks.InsertOnSubmit(dbTaskToInsert);
			this.SubmitChanges();
		}
		
		public void synchronizeAllTaskListsAndAllTasks(TasksService service)
		{
			Debug.WriteLine("synchronize started");
			if (service != null)
			{
				Debug.WriteLine("Service != null");
				IQueryable<DBTaskList> selectedDBTaskLists = from selectedDBTaskList in this.tasklists select selectedDBTaskList;
				IQueryable<DBTask> selectedDBTasks = from selectedDBTask in this.tasks select selectedDBTask;
				
				foreach(DBTaskList dbTaskList in selectedDBTaskLists)
					dbTaskList.wasSynchronized = false;
				
				foreach(DBTask dbTask in selectedDBTasks)
					dbTask.wasSynchronized = false;
				
				this.SubmitChanges();
				
				synchronizeAllTasks(service);
				synchronizeAllTaskLists(service);
				
				createTaskListsOnGoogle(service);
				createTasksOnGoogle(service);
			
				deleteDBTasksAndDBTaskListsNotExistingOnGoogle(service);
			}
		}
		
		private void synchronizeAllTaskLists(TasksService service)
		{
			IQueryable<DBTaskList> selectedDBTaskLists = from selectedDBTaskList in this.tasklists select selectedDBTaskList;
			IQueryable<DBTask> selectedDBTasks = from selectedDBTask in this.tasks select selectedDBTask;
				
			foreach(TaskList gglTaskList in service.Tasklists.List().Fetch().Items)
			{
				DBTaskList dbTaskList = new DBTaskList(gglTaskList);
				if (!this.tasklists.Contains(dbTaskList))
					this.tasklists.InsertOnSubmit(dbTaskList);
				else
					synchronizeExistingTaskList(service, gglTaskList, dbTaskList);
			}
			this.SubmitChanges();
		}
		
		private void synchronizeExistingTaskList(TasksService service, TaskList gglTaskList, DBTaskList dbTaskList)
		{
			DBTaskList dbTaskListToCompare = (from selectedDBTaskList in this.tasklists where selectedDBTaskList.id.Equals(dbTaskList.id) select selectedDBTaskList).ToList()[0];
			int comparisonResult = TaskManagerHelper.convertTaskListUpdatedStringToDateTime(gglTaskList.Updated).CompareTo(dbTaskListToCompare.updated);
			
			//gglTaskList must be updated
			if (comparisonResult < 0)
				{
					if (dbTaskListToCompare.mustBeDeletedOnGoogle)
					{
						try
						{
							Debug.WriteLine("Trying delete TaskList on google");
							service.Tasklists.Delete(gglTaskList.Id).Fetch();
							this.tasklists.DeleteOnSubmit(dbTaskListToCompare);
						}
						catch (Exception ex)
						{
							Debug.WriteLine("Delete gglTaskList FAILED");
						}
					} 
					else
					{
						try
						{
							gglTaskList.Title = dbTaskListToCompare.title;
							gglTaskList.Updated = TaskManagerHelper.convertTaskListDateTimeToUpdatedString(dbTaskListToCompare.updated);
							
							Debug.WriteLine("Trying send TaskList to google");
							service.Tasklists.Update(gglTaskList,gglTaskList.Id).Fetch();
							dbTaskListToCompare.wasSynchronized = true;
						}
						catch (Exception ex)
						{
							Debug.WriteLine("Update gglTaskList FAILED");
						}
					}
				}
				//DBTaskList must be updated
				else if (comparisonResult >= 0)
				{
					dbTaskListToCompare.selfLink = gglTaskList.SelfLink;
					dbTaskListToCompare.title = gglTaskList.Title;
					dbTaskListToCompare.updated = TaskManagerHelper.convertTaskListUpdatedStringToDateTime(gglTaskList.Updated);
					dbTaskListToCompare.wasSynchronized = true;
				}
			this.SubmitChanges();
		}
		
		private void synchronizeAllTasks(TasksService service)
		{
			foreach(TaskList gglTaskList in service.Tasklists.List().Fetch().Items)
			{
				IList<Task> gglTasks = service.Tasks.List(gglTaskList.Id).Fetch().Items;
				
				if (gglTasks != null)
				{
					foreach(Task gglTask in gglTasks)
					{
						DBTask dbTask = new DBTask(gglTask, gglTaskList.Id);
						
						if (!this.tasks.Contains(dbTask))
							this.tasks.InsertOnSubmit(dbTask);
						else
							synchronizeExistingTask(service,gglTaskList,gglTask,dbTask);
					}
					this.SubmitChanges();
				}
				else Debug.WriteLine("gglTask == null");
			}
		}
		
		private void synchronizeExistingTask(TasksService service, TaskList gglTaskList, Task gglTask, DBTask dbTask)
		{
			DBTask dbTaskToCompare = (from selectedDBTask in this.tasks where selectedDBTask.id.Equals(dbTask.id) select selectedDBTask).ToList()[0];
			int comparisonResult = TaskManagerHelper.convertTaskUpdatedStringToDateTime(gglTask.Updated).CompareTo(dbTaskToCompare.updated);

			Debug.WriteLine("gglTask title = " + gglTask.Title);
			Debug.WriteLine("gglTask time = " + TaskManagerHelper.convertTaskUpdatedStringToDateTime(gglTask.Updated));
			Debug.WriteLine("dbTask time = " + dbTaskToCompare.updated);
			Debug.WriteLine("UTC time = " + TimeZoneInfo.ConvertTimeToUtc(dbTaskToCompare.updated));
			
			Debug.WriteLine("parent = " + gglTask.Parent);
			
			//gglTask must be updated
			if (comparisonResult < 0)
				{
					if (dbTaskToCompare.deleted == true)
					{ 
						try
						{
							Debug.WriteLine("Trying delete Task on google");
							service.Tasks.Delete(dbTaskToCompare.taskListId,dbTaskToCompare.id).Fetch();
							this.tasks.DeleteOnSubmit(dbTaskToCompare);
						}
						catch (Exception ex)
						{
							Debug.WriteLine("Delete gglTask FAILED" + ex.ToString());
						}
					}
					else
					{
						try
						{
							gglTask.Title = dbTaskToCompare.title;
							gglTask.Notes = dbTaskToCompare.notes;
							gglTask.Status = dbTaskToCompare.status;
							gglTask.Due = TaskManagerHelper.convertTaskDateTimeToUpdatedString(dbTaskToCompare.due);
							gglTask.Deleted = dbTaskToCompare.deleted;
							gglTask.Updated = TaskManagerHelper.convertTaskDateTimeToUpdatedString(dbTaskToCompare.updated);
							
							if (dbTaskToCompare.status.Equals("completed"))
							{
								gglTask.Completed = TaskManagerHelper.convertTaskDateTimeToUpdatedString(dbTaskToCompare.completed);
							}
							else if (dbTaskToCompare.status.Equals("needsAction"))
							{
								gglTask.Completed = null;
							}
							
							if (dbTaskToCompare.taskListId.Equals(gglTask.Id))
								service.Tasks.Update(gglTask,dbTaskToCompare.taskListId,gglTask.Id).Fetch();
							else
							{
								gglTask.Id = null;
								service.Tasks.Delete(gglTaskList.Id,dbTaskToCompare.id).Fetch();
								service.Tasks.Insert(gglTask,dbTaskToCompare.taskListId).Fetch();
							}
							dbTaskToCompare.wasSynchronized = true;
						}
						catch (Exception ex)
						{
							Debug.WriteLine("Update gglTask FAILED" + ex.ToString());
						}
					}
				}
			//DBTask must be updated
			else if (comparisonResult >= 0)
			{
				Debug.WriteLine("gglTask.status = " + gglTask.Status);
				
				if (gglTask.Deleted == true)
				{
					this.tasks.DeleteOnSubmit(dbTaskToCompare);
				}
				else
				{
					dbTaskToCompare.title = gglTask.Title;
					dbTaskToCompare.selfLink = gglTask.SelfLink;
					dbTaskToCompare.notes = gglTask.Notes;
					dbTaskToCompare.position = gglTask.Position;
					dbTaskToCompare.parent = gglTask.Parent;
					dbTaskToCompare.status = gglTask.Status;
					dbTaskToCompare.due = TaskManagerHelper.convertTaskUpdatedStringToDateTime(gglTask.Due);
					dbTaskToCompare.completed = TaskManagerHelper.convertTaskUpdatedStringToDateTime(gglTask.Completed);
					dbTaskToCompare.deleted = gglTask.Deleted;
					dbTaskToCompare.updated = TaskManagerHelper.convertTaskListUpdatedStringToDateTime(gglTask.Updated);
					dbTaskToCompare.wasSynchronized = true;
					this.SubmitChanges();
				}
			}
		   	this.SubmitChanges();
		}
		
		private void createTaskListsOnGoogle(TasksService service)
		{
			IQueryable<DBTaskList> dbTaskListsToCreate = from selectedDBTaskList in this.tasklists where selectedDBTaskList.mustBeCreatedOnGoogle == true select selectedDBTaskList;
			foreach(DBTaskList dbTaskList in dbTaskListsToCreate)
			{
				TaskList gglTaskList = new TaskList();
				gglTaskList.Title = dbTaskList.title;
				gglTaskList.Updated = TaskManagerHelper.convertTaskListDateTimeToUpdatedString(dbTaskList.updated);
				try
				{
					Debug.WriteLine("Trying create TaskList on google");
					service.Tasklists.Insert(gglTaskList).Fetch(); 
					dbTaskList.mustBeCreatedOnGoogle = false;
					dbTaskList.wasSynchronized = true;
					this.SubmitChanges();
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Create gglTaskList FAILED" + ex.ToString());
				}
			}
		}
		
		private void createTasksOnGoogle(TasksService service)
		{
			IQueryable<DBTask> dbTasksToCreate = from selectedDBTask in this.tasks where selectedDBTask.mustBeCreatedOnGoogle == true select selectedDBTask;
			foreach(DBTask dbTask in dbTasksToCreate)
			{
				Task gglTask = new Task();
				gglTask.Id = null;
				gglTask.Title = dbTask.title;
				gglTask.Notes = dbTask.notes;
				
				try
				{
					Debug.WriteLine("Trying create Task on google");
					gglTask = service.Tasks.Insert(gglTask,dbTask.taskListId).Fetch();
					
					gglTask.Status = dbTask.status;
					gglTask.Due = TaskManagerHelper.convertTaskDateTimeToUpdatedString(dbTask.due);
					gglTask.Deleted = dbTask.deleted;
					gglTask.Updated = TaskManagerHelper.convertTaskDateTimeToUpdatedString(dbTask.updated);
					
					if (dbTask.status.Equals("completed"))
					{
						gglTask.Completed = TaskManagerHelper.convertTaskDateTimeToUpdatedString(dbTask.completed);
					}
					else if (dbTask.status.Equals("needsAction"))
					{
						gglTask.Completed = null;
					}
					
					try
					{
						service.Tasks.Update(gglTask,dbTask.taskListId,dbTask.id).Fetch();
						dbTask.mustBeCreatedOnGoogle = false;
						dbTask.wasSynchronized = true;
						this.SubmitChanges();
					}
					catch (Exception ex)
					{
						Debug.WriteLine("Update of created gglTask FAILED" + ex.ToString());
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Create gglTask FAILED" + ex.ToString());
				}
			}
		}
		
		private void deleteDBTasksAndDBTaskListsNotExistingOnGoogle(TasksService service)
		{
			IList<TaskList> gglTaskLists = service.Tasklists.List().Fetch().Items;

			if (gglTaskLists != null)
			{
				IQueryable<DBTaskList> dbTaskListsToDelete = from dbTaskList in this.tasklists where (dbTaskList.wasSynchronized == false) select dbTaskList;
				IQueryable<DBTask> dbTasksToDelete = from dbTask in this.tasks where (dbTask.wasSynchronized == false) select dbTask;
				
				foreach(DBTaskList dbTaskListToDelete in dbTaskListsToDelete)
				{
					this.tasklists.DeleteOnSubmit(dbTaskListToDelete);
				}
				
				foreach(DBTask dbTaskToDelete in dbTasksToDelete)
				{
					this.tasks.DeleteOnSubmit(dbTaskToDelete);
				}
				this.SubmitChanges();
			}
		}

	}

}
