using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("MessageBox", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class ModalDialog : ScriptControlBase
	{
		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("Buttons", this.Buttons);
			if (this.PopupAtStartup)
			{
				descriptor.AddProperty("PopupAtStartup", true);
			}
			if (this.infos != null)
			{
				descriptor.AddProperty("Infos", this.infos);
			}
		}

		[Browsable(false)]
		public bool PopupAtStartup
		{
			get
			{
				return this.popupAtStartup;
			}
			set
			{
				this.popupAtStartup = value;
			}
		}

		[Browsable(false)]
		public MessageBoxButton Buttons
		{
			get
			{
				return this.buttons;
			}
			set
			{
				this.buttons = value;
			}
		}

		public ModalDialog()
		{
			this.EnableViewState = false;
		}

		public static ModalDialog GetCurrent(Page page)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			return (ModalDialog)page.Items[typeof(ModalDialog)];
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (!base.DesignMode)
			{
				ModalDialog current = ModalDialog.GetCurrent(this.Page);
				if (current != null)
				{
					throw new InvalidOperationException("Only one instance of ModalDialog is allowed in one page.");
				}
				this.Page.Items[typeof(ModalDialog)] = this;
			}
		}

		internal void ShowDialog(string title, string message, string details, ModalDialogType dialogType)
		{
			this.ThrowInvalidOperationExceptionIfShowDialogIsPending();
			InfoCore infoCore = new InfoCore
			{
				JsonTitle = title,
				Message = message,
				Details = details,
				MessageBoxType = dialogType
			};
			this.infos = new InfoCore[]
			{
				infoCore
			};
			this.PopupAtStartup = true;
		}

		internal void ShowDialog(InfoCore[] infos)
		{
			this.ThrowInvalidOperationExceptionIfShowDialogIsPending();
			this.infos = infos;
			this.PopupAtStartup = true;
		}

		protected override void CreateChildControls()
		{
			Panel panel = new Panel();
			panel.ID = "frm";
			this.Controls.Add(panel);
			this.modalPopupExtender = new ModalPopupExtender();
			this.modalPopupExtender.ID = "modalpopupextenderDialog";
			this.modalPopupExtender.BackgroundCssClass = "ModalDlgBackground";
			this.modalPopupExtender.BehaviorID = "modalpopupextenderDialog";
			this.modalPopupExtender.PopupControlID = this.ClientID;
			this.modalPopupExtender.TargetControlID = "hiddenPanel";
			Panel panel2 = new Panel();
			panel2.ID = "hiddenPanel";
			panel2.Attributes.Add("style", "display:none;");
			this.Controls.Add(panel2);
			this.Controls.Add(this.modalPopupExtender);
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			string cssClass = this.CssClass;
			if (string.IsNullOrEmpty(cssClass))
			{
				this.CssClass = "ModalDlg";
			}
			else
			{
				this.CssClass += " ModalDlg";
			}
			base.AddAttributesToRender(writer);
			this.CssClass = cssClass;
			writer.AddAttribute(HtmlTextWriterAttribute.Style, "display:none");
		}

		private void ThrowInvalidOperationExceptionIfShowDialogIsPending()
		{
			if (this.PopupAtStartup)
			{
				string message = string.Format("Cannot change ModalDialog.{0} property while a request to show dialog is pending.", new object[0]);
				throw new InvalidOperationException(message);
			}
		}

		private ModalPopupExtender modalPopupExtender;

		private InfoCore[] infos;

		private bool popupAtStartup;

		private MessageBoxButton buttons = MessageBoxButton.OK;
	}
}
