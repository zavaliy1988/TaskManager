/*
 * Created by SharpDevelop.
 * User: Администратор
 * Date: 6/22/2013
 * Time: 2:01 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace TaskManager
{
	/// <summary>
	/// Description of TreeNodeWithId.
	/// </summary>
	public class TreeNodeWithId : TreeNode
	{
		string _id;
		public string id
		{
			get {return _id;} 
			set {}
		}
		public TreeNodeWithId(string id, string text)
		{
			_id = id;
			this.Text = text;
		}
	}
}
