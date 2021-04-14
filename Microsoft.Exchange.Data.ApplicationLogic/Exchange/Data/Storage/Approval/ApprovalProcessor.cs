using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Approval.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage.Approval
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ApprovalProcessor
	{
		private static MailboxSession CreateMailboxSession(SmtpAddress proxy, CultureInfo cultureInfo)
		{
			if (!proxy.IsValidAddress)
			{
				throw new ArgumentException("Invalid mailbox SmtpAddress " + proxy, "proxy");
			}
			ADSessionSettings adSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(proxy.Domain);
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromProxyAddress(adSettings, (string)proxy, RemotingOptions.AllowCrossSite);
			return MailboxSession.OpenAsAdmin(mailboxOwner, cultureInfo ?? CultureInfo.InvariantCulture, "Client=ApprovalAPI");
		}

		private static bool IsValidDecision(SmtpAddress decisionMakerSmtpAddress, MessageItem initMessageItem)
		{
			if (initMessageItem == null)
			{
				throw new ArgumentNullException("initMessageItem");
			}
			string validDecisionMakers = initMessageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalAllowedDecisionMakers);
			if (string.IsNullOrEmpty(validDecisionMakers))
			{
				ApprovalProcessor.diag.TraceError(0L, "Initiation message has no decision maker listed, ignoring decision");
				return false;
			}
			string decisionMaker = decisionMakerSmtpAddress.ToString();
			if (validDecisionMakers.IndexOf(decisionMaker, StringComparison.OrdinalIgnoreCase) != -1)
			{
				return true;
			}
			ApprovalProcessor.diag.TraceError<string>(0L, "primary smtp {0} is not match, check the secondary smtp address", decisionMaker);
			bool validDecisionFlag = false;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				IRecipientSession recipientSession = ApprovalProcessor.CreateRecipientSessionFromSmtpAddress(decisionMakerSmtpAddress);
				ADRawEntry adrawEntry = recipientSession.FindByProxyAddress(new SmtpProxyAddress(decisionMaker, false), new PropertyDefinition[]
				{
					ADRecipientSchema.EmailAddresses
				});
				if (adrawEntry != null)
				{
					ProxyAddressCollection proxyAddressCollection = adrawEntry[ADRecipientSchema.EmailAddresses] as ProxyAddressCollection;
					if (proxyAddressCollection != null)
					{
						foreach (ProxyAddress proxyAddress in proxyAddressCollection)
						{
							if (ProxyAddressPrefix.Smtp.Equals(proxyAddress.Prefix) && validDecisionMakers.IndexOf(proxyAddress.AddressString, StringComparison.OrdinalIgnoreCase) != -1)
							{
								validDecisionFlag = true;
								break;
							}
						}
					}
				}
			}, 3);
			if (!adoperationResult.Succeeded)
			{
				ApprovalProcessor.diag.TraceError<ADOperationErrorCode, Exception>(0L, "AD Operation Failed, ErrorCode: {0}, Exception: {1}", adoperationResult.ErrorCode, adoperationResult.Exception);
			}
			return validDecisionFlag;
		}

		private static DecisionConflict CheckForExistingDecision(MessageItem initMessageItem, ApprovalStatus newApprovalStatus, string newDecisionMaker, out string existingDecisionMaker, out ApprovalStatus? existingApprovalStatus, out ExDateTime? existingDecisionTime)
		{
			existingDecisionMaker = initMessageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalDecisionMaker);
			existingApprovalStatus = initMessageItem.GetValueAsNullable<ApprovalStatus>(MessageItemSchema.ApprovalStatus);
			existingDecisionTime = initMessageItem.GetValueAsNullable<ExDateTime>(MessageItemSchema.ApprovalDecisionTime);
			if (newApprovalStatus != ApprovalStatus.Approved && newApprovalStatus != ApprovalStatus.Rejected)
			{
				throw new ArgumentException("unexpected approvalStatus, only approved, and rejected are valid for this call.");
			}
			if (existingApprovalStatus == null)
			{
				ApprovalProcessor.diag.TraceError(0L, "There is no approval status");
				return DecisionConflict.Unknown;
			}
			if ((existingApprovalStatus & ApprovalStatus.Cancelled) != (ApprovalStatus)0)
			{
				return DecisionConflict.AlreadyCancelled;
			}
			if ((existingApprovalStatus & ApprovalStatus.Expired) != (ApprovalStatus)0)
			{
				return DecisionConflict.AlreadyExpired;
			}
			if ((existingApprovalStatus & ApprovalStatus.Approved) != (ApprovalStatus)0 || (existingApprovalStatus & ApprovalStatus.Rejected) != (ApprovalStatus)0)
			{
				bool flag = (newApprovalStatus & existingApprovalStatus) == newApprovalStatus;
				if (!string.Equals(newDecisionMaker, existingDecisionMaker, StringComparison.OrdinalIgnoreCase))
				{
					if (flag)
					{
						return DecisionConflict.DifferentApproverSameDecision;
					}
					return DecisionConflict.DifferentApproverDifferentDecision;
				}
				else
				{
					if (flag)
					{
						return DecisionConflict.SameApproverAndDecision;
					}
					return DecisionConflict.SameApproverDifferentDecision;
				}
			}
			else
			{
				if (!((existingApprovalStatus & (ApprovalStatus)(-1048321)) == ApprovalStatus.Unhandled))
				{
					ApprovalProcessor.diag.TraceError<ApprovalStatus?>(0L, "Unexpected state of Approval status, current status is {0}", existingApprovalStatus);
					return DecisionConflict.Unknown;
				}
				if (existingDecisionMaker != null)
				{
					ApprovalProcessor.diag.TraceError(0L, "There is already a decision maker. Unexpected if all users are going thru approvalprocessor");
					return DecisionConflict.HasApproverMissingDecision;
				}
				return DecisionConflict.NoConflict;
			}
		}

		private static ConflictResolutionResult WriteDecisionAndComment(MessageItem initMessageItem, ApprovalStatus approvalStatus, string newDecisionMaker, Body body, byte[] buffer)
		{
			initMessageItem.OpenAsReadWrite();
			initMessageItem[MessageItemSchema.ApprovalDecisionTime] = ExDateTime.UtcNow;
			initMessageItem[MessageItemSchema.ApprovalDecisionMaker] = newDecisionMaker;
			initMessageItem[MessageItemSchema.ApprovalStatus] = approvalStatus;
			if (ApprovalProcessor.BodyHasComments(body))
			{
				using (StreamAttachment streamAttachment = (StreamAttachment)initMessageItem.AttachmentCollection.Create(AttachmentType.Stream))
				{
					using (Stream contentStream = streamAttachment.GetContentStream())
					{
						using (Stream stream = body.OpenReadStream(new BodyReadConfiguration(BodyFormat.TextHtml)))
						{
							streamAttachment.FileName = "DecisionComments.txt";
							ApprovalProcessor.CopyStream(stream, contentStream, buffer);
							contentStream.Flush();
							string charset = body.Charset;
							if (charset != null)
							{
								streamAttachment[AttachmentSchema.TextAttachmentCharset] = charset;
							}
							streamAttachment.Save();
						}
					}
				}
			}
			return initMessageItem.Save(SaveMode.FailOnAnyConflict);
		}

		private static bool BodyHasComments(Body body)
		{
			if (body == null || body.Size == 0L)
			{
				return false;
			}
			using (TextReader textReader = body.OpenTextReader(BodyFormat.TextPlain))
			{
				char[] array = new char[3];
				int num = textReader.Read(array, 0, 3);
				if (num == 3)
				{
					return true;
				}
				for (int i = 0; i < num; i++)
				{
					if (!char.IsWhiteSpace(array[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static DecisionConflict MarkDecisionAndStatus(StoreObjectId itemId, MailboxSession session, ApprovalStatus approvalStatus, SmtpAddress decisionMakerAddress, Body decisionBody, out string existingDecisionMaker, out ApprovalStatus? existingApprovalStatus, out ExDateTime? existingDecisionTime)
		{
			existingDecisionMaker = null;
			existingApprovalStatus = null;
			existingDecisionTime = null;
			if (itemId == null)
			{
				return DecisionConflict.MissingItem;
			}
			if (session == null)
			{
				throw new ArgumentNullException("MailboxSession should not be null.");
			}
			if (!session.ClientInfoString.Equals("Client=ApprovalAPI", StringComparison.Ordinal) && session.LogonType != LogonType.Transport)
			{
				throw new ArgumentException("MailboxSession must be opened by Transport or ApprovalAPI.");
			}
			if (approvalStatus != ApprovalStatus.Approved && approvalStatus != ApprovalStatus.Rejected)
			{
				throw new ArgumentException("unexpected approvalStatus, only approved, and rejected are valid for this call.");
			}
			if (!decisionMakerAddress.IsValidAddress)
			{
				throw new ArgumentException("Invalid decisionMaker SmtpAddress " + decisionMakerAddress, "decisionMakerAddress");
			}
			byte[] buffer = null;
			if (decisionBody != null && decisionBody.Size != 0L)
			{
				buffer = new byte[2048];
			}
			string newDecisionMaker = (string)decisionMakerAddress;
			try
			{
				for (int i = 0; i < 3; i++)
				{
					using (MessageItem messageItem = MessageItem.Bind(session, itemId, ApprovalProcessor.ApprovalProperties))
					{
						if (!ApprovalProcessor.IsValidDecision(decisionMakerAddress, messageItem))
						{
							ApprovalProcessor.diag.TraceError(0L, "Decision maker is not in the allowed list");
							return DecisionConflict.Unauthorized;
						}
						DecisionConflict decisionConflict = ApprovalProcessor.CheckForExistingDecision(messageItem, approvalStatus, newDecisionMaker, out existingDecisionMaker, out existingApprovalStatus, out existingDecisionTime);
						if (decisionConflict != DecisionConflict.NoConflict)
						{
							return decisionConflict;
						}
						ConflictResolutionResult conflictResolutionResult = ApprovalProcessor.WriteDecisionAndComment(messageItem, (existingApprovalStatus.Value & ~ApprovalStatus.Unhandled) | approvalStatus, newDecisionMaker, decisionBody, buffer);
						if (conflictResolutionResult.SaveStatus == SaveResult.Success)
						{
							return DecisionConflict.NoConflict;
						}
						ApprovalProcessor.diag.TraceDebug<SaveResult, int>(0L, "Decision not marked because save returns {0}, retrying for the {1} time", conflictResolutionResult.SaveStatus, i);
					}
				}
			}
			catch (ObjectNotFoundException arg)
			{
				ApprovalProcessor.diag.TraceDebug<ObjectNotFoundException>(0L, "Message is no longer there {0} before it is bound", arg);
				return DecisionConflict.MissingItem;
			}
			ApprovalProcessor.diag.TraceError(0L, "Decision not marked because save failed after retries.");
			return DecisionConflict.DifferentApproverDifferentDecision;
		}

		private static void SendNotification(SmtpAddress arbitrationProxy, StoreObjectId itemId, SmtpAddress[] recipients, LocalizedString subject, string body, string messageClass, CultureInfo messageCulture)
		{
			if (!arbitrationProxy.IsValidAddress)
			{
				throw new ArgumentException("Invalid arbitration mailbox SmtpAddress " + arbitrationProxy, "arbitrationProxy");
			}
			if (recipients == null || recipients.Length == 0)
			{
				throw new ArgumentNullException("Recipients should not be null or empty.");
			}
			foreach (SmtpAddress smtpAddress in recipients)
			{
				if (!smtpAddress.IsValidAddress)
				{
					throw new ArgumentException("Invalid recipient SmtpAddress " + smtpAddress, "recipient");
				}
			}
			using (MailboxSession mailboxSession = ApprovalProcessor.CreateMailboxSession(arbitrationProxy, messageCulture))
			{
				using (Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.Outbox))
				{
					using (MessageItem messageItem = MessageItem.Create(mailboxSession, folder.Id))
					{
						if (messageClass != null)
						{
							messageItem.ClassName = messageClass;
						}
						messageItem.Subject = subject.ToString(messageCulture);
						messageItem.Sender = new Participant(null, (string)arbitrationProxy, "SMTP");
						foreach (SmtpAddress address in recipients)
						{
							try
							{
								messageItem.Recipients.Add(new Participant(null, (string)address, "SMTP"), RecipientItemType.To);
							}
							catch (ObjectNotFoundException)
							{
							}
						}
						if (messageItem.Recipients.Count != 0)
						{
							using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextHtml))
							{
								textWriter.Write(body);
							}
							using (MessageItem messageItem2 = Item.BindAsMessage(mailboxSession, itemId, new PropertyDefinition[]
							{
								ItemSchema.ConversationIndex
							}))
							{
								messageItem.ConversationIndex = ConversationIndex.CreateFromParent(messageItem2.ConversationIndex).ToByteArray();
							}
							messageItem.IsDeliveryReceiptRequested = false;
							messageItem[MessageItemSchema.IsNonDeliveryReceiptRequested] = false;
							messageItem.Send();
						}
					}
				}
			}
		}

		private static void HandleCultureResultForRecipient(Dictionary<CultureInfo, List<RoutingAddress>> list, Result<ADRawEntry> result, RoutingAddress address, int totalRecipients, CultureInfo organizationFallbackCulture)
		{
			ADRawEntry data = result.Data;
			CultureInfo cultureInfo = organizationFallbackCulture;
			if (data == null)
			{
				ApprovalProcessor.diag.TraceDebug<RoutingAddress>(0L, "AD entry not found for '{0}', using default culture", address);
			}
			else
			{
				MultiValuedProperty<CultureInfo> multiValuedProperty = (MultiValuedProperty<CultureInfo>)data[ADOrgPersonSchema.Languages];
				if (!MultiValuedPropertyBase.IsNullOrEmpty(multiValuedProperty))
				{
					foreach (CultureInfo cultureInfo2 in multiValuedProperty)
					{
						if (ClientCultures.IsCultureSupportedForDsn(cultureInfo2))
						{
							cultureInfo = cultureInfo2;
							break;
						}
						ApprovalProcessor.diag.TraceDebug<CultureInfo>(0L, "'{0}' is not available on this server.", cultureInfo2);
					}
				}
			}
			if (!list.ContainsKey(cultureInfo))
			{
				list[cultureInfo] = new List<RoutingAddress>(totalRecipients);
			}
			list[cultureInfo].Add(address);
			ApprovalProcessor.diag.TraceDebug<CultureInfo, RoutingAddress>(0L, "using '{0}' for '{1}'.", cultureInfo, address);
		}

		public static string ResolveDisplayNameForDistributionGroupFromApprovalData(string approvalData, IRecipientSession session)
		{
			string groupDisplayName = string.Empty;
			if (string.IsNullOrEmpty(approvalData))
			{
				throw new ArgumentNullException("approvalData");
			}
			try
			{
				ADNotificationAdapter.TryRunADOperation(delegate()
				{
					ADObjectId entryId = new ADObjectId(new Guid(approvalData.Substring(approvalData.IndexOf(',') + 1)));
					MiniRecipient miniRecipient = session.ReadMiniRecipient(entryId, ApprovalProcessor.DisplayNameProperty);
					if (miniRecipient != null)
					{
						string text = (string)miniRecipient[ADRecipientSchema.DisplayName];
						if (!string.IsNullOrEmpty(text))
						{
							groupDisplayName = text;
						}
					}
				}, 1);
			}
			catch (FormatException)
			{
				ApprovalProcessor.diag.TraceError<string>(0L, " applicationData '{0}' cannot be parsed", approvalData);
			}
			catch (OverflowException)
			{
				ApprovalProcessor.diag.TraceError<string>(0L, " applicationData '{0}' cannot be parsed", approvalData);
			}
			return groupDisplayName;
		}

		public static string GetDisplayNameFromSmtpAddress(string address)
		{
			string address2 = address;
			if (!string.IsNullOrEmpty(address))
			{
				ADNotificationAdapter.TryRunADOperation(delegate()
				{
					IRecipientSession recipientSession = ApprovalProcessor.CreateRecipientSessionFromSmtpAddress(address);
					ADRawEntry adrawEntry = recipientSession.FindByProxyAddress(new SmtpProxyAddress(address, true), ApprovalProcessor.DisplayNameProperty);
					if (adrawEntry != null)
					{
						string text = (string)adrawEntry[ADRecipientSchema.DisplayName];
						if (!string.IsNullOrEmpty(text))
						{
							address = text;
						}
					}
				}, 1);
			}
			return address2;
		}

		public static string GenerateMessageBodyForRequestMessage(LocalizedString headerPart, LocalizedString bodyPart, LocalizedString footerPart, CultureInfo messageCulture)
		{
			StringBuilder stringBuilder = new StringBuilder("<style type=\"text/css\">\r\n                                                                body {\r\n\t                                                                font-family: Tahoma, Arial, Helvetica, sans-serif;\r\n\t                                                                font-size:x-small\r\n                                                                }\r\n\r\n                                                                H1 {\r\n\t                                                                FONT-WEIGHT: bold;\r\n\t                                                                FONT-SIZE: small;\r\n\t                                                                MARGIN: 0in;\r\n\t                                                                COLOR: #000066;\r\n\t                                                                FONT-FAMILY: Tahoma, Arial, Helvetica, sans-serif;\r\n                                                                }\r\n                                                                TABLE {\r\n\t                                                                VERTICAL-ALIGN: top;\r\n\t                                                                COLOR: #000000;\r\n\t                                                                border-width: 0px;\r\n                                                                }\r\n                                                                TR {\r\n                                                                }\r\n                                                                TD {\r\n\t                                                                FONT-SIZE: x-small;\r\n\t                                                                VERTICAL-ALIGN: top;\r\n\t                                                                COLOR: #000000;\r\n\t                                                                font-family: Tahoma, Arial, Helvetica, sans-serif;\r\n                                                                }\r\n                                                                .header {\r\n\t                                                                border-width: 0px;\r\n\t                                                                FONT-SIZE: x-small;\r\n\t                                                                COLOR: #808080;\r\n\t                                                                font-weight: bold;\r\n\t                                                                white-space: nowrap;\r\n\t                                                                width: 5%;\r\n\t                                                                padding-right: 6px;\r\n                                                                }\r\n                                                                A:link {\r\n\t                                                                COLOR: rgb(51,153,255)\r\n                                                                }\r\n                                                                P {\r\n\t                                                                MARGIN: 0in\r\n                                                                }\r\n                                                                .footer {\r\n\t                                                                BORDER-TOP: #ccc 1px solid; FONT-SIZE: smaller; COLOR: #808080; FONT-FAMILY: Arial\r\n                                                                }\r\n                                                                .line {\r\n\t                                                                font-size: 4px;\r\n\t                                                                border-top: 1px #ccc solid\r\n                                                                }\r\n                                                                </style><h1>");
			stringBuilder.Append(headerPart.ToString(messageCulture));
			stringBuilder.Append("</h1><br />");
			stringBuilder.Append(bodyPart.ToString(messageCulture));
			stringBuilder.Append("<br /><br />");
			if (footerPart != LocalizedString.Empty)
			{
				stringBuilder.Append("<p class=\"footer\">");
				stringBuilder.Append(footerPart.ToString(messageCulture));
				stringBuilder.Append("</p>");
			}
			return stringBuilder.ToString();
		}

		public static string GenerateMessageBodyForApprovalMessage(LocalizedString headerPart, LocalizedString bodyPart, LocalizedString footerPart, CultureInfo messageCulture)
		{
			StringBuilder stringBuilder = new StringBuilder("<style type=\"text/css\">\r\n                                                                body {\r\n\t                                                                font-family: Tahoma, Arial, Helvetica, sans-serif;\r\n\t                                                                font-size:x-small\r\n                                                                }\r\n\r\n                                                                H1 {\r\n\t                                                                FONT-WEIGHT: bold;\r\n\t                                                                FONT-SIZE: small;\r\n\t                                                                MARGIN: 0in;\r\n\t                                                                COLOR: #000066;\r\n\t                                                                FONT-FAMILY: Tahoma, Arial, Helvetica, sans-serif;\r\n                                                                }\r\n                                                                TABLE {\r\n\t                                                                VERTICAL-ALIGN: top;\r\n\t                                                                COLOR: #000000;\r\n\t                                                                border-width: 0px;\r\n                                                                }\r\n                                                                TR {\r\n                                                                }\r\n                                                                TD {\r\n\t                                                                FONT-SIZE: x-small;\r\n\t                                                                VERTICAL-ALIGN: top;\r\n\t                                                                COLOR: #000000;\r\n\t                                                                font-family: Tahoma, Arial, Helvetica, sans-serif;\r\n                                                                }\r\n                                                                .header {\r\n\t                                                                border-width: 0px;\r\n\t                                                                FONT-SIZE: x-small;\r\n\t                                                                COLOR: #808080;\r\n\t                                                                font-weight: bold;\r\n\t                                                                white-space: nowrap;\r\n\t                                                                width: 5%;\r\n\t                                                                padding-right: 6px;\r\n                                                                }\r\n                                                                A:link {\r\n\t                                                                COLOR: rgb(51,153,255)\r\n                                                                }\r\n                                                                P {\r\n\t                                                                MARGIN: 0in\r\n                                                                }\r\n                                                                .footer {\r\n\t                                                                BORDER-TOP: #ccc 1px solid; FONT-SIZE: smaller; COLOR: #808080; FONT-FAMILY: Arial\r\n                                                                }\r\n                                                                .line {\r\n\t                                                                font-size: 4px;\r\n\t                                                                border-top: 1px #ccc solid\r\n                                                                }\r\n                                                                </style><h1>");
			stringBuilder.Append(headerPart.ToString(messageCulture));
			stringBuilder.Append("</h1><br />");
			stringBuilder.Append(bodyPart.ToString(messageCulture));
			stringBuilder.Append("<br /><br />");
			if (footerPart != LocalizedString.Empty)
			{
				stringBuilder.Append("<p class=\"footer\">");
				stringBuilder.Append(footerPart.ToString(messageCulture));
				stringBuilder.Append("</p>");
			}
			return stringBuilder.ToString();
		}

		public static string GenerateMessageBodyForRejectMessage(LocalizedString headerPart, LocalizedString bodyPart, LocalizedString footerPart, CultureInfo messageCulture)
		{
			StringBuilder stringBuilder = new StringBuilder("<style type=\"text/css\">\r\n                                                                body {\r\n\t                                                                font-family: Tahoma, Arial, Helvetica, sans-serif;\r\n\t                                                                font-size:x-small\r\n                                                                }\r\n\r\n                                                                H1 {\r\n\t                                                                FONT-WEIGHT: bold;\r\n\t                                                                FONT-SIZE: small;\r\n\t                                                                MARGIN: 0in;\r\n\t                                                                COLOR: #000066;\r\n\t                                                                FONT-FAMILY: Tahoma, Arial, Helvetica, sans-serif;\r\n                                                                }\r\n                                                                TABLE {\r\n\t                                                                VERTICAL-ALIGN: top;\r\n\t                                                                COLOR: #000000;\r\n\t                                                                border-width: 0px;\r\n                                                                }\r\n                                                                TR {\r\n                                                                }\r\n                                                                TD {\r\n\t                                                                FONT-SIZE: x-small;\r\n\t                                                                VERTICAL-ALIGN: top;\r\n\t                                                                COLOR: #000000;\r\n\t                                                                font-family: Tahoma, Arial, Helvetica, sans-serif;\r\n                                                                }\r\n                                                                .header {\r\n\t                                                                border-width: 0px;\r\n\t                                                                FONT-SIZE: x-small;\r\n\t                                                                COLOR: #808080;\r\n\t                                                                font-weight: bold;\r\n\t                                                                white-space: nowrap;\r\n\t                                                                width: 5%;\r\n\t                                                                padding-right: 6px;\r\n                                                                }\r\n                                                                A:link {\r\n\t                                                                COLOR: rgb(51,153,255)\r\n                                                                }\r\n                                                                P {\r\n\t                                                                MARGIN: 0in\r\n                                                                }\r\n                                                                .footer {\r\n\t                                                                BORDER-TOP: #ccc 1px solid; FONT-SIZE: smaller; COLOR: #808080; FONT-FAMILY: Arial\r\n                                                                }\r\n                                                                .line {\r\n\t                                                                font-size: 4px;\r\n\t                                                                border-top: 1px #ccc solid\r\n                                                                }\r\n                                                                </style><h1>");
			stringBuilder.Append(headerPart.ToString(messageCulture));
			stringBuilder.Append("</h1><table width=\"100%\" border=\"0\">\r\n\t\t<tr>\r\n\t\t<td width=\"2%\">&nbsp;</td>\r\n\t\t<td class=\"header\">");
			stringBuilder.Append(bodyPart.ToString(messageCulture));
			stringBuilder.Append("</td>\r\n                                   <td width=\"72%\">&nbsp;</td>\r\n\t                                    </tr>\r\n                                    </table>\r\n                                    <br />\r\n                                    <br />\r\n                                    <br />");
			if (footerPart != LocalizedString.Empty)
			{
				stringBuilder.Append("<p class=\"footer\">");
				stringBuilder.Append(footerPart.ToString(messageCulture));
				stringBuilder.Append("</p>");
			}
			return stringBuilder.ToString();
		}

		public static string GenerateMessageBodyForErrorMessage(LocalizedString headerPart, LocalizedString bodyPart, LocalizedString footerPart, CultureInfo messageCulture)
		{
			StringBuilder stringBuilder = new StringBuilder("<style type=\"text/css\">\r\n                                                                body {\r\n\t                                                                font-family: Tahoma, Arial, Helvetica, sans-serif;\r\n\t                                                                font-size:x-small\r\n                                                                }\r\n\r\n                                                                H1 {\r\n\t                                                                FONT-WEIGHT: bold;\r\n\t                                                                FONT-SIZE: small;\r\n\t                                                                MARGIN: 0in;\r\n\t                                                                COLOR: #000066;\r\n\t                                                                FONT-FAMILY: Tahoma, Arial, Helvetica, sans-serif;\r\n                                                                }\r\n                                                                TABLE {\r\n\t                                                                VERTICAL-ALIGN: top;\r\n\t                                                                COLOR: #000000;\r\n\t                                                                border-width: 0px;\r\n                                                                }\r\n                                                                TR {\r\n                                                                }\r\n                                                                TD {\r\n\t                                                                FONT-SIZE: x-small;\r\n\t                                                                VERTICAL-ALIGN: top;\r\n\t                                                                COLOR: #000000;\r\n\t                                                                font-family: Tahoma, Arial, Helvetica, sans-serif;\r\n                                                                }\r\n                                                                .header {\r\n\t                                                                border-width: 0px;\r\n\t                                                                FONT-SIZE: x-small;\r\n\t                                                                COLOR: #808080;\r\n\t                                                                font-weight: bold;\r\n\t                                                                white-space: nowrap;\r\n\t                                                                width: 5%;\r\n\t                                                                padding-right: 6px;\r\n                                                                }\r\n                                                                A:link {\r\n\t                                                                COLOR: rgb(51,153,255)\r\n                                                                }\r\n                                                                P {\r\n\t                                                                MARGIN: 0in\r\n                                                                }\r\n                                                                .footer {\r\n\t                                                                BORDER-TOP: #ccc 1px solid; FONT-SIZE: smaller; COLOR: #808080; FONT-FAMILY: Arial\r\n                                                                }\r\n                                                                .line {\r\n\t                                                                font-size: 4px;\r\n\t                                                                border-top: 1px #ccc solid\r\n                                                                }\r\n                                                                </style><h1>");
			stringBuilder.Append(headerPart.ToString(messageCulture));
			stringBuilder.Append("</h1><br /><font color=\"#CC3300\">");
			stringBuilder.Append("</font><br /><br /><br /><br />");
			stringBuilder.Append(bodyPart.ToString(messageCulture));
			if (footerPart != LocalizedString.Empty)
			{
				stringBuilder.Append("<p class=\"footer\">");
				stringBuilder.Append(footerPart.ToString(messageCulture));
				stringBuilder.Append("</p>");
			}
			return stringBuilder.ToString();
		}

		public static string SubmitRequest(int applicationId, SmtpAddress arbitrationMbx, SmtpAddress requester, SmtpAddress[] decisionMakers, CultureInfo moderatorCommonCulture, Guid policyTag, int? retentionPeriod, LocalizedString subject)
		{
			return ApprovalProcessor.SubmitRequest(applicationId, arbitrationMbx, requester, decisionMakers, moderatorCommonCulture, policyTag, retentionPeriod, subject, null);
		}

		public static string SubmitRequest(int applicationId, SmtpAddress arbitrationMbx, SmtpAddress requester, SmtpAddress[] decisionMakers, CultureInfo moderatorCommonCulture, Guid policyTag, int? retentionPeriod, LocalizedString subject, string applicationData)
		{
			if (!arbitrationMbx.IsValidAddress)
			{
				throw new ArgumentException("Invalid arbitration mailbox SmtpAddress " + arbitrationMbx, "arbitrationMbx");
			}
			if (!requester.IsValidAddress)
			{
				throw new ArgumentException("Invalid requester SmtpAddress " + requester, "requester");
			}
			foreach (SmtpAddress smtpAddress in decisionMakers)
			{
				if (!smtpAddress.IsValidAddress)
				{
					throw new ArgumentException("Invalid decisionMaker SmtpAddress " + smtpAddress, "decisionMaker");
				}
			}
			string result = string.Empty;
			using (MailboxSession mailboxSession = ApprovalProcessor.CreateMailboxSession(arbitrationMbx, moderatorCommonCulture))
			{
				using (Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.Outbox))
				{
					using (MessageItem messageItem = MessageItem.Create(mailboxSession, folder.Id))
					{
						Participant participant = new Participant(null, (string)arbitrationMbx, "SMTP");
						messageItem.Subject = subject.ToString(moderatorCommonCulture);
						messageItem.Sender = participant;
						messageItem.Recipients.Add(participant, RecipientItemType.To);
						messageItem.ClassName = "IPM.Microsoft.Approval.Initiation";
						messageItem.ConversationIndex = ConversationIndex.CreateNew().ToByteArray();
						messageItem[MessageItemSchema.ApprovalApplicationId] = applicationId;
						messageItem[MessageItemSchema.ApprovalRequestor] = (string)requester;
						string[] value = Array.ConvertAll<SmtpAddress, string>(decisionMakers, (SmtpAddress smtp) => (string)smtp);
						messageItem[MessageItemSchema.ApprovalAllowedDecisionMakers] = string.Join(";", value);
						if (!Guid.Empty.Equals(policyTag))
						{
							messageItem[StoreObjectSchema.PolicyTag] = policyTag.ToByteArray();
							if (retentionPeriod != null)
							{
								messageItem[StoreObjectSchema.RetentionPeriod] = retentionPeriod.Value;
							}
						}
						if (applicationData != null)
						{
							messageItem[MessageItemSchema.ApprovalApplicationData] = applicationData;
						}
						ConflictResolutionResult conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
						if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
						{
							throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(messageItem.InternalObjectId), conflictResolutionResult);
						}
						messageItem.Load(new PropertyDefinition[]
						{
							ItemSchema.InternetMessageId
						});
						result = messageItem.InternetMessageId;
						messageItem.Send();
					}
				}
			}
			return result;
		}

		public static void SendApproveNotification(SmtpAddress arbitrationProxy, StoreObjectId itemId, SmtpAddress[] recipients, LocalizedString subject, LocalizedString headertextstring, LocalizedString bodytextstring, LocalizedString footertextstring, CultureInfo messageCulture)
		{
			ApprovalProcessor.SendNotification(arbitrationProxy, itemId, recipients, subject, ApprovalProcessor.GenerateMessageBodyForApprovalMessage(headertextstring, bodytextstring, footertextstring, messageCulture), "IPM.Note.Microsoft.Approval.Reply.Approve", messageCulture);
		}

		public static void SendRejectNotification(SmtpAddress arbitrationProxy, StoreObjectId itemId, SmtpAddress[] recipients, LocalizedString subject, LocalizedString headertextstring, LocalizedString bodytextstring, LocalizedString footertextstring, CultureInfo messageCulture)
		{
			ApprovalProcessor.SendNotification(arbitrationProxy, itemId, recipients, subject, ApprovalProcessor.GenerateMessageBodyForRejectMessage(headertextstring, bodytextstring, footertextstring, messageCulture), "IPM.Note.Microsoft.Approval.Reply.Reject", messageCulture);
		}

		public static void SendNotification(SmtpAddress arbitrationProxy, StoreObjectId itemId, SmtpAddress[] recipients, LocalizedString subject, LocalizedString headertextstring, LocalizedString bodytextstring, LocalizedString footertextstring, CultureInfo messageCulture)
		{
			ApprovalProcessor.SendNotification(arbitrationProxy, itemId, recipients, subject, ApprovalProcessor.GenerateMessageBodyForErrorMessage(headertextstring, bodytextstring, footertextstring, messageCulture), null, messageCulture);
		}

		public static void CopyStream(Stream source, Stream destination, byte[] buffer)
		{
			int count = buffer.Length;
			int num;
			do
			{
				num = source.Read(buffer, 0, count);
				destination.Write(buffer, 0, num);
			}
			while (num > 0);
		}

		public static DecisionConflict ApproveRequest(SmtpAddress arbitrationProxy, StoreObjectId itemId, SmtpAddress decisionMaker, Body comments)
		{
			DecisionConflict result;
			using (MailboxSession mailboxSession = ApprovalProcessor.CreateMailboxSession(arbitrationProxy, null))
			{
				string text;
				ApprovalStatus? approvalStatus;
				ExDateTime? exDateTime;
				result = ApprovalProcessor.MarkDecisionAndStatus(itemId, mailboxSession, ApprovalStatus.Approved, decisionMaker, comments, out text, out approvalStatus, out exDateTime);
			}
			return result;
		}

		public static DecisionConflict ApproveRequest(MailboxSession session, StoreObjectId itemId, SmtpAddress decisionMaker, Body comments, out string existingDecisionMakerAddress, out ApprovalStatus? existingApprovalStatus, out ExDateTime? existingDecisionTime)
		{
			return ApprovalProcessor.MarkDecisionAndStatus(itemId, session, ApprovalStatus.Approved, decisionMaker, comments, out existingDecisionMakerAddress, out existingApprovalStatus, out existingDecisionTime);
		}

		public static DecisionConflict RejectRequest(SmtpAddress arbitrationProxy, StoreObjectId itemId, SmtpAddress decisionMaker, Body comments)
		{
			DecisionConflict result;
			using (MailboxSession mailboxSession = ApprovalProcessor.CreateMailboxSession(arbitrationProxy, null))
			{
				string text;
				ApprovalStatus? approvalStatus;
				ExDateTime? exDateTime;
				result = ApprovalProcessor.MarkDecisionAndStatus(itemId, mailboxSession, ApprovalStatus.Rejected, decisionMaker, comments, out text, out approvalStatus, out exDateTime);
			}
			return result;
		}

		public static DecisionConflict RejectRequest(MailboxSession session, StoreObjectId itemId, SmtpAddress decisionMaker, Body comments, out string existingDecisionMakerAddress, out ApprovalStatus? existingApprovalStatus, out ExDateTime? existingDecisionTime)
		{
			return ApprovalProcessor.MarkDecisionAndStatus(itemId, session, ApprovalStatus.Rejected, decisionMaker, comments, out existingDecisionMakerAddress, out existingApprovalStatus, out existingDecisionTime);
		}

		public static StoreObjectId[] QueryRequests(SmtpAddress arbitrationProxy, QueryFilter filter, SortBy[] sortColumns)
		{
			return ApprovalProcessor.QueryRequests(arbitrationProxy, SmtpAddress.Empty, SmtpAddress.Empty, SmtpAddress.Empty, null, filter, sortColumns);
		}

		public static StoreObjectId[] QueryRequests(SmtpAddress arbitrationProxy, SmtpAddress allowedDecisionMaker, SmtpAddress decisionMaker, SmtpAddress requester, ApprovalStatus? approvalStatus, SortBy[] sortColumns)
		{
			return ApprovalProcessor.QueryRequests(arbitrationProxy, allowedDecisionMaker, decisionMaker, requester, approvalStatus, null, sortColumns);
		}

		public static StoreObjectId[] QueryRequests(SmtpAddress arbitrationProxy, SmtpAddress allowedDecisionMaker, SmtpAddress decisionMaker, SmtpAddress requester, ApprovalStatus? approvalStatus, QueryFilter additionalFilter, SortBy[] sortColumns)
		{
			if (!arbitrationProxy.IsValidAddress)
			{
				throw new ArgumentException("Invalid arbitration mailbox SmtpAddress " + arbitrationProxy, "arbitrationProxy");
			}
			List<StoreObjectId> list = new List<StoreObjectId>();
			List<QueryFilter> list2 = new List<QueryFilter>(5);
			if (allowedDecisionMaker != SmtpAddress.Empty)
			{
				if (!allowedDecisionMaker.IsValidAddress)
				{
					throw new ArgumentException("Invalid allowed decision maker SmtpAddress " + allowedDecisionMaker, "allowedDecisionMaker");
				}
				list2.Add(new TextFilter(MessageItemSchema.ApprovalAllowedDecisionMakers, (string)allowedDecisionMaker, MatchOptions.SubString, MatchFlags.IgnoreCase));
			}
			if (decisionMaker != SmtpAddress.Empty)
			{
				if (!decisionMaker.IsValidAddress)
				{
					throw new ArgumentException("Invalid decision maker SmtpAddress " + decisionMaker, "decisionMaker");
				}
				list2.Add(new TextFilter(MessageItemSchema.ApprovalDecisionMaker, (string)decisionMaker, MatchOptions.FullString, MatchFlags.IgnoreCase));
			}
			if (requester != SmtpAddress.Empty)
			{
				if (!requester.IsValidAddress)
				{
					throw new ArgumentException("Invalid requester SmtpAddress " + requester, "requester");
				}
				list2.Add(new TextFilter(MessageItemSchema.ApprovalRequestor, (string)requester, MatchOptions.FullString, MatchFlags.IgnoreCase));
			}
			if (approvalStatus != null)
			{
				list2.Add(new BitMaskFilter(MessageItemSchema.ApprovalStatus, (ulong)((long)approvalStatus.Value), true));
			}
			if (additionalFilter != null)
			{
				list2.Add(additionalFilter);
			}
			QueryFilter queryFilter;
			if (list2.Count == 0)
			{
				queryFilter = null;
			}
			else if (list2.Count == 1)
			{
				queryFilter = list2[0];
			}
			else
			{
				queryFilter = new AndFilter(list2.ToArray());
			}
			ApprovalProcessor.diag.TraceInformation<SmtpAddress, string>(0, 0L, "Using the following filter to query in Inbox folder of arbitration mailbox {0}: {1}", arbitrationProxy, (queryFilter == null) ? "null" : queryFilter.ToString());
			using (MailboxSession mailboxSession = ApprovalProcessor.CreateMailboxSession(arbitrationProxy, null))
			{
				using (Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.Inbox))
				{
					using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, queryFilter, sortColumns, new PropertyDefinition[]
					{
						ItemSchema.Id
					}))
					{
						object[][] rows = queryResult.GetRows(10000);
						while (rows != null && rows.Length > 0)
						{
							foreach (object[] array2 in rows)
							{
								list.Add(((VersionedId)array2[0]).ObjectId);
							}
							rows = queryResult.GetRows(10000);
						}
					}
				}
			}
			return list.ToArray();
		}

		public static bool TryGetCulturesForDecisionMakers(IList<RoutingAddress> decisionMakers, IRecipientSession adRecipientSession, CultureInfo organizationFallbackCulture, out Dictionary<CultureInfo, List<RoutingAddress>> decisionMakerCultures)
		{
			decisionMakerCultures = null;
			Dictionary<CultureInfo, List<RoutingAddress>> list = new Dictionary<CultureInfo, List<RoutingAddress>>();
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ProxyAddress[] array = new SmtpProxyAddress[decisionMakers.Count];
				for (int i = 0; i < decisionMakers.Count; i++)
				{
					array[i] = new SmtpProxyAddress((string)decisionMakers[i], true);
				}
				Result<ADRawEntry>[] array2 = adRecipientSession.FindByProxyAddresses(array, ApprovalProcessor.CultureProperties);
				for (int j = 0; j < array2.Length; j++)
				{
					ApprovalProcessor.HandleCultureResultForRecipient(list, array2[j], decisionMakers[j], decisionMakers.Count, organizationFallbackCulture);
				}
			}, 1);
			if (!adoperationResult.Succeeded)
			{
				ApprovalProcessor.diag.TraceDebug<ADOperationErrorCode, Exception>(0L, "Fail to get cultures for decision makers. error code '{0}', exception '{1}'", adoperationResult.ErrorCode, adoperationResult.Exception);
			}
			else
			{
				decisionMakerCultures = list;
			}
			return adoperationResult.Succeeded;
		}

		internal static IRecipientSession CreateRecipientSessionFromSmtpAddress(string address)
		{
			return ApprovalProcessor.CreateRecipientSessionFromSmtpAddress(new SmtpAddress(address));
		}

		internal static IRecipientSession CreateRecipientSessionFromSmtpAddress(SmtpAddress smtpAddress)
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(smtpAddress.Domain), 1629, "CreateRecipientSessionFromSmtpAddress", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\approval\\ApprovalProcessor.cs");
		}

		public const string DecisionCommentsFileName = "DecisionComments.txt";

		private const int RetriesForSaveConflict = 3;

		private const string approvalClientInfoString = "Client=ApprovalAPI";

		private const int CommentCopyingBufferSize = 2048;

		private static readonly ADPropertyDefinition[] DisplayNameProperty = new ADPropertyDefinition[]
		{
			ADRecipientSchema.DisplayName
		};

		internal static readonly PropertyDefinition[] ApprovalProperties = new PropertyDefinition[]
		{
			MessageItemSchema.ApprovalAllowedDecisionMakers,
			MessageItemSchema.ApprovalRequestor,
			MessageItemSchema.ApprovalDecisionMaker,
			MessageItemSchema.ApprovalDecisionTime,
			MessageItemSchema.ApprovalDecision,
			MessageItemSchema.ApprovalStatus,
			MessageItemSchema.ApprovalDecisionMakersNdred,
			MessageItemSchema.ApprovalApplicationId,
			MessageItemSchema.ApprovalApplicationData
		};

		private static readonly ADPropertyDefinition[] CultureProperties = new ADPropertyDefinition[]
		{
			ADOrgPersonSchema.Languages
		};

		private static readonly Trace diag = ExTraceGlobals.GeneralTracer;
	}
}
