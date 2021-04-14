using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OrgStillUsingThisMailFlowPartnerException : LocalizedException
	{
		public OrgStillUsingThisMailFlowPartnerException(string trust, string org) : base(Strings.ErrorOrgStillUsingThisMailFlowPartner(trust, org))
		{
			this.trust = trust;
			this.org = org;
		}

		public OrgStillUsingThisMailFlowPartnerException(string trust, string org, Exception innerException) : base(Strings.ErrorOrgStillUsingThisMailFlowPartner(trust, org), innerException)
		{
			this.trust = trust;
			this.org = org;
		}

		protected OrgStillUsingThisMailFlowPartnerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.trust = (string)info.GetValue("trust", typeof(string));
			this.org = (string)info.GetValue("org", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("trust", this.trust);
			info.AddValue("org", this.org);
		}

		public string Trust
		{
			get
			{
				return this.trust;
			}
		}

		public string Org
		{
			get
			{
				return this.org;
			}
		}

		private readonly string trust;

		private readonly string org;
	}
}
