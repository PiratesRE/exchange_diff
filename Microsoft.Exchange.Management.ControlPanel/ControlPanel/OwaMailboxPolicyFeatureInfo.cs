using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class OwaMailboxPolicyFeatureInfo : MailboxFeatureInfo
	{
		public OwaMailboxPolicyFeatureInfo(string parameter, bool enabled, string featureName) : base(new Identity(parameter, featureName))
		{
			base.UseModalDialogForEdit = false;
			base.Name = featureName;
			this.Status = (enabled ? ClientStrings.EnabledDisplayText : ClientStrings.DisabledDisplayText);
			base.EnableCommandUrl = null;
			base.EditCommandUrl = null;
			this.CanChangeStatus = RbacPrincipal.Current.IsInRole("Set-OwaMailboxPolicy?Identity&" + parameter + "@W:Organization");
		}
	}
}
