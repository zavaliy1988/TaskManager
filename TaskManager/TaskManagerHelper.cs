/*
 * Created by SharpDevelop.
 * User: Администратор
 * Date: 6/19/2013
 * Time: 7:46 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;

using System.Diagnostics;

namespace TaskManager
{
	/// <summary>
	/// Description of TASKMANAGERHELPER.
	/// </summary>
	public static class TaskManagerHelper
	{
		
		public static DateTime convertTaskListUpdatedStringToDateTime(string gglTasklistUpdatedString)
		{
			if (gglTasklistUpdatedString != null)
			{
			
				StringBuilder updatedDateWithSpaceSeparators = new StringBuilder(gglTasklistUpdatedString);
				updatedDateWithSpaceSeparators.Length = 19;
				updatedDateWithSpaceSeparators.Replace('T', ' ');
				updatedDateWithSpaceSeparators.Replace('-', ' ');
				updatedDateWithSpaceSeparators.Replace(':', ' ');
				string [] dateTimeStringParts = updatedDateWithSpaceSeparators.ToString().Split(new char [] {' '});
				int [] dateTimeIntParts = new int[dateTimeStringParts.Length];
				
				//Debug.WriteLine("Updated Date before convert = " + gglTasklistUpdatedString);
				for(int i = 0; i < dateTimeStringParts.Length; i++)
				{
					dateTimeIntParts[i] = Int32.Parse(dateTimeStringParts[i]);
				}
				return new DateTime(dateTimeIntParts[0], dateTimeIntParts[1], dateTimeIntParts[2], dateTimeIntParts[3], dateTimeIntParts[4], dateTimeIntParts[5]);
			}
			else 
				return DateTime.MaxValue;
		}
		
		public static DateTime convertTaskUpdatedStringToDateTime(string gglTaskUpdatedString)
		{
			return convertTaskListUpdatedStringToDateTime(gglTaskUpdatedString);
		}
		
		//UpdatedString example:
		//2013-06-22T00:28:02.000Z
		public static string convertTaskListDateTimeToUpdatedString(DateTime dbTasklistDateTime)
		{
			return dbTasklistDateTime.Year.ToString().PadLeft(4,'0') + "-" + dbTasklistDateTime.Month.ToString().PadLeft(2,'0') + "-" + dbTasklistDateTime.Day.ToString().PadLeft(2,'0') + "T" +
				dbTasklistDateTime.Hour.ToString().PadLeft(2,'0') + ":" + dbTasklistDateTime.Minute.ToString().PadLeft(2,'0') + ":" + dbTasklistDateTime.Second.ToString().PadLeft(2,'0') + ".000Z";
		}
		
		public static string convertTaskDateTimeToUpdatedString(DateTime dbTaskDateTime)
		{
			return convertTaskListDateTimeToUpdatedString(dbTaskDateTime);
		}
		
		public static string createRandomString(int size)
    	{
			Random random = new Random();
       	 	StringBuilder builder = new StringBuilder();
        	char ch;
        	for (int i = 0; i < size; i++)
        	{
           		ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));                 
            	builder.Append(ch);
        	}
     		
        	return builder.ToString();
    	}
		
		public static string createRandomTaskListId()
		{
			//TaskList has length of id = 19
			return createRandomString(19);
		}
		
		public static string createRandomTaskId()
		{
			//Task has length of id = 43
			return createRandomString(43);
		}
		
	}
}
