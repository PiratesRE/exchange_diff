using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class PromptDialog : PropertyPageDialog
	{
		public PromptDialog() : this(true)
		{
		}

		public PromptDialog(bool canInputMessage) : this(canInputMessage, false)
		{
		}

		public PromptDialog(bool canInputMessage, bool multiline)
		{
			this.InitializeComponent();
			this.InitializeTextBoxContent(multiline);
			this.labelMessage.UseMnemonic = false;
			base.RegisterPropertyPage(this.panelAll);
			this.canInputMessage = canInputMessage;
			this.textBoxContent.ReadOnly = !this.canInputMessage;
			this.AllowEmpty = false;
			this.Message = "";
			this.textBoxContent.TextChanged += delegate(object param0, EventArgs param1)
			{
				base.OkEnabled = this.CanOkEnabled;
			};
			base.Shown += delegate(object param0, EventArgs param1)
			{
				this.SetButtons();
				MessageBoxDefaultButton messageBoxDefaultButton = this.DefaultButton;
				if (messageBoxDefaultButton != MessageBoxDefaultButton.Button1)
				{
					if (messageBoxDefaultButton == MessageBoxDefaultButton.Button2)
					{
						((Button)base.CancelButton).Select();
						goto IL_5F;
					}
					if (messageBoxDefaultButton == MessageBoxDefaultButton.Button3)
					{
						goto IL_5F;
					}
				}
				if (this.textBoxContent.ReadOnly)
				{
					((Button)base.AcceptButton).Select();
				}
				else
				{
					this.textBoxContent.Select();
				}
				IL_5F:
				if (this.InputMaxLength != 0)
				{
					this.textBoxContent.MaxLength = this.InputMaxLength;
				}
			};
		}

		[DefaultValue(0)]
		public int InputMaxLength
		{
			get
			{
				return this.textMaxLength;
			}
			set
			{
				this.textMaxLength = value;
			}
		}

		[DefaultValue(false)]
		public bool AllowEmpty
		{
			get
			{
				return this.allowEmpty;
			}
			set
			{
				this.allowEmpty = value;
				base.OkEnabled = this.CanOkEnabled;
			}
		}

		private bool CanOkEnabled
		{
			get
			{
				return this.AllowEmpty || !this.canInputMessage || !string.IsNullOrEmpty(this.ContentText);
			}
		}

		[DefaultValue(null)]
		public object DataSource
		{
			get
			{
				return this.panelAll.BindingSource.DataSource;
			}
			set
			{
				this.panelAll.BindingSource.DataSource = value;
			}
		}

		[DefaultValue("")]
		public string ValueMember
		{
			get
			{
				return this.valueMember;
			}
			set
			{
				if (this.ValueMember != value)
				{
					this.valueMember = (value ?? string.Empty);
					this.textBoxContent.DataBindings.Clear();
					if (!string.IsNullOrEmpty(this.ValueMember))
					{
						Binding binding = this.textBoxContent.DataBindings.Add("Text", this.panelAll.BindingSource, this.ValueMember, true, DataSourceUpdateMode.OnValidation);
						binding.Parse += delegate(object sender, ConvertEventArgs e)
						{
							ConvertEventHandler convertEventHandler = (ConvertEventHandler)base.Events[PromptDialog.EventParse];
							if (convertEventHandler != null)
							{
								convertEventHandler(sender, e);
							}
						};
					}
				}
			}
		}

		public event ConvertEventHandler Parse
		{
			add
			{
				base.Events.AddHandler(PromptDialog.EventParse, value);
			}
			remove
			{
				base.Events.RemoveHandler(PromptDialog.EventParse, value);
			}
		}

		[DefaultValue("")]
		public string Message
		{
			get
			{
				return this.labelMessage.Text;
			}
			set
			{
				this.labelMessage.Text = value;
				this.labelMessage.Visible = !string.IsNullOrEmpty(value);
			}
		}

		[DefaultValue("")]
		public string ContentText
		{
			get
			{
				return this.textBoxContent.Text;
			}
			set
			{
				this.textBoxContent.Text = value;
			}
		}

		public string ExampleText
		{
			get
			{
				return this.exampleLabel.Text;
			}
			set
			{
				this.exampleLabel.Text = value;
				this.exampleLabel.Visible = !string.IsNullOrEmpty(value);
			}
		}

		private void InitializeTextBoxContent(bool multiline)
		{
			base.SuspendLayout();
			this.tableLayoutPanel.SuspendLayout();
			this.textBoxContent.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.textBoxContent.Name = "textBoxContent";
			if (multiline)
			{
				this.textBoxContentPanel = new AutoSizePanel();
				this.textBoxContentPanel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
				this.textBoxContentPanel.BackColor = SystemColors.Window;
				this.textBoxContentPanel.Location = new Point(66, 53);
				this.textBoxContentPanel.Margin = new Padding(3, 0, 0, 0);
				this.textBoxContentPanel.Name = "textBoxContentPanel";
				this.textBoxContentPanel.Size = new Size(346, 100);
				this.textBoxContentPanel.TabIndex = 3;
				this.textBoxContent.Multiline = true;
				this.textBoxContent.Margin = new Padding(0);
				this.textBoxContent.TabIndex = 0;
				this.textBoxContent.ScrollBars = ScrollBars.Vertical;
				this.textBoxContent.Size = new Size(346, 100);
				this.textBoxContent.AcceptsReturn = true;
				this.tableLayoutPanel.Controls.Add(this.textBoxContentPanel, 1, 2);
				this.textBoxContentPanel.Controls.Add(this.textBoxContent);
			}
			else
			{
				this.textBoxContent.Margin = new Padding(3, 0, 0, 0);
				this.textBoxContent.TabIndex = 3;
				this.textBoxContent.AcceptsReturn = false;
				this.tableLayoutPanel.Controls.Add(this.textBoxContent, 1, 2);
			}
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[DefaultValue("")]
		public string ContentLabel
		{
			get
			{
				return this.labelContentLabel.Text;
			}
			set
			{
				this.labelContentLabel.Text = value;
			}
		}

		[DefaultValue("")]
		public string Title
		{
			get
			{
				return this.Text;
			}
			set
			{
				this.Text = value;
			}
		}

		[DefaultValue(0)]
		public DisplayFormatMode DisplayFormatMode
		{
			get
			{
				return this.textBoxContent.FormatMode;
			}
			set
			{
				this.textBoxContent.FormatMode = value;
			}
		}

		[DefaultValue(MessageBoxButtons.YesNo)]
		public MessageBoxButtons Buttons
		{
			get
			{
				return this.buttons;
			}
			set
			{
				if (value != MessageBoxButtons.YesNo && value != MessageBoxButtons.OK && value != MessageBoxButtons.OKCancel)
				{
					throw new ArgumentOutOfRangeException("Buttons");
				}
				if (this.buttons != value)
				{
					this.buttons = value;
					this.SetButtons();
				}
			}
		}

		[DefaultValue(MessageBoxDefaultButton.Button1)]
		public MessageBoxDefaultButton DefaultButton
		{
			get
			{
				return this.defaultButton;
			}
			set
			{
				if (value != MessageBoxDefaultButton.Button1 && value != MessageBoxDefaultButton.Button2)
				{
					throw new ArgumentOutOfRangeException("DefaultButton");
				}
				this.defaultButton = value;
			}
		}

		[DefaultValue(MessageBoxIcon.None)]
		public MessageBoxIcon MessageIcon
		{
			get
			{
				return this.messageIcon;
			}
			set
			{
				if (value != this.messageIcon)
				{
					Icon icon = null;
					if (value <= MessageBoxIcon.Question)
					{
						if (value != MessageBoxIcon.Hand)
						{
							if (value == MessageBoxIcon.Question)
							{
								icon = Icons.Help;
							}
						}
						else
						{
							icon = Icons.Error;
						}
					}
					else if (value != MessageBoxIcon.Exclamation)
					{
						if (value == MessageBoxIcon.Asterisk)
						{
							icon = Icons.Information;
						}
					}
					else
					{
						icon = Icons.Warning;
					}
					this.pictureBoxIcon.Image = ((icon == null) ? null : icon.ToBitmap());
					this.panelIcon.Visible = (null != icon);
					this.pictureBoxIcon.Visible = (null != icon);
				}
			}
		}

		private void SetButtons()
		{
			switch (this.Buttons)
			{
			case MessageBoxButtons.OK:
			case MessageBoxButtons.OKCancel:
				base.OkButtonText = Strings.Ok;
				base.CancelButtonText = Strings.Cancel;
				base.CancelVisible = (this.Buttons == MessageBoxButtons.OKCancel);
				base.AcceptButton.DialogResult = DialogResult.OK;
				base.CancelButton.DialogResult = DialogResult.Cancel;
				return;
			case MessageBoxButtons.YesNo:
				base.OkButtonText = Strings.Yes;
				base.CancelButtonText = Strings.No;
				base.CancelVisible = true;
				base.AcceptButton.DialogResult = DialogResult.Yes;
				base.CancelButton.DialogResult = DialogResult.No;
				return;
			}
			throw new NotSupportedException();
		}

		protected override string DefaultHelpTopic
		{
			get
			{
				return string.Empty;
			}
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			base.HelpVisible = (base.HelpVisible && !string.IsNullOrEmpty(base.HelpTopic));
		}

		protected override void OnHelpRequested(HelpEventArgs hevent)
		{
			if (!hevent.Handled && base.HelpVisible)
			{
				if (base.HelpTopic == this.DefaultHelpTopic && this.DataSource != null)
				{
					base.HelpTopic = string.Format("{0}.{1}.{2}", base.GetType().FullName, this.DataSource.GetType().Name, this.ValueMember);
				}
				ExchangeHelpService.ShowHelpFromHelpTopicId(this, base.HelpTopic);
				hevent.Handled = true;
			}
			base.OnHelpRequested(hevent);
		}

		private bool canInputMessage;

		private int textMaxLength;

		private bool allowEmpty;

		private string valueMember = string.Empty;

		private static readonly object EventParse = new object();

		private MessageBoxButtons buttons = MessageBoxButtons.YesNo;

		private MessageBoxDefaultButton defaultButton;

		private MessageBoxIcon messageIcon;

		private AutoSizePanel textBoxContentPanel;
	}
}
