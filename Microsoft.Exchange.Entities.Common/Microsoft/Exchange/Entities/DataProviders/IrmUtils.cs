using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics.Components.Entities;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Entities.DataProviders
{
	internal static class IrmUtils
	{
		internal static void DecodeIrmMessage(StoreSession storeSession, Item mailboxItem, bool acquireLicense)
		{
			RightsManagedMessageItem rightsManagedMessageItem = mailboxItem as RightsManagedMessageItem;
			if (rightsManagedMessageItem == null)
			{
				return;
			}
			if (!IrmUtils.DoesSessionSupportIrm(storeSession))
			{
				return;
			}
			if (!rightsManagedMessageItem.IsRestricted)
			{
				return;
			}
			if (!rightsManagedMessageItem.CanDecode)
			{
				return;
			}
			if (rightsManagedMessageItem.IsDecoded)
			{
				return;
			}
			OutboundConversionOptions outboundConversionOptions = IrmUtils.GetOutboundConversionOptions(storeSession.MailboxOwner.MailboxInfo.OrganizationId);
			rightsManagedMessageItem.TryDecode(outboundConversionOptions, acquireLicense);
		}

		internal static bool DoesSessionSupportIrm(StoreSession session)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession == null)
			{
				return false;
			}
			OrganizationId organizationId = mailboxSession.MailboxOwner.MailboxInfo.OrganizationId;
			bool result;
			try
			{
				result = RmsClientManager.IRMConfig.IsClientAccessServerEnabledForTenant(organizationId);
			}
			catch (ExchangeConfigurationException innerException)
			{
				throw new RightsManagementTransientException(ServerStrings.RmExceptionGenericMessage, innerException);
			}
			catch (RightsManagementException ex)
			{
				if (ex.IsPermanent)
				{
					throw new RightsManagementPermanentException(ServerStrings.RmExceptionGenericMessage, ex);
				}
				throw new RightsManagementTransientException(ServerStrings.RmExceptionGenericMessage, ex);
			}
			return result;
		}

		internal static AttachmentCollection GetAttachmentCollection(IItem xsoItem)
		{
			return IrmUtils.IsMessageRestrictedAndDecoded(xsoItem) ? ((RightsManagedMessageItem)xsoItem).ProtectedAttachmentCollection : xsoItem.AttachmentCollection;
		}

		internal static Body GetBody(IItem xsoItem)
		{
			RightsManagedMessageItem rightsManagedMessageItem = xsoItem as RightsManagedMessageItem;
			if (rightsManagedMessageItem != null && rightsManagedMessageItem.IsDecoded)
			{
				return rightsManagedMessageItem.ProtectedBody;
			}
			return xsoItem.Body;
		}

		internal static string GetItemPreview(Item xsoItem)
		{
			Body body = IrmUtils.GetBody(xsoItem);
			if (body == null)
			{
				return null;
			}
			return body.PreviewText;
		}

		internal static OutboundConversionOptions GetOutboundConversionOptions(OrganizationId orgId)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 211, "GetOutboundConversionOptions", "f:\\15.00.1497\\sources\\dev\\Entities\\src\\Common\\DataProviders\\IrmUtils.cs");
			return new OutboundConversionOptions(IrmUtils.GetDefaultAcceptedDomainName(orgId))
			{
				ClearCategories = false,
				AllowPartialStnefConversion = true,
				DemoteBcc = true,
				UserADSession = tenantOrRootOrgRecipientSession
			};
		}

		internal static bool IsApplyingRmsTemplate(string complianceString, StoreSession session, out RmsTemplate template)
		{
			template = null;
			return !string.IsNullOrEmpty(complianceString) && "0" != complianceString && IrmUtils.TryLookupRmsTemplate(complianceString, session, out template);
		}

		internal static bool IsIrmEnabled(bool clientSupportsIrm, StoreSession mailboxSession)
		{
			return clientSupportsIrm && IrmUtils.DoesSessionSupportIrm(mailboxSession);
		}

		internal static bool IsMessageRestrictedAndDecoded(IItem item)
		{
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			return rightsManagedMessageItem != null && rightsManagedMessageItem.IsRestricted && rightsManagedMessageItem.IsDecoded;
		}

		internal static bool IsProtectedVoicemailItem(RightsManagedMessageItem item)
		{
			string valueOrDefault = item.GetValueOrDefault<string>(StoreObjectSchema.ItemClass);
			return valueOrDefault.StartsWith("IPM.Note.RPMSG.Microsoft.Voicemail", StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsProtectedVoicemailItem(ItemPart itemPart)
		{
			string valueOrDefault = itemPart.StorePropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
			return valueOrDefault.StartsWith("IPM.Note.RPMSG.Microsoft.Voicemail", StringComparison.OrdinalIgnoreCase);
		}

		internal static void ThrowIfInternalLicensingDisabled(OrganizationId organizationId)
		{
			if (!RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(organizationId))
			{
				throw new RightsManagementPermanentException(Strings.RightsManagementInternalLicensingDisabled, null);
			}
		}

		internal static MailboxSession ValidateAndGetMailboxSession(StoreSession storeSession)
		{
			MailboxSession mailboxSession = storeSession as MailboxSession;
			if (mailboxSession == null)
			{
				throw new RightsManagementPermanentException(Strings.RightsManagementMailboxOnlySupport, null);
			}
			return mailboxSession;
		}

		private static string GetDefaultAcceptedDomainName(OrganizationId organizationId)
		{
			string text;
			if (IrmUtils.DefaultAcceptedDomainTable.TryGetValue(organizationId, out text))
			{
				return text;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 375, "GetDefaultAcceptedDomainName", "f:\\15.00.1497\\sources\\dev\\Entities\\src\\Common\\DataProviders\\IrmUtils.cs");
			AcceptedDomain defaultAcceptedDomain = tenantOrTopologyConfigurationSession.GetDefaultAcceptedDomain();
			if (defaultAcceptedDomain != null)
			{
				text = defaultAcceptedDomain.DomainName.ToString();
			}
			IrmUtils.DefaultAcceptedDomainTable.Add(organizationId, text);
			return text;
		}

		private static RmsTemplate LookupRmsTemplate(Guid templateId, MailboxSession session)
		{
			OrganizationId organizationId = session.MailboxOwner.MailboxInfo.OrganizationId;
			IrmUtils.ThrowIfInternalLicensingDisabled(organizationId);
			IEnumerable<RmsTemplate> source = RmsClientManager.AcquireRmsTemplates(organizationId, false);
			RmsTemplate result;
			try
			{
				result = source.SingleOrDefault((RmsTemplate template) => template.Id.Equals(templateId));
			}
			catch (InvalidOperationException)
			{
				throw new RightsManagementPermanentException(Strings.ErrorRightsManagementDuplicateTemplateId(templateId.ToString()), null);
			}
			return result;
		}

		private static bool TryLookupRmsTemplate(string complianceIdString, StoreSession session, out RmsTemplate template)
		{
			template = null;
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession == null)
			{
				return false;
			}
			Guid guid;
			if (!GuidHelper.TryParseGuid(complianceIdString, out guid) || Guid.Empty.Equals(guid))
			{
				return false;
			}
			Exception ex = null;
			try
			{
				template = IrmUtils.LookupRmsTemplate(guid, mailboxSession);
			}
			catch (RightsManagementPermanentException ex2)
			{
				ex = ex2;
			}
			catch (RightsManagementTransientException ex3)
			{
				ex = ex3;
			}
			catch (ExchangeConfigurationException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ExTraceGlobals.CommonTracer.TraceError<Exception>(0L, "Failed to lookup RMS template due to: {0}", ex);
			}
			return null != template;
		}

		internal const string NoRestrictionComplianceId = "0";

		private const string ProtectedVmItemClassPrefix = "IPM.Note.RPMSG.Microsoft.Voicemail";

		private static readonly MruDictionaryCache<OrganizationId, string> DefaultAcceptedDomainTable = new MruDictionaryCache<OrganizationId, string>(5, 50000, 5);
	}
}
