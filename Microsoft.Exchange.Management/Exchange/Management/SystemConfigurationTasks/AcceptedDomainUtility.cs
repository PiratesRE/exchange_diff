using System;
using System.Management.Automation;
using System.ServiceModel;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class AcceptedDomainUtility
	{
		internal static bool IsDatacenter
		{
			get
			{
				AcceptedDomainUtility.ReloadProvisioningParameters();
				if (AcceptedDomainUtility.isDatacenter == null)
				{
					AcceptedDomainUtility.isDatacenter = new bool?(Datacenter.IsMicrosoftHostedOnly(true));
				}
				return AcceptedDomainUtility.isDatacenter.Value;
			}
		}

		internal static void ReloadProvisioningParameters()
		{
			if (AcceptedDomainUtility.lastReadTime + AcceptedDomainUtility.reloadTimeSpan > DateTime.UtcNow)
			{
				return;
			}
			lock (AcceptedDomainUtility.LockObject)
			{
				if (!(AcceptedDomainUtility.lastReadTime + AcceptedDomainUtility.reloadTimeSpan > DateTime.UtcNow))
				{
					ServiceEndpoint serviceEndpoint = null;
					ServiceEndpoint serviceEndpoint2 = null;
					ServiceEndpoint serviceEndpoint3 = null;
					ServiceEndpoint serviceEndpoint4 = null;
					ServiceEndpoint serviceEndpoint5 = null;
					try
					{
						ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 138, "ReloadProvisioningParameters", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageSecurity\\AcceptedDomain\\AcceptedDomainUtility.cs");
						ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
						serviceEndpoint = endpointContainer.GetEndpoint(ServiceEndpointId.CoexistenceParentDomain);
						serviceEndpoint2 = endpointContainer.GetEndpoint(ServiceEndpointId.CoexistenceDnsEndpoint);
						serviceEndpoint3 = endpointContainer.GetEndpoint(ServiceEndpointId.CoexistenceMailDomainFfo15);
						serviceEndpoint4 = endpointContainer.GetEndpoint(ServiceEndpointId.CoexistenceDnsCname);
						serviceEndpoint5 = endpointContainer.GetEndpoint(ServiceEndpointId.CoexistenceDnsText);
					}
					catch (EndpointContainerNotFoundException)
					{
						TaskLogger.Trace("EndpointContainer was not found.", new object[0]);
					}
					catch (ServiceEndpointNotFoundException)
					{
						TaskLogger.Trace("At least one Coexistence Endpoint was not found.", new object[0]);
					}
					if (serviceEndpoint2 != null)
					{
						AcceptedDomainUtility.dnsRegistrationEndpoint = serviceEndpoint2.Uri;
						if (string.IsNullOrEmpty(serviceEndpoint2.CertificateSubject))
						{
							throw new Exception("Unable to find the certificate.");
						}
						AcceptedDomainUtility.dnsRegistrationCertificateSubject = serviceEndpoint2.CertificateSubject;
						TaskLogger.Trace("Dns Registration Endpoint set to '{0}'", new object[]
						{
							AcceptedDomainUtility.dnsRegistrationEndpoint.ToString()
						});
						TaskLogger.Trace("Dns Registration Subject set to '{0}'", new object[]
						{
							AcceptedDomainUtility.dnsRegistrationCertificateSubject
						});
					}
					AcceptedDomainUtility.coexistenceMailDomainFfo15 = ((serviceEndpoint3 != null) ? serviceEndpoint3.Uri.Host : string.Empty);
					TaskLogger.Trace("Coexistence Mail Domain with FFO 15 set to '{0}'", new object[]
					{
						AcceptedDomainUtility.coexistenceMailDomainFfo15
					});
					AcceptedDomainUtility.coexistenceDnsCnameValue = ((serviceEndpoint4 != null) ? serviceEndpoint4.Uri.Host : "autodiscover.outlook.com");
					TaskLogger.Trace("Coexistence DNS CNAME value set to '{0}'", new object[]
					{
						AcceptedDomainUtility.coexistenceDnsCnameValue
					});
					AcceptedDomainUtility.coexistenceDnsTextValue = string.Format("v=spf1 include:{0} -all", (serviceEndpoint5 != null) ? serviceEndpoint5.Uri.Host : "outlook.com");
					TaskLogger.Trace("Coexistence DNS TEXT value set to '{0}'", new object[]
					{
						AcceptedDomainUtility.coexistenceDnsTextValue
					});
					AcceptedDomainUtility.lastReadTime = DateTime.UtcNow;
					string text = (serviceEndpoint != null) ? serviceEndpoint.Uri.Host : string.Empty;
					if (text != null && !text.StartsWith("."))
					{
						text = "." + text;
					}
					AcceptedDomainUtility.coexistenceParentDomain = ((text != null) ? text : string.Empty);
					TaskLogger.Trace("Coexistence Parent Domain set to '{0}'", new object[]
					{
						AcceptedDomainUtility.coexistenceParentDomain
					});
				}
			}
		}

		internal static string CoexistenceMailDomainFfo15
		{
			get
			{
				AcceptedDomainUtility.ReloadProvisioningParameters();
				return AcceptedDomainUtility.coexistenceMailDomainFfo15;
			}
		}

		internal static string CoexistenceDnsCnameValue
		{
			get
			{
				AcceptedDomainUtility.ReloadProvisioningParameters();
				return AcceptedDomainUtility.coexistenceDnsCnameValue;
			}
		}

		internal static string CoexistenceDnsTextValue
		{
			get
			{
				AcceptedDomainUtility.ReloadProvisioningParameters();
				return AcceptedDomainUtility.coexistenceDnsTextValue;
			}
		}

		internal static string CoexistenceParentDomain
		{
			get
			{
				AcceptedDomainUtility.ReloadProvisioningParameters();
				return AcceptedDomainUtility.coexistenceParentDomain;
			}
		}

		internal static Uri DnsRegistrationEndpoint
		{
			get
			{
				AcceptedDomainUtility.ReloadProvisioningParameters();
				return AcceptedDomainUtility.dnsRegistrationEndpoint;
			}
		}

		internal static string DnsRegistrationCertificateSubject
		{
			get
			{
				AcceptedDomainUtility.ReloadProvisioningParameters();
				return AcceptedDomainUtility.dnsRegistrationCertificateSubject;
			}
		}

		internal static bool IsCoexistenceDomain(string domainName)
		{
			return AcceptedDomainUtility.IsDatacenter && !string.IsNullOrEmpty(AcceptedDomainUtility.CoexistenceParentDomain) && domainName.EndsWith(AcceptedDomainUtility.CoexistenceParentDomain, StringComparison.InvariantCultureIgnoreCase) && domainName.Length > AcceptedDomainUtility.CoexistenceParentDomain.Length;
		}

		internal static void RegisterCoexistenceDomain(string domainName)
		{
			if (string.IsNullOrEmpty(AcceptedDomainUtility.DnsRegistrationEndpoint.ToString()) || string.IsNullOrEmpty(AcceptedDomainUtility.DnsRegistrationCertificateSubject))
			{
				throw new CommunicationException("Cannot Register Coexistence Domain");
			}
			WebSvcDns webSvcDns = new WebSvcDns(AcceptedDomainUtility.DnsRegistrationEndpoint, AcceptedDomainUtility.DnsRegistrationCertificateSubject);
			webSvcDns.RegisterDomain(domainName, AcceptedDomainUtility.CoexistenceMailDomainFfo15, AcceptedDomainUtility.CoexistenceDnsCnameValue, AcceptedDomainUtility.CoexistenceDnsTextValue);
		}

		internal static void DeregisterCoexistenceDomain(string domainName)
		{
			if (string.IsNullOrEmpty(AcceptedDomainUtility.DnsRegistrationEndpoint.ToString()) || string.IsNullOrEmpty(AcceptedDomainUtility.DnsRegistrationCertificateSubject))
			{
				throw new CommunicationException("Cannot Deregister Coexistence Domain");
			}
			WebSvcDns webSvcDns = new WebSvcDns(AcceptedDomainUtility.DnsRegistrationEndpoint, AcceptedDomainUtility.DnsRegistrationCertificateSubject);
			webSvcDns.DeregisterDomain(domainName);
		}

		internal static void ValidateCatchAllRecipient(ADRecipient resolvedCatchAllRecipient, AcceptedDomain dataObject, bool catchAllRecipientModified, Task.TaskErrorLoggingDelegate errorWriter)
		{
			if (resolvedCatchAllRecipient != null)
			{
				if (resolvedCatchAllRecipient.RecipientType != RecipientType.SystemAttendantMailbox && resolvedCatchAllRecipient.RecipientType != RecipientType.SystemMailbox && resolvedCatchAllRecipient.RecipientType != RecipientType.UserMailbox && resolvedCatchAllRecipient.RecipientType != RecipientType.MailContact && resolvedCatchAllRecipient.RecipientType != RecipientType.MailUser)
				{
					errorWriter(new CatchAllRecipientTypeNotAllowedException(), ErrorCategory.InvalidArgument, null);
				}
				if (resolvedCatchAllRecipient.RecipientTypeDetails == RecipientTypeDetails.ArbitrationMailbox)
				{
					errorWriter(new CatchAllRecipientNotAllowedForArbitrationMailboxException(), ErrorCategory.InvalidArgument, null);
				}
			}
			bool flag;
			if (catchAllRecipientModified)
			{
				flag = (resolvedCatchAllRecipient != null);
			}
			else
			{
				flag = (dataObject.CatchAllRecipientID != null);
			}
			if (flag && dataObject.DomainType != AcceptedDomainType.Authoritative)
			{
				errorWriter(new CatchAllRecipientNotAllowedForNonAuthoritativeAcceptedDomainsException(), ErrorCategory.InvalidOperation, null);
			}
		}

		internal static void ValidateIfOutboundConnectorToRouteDomainIsPresent(IConfigDataProvider dataSession, AcceptedDomain dataObject, Task.TaskWarningLoggingDelegate errorWriter)
		{
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.EnforceOutboundConnectorAndAcceptedDomainsRestriction.Enabled && dataObject.DomainType != AcceptedDomainType.Authoritative && !ManageTenantOutboundConnectors.ValidateIfAcceptedDomainCanBeRoutedUsingConnectors(dataSession, dataObject.DomainName))
			{
				errorWriter(Strings.OutboundConnectorToRouteAcceptedDomainNotFound);
			}
		}

		internal static void ValidateMatchSubDomains(bool matchSubDomains, AcceptedDomainType domainType, Task.TaskErrorLoggingDelegate errorWriter)
		{
			if (matchSubDomains && domainType != AcceptedDomainType.InternalRelay)
			{
				errorWriter(new MatchSubDomainsIsInternalRelayOnlyException(), ErrorCategory.InvalidArgument, null);
			}
		}

		private static object LockObject = new object();

		private static TimeSpan reloadTimeSpan = TimeSpan.Parse("00:15:00");

		private static DateTime lastReadTime;

		private static string coexistenceParentDomain;

		private static string coexistenceMailDomainFfo15;

		private static string coexistenceDnsCnameValue;

		private static string coexistenceDnsTextValue;

		private static Uri dnsRegistrationEndpoint;

		private static string dnsRegistrationCertificateSubject;

		private static bool? isDatacenter;
	}
}
