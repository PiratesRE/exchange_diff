using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Service
	{
		internal Service(TopologyServerInfo serverInfo, ServiceType serviceType, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod)
		{
			this.ServerInfo = serverInfo;
			this.ServiceType = serviceType;
			this.ClientAccessType = clientAccessType;
			this.AuthenticationMethod = authenticationMethod;
		}

		public TopologyServerInfo ServerInfo { get; private set; }

		public ServiceType ServiceType { get; private set; }

		public ClientAccessType ClientAccessType { get; private set; }

		public AuthenticationMethod AuthenticationMethod { get; private set; }

		public string ServerFullyQualifiedDomainName
		{
			get
			{
				return this.ServerInfo.ServerFullyQualifiedDomainName;
			}
		}

		public Site Site
		{
			get
			{
				return this.ServerInfo.Site;
			}
		}

		public int ServerVersionNumber
		{
			get
			{
				return this.ServerInfo.VersionNumber;
			}
		}

		public ServerVersion AdminDisplayVersionNumber
		{
			get
			{
				return this.ServerInfo.AdminDisplayVersionNumber;
			}
		}

		public ServerRole ServerRole
		{
			get
			{
				return this.ServerInfo.Role;
			}
		}

		public bool IsOutOfService
		{
			get
			{
				return this.ServerInfo.IsOutOfService;
			}
		}

		public override string ToString()
		{
			return string.Format("Service. Type = {0}. ClientAccessType = {1}. AuthenticationMethod = {2}.", this.ServiceType, this.ClientAccessType, this.AuthenticationMethod);
		}

		internal static ReadOnlyCollection<T> ConvertToReadOnlyCollection<T>(MultiValuedProperty<T> properties)
		{
			if (properties == null)
			{
				return null;
			}
			List<T> list = new List<T>(properties);
			return list.AsReadOnly();
		}

		internal static bool TryCreateService(TopologyServerInfo serverInfo, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			service = new Service(serverInfo, ServiceType.Invalid, clientAccessType, authenticationMethod);
			return true;
		}
	}
}
