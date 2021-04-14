using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class InfobarMessageBuilder : InfobarMessageBuilderBase
	{
		public static void AddImportance(Infobar infobar, IStorePropertyBag storePropertyBag)
		{
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			string importance = InfobarMessageBuilderBase.GetImportance(storePropertyBag);
			if (importance != null)
			{
				infobar.AddMessage(Utilities.SanitizeHtmlEncode(importance), InfobarMessageType.Informational);
			}
		}

		public static void AddSensitivity(Infobar infobar, IStorePropertyBag storePropertyBag)
		{
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			string sensitivity = InfobarMessageBuilderBase.GetSensitivity(storePropertyBag);
			if (sensitivity != null)
			{
				infobar.AddMessage(Utilities.SanitizeHtmlEncode(sensitivity), InfobarMessageType.Informational);
			}
		}

		public static void AddFlag(Infobar infobar, IStorePropertyBag storePropertyBag, UserContext userContext)
		{
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			InfobarMessage flag = InfobarMessageBuilder.GetFlag(storePropertyBag, userContext);
			if (flag != null)
			{
				infobar.AddMessage(flag);
			}
		}

		public static void AddNoEditPermissionWarning(Infobar infobar, Item item, bool isPreviewForm)
		{
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if ((Utilities.IsPublic(item) || Utilities.IsOtherMailbox(item)) && !isPreviewForm && !ItemUtility.UserCanEditItem(item))
			{
				infobar.AddMessage(SanitizedHtmlString.FromStringId(2078257811), InfobarMessageType.Informational);
			}
		}

		public new static InfobarMessage GetFlag(IStorePropertyBag storePropertyBag, UserContext userContext)
		{
			string flag = InfobarMessageBuilderBase.GetFlag(storePropertyBag, userContext);
			if (flag != null)
			{
				return new InfobarMessage(Utilities.SanitizeHtmlEncode(flag), InfobarMessageType.Informational, "divFlg");
			}
			return null;
		}

		public static void AddCompliance(UserContext userContext, Infobar infobar, IStorePropertyBag storePropertyBag, bool isSenderMessage)
		{
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			string compliance = InfobarMessageBuilderBase.GetCompliance(userContext, storePropertyBag, isSenderMessage);
			if (compliance != null)
			{
				infobar.AddMessage(Utilities.SanitizeHtmlEncode(compliance), InfobarMessageType.Informational, "divCmplIB");
			}
		}

		public new static InfobarMessage GetCompliance(UserContext userContext, Guid complianceId)
		{
			string compliance = InfobarMessageBuilderBase.GetCompliance(userContext, complianceId);
			if (compliance != null)
			{
				SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
				sanitizingStringBuilder.Append(compliance);
				if (userContext.ComplianceReader.GetComplianceType(complianceId, userContext.UserCulture) == ComplianceType.RmsTemplate)
				{
					string str = string.Format(LocalizedStrings.GetNonEncoded(1670455506), userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
					sanitizingStringBuilder.Append("<br>");
					sanitizingStringBuilder.Append(str);
				}
				return new InfobarMessage(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational, "divCmplIB");
			}
			return null;
		}

		public static void AddReadReceiptNotice(UserContext userContext, Infobar infobar, IStorePropertyBag storePropertyBag)
		{
			if (InfobarMessageBuilderBase.ShouldRenderReadReceiptNoticeInfobar(userContext, storePropertyBag))
			{
				SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(115261126));
				sanitizingStringBuilder.Append(" <a href=# id=aRqRcpt>");
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1190033799));
				sanitizingStringBuilder.Append("</a>");
				infobar.AddMessage(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational, "divSRR");
			}
		}

		public static void AddIrmInformation(Infobar infobar, MessageItem item, bool isPreviewForm, bool addConversationOwner, bool addRemoveLink, bool addAttachDisclaimer)
		{
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (addAttachDisclaimer)
			{
				infobar.AddMessage(SanitizedHtmlString.FromStringId(-914838464), InfobarMessageType.Informational);
			}
			if (!Utilities.IsIrmRestrictedAndDecrypted(item))
			{
				return;
			}
			RightsManagedMessageItem rightsManagedMessageItem = (RightsManagedMessageItem)item;
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			string str = string.Format(LocalizedStrings.GetNonEncoded(-500320626), rightsManagedMessageItem.Restriction.Name, rightsManagedMessageItem.Restriction.Description);
			sanitizingStringBuilder.Append(str);
			if (addRemoveLink)
			{
				sanitizingStringBuilder.Append(" <a id=\"aIbRR\" href=# ");
				sanitizingStringBuilder.AppendFormat("_sIT=\"IPM.Note\" _sAct=\"{0}\" _fRR=1", new object[]
				{
					isPreviewForm ? "Preview" : string.Empty
				});
				sanitizingStringBuilder.Append(">");
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(540836651));
				sanitizingStringBuilder.Append("</a>");
			}
			if (addConversationOwner && !isPreviewForm && rightsManagedMessageItem.ConversationOwner != null && !string.IsNullOrEmpty(rightsManagedMessageItem.ConversationOwner.EmailAddress))
			{
				string str2 = string.Format(LocalizedStrings.GetNonEncoded(1670455506), rightsManagedMessageItem.ConversationOwner.EmailAddress);
				sanitizingStringBuilder.Append("<br>");
				sanitizingStringBuilder.Append(str2);
			}
			infobar.AddMessage(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational, "divCmplIB");
		}

		public static void AddIrmInformation(Infobar infobar, ItemPart itemPart, bool addRemoveLink)
		{
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			if (itemPart == null)
			{
				throw new ArgumentNullException("itemPart");
			}
			if (!itemPart.IrmInfo.IsRestricted || itemPart.IrmInfo.DecryptionStatus.Failed)
			{
				return;
			}
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			string str = string.Format(LocalizedStrings.GetNonEncoded(-500320626), itemPart.IrmInfo.TemplateName, itemPart.IrmInfo.TemplateDescription);
			sanitizingStringBuilder.Append(str);
			if (addRemoveLink)
			{
				sanitizingStringBuilder.Append(" <a id=\"aIbRR\" href=\"#\">");
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(540836651));
				sanitizingStringBuilder.Append("</a>");
			}
			infobar.AddMessage(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational, "divCmplIB");
		}

		public static void AddDeletePolicyInformation(Infobar infobar, IStorePropertyBag storePropertyBag, UserContext userContext)
		{
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			if (storePropertyBag == null)
			{
				throw new ArgumentNullException("storePropertyBag");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (PolicyProvider.DeletePolicyProvider.IsPolicyEnabled(userContext.MailboxSession) && ItemUtility.HasDeletePolicy(storePropertyBag))
			{
				ExDateTime now = ExDateTime.Now;
				ExDateTime property = ItemUtility.GetProperty<ExDateTime>(storePropertyBag, ItemSchema.RetentionDate, ExDateTime.MinValue);
				if (property > ExDateTime.MinValue)
				{
					TimeSpan timeSpan = property - now;
					string s = string.Format(LocalizedStrings.GetNonEncoded(217128690), (timeSpan.Days < 0) ? 0 : timeSpan.Days);
					Guid? guid = null;
					byte[] property2 = ItemUtility.GetProperty<byte[]>(storePropertyBag, StoreObjectSchema.PolicyTag, null);
					if (property2 != null && property2.Length == InfobarMessageBuilder.SizeOfGuid)
					{
						guid = new Guid?(new Guid(property2));
					}
					PolicyTagList allPolicies = PolicyProvider.DeletePolicyProvider.GetAllPolicies(userContext.MailboxSession);
					List<PolicyTag> list = new List<PolicyTag>(allPolicies.Values.Count);
					if (guid != null && allPolicies.Count > 0)
					{
						foreach (PolicyTag policyTag in allPolicies.Values)
						{
							if (object.Equals(policyTag.PolicyGuid, guid))
							{
								list.Add(policyTag);
							}
						}
					}
					string s2 = string.Format(LocalizedStrings.GetNonEncoded(2084315882), (list.Count > 0) ? list[0].Name : string.Empty, (list.Count > 0) ? PolicyContextMenuBase.TimeSpanToString(list[0].TimeSpanForRetention) : string.Empty, property.ToString("d", CultureInfo.CurrentUICulture));
					infobar.AddMessage(Utilities.SanitizeHtmlEncode(s2), InfobarMessageType.Informational);
					if (timeSpan.Days < 30)
					{
						infobar.AddMessage(Utilities.SanitizeHtmlEncode(s), InfobarMessageType.Informational);
					}
				}
			}
		}

		public const string ComplianceDivId = "divCmplIB";

		public const string FlagDivId = "divFlg";

		public const string SendReadReceiptDivId = "divSRR";

		private static readonly int SizeOfGuid = Marshal.SizeOf(Guid.NewGuid());
	}
}
