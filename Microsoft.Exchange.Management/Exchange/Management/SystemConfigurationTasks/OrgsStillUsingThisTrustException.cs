using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OrgsStillUsingThisTrustException : FederationException
	{
		public OrgsStillUsingThisTrustException(string trust, string orgList) : base(Strings.ErrorOrgsStillUsingThisTrust(trust, orgList))
		{
			this.trust = trust;
			this.orgList = orgList;
		}

		public OrgsStillUsingThisTrustException(string trust, string orgList, Exception innerException) : base(Strings.ErrorOrgsStillUsingThisTrust(trust, orgList), innerException)
		{
			this.trust = trust;
			this.orgList = orgList;
		}

		protected OrgsStillUsingThisTrustException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.trust = (string)info.GetValue("trust", typeof(string));
			this.orgList = (string)info.GetValue("orgList", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("trust", this.trust);
			info.AddValue("orgList", this.orgList);
		}

		public string Trust
		{
			get
			{
				return this.trust;
			}
		}

		public string OrgList
		{
			get
			{
				return this.orgList;
			}
		}

		private readonly string trust;

		private readonly string orgList;
	}
}
