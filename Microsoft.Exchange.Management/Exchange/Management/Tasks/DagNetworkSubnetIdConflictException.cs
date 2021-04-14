using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagNetworkSubnetIdConflictException : LocalizedException
	{
		public DagNetworkSubnetIdConflictException(string subnetId1, string subnetId2) : base(Strings.DagNetworkSubnetIdConflictError(subnetId1, subnetId2))
		{
			this.subnetId1 = subnetId1;
			this.subnetId2 = subnetId2;
		}

		public DagNetworkSubnetIdConflictException(string subnetId1, string subnetId2, Exception innerException) : base(Strings.DagNetworkSubnetIdConflictError(subnetId1, subnetId2), innerException)
		{
			this.subnetId1 = subnetId1;
			this.subnetId2 = subnetId2;
		}

		protected DagNetworkSubnetIdConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.subnetId1 = (string)info.GetValue("subnetId1", typeof(string));
			this.subnetId2 = (string)info.GetValue("subnetId2", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("subnetId1", this.subnetId1);
			info.AddValue("subnetId2", this.subnetId2);
		}

		public string SubnetId1
		{
			get
			{
				return this.subnetId1;
			}
		}

		public string SubnetId2
		{
			get
			{
				return this.subnetId2;
			}
		}

		private readonly string subnetId1;

		private readonly string subnetId2;
	}
}
