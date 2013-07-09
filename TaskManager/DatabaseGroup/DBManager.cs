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
		
		public void synchronizeAllTaskListsAndAllTasks(TasksService service)
		{
			if (service != null)
			{
				IQueryable<DBTaskList> selectedDBTaskLists = from selectedDBTaskList in this.tasklists select selectedDBTaskList;
				IQueryable<DBTask> selectedDBTasks = from selectedDBTask in this.tasks select selectedDBTask;

				synchronizeAllTaskLists(service);
				synchronizeAllTasks(service);
				
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
						
			//Debug.WriteLine("dbManager contains TaskList with title = " + dbTaskListToCompare.title);
			//Debug.WriteLine("gglTaskList title = " + gglTaskList.Title);
			Debug.WriteLine("comparisonResult = " + comparisonResult.ToString());
						
			//gglTaskList must be updated
			if (comparisonResult <= 0)
				{
					gglTaskList.Title = dbTaskListToCompare.title;
					gglTaskList.Updated = TaskManagerHelper.convertTaskListDateTimeToUpdatedString(dbTaskListToCompare.updated);
							
					Debug.WriteLine("gglTaskListToUpdate.title = " + gglTaskList.Title);
					try
					{
						Debug.WriteLine("Trying send TaskList to google");
						service.Tasklists.Update(gglTaskList,gglTaskList.Id).Fetch();
					}
					catch (Exception ex)
					{
						Debug.WriteLine("Update gglTaskList FAILED");
					}
				}
				//DBTaskList must be updated
				else if (comparisonResult > 0)
				{
					dbTaskListToCompare.selfLink = gglTaskList.SelfLink;
					dbTaskListToCompare.title = gglTaskList.Title;
					dbTaskListToCompare.updated = TaskManagerHelper.convertTaskListUpdatedStringToDateTime(gglTaskList.Updated);
					this.SubmitChanges();
				}
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
							synchronizeExistingTask(service,gglTask,dbTask);
					}
					this.SubmitChanges();
				}
				else Debug.WriteLine("gglTask == null");
			}
		}
		
		private void synchronizeExistingTask(TasksService service, Task gglTask, DBTask dbTask)
		{
			DBTask dbTaskToCompare = (from selectedDBTask in this.tasks where selectedDBTask.id.Equals(dbTask.id) select selectedDBTask).ToList()[0];
			int comparisonResult = TaskManagerHelper.convertTaskUpdatedStringToDateTime(gglTask.Updated).CompareTo(dbTaskToCompare.updated);
						
			//Debug.WriteLine("dbManager contains Task with title = " + dbTaskToCompare.title);
			Debug.WriteLine("gglTask date = " + gglTask.Updated);
			Debug.WriteLine("dbTask date = " + dbTask.updated);
				
			//gglTask must be updated
			if (comparisonResult <= 0)
				{
					Debug.WriteLine("dbTask title = " + dbTask.title);
					Debug.WriteLine("gglTask title = " + gglTask.Title);
					Debug.WriteLine("comparisonResult = " + comparisonResult.ToString());
			
					gglTask.Title = dbTaskToCompare.title;
					gglTask.Notes = dbTaskToCompare.notes;
					//gglTask.Position = dbTaskToCompare.position;
					//gglTask.Parent = dbTaskToCompare.parent;
					Debug.WriteLine("dbTask.Status = " + dbTaskToCompare.status);
					//gglTask.Status = dbTaskToCompare.status;
					//gglTask.Status = "needsAction";
					Debug.WriteLine("dbTask.due = " + dbTaskToCompare.due.ToString());
					
					Debug.WriteLine("gglTask.Due Before = " + gglTask.Due);
					//gglTask.Due = TaskManagerHelper.convertTaskDateTimeToUpdatedString(dbTaskToCompare.due);
					Debug.WriteLine("gglTask.Due After = " + gglTask.Due);
					gglTask.Completed = dbTaskToCompare.completed;
					gglTask.Deleted = dbTaskToCompare.deleted;
					gglTask.Updated = TaskManagerHelper.convertTaskDateTimeToUpdatedString(dbTaskToCompare.updated);
							
					Debug.WriteLine("gglTaskListToUpdate.title = " + gglTask.Title);
					try
					{
						service.Tasks.Update(gglTask,dbTaskToCompare.taskListId,gglTask.Id).Fetch();
					}
					catch (Exception ex)
					{
						Debug.WriteLine("Update gglTask FAILED" + ex.ToString());
						Debug.WriteLine(ex.Data.ToString());
						Debug.WriteLine(ex.Message);
					}
				}
			//DBTask must be updated
			else if (comparisonResult > 0)
			{
				dbTaskToCompare.title = gglTask.Title;
				dbTaskToCompare.selfLink = gglTask.SelfLink;
				dbTaskToCompare.notes = gglTask.Notes;
				dbTaskToCompare.position = gglTask.Position;
				dbTaskToCompare.parent = gglTask.Parent;
				dbTaskToCompare.status = gglTask.Status;
				dbTaskToCompare.due = TaskManagerHelper.convertTaskUpdatedStringToDateTime(gglTask.Due);
				dbTaskToCompare.completed = gglTask.Completed;
				dbTaskToCompare.deleted = gglTask.Deleted;
				dbTaskToCompare.updated = TaskManagerHelper.convertTaskListUpdatedStringToDateTime(gglTask.Updated);
				
				this.SubmitChanges();
			}
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
					this.SubmitChanges();
				}
				catch (Exception ex)
				{
					
				}
			}
		}
		
		private void createTasksOnGoogle(TasksService service)
		{
			IQueryable<DBTask> dbTasksToCreate = from selectedDBTask in this.tasks where selectedDBTask.mustBeCreatedOnGoogle == true select selectedDBTask;
			foreach(DBTask dbTask in dbTasksToCreate)
			{
				Task gglTask = dbTask.toGGLTask();
				try
				{
					Debug.WriteLine("Trying create Task on google");
					service.Tasks.Insert(gglTask,dbTask.taskListId).Fetch();
					dbTask.mustBeCreatedOnGoogle = false;
					this.SubmitChanges();
				}
				catch (Exception ex)
				{
					
				}
			}
		}
		
		private void deleteDBTasksAndDBTaskListsNotExistingOnGoogle(TasksService service)
		{
			IList<TaskList> gglTaskLists = service.Tasklists.List().Fetch().Items;
			if (gglTaskLists != null)
			{
				System.Collections.Generic.IEnumerable<string> gglTaskListsIds = from selectedGGLTaskList in gglTaskLists select selectedGGLTaskList.Id;
				IQueryable<DBTaskList> dbTaskListsToDelete = from selectedDBTaskList in this.tasklists where (!gglTaskListsIds.Contains(selectedDBTaskList.id) && (selectedDBTaskList.mustBeCreatedOnGoogle == false)) select selectedDBTaskList;
							
				foreach(DBTaskList dbTaskList in dbTaskListsToDelete)
				{
					IList<Task> gglTasks = service.Tasks.List(dbTaskList.id).Fetch().Items;
					
					if (gglTasks != null)
					{
							System.Collections.Generic.IEnumerable<string> gglTasksIds = from selectedGGLTask in gglTasks select selectedGGLTask.Id;
							IQueryable<DBTask> dbTasksToDelete = from selectedDBTask in this.tasks where (!gglTasksIds.Contains(selectedDBTask.id) && (selectedDBTask.mustBeCreatedOnGoogle == false)) select selectedDBTask;
							foreach(DBTask dbTask in dbTasksToDelete)
								this.tasks.DeleteOnSubmit(dbTask);
					}
					this.tasklists.DeleteOnSubmit(dbTaskList);
				}
				this.SubmitChanges();
			}
		}
		
	}

}
