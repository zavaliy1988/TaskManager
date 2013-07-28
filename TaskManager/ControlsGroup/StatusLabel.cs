/*
 * Created by SharpDevelop.
 * User: Администратор
 * Date: 6/25/2013
 * Time: 12:45 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;

using System.Diagnostics;

namespace TaskManager
{
	/// <summary>
	/// Description of CompletedLabel.
	/// </summary>
	public class StatusLabel : Label
	{
		Label _titleLabel;
		CheckBox _checkBox;
		public event EventHandler checkBoxClick;
		
		public StatusLabel(string titleLabelText)
		{
			this.Size = new Size(245,25);
						
			_titleLabel = new Label();
			_titleLabel.Size = new Size(110,30);
			_titleLabel.Font = new Font("Arial", 14);
			_titleLabel.Text = titleLabelText;
			this.Controls.Add(_titleLabel);
			
			_checkBox = new CheckBox();
			_checkBox.Location = new Point(170,1);
			_checkBox.Appearance = Appearance.Normal;
			_checkBox.Click += new EventHandler(_onCheckBoxClick);
			this.Controls.Add(_checkBox);
		}
		
		public bool checkBoxChecked()
		{
			return _checkBox.Checked;
		}
		
		public void setChecked(bool val)
		{
			_checkBox.Checked = val;
		}
		
		private void _onCheckBoxClick(object sender, EventArgs e)
		{
			if (checkBoxClick != null)
				checkBoxClick(this,e);
		}
	}
}
