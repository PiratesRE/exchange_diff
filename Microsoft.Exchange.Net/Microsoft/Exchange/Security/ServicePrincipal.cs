using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Security
{
	internal class ServicePrincipal : IPrincipal
	{
		public ServicePrincipal(IIdentity identity, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("identity", identity);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.Identity = identity;
			this.Tracer = tracer;
		}

		public IIdentity Identity { get; private set; }

		private protected ITracer Tracer { protected get; private set; }

		protected virtual double LocalIPCacheRefreshInMilliseconds
		{
			get
			{
				return ServicePrincipal.DefaultLocalIPAddressesCacheRefreshInterval;
			}
		}

		public bool IsInRole(string role)
		{
			if (string.IsNullOrWhiteSpace(role))
			{
				return true;
			}
			WindowsIdentity windowsIdentity = this.Identity as WindowsIdentity;
			if (windowsIdentity == null)
			{
				this.Tracer.TraceError<string>((long)this.GetHashCode(), "ServicePrincipal.IsInRole: Unsupported IIdentity. Identity type {0}", this.Identity.GetType().FullName);
				return false;
			}
			WindowsPrincipal principal = new WindowsPrincipal(windowsIdentity);
			string[] array = role.Split(ServicePrincipal.RoleSeparator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string role2 in array)
			{
				if (!this.IsInRoleInternal(principal, role2))
				{
					return false;
				}
			}
			return true;
		}

		protected virtual bool IsInRoleInternal(WindowsPrincipal principal, string role)
		{
			bool flag;
			if (role != null)
			{
				if (role == "LocalAdministrators")
				{
					flag = principal.IsInRole(WindowsBuiltInRole.Administrator);
					this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "ServicePrincipal.IsInternalInRole: User is {0}Administrator.", flag ? string.Empty : "NOT ");
					return flag;
				}
				if (role == "LocalCall")
				{
					flag = this.IsLocalCall();
					this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "ServicePrincipal.IsInternalInRole: User is {0}LocalCall.", flag ? string.Empty : "NOT ");
					return flag;
				}
				if (role == "UserService")
				{
					SecurityIdentifier securityIdentifier = this.Identity.GetSecurityIdentifier();
					flag = (securityIdentifier.IsWellKnown(WellKnownSidType.NetworkServiceSid) || securityIdentifier.IsWellKnown(WellKnownSidType.LocalSystemSid));
					this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "ServicePrincipal.IsInternalInRole: User is {0}NetworkServiceSid or LocalSystemSid.", flag ? string.Empty : "NOT ");
					return flag;
				}
			}
			flag = principal.IsInRole(role);
			return flag;
		}

		private bool IsLocalCall()
		{
			Breadcrumbs<byte> breadcrumbs = new Breadcrumbs<byte>(8);
			if (OperationContext.Current == null)
			{
				return false;
			}
			breadcrumbs.Drop(1);
			if (OperationContext.Current.IncomingMessageProperties == null)
			{
				return false;
			}
			breadcrumbs.Drop(2);
			RemoteEndpointMessageProperty remoteEndpointMessageProperty = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
			if (remoteEndpointMessageProperty == null || string.IsNullOrEmpty(remoteEndpointMessageProperty.Address))
			{
				this.Tracer.TraceDebug((long)this.GetHashCode(), "ServicePrincipal.IsLocalCall: clientEndpoint is null or address is empty");
				return false;
			}
			breadcrumbs.Drop(3);
			if ("localhost".Equals(remoteEndpointMessageProperty.Address, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			breadcrumbs.Drop(4);
			IPAddress address;
			if (!IPAddress.TryParse(remoteEndpointMessageProperty.Address, out address))
			{
				this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "ServicePrincipal.IsLocalCall: Unable to parse {0} to an IP", remoteEndpointMessageProperty.Address);
				return false;
			}
			breadcrumbs.Drop(5);
			if (IPAddress.IsLoopback(address))
			{
				this.Tracer.TraceDebug((long)this.GetHashCode(), "ServicePrincipal.IsLocalCall: Client has loopback address");
				return true;
			}
			breadcrumbs.Drop(6);
			this.RefreshMachineIpAddressesIfRequired();
			List<IPAddress> list = ServicePrincipal.localIPAddresses;
			if (list != null)
			{
				breadcrumbs.Drop(7);
				IPAddress ipaddress = list.Find((IPAddress x) => x.Equals(address));
				this.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "ServicePrincipal.IsLocalCall: Ip {0} {1}found in local set of IPs", remoteEndpointMessageProperty.Address, (ipaddress == null) ? "NOT " : string.Empty);
				return null != ipaddress;
			}
			breadcrumbs.Drop(8);
			this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "ServicePrincipal.IsLocalCall: Ip not found in local set of ips {0}", remoteEndpointMessageProperty.Address);
			return false;
		}

		private void RefreshMachineIpAddressesIfRequired()
		{
			if (-1 != ServicePrincipal.lastIpAddressRefreshTick && TickDiffer.GetTickDifference(ServicePrincipal.lastIpAddressRefreshTick, Environment.TickCount) <= this.LocalIPCacheRefreshInMilliseconds)
			{
				return;
			}
			if (Interlocked.Increment(ref ServicePrincipal.refreshingLocalIpAddresses) != 1)
			{
				Interlocked.Decrement(ref ServicePrincipal.refreshingLocalIpAddresses);
				return;
			}
			List<IPAddress> list = null;
			try
			{
				list = ComputerInformation.GetLocalIPAddresses();
			}
			catch (NetworkInformationException arg)
			{
				this.Tracer.TraceError<Exception>((long)this.GetHashCode(), "ServicePrincipal.RefreshMachineIpAddressesIfRequired: Unable to get local IP addresses {0}", arg);
			}
			finally
			{
				Interlocked.Decrement(ref ServicePrincipal.refreshingLocalIpAddresses);
				if (list != null)
				{
					ServicePrincipal.localIPAddresses = list;
					Interlocked.Exchange(ref ServicePrincipal.lastIpAddressRefreshTick, Environment.TickCount);
				}
			}
		}

		public const string AndRole = "+";

		public const string LocalAdministrators = "LocalAdministrators";

		public const string LocalCall = "LocalCall";

		public const string UserService = "UserService";

		private const string LocalHost = "localhost";

		private static readonly double DefaultLocalIPAddressesCacheRefreshInterval = TimeSpan.FromHours(24.0).TotalMilliseconds;

		private static readonly string[] RoleSeparator = new string[]
		{
			"+"
		};

		private static List<IPAddress> localIPAddresses;

		private static int refreshingLocalIpAddresses = 0;

		private static int lastIpAddressRefreshTick = -1;
	}
}
