using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RemoveAccount : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Remove-Mailbox";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		[DataMember]
		public bool KeepWindowsLiveID
		{
			get
			{
				return (bool)(base["KeepWindowsLiveID"] ?? false);
			}
			set
			{
				base["KeepWindowsLiveID"] = value;
			}
		}
	}
}
