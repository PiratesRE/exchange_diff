using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CustomDateTimePicker : AutoSizePanel
	{
		public CustomDateTimePicker() : this(null)
		{
		}

		public CustomDateTimePicker(DateTime? defaultDateTime)
		{
			this.InitializeComponent();
			this.exchangeDateTimePicker.Enabled = false;
			if (defaultDateTime != null)
			{
				this.exchangeDateTimePicker.Value = defaultDateTime.Value;
			}
			this.exchangeDateTimePicker.ValueChanged += this.dateTimePicker_ValueChanged;
			this.titleCheckBox.CheckedChanged += this.titleCheckBox_CheckedChanged;
		}

		[DefaultValue(true)]
		public bool TimeVisible
		{
			get
			{
				return this.exchangeDateTimePicker.TimeVisible;
			}
			set
			{
				this.exchangeDateTimePicker.TimeVisible = value;
			}
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			this.titleCheckBox = new AutoHeightCheckBox();
			this.exchangeDateTimePicker = new ExchangeDateTimePicker();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Controls.Add(this.titleCheckBox, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.exchangeDateTimePicker, 1, 1);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 2;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(324, 45);
			this.tableLayoutPanel.TabIndex = 0;
			this.titleCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.titleCheckBox.AutoSize = true;
			this.tableLayoutPanel.SetColumnSpan(this.titleCheckBox, 2);
			this.titleCheckBox.Location = new Point(0, 0);
			this.titleCheckBox.Margin = new Padding(3, 0, 0, 0);
			this.titleCheckBox.Name = "titleCheckBox";
			this.titleCheckBox.Size = new Size(324, 17);
			this.titleCheckBox.TabIndex = 1;
			this.titleCheckBox.UseVisualStyleBackColor = true;
			this.exchangeDateTimePicker.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.exchangeDateTimePicker.Location = new Point(16, 25);
			this.exchangeDateTimePicker.Margin = new Padding(0, 8, 0, 0);
			this.exchangeDateTimePicker.Name = "exchangeDateTimePicker";
			this.exchangeDateTimePicker.Size = new Size(308, 20);
			this.exchangeDateTimePicker.TabIndex = 2;
			this.exchangeDateTimePicker.AutoSize = true;
			this.exchangeDateTimePicker.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "CustomDateTimePicker";
			base.Size = new Size(324, 45);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void titleCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			bool @checked = (sender as CheckBox).Checked;
			this.exchangeDateTimePicker.Enabled = @checked;
			this.OnCheckedChanged(EventArgs.Empty);
			this.OnValueChanged(EventArgs.Empty);
		}

		private void dateTimePicker_ValueChanged(object sender, EventArgs e)
		{
			this.OnValueChanged(EventArgs.Empty);
		}

		[DefaultValue(false)]
		public bool Checked
		{
			get
			{
				return this.titleCheckBox.Checked;
			}
			set
			{
				this.titleCheckBox.Checked = value;
			}
		}

		protected virtual void OnCheckedChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[CustomDateTimePicker.EventCheckedChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler CheckedChanged
		{
			add
			{
				base.Events.AddHandler(CustomDateTimePicker.EventCheckedChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(CustomDateTimePicker.EventCheckedChanged, value);
			}
		}

		[DefaultValue("")]
		public string TitleText
		{
			get
			{
				return this.titleCheckBox.Text;
			}
			set
			{
				this.titleCheckBox.Text = value;
			}
		}

		[DefaultValue(null)]
		public DateTime? Value
		{
			get
			{
				if (!this.titleCheckBox.Checked)
				{
					return null;
				}
				return new DateTime?(this.exchangeDateTimePicker.Value);
			}
			set
			{
				if (value != this.Value)
				{
					if (value != null)
					{
						this.exchangeDateTimePicker.Value = value.Value;
						this.Checked = true;
					}
					else
					{
						this.Checked = false;
					}
					this.OnValueChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnValueChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[CustomDateTimePicker.EventValueChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler ValueChanged
		{
			add
			{
				base.Events.AddHandler(CustomDateTimePicker.EventValueChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(CustomDateTimePicker.EventValueChanged, value);
			}
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "Value";
			}
		}

		private AutoHeightCheckBox titleCheckBox;

		private ExchangeDateTimePicker exchangeDateTimePicker;

		private AutoTableLayoutPanel tableLayoutPanel;

		private static readonly object EventCheckedChanged = new object();

		private static readonly object EventValueChanged = new object();
	}
}
