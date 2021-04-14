using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class VirtualDirectoryIdParameter : ServerBasedIdParameter
	{
		public VirtualDirectoryIdParameter()
		{
		}

		public VirtualDirectoryIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public VirtualDirectoryIdParameter(ADVirtualDirectory virtualDirectory) : base(virtualDirectory.Id)
		{
		}

		public VirtualDirectoryIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected VirtualDirectoryIdParameter(string identity) : base(identity)
		{
		}

		internal string Name
		{
			get
			{
				ADObjectId internalADObjectId = base.InternalADObjectId;
				if (internalADObjectId == null || string.IsNullOrEmpty(internalADObjectId.DistinguishedName))
				{
					return base.CommonName;
				}
				string text = internalADObjectId.Rdn.ToString();
				return text.Substring(text.LastIndexOf('=') + 1);
			}
		}

		internal string Server
		{
			get
			{
				ADObjectId internalADObjectId = base.InternalADObjectId;
				if (internalADObjectId == null || string.IsNullOrEmpty(internalADObjectId.DistinguishedName))
				{
					return base.ServerName;
				}
				ADObjectId adobjectId = internalADObjectId.AncestorDN(3);
				if (adobjectId == null)
				{
					return null;
				}
				string text = adobjectId.Rdn.ToString();
				return text.Substring(text.LastIndexOf('=') + 1);
			}
		}

		protected override ServerRole RoleRestriction
		{
			get
			{
				return ServerRole.Cafe | ServerRole.Mailbox | ServerRole.ClientAccess | ServerRole.UnifiedMessaging | ServerRole.HubTransport | ServerRole.FrontendTransport | ServerRole.FfoWebService | ServerRole.OSP;
			}
		}

		public static VirtualDirectoryIdParameter Parse(string identity)
		{
			return new VirtualDirectoryIdParameter(identity);
		}
	}
}
