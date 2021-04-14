using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskServerCanNotContactClusterException : LocalizedException
	{
		public DagTaskServerCanNotContactClusterException(int numberOfOtherServers, string otherServers) : base(Strings.DagTaskServerCanNotContactClusterException(numberOfOtherServers, otherServers))
		{
			this.numberOfOtherServers = numberOfOtherServers;
			this.otherServers = otherServers;
		}

		public DagTaskServerCanNotContactClusterException(int numberOfOtherServers, string otherServers, Exception innerException) : base(Strings.DagTaskServerCanNotContactClusterException(numberOfOtherServers, otherServers), innerException)
		{
			this.numberOfOtherServers = numberOfOtherServers;
			this.otherServers = otherServers;
		}

		protected DagTaskServerCanNotContactClusterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.numberOfOtherServers = (int)info.GetValue("numberOfOtherServers", typeof(int));
			this.otherServers = (string)info.GetValue("otherServers", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("numberOfOtherServers", this.numberOfOtherServers);
			info.AddValue("otherServers", this.otherServers);
		}

		public int NumberOfOtherServers
		{
			get
			{
				return this.numberOfOtherServers;
			}
		}

		public string OtherServers
		{
			get
			{
				return this.otherServers;
			}
		}

		private readonly int numberOfOtherServers;

		private readonly string otherServers;
	}
}
