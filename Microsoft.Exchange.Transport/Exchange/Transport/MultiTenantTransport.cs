using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport
{
	internal static class MultiTenantTransport
	{
		public static bool MultiTenancyEnabled
		{
			get
			{
				return VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled || DatacenterRegistry.IsForefrontForOffice() || DatacenterRegistry.IsPartnerHostedOnly();
			}
		}

		private static AlertingTracer AlertingTracer
		{
			get
			{
				if (MultiTenantTransport.alertingTracer == null)
				{
					MultiTenantTransport.alertingTracer = new AlertingTracer(MultiTenantTransport.Tracer, typeof(MultiTenantTransport).Name);
				}
				return MultiTenantTransport.alertingTracer;
			}
		}

		public static bool ContainsDirectionalityHeader(HeaderList headers)
		{
			Header header = headers.FindFirst("X-MS-Exchange-Organization-MessageDirectionality");
			return header != null;
		}

		public static bool TryParseDirectionality(string directionalityStr, out MailDirectionality directionality)
		{
			directionality = MailDirectionality.Undefined;
			if (!string.IsNullOrEmpty(directionalityStr))
			{
				if (directionalityStr.Equals(MultiTenantTransport.OriginatingStr, StringComparison.OrdinalIgnoreCase))
				{
					directionality = MailDirectionality.Originating;
					MultiTenantTransport.Tracer.TraceDebug(0L, "Parsed Originating directionality");
				}
				else if (directionalityStr.Equals(MultiTenantTransport.IncomingStr, StringComparison.OrdinalIgnoreCase))
				{
					directionality = MailDirectionality.Incoming;
					MultiTenantTransport.Tracer.TraceDebug(0L, "Parsed Incoming directionality");
				}
				else
				{
					MultiTenantTransport.Tracer.TraceError<string>(0L, "Invalid directionality header '{0}'", directionalityStr);
				}
			}
			else
			{
				MultiTenantTransport.Tracer.TraceError(0L, "Directionality header is present but empty");
			}
			return directionality != MailDirectionality.Undefined;
		}

		public static MailDirectionality GetDirectionalityFromHeader(TransportMailItem mailItem)
		{
			Header header = mailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-MessageDirectionality");
			MailDirectionality mailDirectionality;
			if (header == null || !MultiTenantTransport.TryParseDirectionality(Util.GetHeaderValue(header), out mailDirectionality) || mailDirectionality == MailDirectionality.Undefined)
			{
				mailDirectionality = (MultilevelAuth.IsInternalMail(mailItem.RootPart.Headers) ? MailDirectionality.Originating : MailDirectionality.Incoming);
			}
			return mailDirectionality;
		}

		public static OrganizationId GetOrganizationId(MailItem mailItem)
		{
			return MultiTenantTransport.GetOrganizationIdDelegate(mailItem);
		}

		public static MailDirectionality GetDirectionality(MailItem mailItem)
		{
			return MultiTenantTransport.GetDirectionalityDelegate(mailItem);
		}

		public static ADOperationResult TryGetExternalOrgId(OrganizationId orgId, out Guid externalOrgId)
		{
			externalOrgId = Guid.Empty;
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				if (MultiTenantTransport.MultiTenancyEnabled)
				{
					externalOrgId = MultiTenantTransport.SafeTenantId;
				}
				return ADOperationResult.Success;
			}
			ExchangeConfigurationUnit configUnitPassedToDelegate = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 260, "TryGetExternalOrgId", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\transport\\MultiTenantTransport.cs");
				configUnitPassedToDelegate = tenantOrTopologyConfigurationSession.Read<ExchangeConfigurationUnit>(orgId.ConfigurationUnit);
			});
			if (!adoperationResult.Succeeded)
			{
				return adoperationResult;
			}
			if (configUnitPassedToDelegate == null || !Guid.TryParse(configUnitPassedToDelegate.ExternalDirectoryOrganizationId, out externalOrgId))
			{
				return new ADOperationResult(ADOperationErrorCode.PermanentError, null);
			}
			return ADOperationResult.Success;
		}

		public static ADOperationResult TryGetOrganizationId(RoutingAddress address, out OrganizationId orgId)
		{
			MultiTenantTransport.<>c__DisplayClass4 CS$<>8__locals1 = new MultiTenantTransport.<>c__DisplayClass4();
			MultiTenantTransport.<>c__DisplayClass4 CS$<>8__locals2 = CS$<>8__locals1;
			OrganizationId localOrgId;
			orgId = (localOrgId = null);
			CS$<>8__locals2.localOrgId = localOrgId;
			if (!MultiTenantTransport.MultiTenancyEnabled)
			{
				orgId = OrganizationId.ForestWideOrgId;
				return ADOperationResult.Success;
			}
			ADOperationResult success;
			try
			{
				SmtpDomain domain = SmtpDomain.GetDomainPart(address);
				if (domain == null)
				{
					orgId = null;
					MultiTenantTransport.TraceAttributionError("Cannot get organization id for address without a domain: '{0}'", new object[]
					{
						address
					});
				}
				else
				{
					ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						CS$<>8__locals1.localOrgId = ADSessionSettings.FromTenantAcceptedDomain(domain.Domain).GetCurrentOrganizationIdPopulated();
					});
					if (!adoperationResult.Succeeded)
					{
						MultiTenantTransport.TraceAttributionError("Error {0} attributing based on domain {1}", new object[]
						{
							adoperationResult.Exception,
							address
						});
						return adoperationResult;
					}
				}
				orgId = CS$<>8__locals1.localOrgId;
				success = ADOperationResult.Success;
			}
			finally
			{
				if (orgId == null)
				{
					MultiTenantTransport.TraceAttributionError("Attributing to first org since domain lookup failed for {0}", new object[]
					{
						address
					});
					orgId = OrganizationId.ForestWideOrgId;
				}
			}
			return success;
		}

		public static ADOperationResult TryGetOrganizationId(Guid externalOrgId, out OrganizationId orgId, string exoAccountForest = null, string exoTenantContainer = null)
		{
			MultiTenantTransport.<>c__DisplayClassb CS$<>8__locals1 = new MultiTenantTransport.<>c__DisplayClassb();
			CS$<>8__locals1.externalOrgId = externalOrgId;
			CS$<>8__locals1.exoAccountForest = exoAccountForest;
			CS$<>8__locals1.exoTenantContainer = exoTenantContainer;
			ADOperationResult adoperationResult = ADOperationResult.Success;
			orgId = OrganizationId.ForestWideOrgId;
			if (!MultiTenantTransport.MultiTenancyEnabled || CS$<>8__locals1.externalOrgId == MultiTenantTransport.SafeTenantId || CS$<>8__locals1.externalOrgId == Guid.Empty)
			{
				return adoperationResult;
			}
			ADOperationResult result;
			try
			{
				MultiTenantTransport.<>c__DisplayClasse CS$<>8__locals2 = new MultiTenantTransport.<>c__DisplayClasse();
				CS$<>8__locals2.CS$<>8__localsc = CS$<>8__locals1;
				MultiTenantTransport.<>c__DisplayClasse CS$<>8__locals3 = CS$<>8__locals2;
				OrganizationId localOrgId;
				orgId = (localOrgId = null);
				CS$<>8__locals3.localOrgId = localOrgId;
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.UseTenantPartitionToCreateOrganizationId.Enabled && !DatacenterRegistry.IsForefrontForOffice() && !string.IsNullOrEmpty(CS$<>8__locals1.exoAccountForest) && !string.IsNullOrEmpty(CS$<>8__locals1.exoTenantContainer))
				{
					adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						CS$<>8__locals2.localOrgId = ADSessionSettings.FromTenantForestAndCN(CS$<>8__locals2.CS$<>8__localsc.exoAccountForest, CS$<>8__locals2.CS$<>8__localsc.exoTenantContainer).GetCurrentOrganizationIdPopulated();
					});
					if (adoperationResult.Succeeded && CS$<>8__locals2.localOrgId != null)
					{
						orgId = CS$<>8__locals2.localOrgId;
						return adoperationResult;
					}
					MultiTenantTransport.TraceAttributionError("Error {0} reading org from EXO Account Forest: {1} and EXO Tenant Container: {2}", new object[]
					{
						adoperationResult.Exception,
						CS$<>8__locals1.exoAccountForest ?? "<NULL>",
						CS$<>8__locals1.exoTenantContainer ?? "<NULL>"
					});
				}
				adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					CS$<>8__locals2.localOrgId = ADSessionSettings.FromExternalDirectoryOrganizationId(CS$<>8__locals2.CS$<>8__localsc.externalOrgId).GetCurrentOrganizationIdPopulated();
					if (CS$<>8__locals2.localOrgId == null)
					{
						MultiTenantTransport.TraceAttributionError("ADSessionSettings has null CurrentOrganizationId", new object[0]);
						throw new InvalidOperationException("ADSessionSettings has null CurrentOrganizationId");
					}
				});
				if (!adoperationResult.Succeeded)
				{
					MultiTenantTransport.TraceAttributionError("Error {0} reading org from external org id {1}", new object[]
					{
						adoperationResult.Exception,
						CS$<>8__locals1.externalOrgId
					});
				}
				orgId = CS$<>8__locals2.localOrgId;
				result = adoperationResult;
			}
			finally
			{
				if (orgId == null)
				{
					MultiTenantTransport.TraceAttributionError("Org Id is null for external org id {0}. Attributing to First Org", new object[]
					{
						CS$<>8__locals1.externalOrgId
					});
					orgId = OrganizationId.ForestWideOrgId;
				}
			}
			return result;
		}

		public static ADOperationResult TryCreateADRecipientCache(TransportMailItem tmi)
		{
			if (!MultiTenantTransport.MultiTenancyEnabled || tmi.ExternalOrganizationId == Guid.Empty || tmi.ExternalOrganizationId == MultiTenantTransport.SafeTenantId)
			{
				MultiTenantTransport.UpdateADRecipientCacheAndOrganizationScope(tmi, OrganizationId.ForestWideOrgId);
				return ADOperationResult.Success;
			}
			if (tmi.ADRecipientCache != null && tmi.ADRecipientCache.OrganizationId != null)
			{
				return ADOperationResult.Success;
			}
			OrganizationId orgId;
			ADOperationResult adoperationResult = MultiTenantTransport.TryGetOrganizationId(tmi.ExternalOrganizationId, out orgId, tmi.ExoAccountForest, tmi.ExoTenantContainer);
			if (!adoperationResult.Succeeded)
			{
				return adoperationResult;
			}
			MultiTenantTransport.UpdateADRecipientCacheAndOrganizationScope(tmi, orgId);
			return ADOperationResult.Success;
		}

		public static ADOperationResult TryUpdateScopeAndDirectionality(TransportMailItem tmi, MailDirectionality directionality, Guid externalOrgId, string exoAccountForest, string exoTenantContainer)
		{
			OrganizationId orgId;
			ADOperationResult adoperationResult = MultiTenantTransport.TryGetOrganizationId(externalOrgId, out orgId, null, null);
			if (!adoperationResult.Succeeded)
			{
				return adoperationResult;
			}
			tmi.ExternalOrganizationId = externalOrgId;
			tmi.Directionality = directionality;
			tmi.ExoAccountForest = exoAccountForest;
			tmi.ExoTenantContainer = exoTenantContainer;
			MultiTenantTransport.UpdateADRecipientCacheAndOrganizationScope(tmi, orgId);
			return ADOperationResult.Success;
		}

		public static ADOperationResult TryUpdateScopeAndDirectionalityFromOrgId(TransportMailItem mailItem, MailDirectionality directionality, OrganizationId orgId)
		{
			mailItem.Directionality = directionality;
			if (OrganizationId.ForestWideOrgId.Equals(orgId))
			{
				mailItem.ExternalOrganizationId = MultiTenantTransport.SafeTenantId;
				return ADOperationResult.Success;
			}
			Guid externalOrganizationId;
			ADOperationResult adoperationResult = MultiTenantTransport.TryGetExternalOrgId(orgId, out externalOrganizationId);
			if (adoperationResult.Succeeded)
			{
				mailItem.ExternalOrganizationId = externalOrganizationId;
				return ADOperationResult.Success;
			}
			return adoperationResult;
		}

		public static ADOperationResult TryAttributeMessageUsingHeaders(TransportMailItem mailItem)
		{
			mailItem.Directionality = MultiTenantTransport.GetDirectionalityFromHeader(mailItem);
			Guid externalOrganizationId;
			if (MultiTenantTransport.TryGetExternalOrganizationIdFromHeader(mailItem, out externalOrganizationId))
			{
				mailItem.ExternalOrganizationId = externalOrganizationId;
				return ADOperationResult.Success;
			}
			return MultiTenantTransport.TryAttributeFromDomain(mailItem);
		}

		public static ADOperationResult TryAttributeReplayMessage(TransportMailItem mailItem)
		{
			mailItem.Directionality = MultiTenantTransport.GetDirectionalityFromHeader(mailItem);
			return MultiTenantTransport.TryAttributeFromDomain(mailItem);
		}

		public static ADOperationResult TryAttributePickupMessage(TransportMailItem mailItem)
		{
			mailItem.Directionality = MailDirectionality.Incoming;
			return MultiTenantTransport.TryAttributeFromDomain(mailItem);
		}

		public static ADOperationResult TryAttributeFromDomain(TransportMailItem mailItem)
		{
			ADOperationResult adoperationResult = ADOperationResult.Success;
			Guid empty = Guid.Empty;
			if (mailItem.Directionality == MailDirectionality.Originating)
			{
				adoperationResult = MultiTenantTransport.TryGetExternalOrganizationIdForSender(mailItem.RootPart.Headers, mailItem.From, out empty);
			}
			else if (mailItem.Recipients != null && mailItem.Recipients.Count > 0)
			{
				adoperationResult = MultiTenantTransport.TryGetExternalOrganizationIdForRecipient(mailItem.Recipients[0], out empty);
			}
			if (!adoperationResult.Succeeded)
			{
				MultiTenantTransport.TraceAttributionError(string.Format("Error {0} attributing from domain. {1}", adoperationResult.Exception, MultiTenantTransport.ToString(mailItem)), new object[0]);
				return adoperationResult;
			}
			mailItem.ExternalOrganizationId = empty;
			return ADOperationResult.Success;
		}

		public static ADOperationResult TryAttributeProxiedClientSubmission(TransportMailItem mailItem)
		{
			mailItem.Directionality = MailDirectionality.Originating;
			return MultiTenantTransport.TryAttributeFromDomain(mailItem);
		}

		public static void UpdateADRecipientCacheAndOrganizationScope(TransportMailItem mailItem, OrganizationId orgId)
		{
			if (mailItem.ADRecipientCache != null && mailItem.ADRecipientCache.OrganizationId.Equals(orgId))
			{
				return;
			}
			mailItem.ADRecipientCache = MultiTenantTransport.CreateRecipientCache(orgId);
			MultiTenantTransport.Tracer.TraceDebug(0L, "Created recipient cache for scope '{0}'", new object[]
			{
				MultiTenantTransport.ToTrace(orgId)
			});
			MultiTenantTransport.UpdateOrganizationScope(mailItem);
		}

		public static void UpdateOrganizationScope(TransportMailItem mailItem)
		{
			ADRecipientCache<TransportMiniRecipient> adrecipientCache = mailItem.ADRecipientCache;
			if (adrecipientCache == null)
			{
				throw new ArgumentNullException("scopedRecipientCache");
			}
			OrganizationId organizationId = adrecipientCache.OrganizationId;
			if (organizationId == null)
			{
				throw new InvalidOperationException("UpdateScope() called for mail item with null OrganizationId in recipient cache");
			}
			Guid value;
			if (organizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				value = Guid.Empty;
			}
			else
			{
				value = organizationId.ConfigurationUnit.ObjectGuid;
				if (value.Equals(Guid.Empty))
				{
					throw new InvalidOperationException(string.Format("Empty ObjectGuid for config unit '{0}'", organizationId.ConfigurationUnit));
				}
			}
			mailItem.OrganizationScope = new Guid?(value);
		}

		public static void TraceAttributionError(string format, params object[] args)
		{
			try
			{
				string stackTrace = Environment.StackTrace;
				string text = ((args == null || args.Length == 0) ? format : string.Format(format, args)) + stackTrace;
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_MessageAttributionFailed, string.Empty, new object[]
				{
					text
				});
				MultiTenantTransport.AlertingTracer.TraceError(0, text, new object[0]);
			}
			catch (FormatException)
			{
				MultiTenantTransport.AlertingTracer.TraceError(0, "Error Logging", new object[0]);
			}
		}

		public static string ToString(TransportMailItem mailItem)
		{
			if (mailItem == null)
			{
				return "mailItem is <null>";
			}
			string format = "From: {0}, To:{1}, Directionality: {2}, ExternalOrgId: {3}, OrgId: {4}, ";
			object[] array = new object[5];
			array[0] = mailItem.From;
			array[1] = string.Join(",", from r in mailItem.Recipients
			select r.ToString());
			array[2] = mailItem.Directionality;
			array[3] = mailItem.ExternalOrganizationId;
			array[4] = mailItem.OrganizationId;
			return string.Format(format, array);
		}

		private static object ToTrace(OrganizationId orgId)
		{
			ADObjectId result;
			if (!(orgId == null))
			{
				if ((result = orgId.OrganizationalUnit) == null)
				{
					return "<ForestWideOrgId>";
				}
			}
			else
			{
				result = "<null>";
			}
			return result;
		}

		private static TransportMailItem GetTransportMailItem(MailItem mailItem)
		{
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)mailItem;
			return (TransportMailItem)transportMailItemWrapperFacade.TransportMailItem;
		}

		private static ADRecipientCache<TransportMiniRecipient> CreateRecipientCache(OrganizationId orgId)
		{
			return new ADRecipientCache<TransportMiniRecipient>(TransportMiniRecipientSchema.Properties, 0, orgId);
		}

		private static ADOperationResult TryGetExternalOrganizationIdForSender(HeaderList headers, RoutingAddress p1Sender, out Guid externalOrgId)
		{
			OrganizationId organizationId = null;
			externalOrgId = Guid.Empty;
			RoutingAddress routingAddress;
			ADOperationResult adoperationResult;
			if (Util.TryGetP2Sender(headers, out routingAddress))
			{
				adoperationResult = MultiTenantTransport.TryGetOrganizationId(routingAddress, out organizationId);
				if (!adoperationResult.Succeeded)
				{
					MultiTenantTransport.TraceAttributionError("Error {0} when trying to get OrgId from sender P2 domain {1}", new object[]
					{
						adoperationResult.Exception,
						routingAddress
					});
					return adoperationResult;
				}
			}
			if (organizationId == null && p1Sender.IsValid && p1Sender != RoutingAddress.NullReversePath)
			{
				adoperationResult = MultiTenantTransport.TryGetOrganizationId(p1Sender, out organizationId);
				if (!adoperationResult.Succeeded)
				{
					MultiTenantTransport.TraceAttributionError("Error {0} when trying to get OrgId from sender P1 domain {1}", new object[]
					{
						adoperationResult.Exception,
						p1Sender
					});
					return adoperationResult;
				}
			}
			if (organizationId != null)
			{
				adoperationResult = MultiTenantTransport.TryGetExternalOrgId(organizationId, out externalOrgId);
				if (!adoperationResult.Succeeded)
				{
					MultiTenantTransport.TraceAttributionError("Error {0} when trying to get Ext OrgId from orgId {1}", new object[]
					{
						adoperationResult.Exception,
						organizationId
					});
				}
			}
			else
			{
				organizationId = OrganizationId.ForestWideOrgId;
				externalOrgId = MultiTenantTransport.SafeTenantId;
				adoperationResult = ADOperationResult.Success;
			}
			return adoperationResult;
		}

		private static ADOperationResult TryGetExternalOrganizationIdForRecipient(MailRecipient recipient, out Guid externalOrgId)
		{
			OrganizationId orgId;
			ADOperationResult adoperationResult = MultiTenantTransport.TryGetOrganizationId(recipient.Email, out orgId);
			externalOrgId = Guid.Empty;
			if (!adoperationResult.Succeeded)
			{
				MultiTenantTransport.Tracer.TraceError<string, RoutingAddress>(0L, "Error {0} when trying to get OrgId from recipient {1}", adoperationResult.Exception.ToString(), recipient.Email);
				return adoperationResult;
			}
			return MultiTenantTransport.TryGetExternalOrgId(orgId, out externalOrgId);
		}

		private static bool TryGetExternalOrganizationIdFromHeader(TransportMailItem mailItem, out Guid externalOrganizationId)
		{
			externalOrganizationId = Guid.Empty;
			Header header = mailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Id");
			return header != null && Guid.TryParse(header.Value, out externalOrganizationId) && externalOrganizationId != Guid.Empty;
		}

		public static readonly Guid SafeTenantId = new Guid("5afe0b00-7697-4969-b663-5eab37d5f47e");

		public static readonly string OriginatingStr = MailDirectionality.Originating.ToString();

		public static readonly string IncomingStr = MailDirectionality.Incoming.ToString();

		public static Func<MailItem, OrganizationId> GetOrganizationIdDelegate = (MailItem mailItem) => MultiTenantTransport.GetTransportMailItem(mailItem).OrganizationId;

		public static Func<MailItem, MailDirectionality> GetDirectionalityDelegate = (MailItem mailItem) => MultiTenantTransport.GetTransportMailItem(mailItem).Directionality;

		private static readonly Trace Tracer = ExTraceGlobals.GeneralTracer;

		private static AlertingTracer alertingTracer;
	}
}
