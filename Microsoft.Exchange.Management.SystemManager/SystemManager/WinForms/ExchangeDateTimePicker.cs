using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeDateTimePicker : AutoSizePanel
	{
		public ExchangeDateTimePicker()
		{
			this.InitializeComponent();
			this.datePicker.ValueChanged += this.dateTimePicker_ValueChanged;
			this.timePicker.ValueChanged += this.dateTimePicker_ValueChanged;
		}

		[DefaultValue(true)]
		public bool TimeVisible
		{
			get
			{
				return this.timeVisible;
			}
			set
			{
				if (this.TimeVisible != value)
				{
					if (this.TimeVisible)
					{
						this.timePicker.ValueChanged -= this.dateTimePicker_ValueChanged;
						this.tableLayoutPanel.Controls.Remove(this.timePicker);
						this.tableLayoutPanel.SetColumnSpan(this.datePicker, 3);
					}
					else
					{
						this.tableLayoutPanel.SetColumnSpan(this.datePicker, 1);
						this.tableLayoutPanel.Controls.Add(this.timePicker, 2, 0);
						this.timePicker.ValueChanged += this.dateTimePicker_ValueChanged;
					}
					this.timeVisible = value;
				}
			}
		}

		private void InitializeComponent()
		{
			this.datePicker = new ExtendedDateTimePicker();
			this.timePicker = new ExtendedDateTimePicker();
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.datePicker.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.datePicker.Location = new Point(3, 0);
			this.datePicker.Margin = new Padding(3, 0, 0, 0);
			this.datePicker.Name = "datePicker";
			this.datePicker.Size = new Size(222, 20);
			this.datePicker.TabIndex = 2;
			this.timePicker.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.timePicker.Format = DateTimePickerFormat.Time;
			this.timePicker.Location = new Point(236, 0);
			this.timePicker.Margin = new Padding(3, 0, 0, 0);
			this.timePicker.Name = "timePicker";
			this.timePicker.ShowUpDown = true;
			this.timePicker.Size = new Size(72, 20);
			this.timePicker.TabIndex = 3;
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 8f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3f));
			this.tableLayoutPanel.Controls.Add(this.datePicker, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.timePicker, 2, 0);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(308, 20);
			this.tableLayoutPanel.TabIndex = 0;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "ExchangeDateTimePicker";
			base.Size = new Size(308, 20);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void dateTimePicker_ValueChanged(object sender, EventArgs e)
		{
			this.OnValueChanged(EventArgs.Empty);
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DateTime Value
		{
			get
			{
				DateTime date = this.datePicker.Value.Date;
				if (this.TimeVisible)
				{
					date = new DateTime(this.datePicker.Value.Year, this.datePicker.Value.Month, this.datePicker.Value.Day, this.timePicker.Value.Hour, this.timePicker.Value.Minute, this.timePicker.Value.Second);
				}
				return date;
			}
			set
			{
				if (value != this.Value)
				{
					this.datePicker.Value = value.Date;
					if (this.TimeVisible)
					{
						this.timePicker.Value = new DateTime(this.timePicker.MinDate.Year, this.timePicker.MinDate.Month, this.timePicker.MinDate.Day, value.Hour, value.Minute, value.Second);
					}
					this.OnValueChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnValueChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[ExchangeDateTimePicker.EventValueChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler ValueChanged
		{
			add
			{
				base.Events.AddHandler(ExchangeDateTimePicker.EventValueChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(ExchangeDateTimePicker.EventValueChanged, value);
			}
		}

		[DefaultValue(DateTimePickerFormat.Long)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Browsable(true)]
		public DateTimePickerFormat DateFormat
		{
			get
			{
				return this.datePicker.Format;
			}
			set
			{
				this.datePicker.Format = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[DefaultValue(null)]
		[Browsable(true)]
		public string CustomDateFormat
		{
			get
			{
				return this.datePicker.CustomFormat;
			}
			set
			{
				this.datePicker.CustomFormat = value;
			}
		}

		[Browsable(true)]
		[DefaultValue(DateTimePickerFormat.Time)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public DateTimePickerFormat TimeFormat
		{
			get
			{
				return this.timePicker.Format;
			}
			set
			{
				this.timePicker.Format = value;
			}
		}

		[DefaultValue(null)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string CustomTimeFormat
		{
			get
			{
				if (!this.TimeVisible)
				{
					return string.Empty;
				}
				return this.timePicker.CustomFormat;
			}
			set
			{
				this.timePicker.CustomFormat = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateTime MinDate
		{
			get
			{
				return this.datePicker.MinDate;
			}
			set
			{
				this.datePicker.MinDate = value.Date;
			}
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "Value";
			}
		}

		private ExtendedDateTimePicker datePicker;

		private ExtendedDateTimePicker timePicker;

		private AutoTableLayoutPanel tableLayoutPanel;

		private bool timeVisible = true;

		private static readonly object EventValueChanged = new object();
	}
}
