using System;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Ehf;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.PerimeterConfig
{
	[Cmdlet("Set", "DCPerimeterConfig", SupportsShouldProcess = true)]
	public class SetDCPerimeterConfig : DCPerimeterConfigTask
	{
		[Parameter]
		public MultiValuedProperty<IPAddress> OutboundIPAddresses
		{
			get
			{
				return this.outboundIPAddresses;
			}
			set
			{
				this.outboundIPAddresses = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<IPAddress> DNSServerIPAddresses
		{
			get
			{
				return this.dnsServerIPAddresses;
			}
			set
			{
				this.dnsServerIPAddresses = value;
			}
		}

		[Parameter]
		public string FQDNTemplate
		{
			get
			{
				return this.fqdnTemplate;
			}
			set
			{
				this.fqdnTemplate = value;
			}
		}

		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return this.force;
			}
			set
			{
				this.force = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return new LocalizedString("Setting the DC Perimeter Configuration.  Have you shut down EdgeSync on all Hub servers in this AD forest?  Failure to do so can cause data corruption.");
			}
		}

		protected override void InternalValidate()
		{
			if (!this.force && !base.ShouldContinue(this.ConfirmationMessage))
			{
				base.WriteError(new InvalidOperationException("Make sure all EdgeSync services are shut down before running this task."), ErrorCategory.InvalidOperation, null);
			}
			if (this.dnsServerIPAddresses == null && this.fqdnTemplate == null && this.outboundIPAddresses == null)
			{
				base.WriteError(new ArgumentException("No parameters specified."), ErrorCategory.InvalidArgument, null);
			}
			if ((this.dnsServerIPAddresses == null && this.fqdnTemplate != null) || (this.dnsServerIPAddresses != null && this.fqdnTemplate == null))
			{
				base.WriteError(new ArgumentException("If one of DNSServerIPAddresses or FQDNTemplate is specified, both must be specified."), ErrorCategory.InvalidArgument, null);
			}
			if (this.fqdnTemplate != null && !this.fqdnTemplate.Contains("{0}"))
			{
				base.WriteError(new ArgumentException("FQDNTemplate must contain the string '{0}', which will be replaced by the site's partner ID when querying the DNS server."), ErrorCategory.InvalidArgument, null);
			}
			base.InternalValidate();
		}

		internal override void InvokeWebService(IConfigurationSession session, EhfTargetServerConfig config, EhfProvisioningService provisioningService)
		{
			IPAddress[] array = null;
			IPAddress[] array2 = null;
			if (this.dnsServerIPAddresses != null && this.fqdnTemplate != null)
			{
				ADSite site = null;
				ADNotificationAdapter.TryRunADOperation(delegate()
				{
					site = ((ITopologyConfigurationSession)session).GetLocalSite();
				});
				if (site == null)
				{
					base.WriteError(new InvalidOperationException("Unable to find ADSite object"), ErrorCategory.InvalidOperation, null);
				}
				Dns dns = new Dns();
				dns.Timeout = TimeSpan.FromSeconds(30.0);
				dns.ServerList = new DnsServerList();
				dns.ServerList.Initialize(this.dnsServerIPAddresses.ToArray());
				array = this.ResolveInboundVirtualIPs(dns, site.PartnerId, this.fqdnTemplate);
			}
			if (this.outboundIPAddresses != null && this.outboundIPAddresses.Count > 0)
			{
				array2 = this.outboundIPAddresses.ToArray();
			}
			if (array != null || array2 != null)
			{
				CompanyResponseInfo companyResponseInfo = provisioningService.UpdateReseller(config.ResellerId, array, array2);
				if (companyResponseInfo.Status != ResponseStatus.Success)
				{
					this.HandleFailure(companyResponseInfo);
				}
			}
		}

		private IPAddress[] ResolveInboundVirtualIPs(Dns dns, int partnerId, string fqdnTemplate)
		{
			string domainName = string.Format(fqdnTemplate, partnerId);
			IAsyncResult asyncResult = dns.BeginResolveToAddresses(domainName, AddressFamily.InterNetwork, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			IPAddress[] result;
			DnsStatus dnsStatus = Dns.EndResolveToAddresses(asyncResult, out result);
			if (dnsStatus != DnsStatus.Success)
			{
				base.WriteError(new InvalidOperationException("Unable to resolve inbound IPs.  Dns status = " + dnsStatus), ErrorCategory.InvalidOperation, null);
			}
			return result;
		}

		private void HandleFailure(ResponseInfo response)
		{
			string text = "none";
			if (response.TargetValue != null && response.TargetValue.Length > 0)
			{
				text = string.Join(", ", response.TargetValue);
			}
			string message = string.Format("Unable to update perimeter config: FaultId=<{0}>; FaultType=<{1}>; FaultDetail=<{2}>; Target=<{3}>; TargetValues=<{4}>", new object[]
			{
				response.Fault.Id,
				response.Status,
				response.Fault.Detail ?? "null",
				response.Target,
				text
			});
			base.WriteError(new InvalidOperationException(message), ErrorCategory.InvalidOperation, null);
		}

		private MultiValuedProperty<IPAddress> outboundIPAddresses;

		private MultiValuedProperty<IPAddress> dnsServerIPAddresses;

		private string fqdnTemplate;

		private SwitchParameter force;
	}
}
