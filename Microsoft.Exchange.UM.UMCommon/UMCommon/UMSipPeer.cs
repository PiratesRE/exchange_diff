using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class UMSipPeer
	{
		public UMSipPeer(UMSmartHost address, int port, bool allowOutboundCalls, bool useMutualTls, IPAddressFamily ipAddressFamily) : this(address, port, allowOutboundCalls, useMutualTls, false, ipAddressFamily)
		{
		}

		public UMSipPeer(UMSmartHost address, int port, bool allowOutboundCalls, bool useMutualTls, bool isOcs, IPAddressFamily ipAddressFamily)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "UMSipPeer::ctor(Endpoint: {0}:{1} outCallsAllowed={2} IPAddressFamily={3})", new object[]
			{
				address,
				port,
				allowOutboundCalls,
				ipAddressFamily
			});
			this.Address = address;
			this.UseMutualTLS = useMutualTls;
			this.AllowOutboundCalls = allowOutboundCalls;
			this.ResolvedIPAddress = new List<IPAddress>();
			this.IsOcs = isOcs;
			this.IPAddressFamily = ipAddressFamily;
			if (port == 0)
			{
				port = Utils.GetRedirectPort(this.UseMutualTLS);
			}
			this.Port = port;
			if (address.IsIPAddress)
			{
				this.ResolvedIPAddress.Add(address.Address);
			}
			this.NextHopForOutboundRouting = this;
		}

		public static UMSipPeer CreateForTlsAuth(string fqdn)
		{
			return new UMSipPeer(new UMSmartHost(fqdn), 10000, false, true, IPAddressFamily.Any);
		}

		public bool AllowOutboundCalls { get; set; }

		public List<IPAddress> ResolvedIPAddress { get; set; }

		public virtual bool IsOcs { get; private set; }

		public virtual string Name
		{
			get
			{
				return this.Address.ToString();
			}
		}

		public UMSmartHost Address { get; set; }

		public IPAddressFamily IPAddressFamily { get; set; }

		public int Port { get; private set; }

		public bool UseMutualTLS { get; private set; }

		public virtual UMIPGateway ToUMIPGateway(OrganizationId orgId)
		{
			ValidateArgument.NotNull(orgId, "orgId");
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "UMSipPeer.ToUMIPGateway - {0}", new object[]
			{
				orgId
			});
			if (CommonConstants.UseDataCenterCallRouting && OrganizationId.ForestWideOrgId.Equals(orgId))
			{
				ExAssert.RetailAssert(false, "Incorrectly scoped orgId - OrganizationalUnit = '{0}', ConfigurationUnit = '{1}'. Both OrganizationalUnit and ConfigurationUnit should be non-null.", new object[]
				{
					(orgId.OrganizationalUnit != null) ? orgId.OrganizationalUnit.ToString() : "<null>",
					(orgId.ConfigurationUnit != null) ? orgId.ConfigurationUnit.ToString() : "<null>"
				});
			}
			return new UMIPGateway
			{
				Port = this.Port,
				Name = this.Name,
				Address = this.Address,
				OutcallsAllowed = this.AllowOutboundCalls,
				MessageWaitingIndicatorAllowed = true,
				OrganizationId = orgId,
				IPAddressFamily = this.IPAddressFamily
			};
		}

		public UMSipPeer NextHopForOutboundRouting { get; set; }

		public string AddressWithTransport
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}:{1}{2}", new object[]
				{
					this.Address,
					this.Port,
					this.UseMutualTLS ? ";transport=TLS" : ";transport=TCP"
				});
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "({0} {1}:{2} {3} outbound={4} secured={5} )", new object[]
			{
				this.Name,
				this.Address,
				this.Port,
				this.IPAddressFamily,
				this.AllowOutboundCalls,
				this.UseMutualTLS
			});
		}

		public string ToHostPortString()
		{
			return this.Address.ToString() + ":" + this.Port.ToString();
		}
	}
}
