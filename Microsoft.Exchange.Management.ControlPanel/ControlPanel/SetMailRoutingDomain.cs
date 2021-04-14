using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetMailRoutingDomain : SetObjectProperties
	{
		[DataMember]
		public string DomainName
		{
			get
			{
				return (string)base["DomainName"];
			}
			set
			{
				base["DomainName"] = value;
			}
		}

		[DataMember]
		public string DomainType
		{
			get
			{
				return (string)base["DomainType"];
			}
			set
			{
				base["DomainType"] = value;
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-AcceptedDomain";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}
	}
}
