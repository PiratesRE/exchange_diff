using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EmailTransportService : Service
	{
		internal EmailTransportService(TopologyServerInfo serverInfo, ServiceType serviceType, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, MiniEmailTransport emailTransport) : base(serverInfo, serviceType, clientAccessType, authenticationMethod)
		{
			this.PopImapTransport = (emailTransport.IsPop3 || emailTransport.IsImap4);
			if (this.PopImapTransport)
			{
				this.UnencryptedOrTLSPort = EmailTransportService.GetPort(emailTransport.UnencryptedOrTLSBindings);
				this.SSLPort = EmailTransportService.GetPort(emailTransport.SSLBindings);
				this.InternalConnectionSettings = Service.ConvertToReadOnlyCollection<ProtocolConnectionSettings>(emailTransport.InternalConnectionSettings);
				this.ExternalConnectionSettings = Service.ConvertToReadOnlyCollection<ProtocolConnectionSettings>(emailTransport.ExternalConnectionSettings);
				this.LoginType = emailTransport.LoginType;
			}
		}

		public bool PopImapTransport { get; private set; }

		public int UnencryptedOrTLSPort { get; private set; }

		public int SSLPort { get; private set; }

		public ReadOnlyCollection<ProtocolConnectionSettings> InternalConnectionSettings { get; private set; }

		public ReadOnlyCollection<ProtocolConnectionSettings> ExternalConnectionSettings { get; private set; }

		public LoginOptions LoginType { get; private set; }

		internal static bool TryCreateEmailTransportService(MiniEmailTransport emailTransport, TopologyServerInfo serverInfo, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			service = new EmailTransportService(serverInfo, ServiceType.Invalid, clientAccessType, authenticationMethod, emailTransport);
			return true;
		}

		private static int GetPort(MultiValuedProperty<IPBinding> bindings)
		{
			if (bindings == null || bindings.Count <= 0)
			{
				return -1;
			}
			return bindings[0].Port;
		}
	}
}
