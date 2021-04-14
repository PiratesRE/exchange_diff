using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ResolverConfiguration
	{
		public ResolverConfiguration(OrganizationId orgId, PerTenantTransportSettings transportSettings)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			if (transportSettings == null)
			{
				throw new ArgumentNullException("transportSettings");
			}
			this.organizationId = orgId;
			this.transportSettings = transportSettings;
			this.InitializeAcceptedDomainsAndDefaultDomain();
			this.logPath = ResolverConfiguration.GetLogPath();
			this.privilegedSenders = ResolverConfiguration.GetPrivilegedSenders(this.organizationId, this.DefaultDomain, this.transportSettings);
		}

		public static double ResolverRetryInterval
		{
			get
			{
				return Components.TransportAppConfig.Resolver.ResolverRetryInterval;
			}
		}

		public static double DeliverMoveMailboxRetryInterval
		{
			get
			{
				return Components.TransportAppConfig.Resolver.DeliverMoveMailboxRetryInterval;
			}
		}

		public static ResolverLogLevel ResolverLogLevel
		{
			get
			{
				return Components.TransportAppConfig.Resolver.ResolverLogLevel;
			}
		}

		public static int ExpansionSizeLimit
		{
			get
			{
				return Components.TransportAppConfig.Resolver.ExpansionSizeLimit;
			}
		}

		public static int BatchLookupRecipientCount
		{
			get
			{
				return Components.TransportAppConfig.Resolver.BatchLookupRecipientCount;
			}
		}

		public static bool LargeDGLimitEnforcementEnabled
		{
			get
			{
				return Components.TransportAppConfig.Resolver.LargeDGLimitEnforcementEnabled;
			}
		}

		public static ByteQuantifiedSize LargeDGMaxMessageSize
		{
			get
			{
				return Components.TransportAppConfig.Resolver.LargeDGMaxMessageSize;
			}
		}

		public static int LargeDGGroupCount
		{
			get
			{
				return Components.TransportAppConfig.Resolver.LargeDGGroupCount;
			}
		}

		public static int LargeDGGroupCountForUnRestrictedDG
		{
			get
			{
				return Components.TransportAppConfig.Resolver.LargeDGGroupCountForUnRestrictedDG;
			}
		}

		public static string ServerDN
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer.Id.DistinguishedName;
			}
		}

		public AcceptedDomainTable AcceptedDomains
		{
			get
			{
				return this.acceptedDomains;
			}
		}

		public string DefaultDomain
		{
			get
			{
				return this.defaultDomain;
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxReceiveSize
		{
			get
			{
				return this.transportSettings.MaxReceiveSize;
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxSendSize
		{
			get
			{
				return this.transportSettings.MaxSendSize;
			}
		}

		public string LogPath
		{
			get
			{
				return this.logPath;
			}
		}

		public IList<RoutingAddress> PrivilegedSenders
		{
			get
			{
				return this.privilegedSenders;
			}
		}

		public static IList<RoutingAddress> GetPrivilegedSenders(OrganizationId orgId, string defaultDomain, PerTenantTransportSettings transportSettings)
		{
			IList<RoutingAddress> list = new List<RoutingAddress>(Components.RoutingComponent.MailRouter.ExternalPostmasterAddresses);
			RoutingAddress item;
			if (RoutingUtils.TryConvertToRoutingAddress(transportSettings.ExternalPostmasterAddress, out item) && !list.Contains(item))
			{
				list.Add(item);
			}
			RoutingAddress item2 = new RoutingAddress("postmaster", defaultDomain);
			if (!list.Contains(item2))
			{
				list.Add(item2);
			}
			RoutingAddress defaultExternalPostmasterAddress = DsnGenerator.GetDefaultExternalPostmasterAddress(orgId);
			if (!list.Contains(defaultExternalPostmasterAddress))
			{
				list.Add(defaultExternalPostmasterAddress);
			}
			return list;
		}

		private static string GetLogPath()
		{
			string result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
			{
				if (registryKey == null)
				{
					result = null;
				}
				else
				{
					string text = registryKey.GetValue("MsiInstallPath") as string;
					if (text != null)
					{
						try
						{
							return Path.Combine(text, "Logging\\Resolver");
						}
						catch (ArgumentException)
						{
							ExTraceGlobals.ResolverTracer.TraceError<string>(0L, "Cannot use log path '{0}'.", text);
						}
					}
					result = null;
				}
			}
			return result;
		}

		private void InitializeAcceptedDomainsAndDefaultDomain()
		{
			PerTenantAcceptedDomainTable acceptedDomainTable = Components.Configuration.GetAcceptedDomainTable(this.organizationId);
			this.acceptedDomains = acceptedDomainTable.AcceptedDomainTable;
			this.defaultDomain = this.acceptedDomains.DefaultDomainName;
			if (string.IsNullOrEmpty(this.defaultDomain))
			{
				ExTraceGlobals.ResolverTracer.TraceError<OrganizationId>(0L, "Cannot find default authoritative domain for organization {0}.", this.organizationId);
				throw new DefaultAuthoritativeDomainNotFoundException(this.organizationId);
			}
		}

		private OrganizationId organizationId;

		private PerTenantTransportSettings transportSettings;

		private AcceptedDomainTable acceptedDomains;

		private string logPath;

		private IList<RoutingAddress> privilegedSenders;

		private string defaultDomain;
	}
}
