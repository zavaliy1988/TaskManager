/*
 * Created by SharpDevelop.
 * User: Администратор
 * Date: 6/22/2013
 * Time: 12:29 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;

using System.Diagnostics;

namespace TaskManager
{
	/// <summary>
	/// Description of CustomTreeView.
	/// </summary>
	public class CustomTreeView : TreeView
	{
		IList<DBTaskList> _dbTaskLists;
		IList<DBTask> _dbTasks;
		TreeNodeWithId draggedNode;
		
		public CustomTreeView()
		{
			this.AllowDrop = true;
			
			_dbTaskLists = new List<DBTaskList>();
			_dbTasks = new List<DBTask>();
  	
   			this.ItemDrag += new ItemDragEventHandler(_ItemDrag);
   			this.DragEnter += new DragEventHandler(_DragEnter);
   			this.DragDrop += new DragEventHandler(_DragDrop);
		}
		
		public void reloadData(IQueryable<DBTaskList> dbTaskLists, IQueryable<DBTask> dbTasks)
		{
			_dbTaskLists.Clear();
			_dbTasks.Clear();
			foreach(DBTaskList dbTaskList in dbTaskLists)
			{
				_dbTaskLists.Add(dbTaskList);
			}				
			foreach(DBTask dbTask in dbTasks)
			{
				_dbTasks.Add(dbTask);
			}
		
			this.Nodes.Clear();
			for(int i = 0; i < _dbTaskLists.Count; i++)
			{
				this.Nodes.Add(new TreeNodeWithId(_dbTaskLists[i].id, _dbTaskLists[i].title));
				
				IEnumerable<DBTask> dbTasksForCurrentDBTaskList = from dbTask in _dbTasks where dbTask.taskListId == _dbTaskLists[i].id select dbTask;
				foreach(DBTask dbTask in dbTasksForCurrentDBTaskList)
				{
					TreeNodeWithId newNode = new TreeNodeWithId(dbTask.id, dbTask.title);
					
					Debug.WriteLine("dbTask.due = " + dbTask.due.ToString());
					if ((dbTask.due <= DateTime.Now) && (dbTask.status.Equals("needsAction"))) newNode.BackColor = Color.Red;
					if ((dbTask.due <= DateTime.Now) && (dbTask.status.Equals("completed"))) newNode.BackColor = Color.Gray;
					if ((dbTask.due > DateTime.Now) && (dbTask.status.Equals("needsAction"))) newNode.BackColor = Color.Yellow;
					if ((dbTask.due > DateTime.Now) && (dbTask.status.Equals("completed"))) newNode.BackColor = Color.LightGreen;
					this.Nodes[i].Nodes.Add(newNode);
					
				}
			}
		}
		
		public bool isTaskNode(TreeNodeWithId node)
		{
			if (node != null)
			{		
				if ((from dbTask in _dbTasks where dbTask.id == node.id select dbTask.id).Contains(node.id))
			    	return true;
				else
					return false;
			}
			else
				return false;
		}
		
		public bool isTaskListNode(TreeNodeWithId node)
		{
			if (node != null)
			{		
				if ((from dbTaskList in _dbTaskLists where dbTaskList.id == node.id select dbTaskList.id).Contains(node.id))
			    	return true;
				else
					return false;
			}
			else
				return false;
		}
		
		
		private void _ItemDrag(object sender, ItemDragEventArgs e)
		{
			draggedNode = (TreeNodeWithId)e.Item;
			DoDragDrop(draggedNode, DragDropEffects.Move);
		}
		
		
		private void _DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
		}
		
		private void _DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			Point pt = ((CustomTreeView)sender).PointToClient(new Point(e.X, e.Y));
			TreeNodeWithId destinationNode = (TreeNodeWithId)((CustomTreeView)sender).GetNodeAt(pt);
			
			if ((from dbTask in _dbTasks select dbTask.id).Contains(draggedNode.id))
			{
				destinationNode.Parent.Nodes.Add(new TreeNodeWithId(draggedNode.id, draggedNode.Text));
				destinationNode.Expand();
			    
				IEnumerable<DBTask> dbTasksDragged = from dbTask in _dbTasks where draggedNode.id == dbTask.id select dbTask;
				foreach(DBTask dbTaskDragged in dbTasksDragged)
				{
					dbTaskDragged.taskListId = ((TreeNodeWithId)destinationNode.Parent).id;
					//DBMANAGER MUST BE CALLED TO SAVE CHANGES
				}
			
				draggedNode.Remove();
			}
		}
	}
}
