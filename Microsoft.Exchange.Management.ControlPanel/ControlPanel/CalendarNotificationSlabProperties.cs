using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.SMSProperties.js")]
	public sealed class CalendarNotificationSlabProperties : SmsSlabProperties
	{
		public CalendarNotificationSlabProperties() : base(null, "../sms/EditNotification.aspx")
		{
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "CalendarNotificationSlabProperties";
			return scriptDescriptor;
		}
	}
}
