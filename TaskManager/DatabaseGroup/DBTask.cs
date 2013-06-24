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
		public DBTask(Task gglTask, string gglTaskListId)
		{
			this.id = gglTask.Id;
			this.taskListId = gglTaskListId;
			this.title = gglTask.Title;
			this.selfLink = gglTask.SelfLink;
			this.updated = TaskManagerHelper.convertTaskUpdateDateToDateTime(gglTask.Updated);
			this.notes = gglTask.Notes;
			this.position = gglTask.Position;
			this.parent = gglTask.Parent;
			this.status = gglTask.Status;
			this.due = TaskManagerHelper.convertTaskUpdateDateToDateTime(gglTask.Due);
			this.completed = gglTask.Completed;
			this.deleted = gglTask.Deleted;
		}
		
		public DBTask()
		{
			
		}
		
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
    	public string completed;
    	[Column]
    	public Nullable<bool> deleted;
	}
}
