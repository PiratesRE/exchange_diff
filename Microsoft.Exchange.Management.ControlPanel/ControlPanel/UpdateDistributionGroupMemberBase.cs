using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class UpdateDistributionGroupMemberBase : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Update-DistributionGroupMember";
			}
		}

		[DataMember]
		public Identity[] Members
		{
			get
			{
				return (Identity[])base[DistributionGroupSchema.Members];
			}
			set
			{
				base[DistributionGroupSchema.Members] = value.ToIdParameters();
			}
		}
	}
}
