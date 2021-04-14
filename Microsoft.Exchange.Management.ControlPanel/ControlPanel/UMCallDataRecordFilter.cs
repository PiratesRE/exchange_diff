using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMCallDataRecordFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-UMCallDataRecord";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
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

		public const string RbacParameters = "?Mailbox";
	}
}
