using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Mobility;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[Cmdlet("Disable", "PushNotificationProxy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	[OutputType(new Type[]
	{
		typeof(PushNotificationProxyPresentationObject)
	})]
	public sealed class DisablePushNotificationProxy : ProxyCmdletBaseClass
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageDisableProxy(this.Identity.ToString(), base.CurrentProxyStatus);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			PushNotificationApp pushNotificationApp = (PushNotificationApp)base.PrepareDataObject();
			pushNotificationApp.Enabled = new bool?(false);
			return pushNotificationApp;
		}
	}
}
