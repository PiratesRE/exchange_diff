using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MobileDeviceFilter : SelfMailboxParameters
	{
		public MobileDeviceFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			base["ShowRecoveryPassword"] = new SwitchParameter(true);
			base["ActiveSync"] = new SwitchParameter(true);
		}

		[DataMember]
		public Identity Mailbox
		{
			get
			{
				return (Identity)base["Mailbox"];
			}
			set
			{
				base["Mailbox"] = value;
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-MobileDeviceStatistics";
			}
		}

		public override string RbacScope
		{
			get
			{
				return string.Empty;
			}
		}

		public new const string RbacParameters = "?Mailbox&ShowRecoveryPassword&ActiveSync";

		public new const string RbacParametersWithIdentity = "?Mailbox&ShowRecoveryPassword&ActiveSync&Identity";
	}
}
