/*
 * Created by SharpDevelop.
 * User: Администратор
 * Date: 6/20/2013
 * Time: 2:00 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using System.Data.Linq;
using System.Data.Linq.Mapping;

using Google.Apis.Tasks.v1.Data;

namespace TaskManager
{
	/// <summary>
	/// Description of DBTask.
	/// </summary>
	[Table(Name = "tasks")]
	public class DBTask
	{
		[Column(IsPrimaryKey = true)]
		public string id;
		[Column]
		public string taskListId;
		[Column]
		public string title;
		[Column]
    	public string selfLink;
    	[Column]
    	public DateTime updated;
    	[Column]
    	public string notes;
    	[Column]
    	public string position;
    	[Column]
    	public string parent;
    	[Column]
    	public string status;
    	[Column]
    	public DateTime due;
    	[Column]
    	public DateTime completed;
    	[Column]
    	public Nullable<bool> deleted;
    	[Column]
    	public bool mustBeCreatedOnGoogle;
    	[Column]
    	public bool wasSynchronized;
		
		public DBTask(Task gglTask, string gglTaskListId)
		{
			this.id = gglTask.Id;
			this.taskListId = gglTaskListId;
			this.title = gglTask.Title;
			this.selfLink = gglTask.SelfLink;
			this.updated = TaskManagerHelper.convertTaskUpdatedStringToDateTime(gglTask.Updated);
			this.notes = gglTask.Notes;
			this.position = gglTask.Position;
			this.parent = gglTask.Parent;
			this.status = gglTask.Status;
			this.due = TaskManagerHelper.convertTaskUpdatedStringToDateTime(gglTask.Due);
			this.completed = TaskManagerHelper.convertTaskUpdatedStringToDateTime(gglTask.Completed);
			this.deleted = gglTask.Deleted;
			
			this.mustBeCreatedOnGoogle = false;
			this.wasSynchronized = true;
		}
		
		public DBTask()
		{
			
		}
		    	
    	public Task toGGLTask()
		{
    		Task gglTask = new Task();
    		gglTask.Id = this.id;
    		gglTask.Title = this.title;
    		gglTask.SelfLink = this.selfLink;
    		gglTask.Updated = TaskManagerHelper.convertTaskDateTimeToUpdatedString(this.updated);
    		gglTask.Notes = this.notes;
    		gglTask.Position = this.position;
    		gglTask.Parent = this.parent;
    		gglTask.Status = this.status;
    		gglTask.Due = TaskManagerHelper.convertTaskDateTimeToUpdatedString(this.due);
    		gglTask.Completed = TaskManagerHelper.convertTaskDateTimeToUpdatedString(this.completed);
    		gglTask.Deleted = this.deleted;
    		
    		return gglTask;
    	}
	}
}
