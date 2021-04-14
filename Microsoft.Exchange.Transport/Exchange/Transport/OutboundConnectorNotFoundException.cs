using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OutboundConnectorNotFoundException : ExchangeConfigurationException
	{
		public OutboundConnectorNotFoundException(string name, OrganizationId orgId) : base(Strings.OutboundConnectorNotFound(name, orgId))
		{
			this.name = name;
			this.orgId = orgId;
		}

		public OutboundConnectorNotFoundException(string name, OrganizationId orgId, Exception innerException) : base(Strings.OutboundConnectorNotFound(name, orgId), innerException)
		{
			this.name = name;
			this.orgId = orgId;
		}

		protected OutboundConnectorNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.orgId = (OrganizationId)info.GetValue("orgId", typeof(OrganizationId));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("orgId", this.orgId);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public OrganizationId OrgId
		{
			get
			{
				return this.orgId;
			}
		}

		private readonly string name;

		private readonly OrganizationId orgId;
	}
}
