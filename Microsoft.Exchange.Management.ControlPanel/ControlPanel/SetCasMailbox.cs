using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SetCasMailbox : SetObjectProperties
	{
		[DataMember]
		public bool? ActiveSyncEnabled
		{
			get
			{
				return (bool?)base["ActiveSyncEnabled"];
			}
			set
			{
				base["ActiveSyncEnabled"] = value;
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-CasMailbox";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self|Organization";
			}
		}
	}
}
