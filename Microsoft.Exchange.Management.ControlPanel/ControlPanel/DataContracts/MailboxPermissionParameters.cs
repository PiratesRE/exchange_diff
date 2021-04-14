using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel.DataContracts
{
	[DataContract]
	public abstract class MailboxPermissionParameters : SetObjectProperties
	{
		[DataMember]
		public Identity Identity
		{
			get
			{
				return (Identity)base["Identity"];
			}
			set
			{
				base["Identity"] = value;
			}
		}
	}
}
