/*
 * Created by SharpDevelop.
 * User: Администратор
 * Date: 6/23/2013
 * Time: 7:35 PM
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
	/// Description of DateTimeLabel.
	/// </summary>
	public class DateTimeLabel : Label
	{
		Label _titleLabel;
		DateTimePicker _dateTimePicker;
		DateTime _settedDateTime;
		
		public DateTimeLabel(string titleLabelText)
		{
			this.Size = new Size(245,25);
			
			_titleLabel = new Label();
			_titleLabel.Size = new Size(90,30);
			_titleLabel.Font = new Font("Arial", 14);
			_titleLabel.Text = titleLabelText;
			this.Controls.Add(_titleLabel);
			
			_dateTimePicker = new DateTimePicker();
			_dateTimePicker.Location = new Point(110,2);
			_dateTimePicker.Size = new Size(130,30);
			_dateTimePicker.Format = DateTimePickerFormat.Custom;
			_dateTimePicker.CustomFormat = "dd-MM-yyyy   hh:mm:ss";
			_dateTimePicker.ValueChanged += new EventHandler(_onDateTimePickerValueChange);
			this.Controls.Add(_dateTimePicker);
		}
		
		public DateTime getDate()
		{
			return _settedDateTime;
		}
		
		public void setDate(DateTime dateTime)
		{
			_settedDateTime = dateTime;
		
			int dateTimeComparisonResult = DateTime.Compare(_settedDateTime, DateTimePicker.MaximumDateTime);
			if (dateTimeComparisonResult <= 0)
				_dateTimePicker.Value = _settedDateTime;
			else 
				_dateTimePicker.Value = DateTimePicker.MaximumDateTime;
		}
		
		private void _onDateTimePickerValueChange(object sender, EventArgs e)
		{
			Debug.WriteLine("Date changed");
		}
		
		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			_titleLabel.Enabled = true;
		}
	}
}
