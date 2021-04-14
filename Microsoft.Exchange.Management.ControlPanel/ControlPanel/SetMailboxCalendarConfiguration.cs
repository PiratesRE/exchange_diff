using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetMailboxCalendarConfiguration : SetResourceConfigurationBase
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-MailboxCalendarConfiguration";
			}
		}

		[DataMember]
		public bool DisableReminders
		{
			get
			{
				return (bool)(base["RemindersEnabled"] ?? true);
			}
			set
			{
				base["RemindersEnabled"] = !value;
			}
		}
	}
}
