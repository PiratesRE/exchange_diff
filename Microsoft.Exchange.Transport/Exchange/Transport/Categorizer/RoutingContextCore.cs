using System;
using System.Collections.Generic;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingContextCore
	{
		public RoutingContextCore(ProcessTransportRole role, TransportAppConfig.RoutingConfig settings, RoutingDependencies dependencies, EdgeRoutingDependencies edgeDependencies, RoutingPerformanceCounters perfCounters)
		{
			RoutingUtils.ThrowIfNull(settings, "settings");
			RoutingUtils.ThrowIfNull(dependencies, "dependencies");
			RoutingUtils.ThrowIfNull(perfCounters, "perfCounters");
			if (role == ProcessTransportRole.Edge)
			{
				RoutingUtils.ThrowIfNull(edgeDependencies, "edgeDependencies");
			}
			this.role = role;
			this.settings = settings;
			this.dependencies = dependencies;
			this.edgeDependencies = edgeDependencies;
			this.perfCounters = perfCounters;
			this.proxyRoutingAllowedTargetVersions = new List<int>(0);
			if (this.ProxyRoutingXVersionSupported)
			{
				foreach (int item in this.settings.ProxyRoutingAllowedTargetVersions)
				{
					if (!this.proxyRoutingAllowedTargetVersions.Contains(item))
					{
						this.proxyRoutingAllowedTargetVersions.Add(item);
					}
				}
			}
		}

		public RoutingDependencies Dependencies
		{
			get
			{
				return this.dependencies;
			}
		}

		public EdgeRoutingDependencies EdgeDependencies
		{
			get
			{
				return this.edgeDependencies;
			}
		}

		public RoutingPerformanceCounters PerfCounters
		{
			get
			{
				return this.perfCounters;
			}
		}

		public TransportAppConfig.RoutingConfig Settings
		{
			get
			{
				return this.settings;
			}
		}

		public bool IsEdgeMode
		{
			get
			{
				return this.role == ProcessTransportRole.Edge;
			}
		}

		public bool ConnectorRoutingSupported
		{
			get
			{
				return this.MessageQueuesSupported;
			}
		}

		public bool DeliveryGroupMembershipSupported
		{
			get
			{
				return this.role == ProcessTransportRole.Hub || this.role == ProcessTransportRole.MailboxSubmission || this.role == ProcessTransportRole.MailboxDelivery;
			}
		}

		public bool MailboxDeliveryQueuesSupported
		{
			get
			{
				return this.role == ProcessTransportRole.Hub;
			}
		}

		public bool MessageQueuesSupported
		{
			get
			{
				return this.role == ProcessTransportRole.Hub || this.role == ProcessTransportRole.Edge;
			}
		}

		public bool ProxyRoutingSupported
		{
			get
			{
				return !this.MessageQueuesSupported;
			}
		}

		public bool ProxyRoutingXVersionSupported
		{
			get
			{
				return this.role == ProcessTransportRole.FrontEnd;
			}
		}

		public IList<int> ProxyRoutingAllowedTargetVersions
		{
			get
			{
				return this.proxyRoutingAllowedTargetVersions;
			}
		}

		public bool ServerRoutingSupported
		{
			get
			{
				return this.role == ProcessTransportRole.Hub || this.role == ProcessTransportRole.FrontEnd || this.role == ProcessTransportRole.MailboxSubmission || this.role == ProcessTransportRole.MailboxDelivery;
			}
		}

		public bool ShadowRoutingSupported
		{
			get
			{
				return this.role == ProcessTransportRole.Hub;
			}
		}

		public ProcessTransportRole GetProcessRoleForDiagnostics()
		{
			return this.role;
		}

		public bool VerifyFrontendComponentStateRestriction(RoutingServerInfo serverInfo)
		{
			RoutingUtils.ThrowIfNull(serverInfo, "serverInfo");
			return this.settings.RoutingToNonActiveServersEnabled || serverInfo.IsFrontendTransportActive;
		}

		public bool VerifyHubComponentStateRestriction(RoutingServerInfo serverInfo)
		{
			RoutingUtils.ThrowIfNull(serverInfo, "serverInfo");
			return this.settings.RoutingToNonActiveServersEnabled || serverInfo.IsHubTransportActive;
		}

		private readonly ProcessTransportRole role;

		private TransportAppConfig.RoutingConfig settings;

		private RoutingDependencies dependencies;

		private EdgeRoutingDependencies edgeDependencies;

		private RoutingPerformanceCounters perfCounters;

		private IList<int> proxyRoutingAllowedTargetVersions;
	}
}
