using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ScheduleControl : ExchangeUserControl
	{
		public ScheduleControl()
		{
			this.InitializeComponent();
			this.customizeButton.Text = Strings.ScheduleCustomizeButton;
			this.customizeButton.Click += delegate(object param0, EventArgs param1)
			{
				ScheduleEditorDialog scheduleEditorDialog = new ScheduleEditorDialog();
				scheduleEditorDialog.Schedule = this.Schedule;
				if (scheduleEditorDialog.ShowDialog(this) == DialogResult.OK)
				{
					this.SetCurrentSchedule(scheduleEditorDialog.Schedule);
				}
			};
			this.scheduleIntervalCombo.DataSource = this.cannedSchedules;
			this.scheduleIntervalCombo.ValueMember = "Schedule";
			this.scheduleIntervalCombo.DisplayMember = "Description";
			this.scheduleIntervalCombo.SelectedIndexChanged += delegate(object param0, EventArgs param1)
			{
				if (this.SetCurrentSchedule(this.scheduleIntervalCombo.SelectedIndex))
				{
					if (this.scheduleIntervalCombo.SelectedIndex != this.GetIndexOfUserCustomizedSchedule())
					{
						this.cannedSchedules[this.GetIndexOfUserCustomizedSchedule()].Schedule = this.Schedule;
						return;
					}
				}
				else
				{
					this.OnScheduleChanged(EventArgs.Empty);
				}
			};
			this.cannedSchedules.Add(new ScheduleControl.ScheduleWithDescription(this.Schedule, Strings.UseCustomSchedule));
			this.scheduleIntervalCombo.SelectedIndex = this.GetIndexOfUserCustomizedSchedule();
		}

		protected override UIValidationError[] GetValidationErrors()
		{
			if (this.IsCustomizeWithoutSelectSchedule())
			{
				return new UIValidationError[]
				{
					new UIValidationError(Strings.CustomizedWithoutSchedule, this.customizeButton)
				};
			}
			return base.GetValidationErrors();
		}

		internal bool IsCustomizeWithoutSelectSchedule()
		{
			int indexOfUserCustomizedSchedule = this.GetIndexOfUserCustomizedSchedule();
			int positionOfSchedueInCannedSchedulesExceptUserCustomizedSchedule = this.GetPositionOfSchedueInCannedSchedulesExceptUserCustomizedSchedule(this.cannedSchedules[indexOfUserCustomizedSchedule].Schedule);
			return this.scheduleIntervalCombo.SelectedIndex == indexOfUserCustomizedSchedule && positionOfSchedueInCannedSchedulesExceptUserCustomizedSchedule != -1;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			this.customizeButton = new ExchangeButton();
			this.scheduleIntervalLabel = new Label();
			this.scheduleIntervalCombo = new ExchangeComboBox();
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.customizeButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.customizeButton.AutoSize = true;
			this.customizeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.customizeButton.Location = new Point(178, 16);
			this.customizeButton.Margin = new Padding(3, 3, 0, 0);
			this.customizeButton.Name = "customizeButton";
			this.customizeButton.Size = new Size(6, 23);
			this.customizeButton.TabIndex = 2;
			this.scheduleIntervalLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.scheduleIntervalLabel.AutoSize = true;
			this.tableLayoutPanel.SetColumnSpan(this.scheduleIntervalLabel, 3);
			this.scheduleIntervalLabel.Location = new Point(0, 0);
			this.scheduleIntervalLabel.Margin = new Padding(0);
			this.scheduleIntervalLabel.Name = "scheduleIntervalLabel";
			this.scheduleIntervalLabel.Size = new Size(184, 13);
			this.scheduleIntervalLabel.TabIndex = 0;
			this.scheduleIntervalCombo.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.scheduleIntervalCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			this.scheduleIntervalCombo.FormattingEnabled = true;
			this.scheduleIntervalCombo.Location = new Point(3, 18);
			this.scheduleIntervalCombo.Margin = new Padding(3, 5, 0, 0);
			this.scheduleIntervalCombo.Name = "scheduleIntervalCombo";
			this.scheduleIntervalCombo.Size = new Size(164, 21);
			this.scheduleIntervalCombo.TabIndex = 1;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 8f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel.Controls.Add(this.customizeButton, 2, 1);
			this.tableLayoutPanel.Controls.Add(this.scheduleIntervalCombo, 0, 1);
			this.tableLayoutPanel.Controls.Add(this.scheduleIntervalLabel, 0, 0);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Padding = new Padding(0, 0, 16, 0);
			this.tableLayoutPanel.RowCount = 2;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(200, 39);
			this.tableLayoutPanel.TabIndex = 0;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "ScheduleControl";
			base.Size = new Size(200, 39);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[DefaultValue(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Browsable(true)]
		public bool CaptionVisible
		{
			get
			{
				return this.scheduleIntervalLabel.Visible;
			}
			set
			{
				this.scheduleIntervalLabel.Visible = value;
			}
		}

		[DefaultValue("")]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string Caption
		{
			get
			{
				return this.scheduleIntervalLabel.Text;
			}
			set
			{
				this.scheduleIntervalLabel.Text = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ButtonText
		{
			get
			{
				return this.customizeButton.Text;
			}
			set
			{
				this.customizeButton.Text = value;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(400, 39);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Schedule Schedule
		{
			get
			{
				return this.schedule;
			}
			set
			{
				Schedule currentSchedule = value ?? Schedule.Never;
				if (!this.Schedule.Equals(currentSchedule))
				{
					this.SetCurrentSchedule(currentSchedule);
				}
			}
		}

		public event EventHandler ScheduleChanged
		{
			add
			{
				base.Events.AddHandler(ScheduleControl.ScheduleChangedObject, value);
			}
			remove
			{
				base.Events.RemoveHandler(ScheduleControl.ScheduleChangedObject, value);
			}
		}

		protected virtual void OnScheduleChanged(EventArgs e)
		{
			base.UpdateError();
			EventHandler eventHandler = (EventHandler)base.Events[ScheduleControl.ScheduleChangedObject];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public static void ConvertScheduleToScheduleIntervalArray(ConvertEventArgs args)
		{
			ReadOnlyCollection<ScheduleInterval> readOnlyCollection = (args.Value == null) ? null : ((Schedule)args.Value).Intervals;
			ScheduleInterval[] array;
			if (readOnlyCollection == null)
			{
				array = new ScheduleInterval[0];
			}
			else
			{
				array = new ScheduleInterval[readOnlyCollection.Count];
				readOnlyCollection.CopyTo(array, 0);
			}
			args.Value = array;
		}

		public static void ConvertScheduleIntervalArrayToSchedule(ConvertEventArgs args)
		{
			args.Value = ((args.Value == null) ? null : new Schedule((ScheduleInterval[])args.Value));
		}

		public void AddCannedSchedule(Schedule cannedSchedule, string scheduleDescription)
		{
			int positionOfSchedueInCannedSchedulesExceptUserCustomizedSchedule = this.GetPositionOfSchedueInCannedSchedulesExceptUserCustomizedSchedule(cannedSchedule);
			if (positionOfSchedueInCannedSchedulesExceptUserCustomizedSchedule == -1)
			{
				this.cannedSchedules.Insert(this.GetIndexOfInsertingNewCannedSchedule(), new ScheduleControl.ScheduleWithDescription(cannedSchedule, scheduleDescription));
				this.SetCurrentSchedule(this.Schedule);
			}
		}

		private int GetPositionOfSchedueInCannedSchedulesExceptUserCustomizedSchedule(Schedule targetSchedule)
		{
			int num = -1;
			for (int i = 0; i < this.cannedSchedules.Count; i++)
			{
				if (i != this.GetIndexOfUserCustomizedSchedule() && Schedule.Equals(this.cannedSchedules[i].Schedule, targetSchedule))
				{
					num = i;
				}
				if (num > 0)
				{
					break;
				}
			}
			return num;
		}

		private int GetIndexOfInsertingNewCannedSchedule()
		{
			return this.GetIndexOfUserCustomizedSchedule();
		}

		private int GetIndexOfUserCustomizedSchedule()
		{
			return this.cannedSchedules.Count - 1;
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			return this.tableLayoutPanel.GetPreferredSize(proposedSize);
		}

		private void SetCurrentSchedule(Schedule newSchedule)
		{
			int num = this.GetPositionOfSchedueInCannedSchedulesExceptUserCustomizedSchedule(newSchedule);
			if (num == -1)
			{
				this.cannedSchedules[this.GetIndexOfUserCustomizedSchedule()].Schedule = newSchedule;
				num = this.GetIndexOfUserCustomizedSchedule();
			}
			if (this.scheduleIntervalCombo.SelectedIndex != num)
			{
				this.scheduleIntervalCombo.SelectedIndex = num;
				return;
			}
			if (num == this.GetIndexOfUserCustomizedSchedule())
			{
				this.SetCurrentSchedule(num);
			}
		}

		private bool SetCurrentSchedule(int newSelectedIndex)
		{
			bool result = false;
			Schedule secondSchedule = this.cannedSchedules[newSelectedIndex].Schedule;
			if (!Schedule.Equals(this.Schedule, secondSchedule))
			{
				this.schedule = secondSchedule;
				this.OnScheduleChanged(EventArgs.Empty);
				result = true;
			}
			return result;
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "Schedule";
			}
		}

		private BindingList<ScheduleControl.ScheduleWithDescription> cannedSchedules = new BindingList<ScheduleControl.ScheduleWithDescription>();

		private IContainer components;

		private ExchangeButton customizeButton;

		private Label scheduleIntervalLabel;

		private ExchangeComboBox scheduleIntervalCombo;

		private AutoTableLayoutPanel tableLayoutPanel;

		private Schedule schedule = Schedule.Never;

		private static readonly object ScheduleChangedObject = new object();

		public class ScheduleWithDescription
		{
			public ScheduleWithDescription()
			{
			}

			public ScheduleWithDescription(Schedule schedule, string description)
			{
				this.Schedule = schedule;
				this.Description = description;
			}

			public Schedule Schedule
			{
				get
				{
					return this.schedule;
				}
				internal set
				{
					this.schedule = value;
				}
			}

			public string Description
			{
				get
				{
					return this.description;
				}
				internal set
				{
					this.description = value;
				}
			}

			private Schedule schedule;

			private string description;
		}
	}
}
