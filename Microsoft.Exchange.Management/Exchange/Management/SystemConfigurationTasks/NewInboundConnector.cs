using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "InboundConnector", SupportsShouldProcess = true)]
	public class NewInboundConnector : NewMultitenancySystemConfigurationObjectTask<TenantInboundConnector>
	{
		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return this.DataObject.Enabled;
			}
			set
			{
				this.DataObject.Enabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TenantConnectorType ConnectorType
		{
			get
			{
				return this.DataObject.ConnectorType;
			}
			set
			{
				this.DataObject.ConnectorType = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TenantConnectorSource ConnectorSource
		{
			get
			{
				return this.DataObject.ConnectorSource;
			}
			set
			{
				this.DataObject.ConnectorSource = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comment
		{
			get
			{
				return this.DataObject.Comment;
			}
			set
			{
				this.DataObject.Comment = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPRange> SenderIPAddresses
		{
			get
			{
				return this.DataObject.SenderIPAddresses;
			}
			set
			{
				this.DataObject.SenderIPAddresses = value;
			}
		}

		[Parameter(Mandatory = true)]
		public MultiValuedProperty<AddressSpace> SenderDomains
		{
			get
			{
				return this.DataObject.SenderDomains;
			}
			set
			{
				this.DataObject.SenderDomains = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireTls
		{
			get
			{
				return this.DataObject.RequireTls;
			}
			set
			{
				this.DataObject.RequireTls = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RestrictDomainsToCertificate
		{
			get
			{
				return this.DataObject.RestrictDomainsToCertificate;
			}
			set
			{
				this.DataObject.RestrictDomainsToCertificate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RestrictDomainsToIPAddresses
		{
			get
			{
				return this.DataObject.RestrictDomainsToIPAddresses;
			}
			set
			{
				this.DataObject.RestrictDomainsToIPAddresses = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CloudServicesMailEnabled
		{
			get
			{
				return this.DataObject.CloudServicesMailEnabled;
			}
			set
			{
				this.DataObject.CloudServicesMailEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TlsCertificate TlsSenderCertificateName
		{
			get
			{
				return this.DataObject.TlsSenderCertificateName;
			}
			set
			{
				this.DataObject.TlsSenderCertificateName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<AcceptedDomainIdParameter> AssociatedAcceptedDomains
		{
			get
			{
				return (MultiValuedProperty<AcceptedDomainIdParameter>)base.Fields["AssociatedAcceptedDomains"];
			}
			set
			{
				base.Fields["AssociatedAcceptedDomains"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BypassValidation
		{
			get
			{
				return base.Fields.Contains("BypassValidation") && (bool)base.Fields["BypassValidation"];
			}
			set
			{
				base.Fields["BypassValidation"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewInboundConnector(base.Name);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TenantInboundConnector tenantInboundConnector = (TenantInboundConnector)base.PrepareDataObject();
			tenantInboundConnector.SetId(base.DataSession as IConfigurationSession, base.Name);
			return tenantInboundConnector;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.Fields.IsModified("AssociatedAcceptedDomains"))
			{
				NewInboundConnector.ValidateAssociatedAcceptedDomains(this.AssociatedAcceptedDomains, base.DataSession, this.DataObject, this.RootId, this, new Func<IIdentityParameter, IConfigDataProvider, ObjectId, LocalizedString?, LocalizedString?, IConfigurable>(base.GetDataObject<AcceptedDomain>));
			}
			NewInboundConnector.ValidateSenderIPAddresses(this.DataObject.SenderIPAddresses, this, this.BypassValidation);
			if (this.DataObject.ConnectorType == TenantConnectorType.OnPremises && !this.BypassValidation)
			{
				MultiValuedProperty<IPRange> ffoDCIPs;
				MultiValuedProperty<SmtpX509IdentifierEx> ffoFDSmtpCerts;
				MultiValuedProperty<ServiceProviderSettings> serviceProviders;
				if (!HygieneDCSettings.GetSettings(out ffoDCIPs, out ffoFDSmtpCerts, out serviceProviders))
				{
					base.WriteError(new ConnectorValidationFailedException(), ErrorCategory.ConnectionError, null);
				}
				NewInboundConnector.ValidateSenderIPAddressRestrictions(this.DataObject.SenderIPAddresses, ffoDCIPs, serviceProviders, this);
				NewInboundConnector.ValidateTlsSenderCertificateRestrictions(this.DataObject.TlsSenderCertificateName, ffoFDSmtpCerts, serviceProviders, this);
			}
			IEnumerable<TenantInboundConnector> enumerable = base.DataSession.FindPaged<TenantInboundConnector>(null, ((IConfigurationSession)base.DataSession).GetOrgContainerId().GetDescendantId(this.DataObject.ParentPath), false, null, ADGenericPagedReader<TenantInboundConnector>.DefaultPageSize);
			foreach (TenantInboundConnector tenantInboundConnector in enumerable)
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(this.DataObject.Name, tenantInboundConnector.Name))
				{
					base.WriteError(new ErrorInboundConnectorAlreadyExistsException(tenantInboundConnector.Name), ErrorCategory.InvalidOperation, null);
					break;
				}
			}
			NewInboundConnector.CheckSenderIpAddressesOverlap(base.DataSession, this.DataObject, this);
			TaskLogger.LogExit();
		}

		internal static void ValidateSenderIPAddresses(IEnumerable<IPRange> addressRanges, Task task, bool bypassValidation)
		{
			if (addressRanges == null)
			{
				return;
			}
			foreach (IPRange iprange in addressRanges)
			{
				if (iprange.LowerBound.AddressFamily == AddressFamily.InterNetworkV6 || iprange.UpperBound.AddressFamily == AddressFamily.InterNetworkV6)
				{
					task.WriteError(new IPv6AddressesRangesAreNotAllowedInConnectorException(iprange.Expression), ErrorCategory.InvalidArgument, null);
				}
				if (iprange.RangeFormat != IPRange.Format.SingleAddress && iprange.RangeFormat != IPRange.Format.CIDR)
				{
					task.WriteError(new InvalidIPRangeFormatException(iprange.Expression), ErrorCategory.InvalidArgument, null);
				}
				if (iprange.RangeFormat == IPRange.Format.CIDR && iprange.CIDRLength < 24)
				{
					task.WriteError(new InvalidCidrRangeException(iprange.Expression, 24), ErrorCategory.InvalidArgument, null);
				}
				NewInboundConnector.ValidateIPAddress(iprange, iprange.UpperBound, task, bypassValidation);
				NewInboundConnector.ValidateIPAddress(iprange, iprange.LowerBound, task, bypassValidation);
			}
		}

		internal static void CheckSenderIpAddressesOverlap(IConfigDataProvider dataSession, TenantInboundConnector dataObject, Task task)
		{
			if (task == null || dataObject.SenderIPAddresses == null)
			{
				return;
			}
			TenantInboundConnector[] array = (TenantInboundConnector[])dataSession.Find<TenantInboundConnector>(null, null, true, null);
			List<string> list = null;
			List<string> list2 = null;
			using (MultiValuedProperty<IPRange>.Enumerator enumerator = dataObject.SenderIPAddresses.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IPRange ipRange = enumerator.Current;
					bool flag = false;
					foreach (TenantInboundConnector tenantInboundConnector in array)
					{
						if (tenantInboundConnector.Enabled && tenantInboundConnector.SenderIPAddresses != null && ((ADObjectId)tenantInboundConnector.Identity).ObjectGuid != ((ADObjectId)dataObject.Identity).ObjectGuid)
						{
							if (tenantInboundConnector.SenderIPAddresses.Any((IPRange exisingSenderIpRange) => exisingSenderIpRange.Overlaps(ipRange)))
							{
								if (list != null)
								{
									list.Add(tenantInboundConnector.Name);
								}
								else
								{
									list = new List<string>
									{
										tenantInboundConnector.Name
									};
								}
								flag = true;
							}
						}
					}
					if (flag)
					{
						if (list2 != null)
						{
							list2.Add(ipRange.Expression);
						}
						else
						{
							list2 = new List<string>
							{
								ipRange.Expression
							};
						}
					}
				}
			}
			if (list != null && list2 != null)
			{
				task.WriteWarning(Strings.SenderIPAddressOverlapsExistingTenantInboundConnectors(string.Join(", ", list2), string.Join(", ", list)));
			}
		}

		internal static void ValidateSenderIPAddressRestrictions(MultiValuedProperty<IPRange> addressRanges, MultiValuedProperty<IPRange> ffoDCIPs, MultiValuedProperty<ServiceProviderSettings> serviceProviders, Task task)
		{
			if (MultiValuedPropertyBase.IsNullOrEmpty(addressRanges))
			{
				return;
			}
			using (MultiValuedProperty<IPRange>.Enumerator enumerator = addressRanges.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IPRange ipRange = enumerator.Current;
					if (!MultiValuedPropertyBase.IsNullOrEmpty(ffoDCIPs))
					{
						if (ffoDCIPs.Any((IPRange ffoDCIP) => ffoDCIP.Overlaps(ipRange)))
						{
							task.WriteError(new SenderIPAddressOverlapsFfoDCIPAddressesException(ipRange.Expression), ErrorCategory.InvalidArgument, null);
						}
					}
					if (!MultiValuedPropertyBase.IsNullOrEmpty(serviceProviders))
					{
						if (serviceProviders.Any((ServiceProviderSettings serviceProvider) => serviceProvider.IPRanges != null && serviceProvider.IPRanges.Any((IPRange providerIPRange) => providerIPRange != null && providerIPRange.Overlaps(ipRange))))
						{
							task.WriteError(new SenderIPAddressOverlapsServiceProviderIPAddressesException(ipRange.Expression), ErrorCategory.InvalidArgument, null);
						}
					}
				}
			}
		}

		internal static void ValidateTlsSenderCertificateRestrictions(TlsCertificate certificate, MultiValuedProperty<SmtpX509IdentifierEx> ffoFDSmtpCerts, MultiValuedProperty<ServiceProviderSettings> serviceProviders, Task task)
		{
			if (certificate == null)
			{
				return;
			}
			if (!MultiValuedPropertyBase.IsNullOrEmpty(ffoFDSmtpCerts) && ffoFDSmtpCerts.Any((SmtpX509IdentifierEx ffoFDSmtpCert) => ffoFDSmtpCert.Matches(certificate)))
			{
				task.WriteError(new TlsSenderCertificateNameMatchesFfoFDSmtpCertificateException(certificate.ToString()), ErrorCategory.InvalidArgument, null);
			}
			if (!MultiValuedPropertyBase.IsNullOrEmpty(serviceProviders) && serviceProviders.Any((ServiceProviderSettings serviceProvider) => serviceProvider.Certificates != null && serviceProvider.Certificates.Any((TlsCertificate providerCertificate) => providerCertificate != null && providerCertificate.Equals(certificate))))
			{
				task.WriteError(new TlsSenderCertificateNameMatchesServiceProviderCertificateException(certificate.ToString()), ErrorCategory.InvalidArgument, null);
			}
		}

		internal static void ValidateAssociatedAcceptedDomains(MultiValuedProperty<AcceptedDomainIdParameter> domainIdParameters, IConfigDataProvider dataSession, TenantInboundConnector dataObject, ObjectId rootId, Task task, Func<IIdentityParameter, IConfigDataProvider, ObjectId, LocalizedString?, LocalizedString?, IConfigurable> acceptedDomainsGetter)
		{
			if (domainIdParameters != null)
			{
				NewInboundConnector.ValidateCentralizedMailControlAndAssociatedAcceptedDomainsRestriction(dataSession, dataObject, task);
				dataObject.AssociatedAcceptedDomains.Clear();
				using (MultiValuedProperty<AcceptedDomainIdParameter>.Enumerator enumerator = domainIdParameters.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AcceptedDomainIdParameter acceptedDomainIdParameter = enumerator.Current;
						AcceptedDomain acceptedDomain = (AcceptedDomain)acceptedDomainsGetter(acceptedDomainIdParameter, dataSession, rootId, new LocalizedString?(Strings.ErrorDefaultDomainNotFound(acceptedDomainIdParameter)), new LocalizedString?(Strings.ErrorDefaultDomainNotUnique(acceptedDomainIdParameter)));
						dataObject.AssociatedAcceptedDomains.Add(acceptedDomain.Id);
					}
					return;
				}
			}
			dataObject.AssociatedAcceptedDomains.Clear();
		}

		internal static bool FindTenantScopedOnPremiseInboundConnector(IConfigDataProvider dataSession, Func<TenantInboundConnector, bool> connectorMatches = null)
		{
			TenantInboundConnector[] array = (TenantInboundConnector[])dataSession.Find<TenantInboundConnector>(null, null, true, null);
			foreach (TenantInboundConnector tenantInboundConnector in array)
			{
				if (tenantInboundConnector.Enabled && tenantInboundConnector.ConnectorType == TenantConnectorType.OnPremises && (tenantInboundConnector.AssociatedAcceptedDomains == null || tenantInboundConnector.AssociatedAcceptedDomains.Count == 0) && (connectorMatches == null || connectorMatches(tenantInboundConnector)))
				{
					return true;
				}
			}
			return false;
		}

		private static void ValidateCentralizedMailControlAndAssociatedAcceptedDomainsRestriction(IConfigDataProvider dataSession, TenantInboundConnector dataObject, Task task)
		{
			TenantOutboundConnector[] array = (TenantOutboundConnector[])dataSession.Find<TenantOutboundConnector>(null, null, true, null);
			foreach (TenantOutboundConnector tenantOutboundConnector in array)
			{
				if (tenantOutboundConnector.Enabled && tenantOutboundConnector.RouteAllMessagesViaOnPremises)
				{
					if (!NewInboundConnector.FindTenantScopedOnPremiseInboundConnector(dataSession, (TenantInboundConnector c) => ((ADObjectId)c.Identity).ObjectGuid != ((ADObjectId)dataObject.Identity).ObjectGuid))
					{
						task.WriteError(new TenantScopedInboundConnectorRequiredForCMCConnectorException(tenantOutboundConnector.Name), ErrorCategory.InvalidArgument, null);
					}
					break;
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			FfoDualWriter.SaveToFfo<TenantInboundConnector>(this, this.DataObject, null);
			TaskLogger.LogExit();
		}

		private static void ValidateIPAddress(IPRange ipRange, IPAddress address, Task task, bool bypassValidation)
		{
			if (!IPAddressValidation.IsValidIPv4Address(address.ToString()))
			{
				task.WriteError(new ConnectorIPRangeContainsInvalidIPv4AddressException(ipRange.Expression), ErrorCategory.InvalidArgument, null);
			}
			if (!bypassValidation && IPAddressValidation.IsReservedIPv4Address(address.ToString()))
			{
				task.WriteError(new IPRangeInConnectorContainsReservedIPAddressesException(ipRange.Expression), ErrorCategory.InvalidArgument, null);
			}
		}

		private const string AssociatedAcceptedDomainsField = "AssociatedAcceptedDomains";
	}
}
