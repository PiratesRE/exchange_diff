using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net.Sockets;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class ManageTenantOutboundConnectors
	{
		public static void ClearSmartHostsListIfNecessary(TenantOutboundConnector outboundConnector)
		{
			if (outboundConnector.UseMXRecord && outboundConnector.SmartHosts != null && outboundConnector.SmartHosts.Any<SmartHost>())
			{
				outboundConnector.SmartHosts = null;
			}
		}

		public static LocalizedException ValidateConnectorNameReferences(TenantOutboundConnector tenantOutboundConnector, string previousName, IConfigDataProvider dataSession)
		{
			IEnumerable<TransportRule> source;
			if ((tenantOutboundConnector.IsChanged(TenantOutboundConnectorSchema.Enabled) || tenantOutboundConnector.IsChanged(TenantOutboundConnectorSchema.IsTransportRuleScoped) || tenantOutboundConnector.IsChanged(ADObjectSchema.Name)) && (!tenantOutboundConnector.Enabled || !tenantOutboundConnector.IsTransportRuleScoped || !string.Equals(previousName, tenantOutboundConnector.Name, StringComparison.OrdinalIgnoreCase)) && Utils.TryGetTransportRules(dataSession, new Utils.TransportRuleSelectionDelegate(Utils.RuleHasOutboundConnectorReference), out source, previousName) && source.Any<TransportRule>())
			{
				return new ConnectorIncorrectUsageConnectorStillReferencedException();
			}
			return null;
		}

		public static void ValidateOutboundConnectorDataObject(TenantOutboundConnector tenantOutboundConnector, Task task, IConfigDataProvider dataSession, bool skipIpIsNotReservedValidation)
		{
			ManageTenantOutboundConnectors.ValidateUseMxRecordRestriction(tenantOutboundConnector, task);
			ManageTenantOutboundConnectors.ValidateSmartHostRestrictions(tenantOutboundConnector, task, skipIpIsNotReservedValidation);
			ManageTenantOutboundConnectors.ValidateRouteAllMessagesViaOnPremisesParameter(tenantOutboundConnector, dataSession, task);
			ManageTenantOutboundConnectors.ValidateRecipientDomainsParameter(tenantOutboundConnector, task);
		}

		public static void ValidateIfAcceptedDomainsCanBeRoutedWithConnectors(TenantOutboundConnector dataObject, IConfigDataProvider dataSession, Task task, bool dataObjectDeleted = false)
		{
			ArgumentValidator.ThrowIfNull("dataObject", dataObject);
			ArgumentValidator.ThrowIfNull("dataSession", dataSession);
			ArgumentValidator.ThrowIfNull("task", task);
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.Transport.EnforceOutboundConnectorAndAcceptedDomainsRestriction.Enabled)
			{
				return;
			}
			if (!dataObjectDeleted && dataObject.Enabled && dataObject.ConnectorType == TenantConnectorType.OnPremises && dataObject.AllAcceptedDomains)
			{
				return;
			}
			List<SmtpDomainWithSubdomains> nonAuthoritativeAcceptedDomains = ManageTenantOutboundConnectors.GetNonAuthoritativeAcceptedDomains(dataSession);
			if (nonAuthoritativeAcceptedDomains == null)
			{
				return;
			}
			List<SmtpDomainWithSubdomains> source;
			if (!ManageTenantOutboundConnectors.ValidateIfConnectorsCanRouteDomains(ManageTenantOutboundConnectors.GetOutboundConnectorsToValidate(dataSession, dataObjectDeleted ? null : dataObject), nonAuthoritativeAcceptedDomains, out source))
			{
				task.WriteWarning(Strings.AcceptedDomainsCannotBeRoutedByOutboundConnector(string.Join(",", from d in source
				select d.Domain)));
			}
		}

		public static bool ValidateIfAcceptedDomainCanBeRoutedUsingConnectors(IConfigDataProvider dataSession, SmtpDomainWithSubdomains acceptedDomain)
		{
			ArgumentValidator.ThrowIfNull("dataSession", dataSession);
			ArgumentValidator.ThrowIfNull("acceptedDomain", acceptedDomain);
			TenantOutboundConnector[] connectors = (TenantOutboundConnector[])dataSession.Find<TenantOutboundConnector>(null, null, true, null);
			List<SmtpDomainWithSubdomains> list;
			return ManageTenantOutboundConnectors.ValidateIfConnectorsCanRouteDomains(connectors, new SmtpDomainWithSubdomains[]
			{
				acceptedDomain
			}, out list);
		}

		private static void ValidateSmartHostRestrictions(TenantOutboundConnector tenantOutboundConnector, Task task, bool skipIpIsNotReservedValidation)
		{
			if (tenantOutboundConnector.IsChanged(TenantOutboundConnectorSchema.SmartHosts) && !MultiValuedPropertyBase.IsNullOrEmpty(tenantOutboundConnector.SmartHosts))
			{
				MultiValuedProperty<IPRange> multiValuedProperty = null;
				bool flag = false;
				using (MultiValuedProperty<SmartHost>.Enumerator enumerator = tenantOutboundConnector.SmartHosts.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SmartHost smartHost = enumerator.Current;
						if (smartHost.IsIPAddress)
						{
							if (smartHost.Address.AddressFamily != AddressFamily.InterNetwork || !IPAddressValidation.IsValidIPv4Address(smartHost.Address.ToString()))
							{
								task.WriteError(new SmartHostsIPValidationFailedException(smartHost.Address.ToString()), ErrorCategory.InvalidArgument, null);
							}
							if (!skipIpIsNotReservedValidation)
							{
								if (IPAddressValidation.IsReservedIPv4Address(smartHost.Address.ToString()))
								{
									task.WriteError(new IPRangeInConnectorContainsReservedIPAddressesException(smartHost.Address.ToString()), ErrorCategory.InvalidArgument, null);
								}
								if (!flag)
								{
									if (!HygieneDCSettings.GetFfoDCPublicIPAddresses(out multiValuedProperty))
									{
										task.WriteError(new ConnectorValidationFailedException(), ErrorCategory.ConnectionError, null);
									}
									flag = true;
								}
								if (!MultiValuedPropertyBase.IsNullOrEmpty(multiValuedProperty))
								{
									if (multiValuedProperty.Any((IPRange ffoDCIP) => ffoDCIP.Contains(smartHost.Address)))
									{
										task.WriteError(new IPRangeInConnectorContainsReservedIPAddressesException(smartHost.Address.ToString()), ErrorCategory.InvalidArgument, null);
									}
								}
							}
						}
					}
				}
			}
		}

		private static void ValidateUseMxRecordRestriction(TenantOutboundConnector tenantOutboundConnector, Task task)
		{
			if (tenantOutboundConnector.ConnectorType == TenantConnectorType.OnPremises && tenantOutboundConnector.UseMXRecord)
			{
				task.WriteError(new OnPremisesConnectorHasRouteUsingMXException(), ErrorCategory.InvalidArgument, null);
			}
		}

		private static void ValidateRouteAllMessagesViaOnPremisesParameter(TenantOutboundConnector dataObject, IConfigDataProvider dataSession, Task task)
		{
			if (dataObject.RouteAllMessagesViaOnPremises && !NewInboundConnector.FindTenantScopedOnPremiseInboundConnector(dataSession, null))
			{
				task.WriteError(new CMCConnectorRequiresTenantScopedInboundConnectorException(), ErrorCategory.InvalidArgument, null);
			}
		}

		private static void ValidateRecipientDomainsParameter(TenantOutboundConnector dataObject, Task task)
		{
			if (dataObject.ConnectorType == TenantConnectorType.OnPremises && !dataObject.RouteAllMessagesViaOnPremises && dataObject.RecipientDomains != null && dataObject.RecipientDomains.Count > 0)
			{
				foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in dataObject.RecipientDomains)
				{
					if (smtpDomainWithSubdomains.IsStar)
					{
						task.WriteError(new RecipientDomainStarOnPremiseOutboundConnectorException(), ErrorCategory.InvalidArgument, null);
					}
				}
			}
		}

		private static bool ValidateIfConnectorsCanRouteDomains(IEnumerable<TenantOutboundConnector> connectors, IEnumerable<SmtpDomainWithSubdomains> acceptedDomains, out List<SmtpDomainWithSubdomains> domainsThatCannotBeRouted)
		{
			domainsThatCannotBeRouted = null;
			using (IEnumerator<SmtpDomainWithSubdomains> enumerator = acceptedDomains.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SmtpDomainWithSubdomains domain = enumerator.Current;
					bool flag = false;
					foreach (TenantOutboundConnector tenantOutboundConnector in connectors)
					{
						if (tenantOutboundConnector.Enabled && tenantOutboundConnector.ConnectorType == TenantConnectorType.OnPremises)
						{
							if (tenantOutboundConnector.AllAcceptedDomains)
							{
								return true;
							}
							if (tenantOutboundConnector.RecipientDomains.Any((SmtpDomainWithSubdomains r) => new WildcardPattern(r.Address).Match(domain.Address) != -1))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						if (domainsThatCannotBeRouted == null)
						{
							domainsThatCannotBeRouted = new List<SmtpDomainWithSubdomains>();
						}
						domainsThatCannotBeRouted.Add(domain);
					}
				}
			}
			return domainsThatCannotBeRouted == null;
		}

		private static List<SmtpDomainWithSubdomains> GetNonAuthoritativeAcceptedDomains(IConfigDataProvider dataSession)
		{
			List<SmtpDomainWithSubdomains> list = null;
			AcceptedDomain[] array = (AcceptedDomain[])dataSession.Find<AcceptedDomain>(null, null, true, null);
			foreach (AcceptedDomain acceptedDomain in array)
			{
				if (acceptedDomain.DomainType != AcceptedDomainType.Authoritative && acceptedDomain.DomainName != null)
				{
					if (list == null)
					{
						list = new List<SmtpDomainWithSubdomains>();
					}
					list.Add(acceptedDomain.DomainName);
				}
			}
			return list;
		}

		private static IEnumerable<TenantOutboundConnector> GetOutboundConnectorsToValidate(IConfigDataProvider dataSession, TenantOutboundConnector connectorBeingModified)
		{
			if (connectorBeingModified != null)
			{
				yield return connectorBeingModified;
			}
			TenantOutboundConnector[] connectors = (TenantOutboundConnector[])dataSession.Find<TenantOutboundConnector>(null, null, true, null);
			foreach (TenantOutboundConnector connector in connectors)
			{
				if (connectorBeingModified == null || ((ADObjectId)connector.Identity).ObjectGuid != ((ADObjectId)connectorBeingModified.Identity).ObjectGuid)
				{
					yield return connector;
				}
			}
			yield break;
		}
	}
}
