using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ReceiveConnectorIdParameter : ServerBasedIdParameter
	{
		public ReceiveConnectorIdParameter()
		{
		}

		public ReceiveConnectorIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ReceiveConnectorIdParameter(ReceiveConnector connector) : base(connector.Id)
		{
		}

		public ReceiveConnectorIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected ReceiveConnectorIdParameter(string identity) : base(identity)
		{
			string[] array = identity.Split(new char[]
			{
				'\\'
			});
			switch (array.Length)
			{
			case 1:
			case 2:
				foreach (string value in array)
				{
					if (string.IsNullOrEmpty(value))
					{
						throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "identity");
					}
				}
				return;
			default:
				throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "identity");
			}
		}

		protected override ServerRole RoleRestriction
		{
			get
			{
				return ServerRole.All;
			}
		}

		public static ReceiveConnectorIdParameter Parse(string identity)
		{
			return new ReceiveConnectorIdParameter(identity);
		}

		private const char CommonNameSeperatorChar = '\\';
	}
}
