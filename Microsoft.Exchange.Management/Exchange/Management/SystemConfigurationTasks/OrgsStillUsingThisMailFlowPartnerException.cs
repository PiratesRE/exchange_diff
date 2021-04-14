using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OrgsStillUsingThisMailFlowPartnerException : LocalizedException
	{
		public OrgsStillUsingThisMailFlowPartnerException(string trust, string org, string remainingCount) : base(Strings.ErrorOrgsStillUsingThisMailFlowPartner(trust, org, remainingCount))
		{
			this.trust = trust;
			this.org = org;
			this.remainingCount = remainingCount;
		}

		public OrgsStillUsingThisMailFlowPartnerException(string trust, string org, string remainingCount, Exception innerException) : base(Strings.ErrorOrgsStillUsingThisMailFlowPartner(trust, org, remainingCount), innerException)
		{
			this.trust = trust;
			this.org = org;
			this.remainingCount = remainingCount;
		}

		protected OrgsStillUsingThisMailFlowPartnerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.trust = (string)info.GetValue("trust", typeof(string));
			this.org = (string)info.GetValue("org", typeof(string));
			this.remainingCount = (string)info.GetValue("remainingCount", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("trust", this.trust);
			info.AddValue("org", this.org);
			info.AddValue("remainingCount", this.remainingCount);
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

		public string RemainingCount
		{
			get
			{
				return this.remainingCount;
			}
		}

		private readonly string trust;

		private readonly string org;

		private readonly string remainingCount;
	}
}
