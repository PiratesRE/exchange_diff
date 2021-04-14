using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class BaseErrorDialog : ExchangeForm
	{
		protected BaseErrorDialog()
		{
			this.InitializeComponent();
			this.iconBox.Image = IconLibrary.ToBitmap(Icons.Error, this.iconBox.Size);
			this.cancelButton.Text = Strings.Ok;
			this.messageLabel.UseMnemonic = false;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys)131139)
			{
				this.CopyTechnicalDetails();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void CopyTechnicalDetails()
		{
			using (new ControlWaitCursor(this))
			{
				WinformsHelper.SetDataObjectToClipboard(this.TechnicalDetails, true);
			}
		}

		public virtual string TechnicalDetails
		{
			get
			{
				return string.Empty;
			}
		}

		public string Message
		{
			get
			{
				return this.messageLabel.Text;
			}
			set
			{
				this.messageLabel.Text = value;
			}
		}

		public bool IsWarningOnly
		{
			get
			{
				return this.isWarningOnly;
			}
			set
			{
				if (value != this.IsWarningOnly)
				{
					this.isWarningOnly = value;
					this.OnIsWarningOnlyChanged();
				}
			}
		}

		protected virtual void OnIsWarningOnlyChanged()
		{
			this.iconBox.Image = (this.IsWarningOnly ? Icons.Warning : Icons.Error).ToBitmap();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Panel ContentPanel
		{
			get
			{
				return this.contentPanel;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TableLayoutPanel ButtonsPanel
		{
			get
			{
				return this.buttonsPanel;
			}
		}

		protected override string DefaultHelpTopic
		{
			get
			{
				return string.Empty;
			}
		}

		private bool isWarningOnly;
	}
}
