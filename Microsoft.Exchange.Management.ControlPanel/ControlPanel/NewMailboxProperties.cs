using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("NewMailboxProperties", "Microsoft.Exchange.Management.ControlPanel.Client.Mailbox.js")]
	public sealed class NewMailboxProperties : Properties, IScriptControl
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (base.ObjectIdentity != null)
			{
				BaseForm baseForm = this.Page as BaseForm;
				if (baseForm != null)
				{
					baseForm.Title = (baseForm.Caption = Strings.RecoverMailboxCaption);
					baseForm.CommitButtonText = Strings.RecoverButtonText;
					baseForm.HelpId = EACHelpId.RecoverMailbox.ToString();
				}
			}
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			Control control = this.FindControl("SoftDeletedDataSection");
			if (control != null)
			{
				scriptDescriptor.AddElementProperty("SoftDeletedDataSection", control.ClientID, true);
			}
			return scriptDescriptor;
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptDescriptor = this.GetScriptDescriptor();
			scriptDescriptor.Type = "NewMailboxProperties";
			return new ScriptControlDescriptor[]
			{
				scriptDescriptor
			};
		}
	}
}
