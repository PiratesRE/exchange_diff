using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DefaultAuthoritativeDomainNotFoundException : ExchangeConfigurationException
	{
		public DefaultAuthoritativeDomainNotFoundException(OrganizationId orgId) : base(Strings.DefaultAuthoritativeDomainNotFound(orgId))
		{
			this.orgId = orgId;
		}

		public DefaultAuthoritativeDomainNotFoundException(OrganizationId orgId, Exception innerException) : base(Strings.DefaultAuthoritativeDomainNotFound(orgId), innerException)
		{
			this.orgId = orgId;
		}

		protected DefaultAuthoritativeDomainNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.orgId = (OrganizationId)info.GetValue("orgId", typeof(OrganizationId));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("orgId", this.orgId);
		}

		public OrganizationId OrgId
		{
			get
			{
				return this.orgId;
			}
		}

		private readonly OrganizationId orgId;
	}
}
