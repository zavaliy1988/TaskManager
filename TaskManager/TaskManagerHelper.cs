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
		
		public static DateTime convertTasklistUpdateDateToDateTime(string updatedDate)
		{
			if (updatedDate != null)
			{
			
				StringBuilder updatedDateWithSpaceSeparators = new StringBuilder(updatedDate);
				updatedDateWithSpaceSeparators.Length = 19;
				updatedDateWithSpaceSeparators.Replace('T', ' ');
				updatedDateWithSpaceSeparators.Replace('-', ' ');
				updatedDateWithSpaceSeparators.Replace(':', ' ');
				string [] dateTimeStringParts = updatedDateWithSpaceSeparators.ToString().Split(new char [] {' '});
				int [] dateTimeIntParts = new int[dateTimeStringParts.Length];
				
				Debug.WriteLine("Updated Date before convert = " + updatedDate);
				for(int i = 0; i < dateTimeStringParts.Length; i++)
				{
					dateTimeIntParts[i] = Int32.Parse(dateTimeStringParts[i]);
				}
				return new DateTime(dateTimeIntParts[0], dateTimeIntParts[1], dateTimeIntParts[2], dateTimeIntParts[3], dateTimeIntParts[4], dateTimeIntParts[5]);
			}
			else 
				return DateTime.MaxValue;
		}
		
		public static DateTime convertTaskUpdateDateToDateTime(string updateDate)
		{
			return convertTasklistUpdateDateToDateTime(updateDate);
		}
	}
}
