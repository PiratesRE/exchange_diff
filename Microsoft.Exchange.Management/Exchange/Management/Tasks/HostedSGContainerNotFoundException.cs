using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class HostedSGContainerNotFoundException : LocalizedException
	{
		public HostedSGContainerNotFoundException(string orgName) : base(Strings.HostedSGContainerNotFoundException(orgName))
		{
			this.orgName = orgName;
		}

		public HostedSGContainerNotFoundException(string orgName, Exception innerException) : base(Strings.HostedSGContainerNotFoundException(orgName), innerException)
		{
			this.orgName = orgName;
		}

		protected HostedSGContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.orgName = (string)info.GetValue("orgName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("orgName", this.orgName);
		}

		public string OrgName
		{
			get
			{
				return this.orgName;
			}
		}

		private readonly string orgName;
	}
}
