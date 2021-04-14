using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SmtpService : Service
	{
		private SmtpService(TopologyServerInfo serverInfo, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, MiniReceiveConnector smtpReceiveConnector, Hostname hostname) : base(serverInfo, ServiceType.Smtp, clientAccessType, authenticationMethod)
		{
			bool flag = (smtpReceiveConnector.AuthMechanism & (AuthMechanisms.Tls | AuthMechanisms.BasicAuthRequireTLS)) != AuthMechanisms.None;
			EncryptionType? encryptionType = null;
			if (flag)
			{
				encryptionType = new EncryptionType?(EncryptionType.TLS);
			}
			List<ProtocolConnectionSettings> list = new List<ProtocolConnectionSettings>(smtpReceiveConnector.Bindings.Count);
			HashSet<int> hashSet = new HashSet<int>();
			foreach (IPBinding ipbinding in smtpReceiveConnector.Bindings)
			{
				if (!hashSet.Contains(ipbinding.Port))
				{
					list.Add(new ProtocolConnectionSettings(hostname, ipbinding.Port, encryptionType));
					hashSet.Add(ipbinding.Port);
				}
			}
			this.ProtocolConnectionSettings = new ReadOnlyCollection<ProtocolConnectionSettings>(list);
		}

		public ReadOnlyCollection<ProtocolConnectionSettings> ProtocolConnectionSettings { get; private set; }

		internal static bool TryCreateSmtpService(MiniReceiveConnector smtpReceiveConnector, TopologyServerInfo serverInfo, ClientAccessType clientAccessType, out Service service)
		{
			Hostname hostname = null;
			service = null;
			if (!smtpReceiveConnector.AdvertiseClientSettings)
			{
				ExTraceGlobals.SmtpServiceTracer.TraceDebug<string>(0L, "SMTP Receive Connector: {0}, does not have AdvertiseClientSettings set.", smtpReceiveConnector.Name);
				return false;
			}
			if (smtpReceiveConnector.ServiceDiscoveryFqdn == null && smtpReceiveConnector.Fqdn == null)
			{
				ExTraceGlobals.SmtpServiceTracer.TraceDebug<string>(0L, "SMTP Receive Connector: {0}, has null Fqdn and ServiceDiscoveryFqdn.", smtpReceiveConnector.Name);
				return false;
			}
			if (smtpReceiveConnector.ServiceDiscoveryFqdn != null)
			{
				if (!Hostname.TryParse(smtpReceiveConnector.ServiceDiscoveryFqdn.ToString(), out hostname))
				{
					ExTraceGlobals.SmtpServiceTracer.TraceDebug<string>(0L, "SMTP Receive Connector: {0}, has unparsable ServiceDiscoveryFqdn.", smtpReceiveConnector.Name);
					return false;
				}
			}
			else if (smtpReceiveConnector.Fqdn != null && !Hostname.TryParse(smtpReceiveConnector.Fqdn.ToString(), out hostname))
			{
				ExTraceGlobals.SmtpServiceTracer.TraceDebug<string>(0L, "SMTP Receive Connector: {0}, has unparsable FQDN.", smtpReceiveConnector.Name);
				return false;
			}
			AuthenticationMethod authenticationMethod = AuthenticationMethod.None;
			authenticationMethod |= (((smtpReceiveConnector.AuthMechanism & (AuthMechanisms.BasicAuth | AuthMechanisms.BasicAuthRequireTLS)) != AuthMechanisms.None) ? AuthenticationMethod.Basic : AuthenticationMethod.None);
			authenticationMethod |= (((smtpReceiveConnector.AuthMechanism & AuthMechanisms.Integrated) != AuthMechanisms.None) ? AuthenticationMethod.WindowsIntegrated : AuthenticationMethod.None);
			service = new SmtpService(serverInfo, clientAccessType, authenticationMethod, smtpReceiveConnector, hostname);
			return true;
		}
	}
}
