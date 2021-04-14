using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TenantScopedInboundConnectorRequiredForCMCConnectorException : LocalizedException
	{
		public TenantScopedInboundConnectorRequiredForCMCConnectorException(string name) : base(Strings.TenantScopedInboundConnectorRequiredForCMCConnector(name))
		{
			this.name = name;
		}

		public TenantScopedInboundConnectorRequiredForCMCConnectorException(string name, Exception innerException) : base(Strings.TenantScopedInboundConnectorRequiredForCMCConnector(name), innerException)
		{
			this.name = name;
		}

		protected TenantScopedInboundConnectorRequiredForCMCConnectorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
