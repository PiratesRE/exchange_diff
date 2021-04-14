using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed partial class PromptForChoicesDialog : ExchangeForm
	{
		internal ConfirmationChoice UserChoice
		{
			get
			{
				return this.internalUserChoice;
			}
			set
			{
				this.internalUserChoice = value;
			}
		}

		public string Message
		{
			get
			{
				return this.warningMessageLabel.Text;
			}
			set
			{
				this.warningMessageLabel.Text = value;
			}
		}

		public PromptForChoicesDialog()
		{
			this.InitializeComponent();
			this.yesButton.Text = Strings.Yes;
			this.yesButton.Click += delegate(object param0, EventArgs param1)
			{
				this.UserChoice = ConfirmationChoice.Yes;
				base.Close();
			};
			this.yesToAllButton.Text = Strings.YesToAll;
			this.yesToAllButton.Click += delegate(object param0, EventArgs param1)
			{
				this.UserChoice = ConfirmationChoice.YesToAll;
				base.Close();
			};
			this.noButton.Text = Strings.No;
			this.noButton.Click += delegate(object param0, EventArgs param1)
			{
				this.UserChoice = ConfirmationChoice.No;
				base.Close();
			};
			this.cancelButton.Text = Strings.Cancel;
			this.cancelButton.Click += delegate(object param0, EventArgs param1)
			{
				this.UserChoice = ConfirmationChoice.NoToAll;
				base.Close();
			};
			this.warningIconPictureBox.Image = IconLibrary.ToBitmap(Icons.Warning, this.warningIconPictureBox.Size);
			base.AcceptButton = this.yesButton;
			base.CancelButton = this.cancelButton;
			this.UpdateButtonStatus(true);
		}

		internal PromptForChoicesDialog(string message, ConfirmationChoice defaultChoice) : this()
		{
			this.Message = message;
			switch (defaultChoice)
			{
			case ConfirmationChoice.Yes:
				base.AcceptButton = this.yesButton;
				return;
			case ConfirmationChoice.YesToAll:
				base.AcceptButton = this.yesToAllButton;
				return;
			case ConfirmationChoice.No:
				base.AcceptButton = this.noButton;
				return;
			default:
				base.AcceptButton = this.cancelButton;
				return;
			}
		}

		[DefaultValue(true)]
		public bool HasChoiceForMultipleObjects
		{
			get
			{
				return this.hasChoiceForMultipleObjects;
			}
			set
			{
				if (this.HasChoiceForMultipleObjects != value)
				{
					if (!value && base.AcceptButton == this.yesToAllButton)
					{
						throw new ArgumentException("Can not change to single confirmation mode when default choice is YesToAll");
					}
					this.UpdateButtonStatus(value);
					this.hasChoiceForMultipleObjects = value;
				}
			}
		}

		private void UpdateButtonStatus(bool isMultiple)
		{
			this.yesToAllButton.Visible = isMultiple;
			this.cancelButton.Visible = isMultiple;
			this.buttonsPanel.SuspendLayout();
			if (isMultiple)
			{
				this.buttonsPanel.ColumnStyles[0].Width = 25f;
				this.buttonsPanel.ColumnStyles[2].Width = 25f;
				this.buttonsPanel.ColumnStyles[3].Width = 8f;
				this.buttonsPanel.ColumnStyles[4].Width = 25f;
				this.buttonsPanel.ColumnStyles[5].Width = 8f;
				this.buttonsPanel.ColumnStyles[6].Width = 25f;
			}
			else
			{
				this.buttonsPanel.ColumnStyles[0].Width = 50f;
				this.buttonsPanel.ColumnStyles[2].Width = 0f;
				this.buttonsPanel.ColumnStyles[3].Width = 0f;
				this.buttonsPanel.ColumnStyles[4].Width = 50f;
				this.buttonsPanel.ColumnStyles[5].Width = 0f;
				this.buttonsPanel.ColumnStyles[6].Width = 0f;
			}
			this.buttonsPanel.ResumeLayout(false);
			this.buttonsPanel.PerformLayout();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Button button = base.AcceptButton as Button;
			if (button != null)
			{
				button.Select();
			}
			Size sz = base.Size - base.ClientSize;
			base.Size = this.tableLayoutPanel.Size + base.Padding.Size + sz;
		}

		private ConfirmationChoice internalUserChoice = ConfirmationChoice.NoToAll;

		private bool hasChoiceForMultipleObjects;
	}
}
