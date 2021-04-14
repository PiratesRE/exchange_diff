using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class PickerTextBoxBase : ExchangeUserControl, IFormatModeProvider, IBindableComponent, IComponent, IDisposable
	{
		public PickerTextBoxBase()
		{
			this.InitializeComponent();
			base.Size = this.DefaultSize;
			this.browseButton.Text = Strings.BrowseButtonText;
			this.textBox.TextChanged += this.textBox_TextChanged;
			this.textBox.Validating += this.textBox_Validating;
			this.textBox.FormatModeChanged += delegate(object param0, EventArgs param1)
			{
				this.OnFormatModeChanged(EventArgs.Empty);
			};
			this.textBox.FocusSetted += delegate(object param0, EventArgs param1)
			{
				this.textBox.Modified = false;
			};
		}

		private void textBox_Validating(object sender, CancelEventArgs e)
		{
			this.RaiseValidatingEvent();
		}

		private void RaiseValidatingEvent()
		{
			if (!this.TextBoxReadOnly && this.textBox.Modified)
			{
				this.OnValidating(new CancelEventArgs());
			}
		}

		private void textBox_TextChanged(object sender, EventArgs e)
		{
			this.OnTextChanged(EventArgs.Empty);
		}

		[DefaultValue(true)]
		public bool TextBoxReadOnly
		{
			get
			{
				return this.textBoxReadOnly;
			}
			set
			{
				this.InitTextBoxReadOnly(value, this, this.ExposedPropertyName);
			}
		}

		internal void InitTextBoxReadOnly(bool readOnly, IFormatModeProvider owner, string bindingPropertyName)
		{
			if (this.textBox.IsHandleCreated)
			{
				this.textBox.ReadOnly = readOnly;
			}
			this.textBoxReadOnly = readOnly;
			if (!this.TextBoxReadOnly)
			{
				new TextBoxConstraintProvider(owner, bindingPropertyName, this.textBox);
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			this.textBox.ReadOnly = this.TextBoxReadOnly;
			base.OnHandleCreated(e);
		}

		private void BrowseButton_Click(object sender, EventArgs e)
		{
			CancelEventArgs cancelEventArgs = new CancelEventArgs();
			this.OnBrowseButtonClick(cancelEventArgs);
			if (!cancelEventArgs.Cancel)
			{
				this.RaiseValidatingEvent();
			}
		}

		protected virtual void OnBrowseButtonClick(CancelEventArgs e)
		{
			if (this.BrowseButtonClick != null)
			{
				this.BrowseButtonClick(this, e);
			}
		}

		public event CancelEventHandler BrowseButtonClick;

		protected override Size DefaultSize
		{
			get
			{
				return new Size(336, 25);
			}
		}

		protected override Padding DefaultMargin
		{
			get
			{
				return new Padding(0);
			}
		}

		public override string Text
		{
			get
			{
				return this.textBox.Text;
			}
			set
			{
				this.textBox.Text = value;
			}
		}

		[DefaultValue("")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ToolTipText
		{
			get
			{
				if (this.toolTip != null)
				{
					return this.toolTip.GetToolTip(this.textBox);
				}
				return string.Empty;
			}
			set
			{
				if (this.toolTip == null)
				{
					this.toolTip = new ToolTip();
				}
				this.toolTip.SetToolTip(this.textBox, value);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string ButtonText
		{
			get
			{
				return this.browseButton.Text;
			}
			set
			{
				this.browseButton.Text = value;
			}
		}

		[DefaultValue(true)]
		public bool CanBrowse
		{
			get
			{
				return this.canBrowse;
			}
			set
			{
				if (this.CanBrowse != value)
				{
					this.canBrowse = value;
					this.UpdateBrowseButtonState();
					this.OnCanBrowseChanged(EventArgs.Empty);
				}
			}
		}

		protected void UpdateBrowseButtonState()
		{
			this.browseButton.Enabled = (this.CanBrowse && this.ButtonAvailable());
		}

		protected virtual bool ButtonAvailable()
		{
			return true;
		}

		protected virtual void OnCanBrowseChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[PickerTextBoxBase.EventCanBrowseChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler CanBrowseChanged
		{
			add
			{
				base.Events.AddHandler(PickerTextBoxBase.EventCanBrowseChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(PickerTextBoxBase.EventCanBrowseChanged, value);
			}
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
			this.browseButton = new ExchangeButton();
			this.tableLayoutPanel = new TableLayoutPanel();
			this.textBox = new ExchangeTextBox();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.browseButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.browseButton.AutoSize = true;
			this.browseButton.BackColor = SystemColors.Control;
			this.browseButton.Location = new Point(260, 0);
			this.browseButton.Margin = new Padding(3, 0, 0, 0);
			this.browseButton.MinimumSize = new Size(75, 23);
			this.browseButton.Name = "browseButton";
			this.browseButton.Size = new Size(76, 23);
			this.browseButton.TabIndex = 2;
			this.browseButton.UseVisualStyleBackColor = false;
			this.browseButton.UseCompatibleTextRendering = false;
			this.browseButton.Click += this.BrowseButton_Click;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel.Controls.Add(this.textBox, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.browseButton, 2, 0);
			this.tableLayoutPanel.Dock = DockStyle.Fill;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Size = new Size(336, 23);
			this.tableLayoutPanel.TabIndex = 3;
			this.textBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.textBox.Location = new Point(0, 2);
			this.textBox.Margin = new Padding(3, 2, 0, 1);
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = true;
			this.textBox.Size = new Size(252, 20);
			this.textBox.TabIndex = 1;
			base.Controls.Add(this.tableLayoutPanel);
			base.Margin = new Padding(0);
			base.Name = "PickerTextBoxBase";
			base.Size = new Size(336, 23);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[DefaultValue(0)]
		public DisplayFormatMode FormatMode
		{
			get
			{
				return this.textBox.FormatMode;
			}
			set
			{
				this.textBox.FormatMode = value;
			}
		}

		protected virtual void OnFormatModeChanged(EventArgs e)
		{
			if (this.FormatModeChanged != null)
			{
				this.FormatModeChanged(this, e);
			}
		}

		public event EventHandler FormatModeChanged;

		void IFormatModeProvider.add_BindingContextChanged(EventHandler A_1)
		{
			base.BindingContextChanged += A_1;
		}

		void IFormatModeProvider.remove_BindingContextChanged(EventHandler A_1)
		{
			base.BindingContextChanged -= A_1;
		}

		private bool textBoxReadOnly = true;

		private ToolTip toolTip;

		private bool canBrowse = true;

		private static readonly object EventCanBrowseChanged = new object();

		private IContainer components;

		private ExchangeButton browseButton;

		private ExchangeTextBox textBox;

		private TableLayoutPanel tableLayoutPanel;
	}
}
