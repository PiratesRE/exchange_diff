using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("UMBasePopupProperties", "Microsoft.Exchange.Management.ControlPanel.Client.UnifiedMessaging.js")]
	public abstract class UMBasePopupProperties : Properties
	{
		protected bool IsNewRequest
		{
			get
			{
				return this.newRequest;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.ValidateQueryStrings();
			base.CaptionTextField = string.Empty;
			PopupForm popupForm = (PopupForm)this.Page;
			popupForm.Title = string.Empty;
			this.SetTitleAndCaption(popupForm);
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.AddProperty("IsNewRequest", this.IsNewRequest);
			scriptDescriptor.Type = "UMBasePopupProperties";
			return scriptDescriptor;
		}

		protected virtual void ValidateQueryStrings()
		{
			if (!bool.TryParse(this.Page.Request["new"], out this.newRequest))
			{
				throw new BadQueryParameterException("new");
			}
		}

		protected abstract void SetTitleAndCaption(PopupForm form);

		private bool newRequest;
	}
}
