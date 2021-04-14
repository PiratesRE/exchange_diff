using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class ExchangeServerRole : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return ExchangeServerRole.schema;
			}
		}

		public ExchangeServerRole()
		{
		}

		public ExchangeServerRole(Server dataObject) : base(dataObject)
		{
		}

		[Parameter(Mandatory = false)]
		public bool IsHubTransportServer
		{
			get
			{
				return (bool)this[ExchangeServerRoleSchema.IsHubTransportServer];
			}
			set
			{
				this[ExchangeServerRoleSchema.IsHubTransportServer] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsClientAccessServer
		{
			get
			{
				return (bool)this[ExchangeServerRoleSchema.IsClientAccessServer];
			}
			set
			{
				this[ExchangeServerRoleSchema.IsClientAccessServer] = value;
			}
		}

		public bool IsExchange2007OrLater
		{
			get
			{
				return (bool)this[ExchangeServerRoleSchema.IsExchange2007OrLater];
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsEdgeServer
		{
			get
			{
				return (bool)this[ExchangeServerRoleSchema.IsEdgeServer];
			}
			set
			{
				this[ExchangeServerRoleSchema.IsEdgeServer] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsMailboxServer
		{
			get
			{
				return (bool)this[ExchangeServerRoleSchema.IsMailboxServer];
			}
			set
			{
				this[ExchangeServerRoleSchema.IsMailboxServer] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsUnifiedMessagingServer
		{
			get
			{
				return (bool)this[ExchangeServerRoleSchema.IsUnifiedMessagingServer];
			}
			set
			{
				this[ExchangeServerRoleSchema.IsUnifiedMessagingServer] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsProvisionedServer
		{
			get
			{
				return (bool)this[ExchangeServerRoleSchema.IsProvisionedServer];
			}
			set
			{
				this[ExchangeServerRoleSchema.IsProvisionedServer] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsCafeServer
		{
			get
			{
				return (bool)this[ExchangeServerRoleSchema.IsCafeServer];
			}
			set
			{
				this[ExchangeServerRoleSchema.IsCafeServer] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsFrontendTransportServer
		{
			get
			{
				return (bool)this[ExchangeServerRoleSchema.IsFrontendTransportServer];
			}
			set
			{
				this[ExchangeServerRoleSchema.IsFrontendTransportServer] = value;
			}
		}

		private static ExchangeServerRoleSchema schema = ObjectSchema.GetInstance<ExchangeServerRoleSchema>();
	}
}
