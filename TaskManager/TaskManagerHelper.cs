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
			return dbTasklistDateTime.Year.ToString() + "-" + dbTasklistDateTime.Month.ToString() + "-" + dbTasklistDateTime.Day.ToString() + "T" +
			           dbTasklistDateTime.Hour.ToString() + ":" + dbTasklistDateTime.Minute.ToString() + ":" + dbTasklistDateTime.Second.ToString() + ".000Z";
		}
		
		public static string convertTaskDateTimeToUpdatedString(DateTime dbTaskDateTime)
		{
			return convertTaskListDateTimeToUpdatedString(dbTaskDateTime);
		}
		
		
	}
}
