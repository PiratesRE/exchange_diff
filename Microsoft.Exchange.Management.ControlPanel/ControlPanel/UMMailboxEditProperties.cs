using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("UMMailboxEditProperties", "Microsoft.Exchange.Management.ControlPanel.Client.UnifiedMessaging.js")]
	public sealed class UMMailboxEditProperties : Properties
	{
		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "UMMailboxEditProperties";
			Section section = base.Sections["UMMailboxConfigurationSection"];
			scriptDescriptor.AddElementProperty("LockedOutStatusLabel", section.FindControl("txtLockedOutStatus").ClientID);
			scriptDescriptor.AddElementProperty("LockedOutStatusActionLabel", section.FindControl("lblLockedOutStatusAction").ClientID);
			return scriptDescriptor;
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.SetResetPinUrl();
			base.OnPreRender(e);
			PowerShellResults<SetUMMailboxConfiguration> powerShellResults = (PowerShellResults<SetUMMailboxConfiguration>)base.Results;
			if (powerShellResults != null && powerShellResults.SucceededWithValue)
			{
				this.SetDynamicLabels(powerShellResults);
				this.SetAddExtensionUrl(powerShellResults);
			}
		}

		private void SetResetPinUrl()
		{
			Section section = base.Sections["UMMailboxConfigurationSection"];
			PopupLauncher popupLauncher = (PopupLauncher)section.FindControl("popupLauncherDataCenter");
			popupLauncher.NavigationUrl = EcpUrl.AppendQueryParameter(popupLauncher.NavigationUrl, "id", base.ObjectIdentity.RawIdentity);
		}

		private void SetAddExtensionUrl(PowerShellResults<SetUMMailboxConfiguration> result)
		{
			Section section = base.Sections["UMMailboxExtensionSection"];
			EcpCollectionEditor ecpCollectionEditor = (EcpCollectionEditor)section.FindControl("ceExtensions");
			ecpCollectionEditor.PickerFormUrl = EcpUrl.AppendQueryParameter(ecpCollectionEditor.PickerFormUrl, "dialPlanId", result.Value.DialPlanId);
		}

		private void SetDynamicLabels(PowerShellResults<SetUMMailboxConfiguration> result)
		{
			Section section = base.Sections["UMMailboxConfigurationSection"];
			Label label = (Label)section.FindControl("txtExtension_label");
			Label label2 = (Label)section.FindControl("lblLockedOutStatusAction");
			if (result.Output[0].IsSipDialPlan)
			{
				label.Text = Strings.UMMailboxSipLabel;
			}
			else if (result.Output[0].IsE164DialPlan)
			{
				label.Text = Strings.UMMailboxE164Label;
			}
			else
			{
				label.Text = Strings.UMMailboxExtensionLabel;
			}
			if (result.Output[0].AccountLockedOut)
			{
				label2.Style["display"] = "inline";
				return;
			}
			label2.Style["display"] = "none";
		}
	}
}
