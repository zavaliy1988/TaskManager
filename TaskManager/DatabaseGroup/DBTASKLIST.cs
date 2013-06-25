/*
 * Created by SharpDevelop.
 * User: Администратор
 * Date: 6/18/2013
 * Time: 11:50 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Google.Apis.Tasks.v1.Data;
using System.Text;
using TaskManager;

namespace TaskManager
{
	/// <summary>
	/// Description of DBTASKLIST.
	/// </summary>
	[Table(Name = "tasklists")]
	public class DBTaskList
	{
		public DBTaskList(TaskList gglTasklist) 
		{
			this.id = gglTasklist.Id;
			this.title = gglTasklist.Title;
			this.selfLink = gglTasklist.SelfLink;
			
			this.updated = TaskManagerHelper.convertTaskListUpdatedStringToDateTime(gglTasklist.Updated);
		}
		
		public DBTaskList()
		{
		
		}
		
    	[Column(IsPrimaryKey = true)]
    	public string id;
    	[Column]
    	public string title;
		[Column]
    	public string selfLink;
    	[Column]
    	public DateTime updated;
	}
}
