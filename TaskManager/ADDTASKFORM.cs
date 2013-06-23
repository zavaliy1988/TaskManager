/*
 * Created by SharpDevelop.
 * User: Aleks
 * Date: 06.06.2013
 * Time: 3:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;

namespace ToDoManager
{
	/// <summary>
	/// Description of ADDTASKFORM.
	/// </summary>
	public class ADDTASKFORM : Form
	{
		TextBox groupTextBox;
		TextBox contentTextBox;
		Button okButton;
		Button cancelButton;
		
		public ADDTASKFORM()
		{
			this.Text = "Add Task";
			this.FormBorderStyle = FormBorderStyle.Fixed3D;
			
			this.Size = new Size(400,300);
			
			groupTextBox = new TextBox();
			groupTextBox.Location = new Point(5,5);
			groupTextBox.Size = new Size(390,40);
			
			contentTextBox = new TextBox();
			contentTextBox.Location = new Point(5,50);
			contentTextBox.Size = new Size(390,40);
			
			okButton = new Button();
			okButton.Size = new Size(390,40);
			
			cancelButton = new Button();
			cancelButton.Size = new Size(390,40);
			
			this.Controls.Add(groupTextBox);
			this.Controls.Add(contentTextBox);
			this.Controls.Add(okButton);
			this.Controls.Add(cancelButton);
		}
	}
}
