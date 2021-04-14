using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatacenterMissingHostsPermanentException : MailboxReplicationPermanentException
	{
		public DatacenterMissingHostsPermanentException(string datacenterName) : base(MrsStrings.DatacenterMissingHosts(datacenterName))
		{
			this.datacenterName = datacenterName;
		}

		public DatacenterMissingHostsPermanentException(string datacenterName, Exception innerException) : base(MrsStrings.DatacenterMissingHosts(datacenterName), innerException)
		{
			this.datacenterName = datacenterName;
		}

		protected DatacenterMissingHostsPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.datacenterName = (string)info.GetValue("datacenterName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("datacenterName", this.datacenterName);
		}

		public string DatacenterName
		{
			get
			{
				return this.datacenterName;
			}
		}

		private readonly string datacenterName;
	}
}
