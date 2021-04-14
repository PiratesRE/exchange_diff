using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
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
			OutboundConversionOptions outboundConversionOptions = IrmUtils.GetOutboundConversionOptions(((MailboxSession)storeSession).MailboxOwner.MailboxInfo.OrganizationId);
			rightsManagedMessageItem.TryDecode(outboundConversionOptions, acquireLicense);
		}

		internal static bool IsMessageRestrictedAndDecoded(Item item)
		{
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			return rightsManagedMessageItem != null && rightsManagedMessageItem.IsRestricted && rightsManagedMessageItem.IsDecoded;
		}

		internal static bool IsIrmEnabled(bool clientSupportsIrm, StoreSession mailboxSession)
		{
			return clientSupportsIrm && IrmUtils.DoesSessionSupportIrm(mailboxSession);
		}

		internal static MailboxSession ValidateAndGetMailboxSession(StoreSession storeSession)
		{
			MailboxSession mailboxSession = storeSession as MailboxSession;
			if (mailboxSession == null)
			{
				throw new RightsManagementPermanentException(CoreResources.RightsManagementMailboxOnlySupport, null);
			}
			return mailboxSession;
		}

		internal static string GetItemPreview(Item xsoItem)
		{
			Body effectiveBody = Util.GetEffectiveBody(xsoItem);
			if (effectiveBody == null)
			{
				return null;
			}
			return effectiveBody.PreviewText;
		}

		internal static Body GetBody(Item xsoItem)
		{
			RightsManagedMessageItem rightsManagedMessageItem = xsoItem as RightsManagedMessageItem;
			if (rightsManagedMessageItem != null && rightsManagedMessageItem.IsDecoded)
			{
				return rightsManagedMessageItem.ProtectedBody;
			}
			return xsoItem.Body;
		}

		internal static AttachmentCollection GetAttachmentCollection(Item xsoItem)
		{
			AttachmentCollection result;
			if (IrmUtils.IsMessageRestrictedAndDecoded(xsoItem))
			{
				result = ((RightsManagedMessageItem)xsoItem).ProtectedAttachmentCollection;
			}
			else
			{
				result = xsoItem.AttachmentCollection;
			}
			return result;
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

		internal static bool IsApplyingRmsTemplate(string complianceString, StoreSession session, out RmsTemplate template)
		{
			template = null;
			return !string.IsNullOrEmpty(complianceString) && "0" != complianceString && IrmUtils.TryLookupRmsTemplate(complianceString, session, out template);
		}

		private static Guid ValidateAndGetComplianceId(string complianceIdString)
		{
			Guid guid;
			if (!GuidHelper.TryParseGuid(complianceIdString, out guid) || Guid.Empty.Equals(guid))
			{
				throw new RightsManagementPermanentException(CoreResources.ErrorInvalidComplianceId, null);
			}
			return guid;
		}

		private static CultureInfo CalculateClientCulture(StoreSession session)
		{
			CultureInfo cultureInfo = session.Capabilities.CanHaveCulture ? session.PreferedCulture : CultureInfo.CurrentUICulture;
			return CallContext.Current.ClientCulture ?? cultureInfo;
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
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<Exception>(0L, "Failed to lookup RMS template due to: {0}", ex);
			}
			return null != template;
		}

		private static bool IsMessageDecoded(Item item)
		{
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			return rightsManagedMessageItem != null && rightsManagedMessageItem.IsDecoded;
		}

		private static T GetProperty<T>(IStorePropertyBag propertyBag, PropertyDefinition propertyDefinition, T defaultValue)
		{
			object obj = propertyBag.TryGetProperty(propertyDefinition);
			if (obj is PropertyError || obj == null)
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		internal static RightsManagedMessageDecryptionStatus TryDecrypt(RightsManagedMessageItem item, OrganizationId organizationId)
		{
			if (!item.CanDecode)
			{
				return RightsManagedMessageDecryptionStatus.NotSupported;
			}
			if (item.IsDecoded)
			{
				return RightsManagedMessageDecryptionStatus.Success;
			}
			bool flag;
			try
			{
				flag = RmsClientManager.IRMConfig.IsClientAccessServerEnabledForTenant(organizationId);
			}
			catch (ExchangeConfigurationException arg)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<ExchangeConfigurationException>(0L, "Failed to get IRM tenant config due to: {0}", arg);
				flag = false;
			}
			if (!flag)
			{
				return RightsManagedMessageDecryptionStatus.FeatureDisabled;
			}
			return item.TryDecode(IrmUtils.GetOutboundConversionOptions(organizationId), true);
		}

		internal static void DecryptForGetAttachment(StoreSession session, RightsManagedMessageItem irmItem)
		{
			if (irmItem.IsDecoded)
			{
				return;
			}
			if (!IrmUtils.DoesSessionSupportIrm(session))
			{
				throw new RightsManagementPermanentException(RightsManagementFailureCode.FeatureDisabled, CoreResources.IrmRmsErrorMessage("IRM not enabled on CAS"));
			}
			if (!irmItem.CanDecode)
			{
				throw new RightsManagementPermanentException(RightsManagementFailureCode.NotSupported, CoreResources.IrmRmsErrorMessage("Cannot Decode Irm Item."));
			}
			OrganizationId organizationId = session.MailboxOwner.MailboxInfo.OrganizationId;
			irmItem.Decode(IrmUtils.GetOutboundConversionOptions(organizationId), true);
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

		internal static Body GetBodyFromDecryptedIrmItem(RightsManagedMessageItem decryptedIrmItem)
		{
			RightsManagedMessageDecryptionStatus decryptionStatus = decryptedIrmItem.DecryptionStatus;
			if (RightsManagedMessageDecryptionStatus.FeatureDisabled.Equals(decryptionStatus))
			{
				throw new RightsManagementPermanentException(CoreResources.IrmFeatureDisabled, null);
			}
			if (!decryptedIrmItem.CanDecode)
			{
				throw new RightsManagementPermanentException(CoreResources.ErrorIrmNotSupported, null);
			}
			if (!RightsManagedMessageDecryptionStatus.Success.Equals(decryptionStatus))
			{
				throw new RightsManagementPermanentException(CoreResources.IrmRmsErrorMessage(decryptionStatus.ToString()), null);
			}
			return decryptedIrmItem.ProtectedBody;
		}

		internal static void CopyMessageClassificationProperties(Item oldItem, Item newItem)
		{
			bool property = IrmUtils.GetProperty<bool>(oldItem, ItemSchema.ClassificationKeep, false);
			if (property)
			{
				string property2 = IrmUtils.GetProperty<string>(oldItem, ItemSchema.ClassificationGuid, string.Empty);
				if (!string.IsNullOrEmpty(property2))
				{
					OrganizationId organizationId = (CallContext.Current.AccessingPrincipal == null) ? OrganizationId.ForestWideOrgId : CallContext.Current.AccessingPrincipal.MailboxInfo.OrganizationId;
					Guid classificationId = IrmUtils.ValidateAndGetComplianceId(property2);
					CultureInfo clientCulture = IrmUtils.CalculateClientCulture(oldItem.Session);
					ClassificationSummary classificationSummary = IrmUtils.LookupMessageClassification(classificationId, organizationId, clientCulture);
					if (classificationSummary != null)
					{
						newItem[ItemSchema.ClassificationGuid] = property2;
						newItem[ItemSchema.IsClassified] = IrmUtils.GetProperty<bool>(oldItem, ItemSchema.IsClassified, false);
						newItem[ItemSchema.Classification] = IrmUtils.GetProperty<string>(oldItem, ItemSchema.Classification, string.Empty);
						newItem[ItemSchema.ClassificationDescription] = IrmUtils.GetProperty<string>(oldItem, ItemSchema.ClassificationDescription, string.Empty);
						newItem[ItemSchema.ClassificationKeep] = IrmUtils.GetProperty<bool>(oldItem, ItemSchema.ClassificationKeep, false);
					}
				}
			}
		}

		internal static void UpdateCompliance(string complianceIdString, Item xsoItem, RmsTemplate template)
		{
			if ("0".Equals(complianceIdString, StringComparison.OrdinalIgnoreCase))
			{
				xsoItem[ItemSchema.IsClassified] = false;
				xsoItem[ItemSchema.ClassificationGuid] = string.Empty;
				xsoItem[ItemSchema.ClassificationDescription] = string.Empty;
				xsoItem[ItemSchema.Classification] = string.Empty;
				if (IrmUtils.IsMessageRestrictedAndDecoded(xsoItem))
				{
					((RightsManagedMessageItem)xsoItem).SetRestriction(null);
				}
				return;
			}
			Guid guid = Guid.Empty;
			if (template == null)
			{
				guid = IrmUtils.ValidateAndGetComplianceId(complianceIdString);
				OrganizationId organizationId = (CallContext.Current.AccessingPrincipal == null) ? OrganizationId.ForestWideOrgId : CallContext.Current.AccessingPrincipal.MailboxInfo.OrganizationId;
				CultureInfo clientCulture = IrmUtils.CalculateClientCulture(xsoItem.Session);
				ClassificationSummary classificationSummary = IrmUtils.LookupMessageClassification(guid, organizationId, clientCulture);
				if (classificationSummary != null)
				{
					xsoItem[ItemSchema.IsClassified] = true;
					xsoItem[ItemSchema.ClassificationGuid] = classificationSummary.ClassificationID.ToString();
					xsoItem[ItemSchema.ClassificationDescription] = classificationSummary.SenderDescription;
					xsoItem[ItemSchema.Classification] = classificationSummary.DisplayName;
					xsoItem[ItemSchema.ClassificationKeep] = classificationSummary.RetainClassificationEnabled;
					if (IrmUtils.IsMessageRestrictedAndDecoded(xsoItem))
					{
						((RightsManagedMessageItem)xsoItem).SetRestriction(null);
					}
					return;
				}
			}
			MailboxSession mailboxSession = xsoItem.Session as MailboxSession;
			if (mailboxSession != null && IrmUtils.IsMessageDecoded(xsoItem))
			{
				if (template == null)
				{
					template = IrmUtils.LookupRmsTemplate(guid, mailboxSession);
					if (template == null)
					{
						throw new RightsManagementPermanentException(CoreResources.ErrorRightsManagementTemplateNotFound(complianceIdString), null);
					}
				}
				RightsManagedMessageItem rightsManagedMessageItem = (RightsManagedMessageItem)xsoItem;
				rightsManagedMessageItem.SetRestriction(template);
				if (null == rightsManagedMessageItem.Sender)
				{
					rightsManagedMessageItem.Sender = new Participant(mailboxSession.MailboxOwner);
				}
			}
			xsoItem[ItemSchema.IsClassified] = false;
			xsoItem[ItemSchema.ClassificationGuid] = string.Empty;
			xsoItem[ItemSchema.ClassificationDescription] = string.Empty;
			xsoItem[ItemSchema.Classification] = string.Empty;
		}

		internal static OutboundConversionOptions GetOutboundConversionOptions(OrganizationId orgId)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 341, "GetOutboundConversionOptions", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Server\\ServiceCommands\\IrmUtils.cs");
			return new OutboundConversionOptions(IrmUtils.GetDefaultAcceptedDomainName(orgId))
			{
				ClearCategories = false,
				AllowPartialStnefConversion = true,
				DemoteBcc = true,
				UserADSession = tenantOrRootOrgRecipientSession
			};
		}

		internal static object GetAlternateBodyForIrm(Microsoft.Exchange.Data.Storage.BodyFormat bodyFormat, RightsManagedMessageDecryptionStatus decryptionStatus, bool isProtectedVoicemail)
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			if (bodyFormat == Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml)
			{
				sanitizingStringBuilder.Append("<font face=\"Calibri, sans-serif\" style=\"font-size:11pt\">");
			}
			RightsManagementFailureCode failureCode = decryptionStatus.FailureCode;
			if (failureCode > RightsManagementFailureCode.PreLicenseAcquisitionFailed)
			{
				switch (failureCode)
				{
				case RightsManagementFailureCode.FailedToExtractTargetUriFromMex:
				case RightsManagementFailureCode.FailedToDownloadMexData:
					sanitizingStringBuilder.Append<LocalizedString>(CoreResources.IrmReachNotConfigured);
					goto IL_2BA;
				case RightsManagementFailureCode.GetServerInfoFailed:
					goto IL_17D;
				case RightsManagementFailureCode.InternalLicensingDisabled:
					break;
				case RightsManagementFailureCode.ExternalLicensingDisabled:
					sanitizingStringBuilder.Append<LocalizedString>(CoreResources.IrmExternalLicensingDisabled);
					goto IL_2BA;
				default:
					switch (failureCode)
					{
					case RightsManagementFailureCode.ServerRightNotGranted:
						sanitizingStringBuilder.Append<LocalizedString>(CoreResources.IrmServerMisConfigured);
						goto IL_2BA;
					case RightsManagementFailureCode.InvalidLicensee:
						goto IL_10B;
					case RightsManagementFailureCode.FeatureDisabled:
						break;
					case RightsManagementFailureCode.NotSupported:
						sanitizingStringBuilder.Append<LocalizedString>(isProtectedVoicemail ? CoreResources.IrmProtectedVoicemailFeatureDisabled : CoreResources.IrmFeatureDisabled);
						goto IL_2BA;
					case RightsManagementFailureCode.CorruptData:
						sanitizingStringBuilder.Append<LocalizedString>(CoreResources.IrmCorruptProtectedMessage);
						goto IL_2BA;
					case RightsManagementFailureCode.MissingLicense:
					{
						MissingRightsManagementLicenseException ex = (MissingRightsManagementLicenseException)decryptionStatus.Exception;
						if (bodyFormat == Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml)
						{
							sanitizingStringBuilder.AppendFormat("<div id=\"divIrmReqSpinner\" sReqCorrelator=\"{0}\" style=\"text-align:center;\">", new object[]
							{
								ex.RequestCorrelator
							});
						}
						sanitizingStringBuilder.Append<LocalizedString>(CoreResources.Loading);
						if (bodyFormat == Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml)
						{
							sanitizingStringBuilder.Append("</div>");
							goto IL_2BA;
						}
						goto IL_2BA;
					}
					default:
						if (failureCode != RightsManagementFailureCode.Success)
						{
							goto IL_17D;
						}
						goto IL_2BA;
					}
					break;
				}
				sanitizingStringBuilder.Append<LocalizedString>(isProtectedVoicemail ? CoreResources.IrmProtectedVoicemailFeatureDisabled : CoreResources.IrmFeatureDisabled);
				goto IL_2BA;
			}
			if (failureCode == RightsManagementFailureCode.UserRightNotGranted)
			{
				sanitizingStringBuilder.Append<LocalizedString>(CoreResources.IrmViewRightNotGranted);
				goto IL_2BA;
			}
			if (failureCode != RightsManagementFailureCode.PreLicenseAcquisitionFailed)
			{
				goto IL_17D;
			}
			IL_10B:
			sanitizingStringBuilder.Append<LocalizedString>(CoreResources.IrmPreLicensingFailure);
			goto IL_2BA;
			IL_17D:
			sanitizingStringBuilder.Append<LocalizedString>(CoreResources.IrmRmsError);
			Exception exception = decryptionStatus.Exception;
			if (Global.ShowDebugInformation && bodyFormat == Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml && exception != null && exception.InnerException != null)
			{
				sanitizingStringBuilder.AppendFormat("<hr><div onclick=\"document.getElementById('divDtls').style.display='';this.style.display='none';\" style=\"cursor: pointer; color: #3165cd;\">", new object[0]);
				sanitizingStringBuilder.AppendFormat("{0}</div><br><div id=\"divDtls\" style='display:none'>", new object[]
				{
					CoreResources.ShowDetails
				});
				string text = string.Empty;
				RightsManagementFailureCode failureCode2 = decryptionStatus.FailureCode;
				Exception innerException = exception.InnerException;
				if (innerException is RightsManagementException)
				{
					RightsManagementException ex2 = (RightsManagementException)innerException;
					text = ex2.RmsUrl;
				}
				int num = 0;
				while (num < 10 && innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
					num++;
				}
				sanitizingStringBuilder.Append(WebUtility.HtmlEncode(CoreResources.IrmRmsErrorMessage(innerException.Message)));
				if (!string.IsNullOrEmpty(text))
				{
					sanitizingStringBuilder.Append("<br>");
					sanitizingStringBuilder.Append(WebUtility.HtmlEncode(CoreResources.IrmRmsErrorLocation(text)));
				}
				if (failureCode2 != RightsManagementFailureCode.Success)
				{
					sanitizingStringBuilder.Append("<br>");
					sanitizingStringBuilder.Append(WebUtility.HtmlEncode(CoreResources.IrmRmsErrorCode(failureCode2.ToString())));
				}
				sanitizingStringBuilder.Append("</div>");
			}
			IL_2BA:
			if (bodyFormat == Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml)
			{
				sanitizingStringBuilder.Append("</font>");
			}
			return sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>();
		}

		internal static void ThrowIfInternalLicensingDisabled(OrganizationId organizationId)
		{
			if (!RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(organizationId))
			{
				throw new RightsManagementPermanentException(CoreResources.RightsManagementInternalLicensingDisabled, null);
			}
		}

		private static string GetDefaultAcceptedDomainName(OrganizationId organizationId)
		{
			string text;
			if (IrmUtils.defaultAcceptedDomainTable.TryGetValue(organizationId, out text))
			{
				return text;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 582, "GetDefaultAcceptedDomainName", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Server\\ServiceCommands\\IrmUtils.cs");
			AcceptedDomain defaultAcceptedDomain = tenantOrTopologyConfigurationSession.GetDefaultAcceptedDomain();
			if (defaultAcceptedDomain != null)
			{
				text = defaultAcceptedDomain.DomainName.ToString();
			}
			IrmUtils.defaultAcceptedDomainTable.Add(organizationId, text);
			return text;
		}

		private static ClassificationSummary LookupMessageClassification(Guid classificationId, OrganizationId organizationId, CultureInfo clientCulture)
		{
			IrmUtils.classificationConfig = (IrmUtils.classificationConfig ?? new ClassificationConfig());
			return IrmUtils.classificationConfig.GetClassification(organizationId, classificationId, clientCulture);
		}

		private static RmsTemplate LookupRmsTemplate(Guid templateId, MailboxSession session)
		{
			OrganizationId organizationId = session.MailboxOwner.MailboxInfo.OrganizationId;
			IrmUtils.ThrowIfInternalLicensingDisabled(organizationId);
			IEnumerable<RmsTemplate> source = RmsClientManager.AcquireRmsTemplates(organizationId, false);
			RmsTemplate result = null;
			try
			{
				result = source.SingleOrDefault((RmsTemplate template) => template.Id.Equals(templateId));
			}
			catch (InvalidOperationException)
			{
				throw new RightsManagementPermanentException(CoreResources.ErrorRightsManagementDuplicateTemplateId(templateId.ToString()), null);
			}
			return result;
		}

		private const string ProtectedVMItemClassPrefix = "IPM.Note.RPMSG.Microsoft.Voicemail";

		internal const string NoRestrictionComplianceId = "0";

		private static MruDictionaryCache<OrganizationId, string> defaultAcceptedDomainTable = new MruDictionaryCache<OrganizationId, string>(5, 50000, 5);

		private static ClassificationConfig classificationConfig;
	}
}
