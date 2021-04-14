using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Supervision;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SupervisionStatus : BaseRow
	{
		public SupervisionStatus(SupervisionPolicy supervisionStatus) : base(null, supervisionStatus)
		{
			this.MySupervisionStatus = supervisionStatus;
		}

		[DataMember]
		public bool ClosedCampusPolicyEnabled
		{
			get
			{
				return this.MySupervisionStatus.ClosedCampusInboundPolicyEnabled && this.MySupervisionStatus.ClosedCampusOutboundPolicyEnabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<RecipientObjectResolverRow> ClosedCampusInboundGroupExceptions
		{
			get
			{
				if (this.MySupervisionStatus.ClosedCampusInboundPolicyGroupExceptions == null)
				{
					return null;
				}
				return RecipientObjectResolver.Instance.ResolveSmtpAddress(this.MySupervisionStatus.ClosedCampusInboundPolicyGroupExceptions.ToArray());
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string[] ClosedCampusInboundDomainExceptions
		{
			get
			{
				if (this.MySupervisionStatus.ClosedCampusInboundPolicyDomainExceptions == null)
				{
					return null;
				}
				return this.MySupervisionStatus.ClosedCampusInboundPolicyDomainExceptions.ToStringArray<SmtpDomain>();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool BadWordsPolicyEnabled
		{
			get
			{
				return this.MySupervisionStatus.BadWordsPolicyEnabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string BadWordsList
		{
			get
			{
				if (this.MySupervisionStatus.BadWordsList != null)
				{
					return this.MySupervisionStatus.BadWordsList;
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AntiBullyingPolicyEnabled
		{
			get
			{
				return this.MySupervisionStatus.AntiBullyingPolicyEnabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private readonly SupervisionPolicy MySupervisionStatus;
	}
}
