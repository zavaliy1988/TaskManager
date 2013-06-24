/*
 * Created by SharpDevelop.
 * User: Администратор
 * Date: 6/18/2013
 * Time: 10:41 PM
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
	/// Description of DBTEST.
	/// </summary>
	public class DBManager : DataContext
	{
		public Table<DBTaskList> tasklists;
		public Table<DBTask> tasks;
		
		public DBManager(string connection) : base(connection)
		{

		}
	}

}
