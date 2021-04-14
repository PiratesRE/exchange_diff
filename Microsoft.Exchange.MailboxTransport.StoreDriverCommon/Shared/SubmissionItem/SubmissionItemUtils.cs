using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.MailboxTransport.Shared.Providers;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Shared.SubmissionItem
{
	internal class SubmissionItemUtils
	{
		internal static void CopySenderTo(SubmissionItemBase submissionItem, TransportMailItem message)
		{
			string messageClass = submissionItem.MessageClass;
			if (messageClass.StartsWith("IPM.Note.Rules.OofTemplate.", StringComparison.OrdinalIgnoreCase) || messageClass.StartsWith("IPM.Note.Rules.ExternalOofTemplate.", StringComparison.OrdinalIgnoreCase) || messageClass.StartsWith("IPM.Recall.Report.", StringComparison.OrdinalIgnoreCase) || messageClass.StartsWith("IPM.Conflict.Message", StringComparison.OrdinalIgnoreCase) || messageClass.StartsWith("IPM.Conflict.Folder", StringComparison.OrdinalIgnoreCase))
			{
				TraceHelper.StoreDriverTracer.TracePass<string>(TraceHelper.MessageProbeActivityId, 0L, "Message class is {0}, setting <> as the P1 reverse path", submissionItem.MessageClass);
				message.From = RoutingAddress.NullReversePath;
				return;
			}
			RoutingAddress from;
			if (SubmissionItemUtils.TryGetRoutingAddressFromParticipant(message.ADRecipientCache, submissionItem.Sender, "Sender", out from))
			{
				message.From = from;
				return;
			}
			throw new InvalidSenderException(submissionItem.Sender);
		}

		internal static bool PatchQuarantineSender(TransportMailItem mailItem, string quarantineSender)
		{
			if (string.IsNullOrEmpty(quarantineSender) || Components.DsnGenerator.QuarantineConfig == null || string.IsNullOrEmpty(ConfigurationProvider.GetQuarantineMailbox()))
			{
				return true;
			}
			quarantineSender = quarantineSender.Trim(new char[]
			{
				'<',
				'>'
			});
			RoutingAddress routingAddress = new RoutingAddress(quarantineSender);
			if (!routingAddress.IsValid || !QuarantineMailboxConfig.Instance.IsUserQuarantineMailbox(mailItem.From.ToString(), Components.DsnGenerator.QuarantineConfig.Mailbox, mailItem.OrganizationId))
			{
				TraceHelper.StoreDriverTracer.TraceFail<RoutingAddress>(TraceHelper.MessageProbeActivityId, 0L, "Invalid quarantine sender {0} for released quarantine-DSN. Sender unpatched", routingAddress);
				return false;
			}
			mailItem.From = routingAddress;
			mailItem.Message.From = new EmailRecipient(null, quarantineSender);
			return false;
		}

		internal static int CopyRecipientsTo(SubmissionItemBase submissionItem, TransportMailItem mailItem, SubmissionRecipientHandler recipientHandler, ref List<string> unresolvableRecipients, ref List<string> notResponsibleRecipientsList)
		{
			List<string> list;
			return SubmissionItemUtils.CopyRecipientsTo(submissionItem, mailItem, recipientHandler, ref unresolvableRecipients, ref notResponsibleRecipientsList, false, out list);
		}

		internal static int CopyRecipientsTo(SubmissionItemBase submissionItem, TransportMailItem mailItem, SubmissionRecipientHandler recipientHandler, ref List<string> unresolvableRecipients, ref List<string> notResponsibleRecipientsList, bool useParticipantSmtpEmailAddressIfNecessary, out List<string> participantSmtpEmailAddressBeneficiaries)
		{
			participantSmtpEmailAddressBeneficiaries = new List<string>();
			TraceHelper.StoreDriverTracer.TracePass<long>(TraceHelper.MessageProbeActivityId, 0L, "SubmissionItemUtils.CopyRecipientsTo: Copy recipients to mailitem {0}", mailItem.RecordId);
			bool resubmittedMessage = submissionItem.ResubmittedMessage;
			mailItem.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.ResentMapiMessage", resubmittedMessage);
			List<MimeRecipient> list = new List<MimeRecipient>();
			List<string> list2 = null;
			List<string> list3 = null;
			int num = 0;
			TraceHelper.StoreDriverTracer.TracePass<string>(TraceHelper.MessageProbeActivityId, 0L, "Copying recipients for {0} message", resubmittedMessage ? "resubmitted" : "regular");
			foreach (Recipient recipient in submissionItem.Recipients)
			{
				string emailAddress = recipient.Participant.EmailAddress;
				TraceHelper.StoreDriverTracer.TracePass<string>(TraceHelper.MessageProbeActivityId, 0L, "Processing recipient: {0}", emailAddress);
				int? num2 = null;
				RoutingAddress arg;
				bool flag;
				if (!SubmissionItemUtils.GetP2RecipientType(resubmittedMessage, recipient, out num2))
				{
					TraceHelper.StoreDriverTracer.TracePass<int>(TraceHelper.MessageProbeActivityId, 0L, "SubmissionItemUtils.CopyRecipientsTo: Saving recipient type {0} of P2 recipient on resubmitted message", num2.Value);
					SubmissionItemUtils.SaveP2Recipient(mailItem, emailAddress, num2.Value, ref list2, ref list3);
				}
				else if (!SubmissionItemBase.GetValueTypePropValue<bool>(recipient, ItemSchema.Responsibility).GetValueOrDefault())
				{
					TraceHelper.StoreDriverTracer.TracePass<Participant>(TraceHelper.MessageProbeActivityId, 0L, "SubmissionItemUtils.CopyRecipientsTo: Skip recipient {0} since PR_RESPONSIBILITY is not true", recipient.Participant);
					if (notResponsibleRecipientsList == null)
					{
						notResponsibleRecipientsList = new List<string>(1);
					}
					string item = SubmissionItemUtils.BuildParticipantString(recipient.Participant);
					notResponsibleRecipientsList.Add(item);
				}
				else if (!SubmissionItemUtils.TryGetRoutingAddressFromParticipant(mailItem.ADRecipientCache, recipient.Participant, "Recipient", out arg, out flag, useParticipantSmtpEmailAddressIfNecessary))
				{
					if (recipientHandler != null)
					{
						recipientHandler(num2, recipient, mailItem, null);
					}
					if (unresolvableRecipients == null)
					{
						unresolvableRecipients = new List<string>(1);
					}
					string text = SubmissionItemUtils.BuildParticipantString(recipient.Participant);
					unresolvableRecipients.Add(text);
					TraceHelper.StoreDriverTracer.TraceFail<string>(TraceHelper.MessageProbeActivityId, 0L, "SubmissionItemUtils.CopyRecipientsTo: Invalid recipient {0}", text);
				}
				else
				{
					TraceHelper.StoreDriverTracer.TracePass<RoutingAddress, int?>(TraceHelper.MessageProbeActivityId, 0L, "Added recipient: {0}, Type: {1}", arg, num2);
					string text2 = arg.ToString();
					if (flag)
					{
						participantSmtpEmailAddressBeneficiaries.Add(text2);
					}
					MailRecipient mailRecipient = mailItem.Recipients.Add(text2);
					num++;
					if (recipientHandler != null)
					{
						recipientHandler(num2, recipient, mailItem, mailRecipient);
					}
					string refTypePropValue = SubmissionItemBase.GetRefTypePropValue<string>(recipient, StoreObjectSchema.DisplayName);
					SubmissionItemUtils.CopyRecipientPropsFromXSOToTransport(submissionItem, recipient, mailRecipient, num2, refTypePropValue);
					if (num2 != null && num2 == 3)
					{
						MimeRecipient item2 = new MimeRecipient(refTypePropValue, text2);
						list.Add(item2);
					}
				}
			}
			SubmissionItemUtils.AddExchangeOrganizationBccHeader(list, mailItem);
			if (list2 != null)
			{
				mailItem.ExtendedProperties.SetValue<List<string>>("Microsoft.Exchange.Transport.ResentMapiP2ToRecipients", list2);
			}
			if (list3 != null)
			{
				mailItem.ExtendedProperties.SetValue<List<string>>("Microsoft.Exchange.Transport.ResentMapiP2CcRecipients", list3);
			}
			return num;
		}

		internal static string BuildParticipantString(Participant participant)
		{
			if (participant == null)
			{
				return string.Empty;
			}
			string text = participant.RoutingType ?? string.Empty;
			string text2 = participant.EmailAddress ?? string.Empty;
			string text3 = participant.DisplayName ?? string.Empty;
			StringBuilder stringBuilder = new StringBuilder(text.Length + text2.Length + text3.Length + 6);
			stringBuilder.Append('"');
			stringBuilder.Append(text3);
			stringBuilder.Append("\" (");
			stringBuilder.Append(text);
			stringBuilder.Append(':');
			stringBuilder.Append(text2);
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		internal static void CopyRecipientPropsFromXSOToTransport(SubmissionItemBase submissionItem, Recipient recipient, MailRecipient transportRecipient, int? recipientType, string displayName)
		{
			if (!string.IsNullOrEmpty(displayName))
			{
				transportRecipient.ExtendedProperties.SetValue<string>("Microsoft.Exchange.MapiDisplayName", displayName);
			}
			if (recipientType != null)
			{
				transportRecipient.ExtendedProperties.SetValue<int>("Microsoft.Exchange.Transport.RecipientP2Type", recipientType.Value);
			}
			int? valueTypePropValue = SubmissionItemBase.GetValueTypePropValue<int>(recipient, ItemSchema.SendInternetEncoding);
			if (valueTypePropValue != null)
			{
				transportRecipient.ExtendedProperties.SetValue<int>("Microsoft.Exchange.Transport.ClientRequestedInternetEncoding", valueTypePropValue.Value);
			}
			bool? valueTypePropValue2 = SubmissionItemBase.GetValueTypePropValue<bool>(recipient, ItemSchema.SendRichInfo);
			if (valueTypePropValue2 != null)
			{
				transportRecipient.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.ClientRequestedSendRichInfo", valueTypePropValue2.Value);
			}
			bool? valueTypePropValue3 = SubmissionItemBase.GetValueTypePropValue<bool>(recipient, MessageItemSchema.IsDeliveryReceiptRequested);
			if (valueTypePropValue3 == null)
			{
				valueTypePropValue3 = submissionItem.GetValueTypePropValue<bool>(MessageItemSchema.IsDeliveryReceiptRequested);
			}
			if (valueTypePropValue3.GetValueOrDefault())
			{
				transportRecipient.DsnRequested = (DsnRequestedFlags.Success | DsnRequestedFlags.Failure | DsnRequestedFlags.Delay);
			}
			bool? valueTypePropValue4 = SubmissionItemBase.GetValueTypePropValue<bool>(recipient, MessageItemSchema.IsNonDeliveryReceiptRequested);
			if (valueTypePropValue4 == null)
			{
				valueTypePropValue4 = submissionItem.GetValueTypePropValue<bool>(MessageItemSchema.IsNonDeliveryReceiptRequested);
			}
			if (valueTypePropValue4.GetValueOrDefault())
			{
				transportRecipient.DsnRequested |= DsnRequestedFlags.Failure;
			}
			bool? flag = new bool?(false);
			if ((valueTypePropValue3 == flag && valueTypePropValue4 == flag) || string.Equals(submissionItem.MessageClass, "ipm.replication", StringComparison.OrdinalIgnoreCase))
			{
				transportRecipient.DsnRequested = DsnRequestedFlags.Never;
			}
		}

		internal static bool IsValidP2RecipientType(int? recipientType)
		{
			if (recipientType == null)
			{
				TraceHelper.StoreDriverTracer.TraceFail(TraceHelper.MessageProbeActivityId, 0L, "Property PR_RECIPIENT_TYPE doesn't exist");
				return false;
			}
			bool flag = 0 <= recipientType.Value && recipientType.Value <= 3;
			if (!flag)
			{
				TraceHelper.StoreDriverTracer.TraceFail<int>(TraceHelper.MessageProbeActivityId, 0L, "Unexpected value {0} for property PR_RECIPIENT_TYPE", recipientType.Value);
			}
			return flag;
		}

		internal static bool TryGetRoutingAddress(IADRecipientCache recipientCache, string address, string type, string context, out RoutingAddress result)
		{
			bool flag;
			return SubmissionItemUtils.TryGetRoutingAddress(recipientCache, address, type, context, string.Empty, out result, out flag, false);
		}

		internal static bool TryGetRoutingAddress(IADRecipientCache recipientCache, string participantEmailAddress, string type, string context, string participantSmtpAddress, out RoutingAddress result, out bool usedParticipantSmtpEmailAddress, bool useParticipantSmtpEmailAddressIfNecessary = false)
		{
			result = RoutingAddress.Empty;
			usedParticipantSmtpEmailAddress = false;
			TraceHelper.StoreDriverTracer.TracePass<string, string>(TraceHelper.MessageProbeActivityId, 0L, "Try to get routing address for {0}:{1}.", type, participantEmailAddress);
			string text = SubmissionItemUtils.StripSingleQuotes(participantEmailAddress);
			if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(text))
			{
				TraceHelper.StoreDriverTracer.TracePass<string>(TraceHelper.MessageProbeActivityId, 0L, "Required {0} properties are empty", context);
				return false;
			}
			if (type.Equals("SMTP", StringComparison.OrdinalIgnoreCase))
			{
				result = new RoutingAddress(text);
				if (result.IsValid)
				{
					return true;
				}
				string text2;
				if (ConfigurationProvider.TryGetDefaultDomainName(recipientCache.OrganizationId, out text2) && !string.IsNullOrEmpty(text2))
				{
					result = new RoutingAddress(text, text2);
					if (result.IsValid)
					{
						return true;
					}
				}
				result = RoutingAddress.Empty;
				TraceHelper.StoreDriverTracer.TracePass<string, string>(TraceHelper.MessageProbeActivityId, 0L, "{0} Smtp Address is invalid {1}", context, text);
				return false;
			}
			else
			{
				if (SubmissionItemUtils.TryGetRoutingAddressFromAD(recipientCache, participantEmailAddress, type, out result))
				{
					return true;
				}
				if (type.Equals("X400", StringComparison.OrdinalIgnoreCase))
				{
					bool flag;
					if (!X400AddressParser.TryGetCanonical(participantEmailAddress, false, out text, out flag))
					{
						return false;
					}
					if (string.IsNullOrEmpty(text))
					{
						return false;
					}
				}
				if (useParticipantSmtpEmailAddressIfNecessary && !string.IsNullOrEmpty(participantSmtpAddress))
				{
					result = new RoutingAddress(participantSmtpAddress);
					if (result.IsValid)
					{
						usedParticipantSmtpEmailAddress = true;
						return true;
					}
				}
				result = RoutingAddress.Empty;
				SmtpProxyAddress smtpProxyAddress;
				if (SmtpProxyAddress.TryEncapsulate(type, text, ConfigurationProvider.GetDefaultDomainName(), out smtpProxyAddress))
				{
					result = (RoutingAddress)smtpProxyAddress.SmtpAddress;
					return true;
				}
				TraceHelper.StoreDriverTracer.TracePass<string, string>(TraceHelper.MessageProbeActivityId, 0L, "Couldn't encapsulate address {0}:{1}.", type, participantEmailAddress);
				return false;
			}
		}

		internal static bool GetP2RecipientType(bool resubmittedMessage, Recipient recipient, out int? recipientType)
		{
			recipientType = SubmissionItemBase.GetValueTypePropValue<int>(recipient, ItemSchema.RecipientType);
			if (resubmittedMessage && recipientType != null && recipientType.Value == 268435456)
			{
				recipientType = new int?(0);
				TraceHelper.StoreDriverTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "PR_RECIPIENT_TYPE is MAPI_P1. This is a P1 (resent) recipient");
				return true;
			}
			if (!SubmissionItemUtils.IsValidP2RecipientType(recipientType))
			{
				TraceHelper.StoreDriverTracer.TraceFail<int?>(TraceHelper.MessageProbeActivityId, 0L, "PR_RECIPIENT_TYPE for resubmitted message is not set or invalid: {0}. RecipientP2Type will be assumed to be \"Unknown\"", recipientType);
				recipientType = new int?(0);
			}
			TraceHelper.StoreDriverTracer.TracePass<string>(TraceHelper.MessageProbeActivityId, 0L, "This is recipient is {0}", resubmittedMessage ? "a P2 (non-deliverable) recipient on resubmitted message" : "an ordinary recipient on an ordinary (non-resubmitted) message");
			return !resubmittedMessage;
		}

		internal static bool TryGetRoutingAddressFromParticipant(IADRecipientCache recipientCache, Participant participant, string context, out RoutingAddress result)
		{
			bool flag;
			return SubmissionItemUtils.TryGetRoutingAddressFromParticipant(recipientCache, participant, context, out result, out flag, false);
		}

		internal static bool TryGetRoutingAddressFromParticipant(IADRecipientCache recipientCache, Participant participant, string context, out RoutingAddress result, out bool usedParticipantSmtpEmailAddress, bool useParticipantSmtpEmailAddressIfNecessary)
		{
			usedParticipantSmtpEmailAddress = false;
			if (participant == null)
			{
				TraceHelper.StoreDriverTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "Null participant");
				return false;
			}
			return SubmissionItemUtils.TryGetRoutingAddress(recipientCache, participant.EmailAddress, participant.RoutingType, context, participant.SmtpEmailAddress, out result, out usedParticipantSmtpEmailAddress, useParticipantSmtpEmailAddressIfNecessary);
		}

		internal static string StripSingleQuotes(string emailAddress)
		{
			if (emailAddress == null || emailAddress.Length < 3)
			{
				return emailAddress;
			}
			if (emailAddress[0] == '\'' && emailAddress[emailAddress.Length - 1] == '\'')
			{
				return emailAddress.Substring(1, emailAddress.Length - 2);
			}
			return emailAddress;
		}

		internal static bool TryGetRoutingAddressFromAD(IADRecipientCache recipientCache, string address, string type, out RoutingAddress result)
		{
			ProxyAddress proxyAddress = ProxyAddress.Parse(type, address);
			if (proxyAddress is InvalidProxyAddress)
			{
				TraceHelper.StoreDriverTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "Proxy address is invalid");
				return false;
			}
			Result<ADRawEntry> result2 = recipientCache.FindAndCacheRecipient(proxyAddress);
			if (result2.Data != null)
			{
				string primarySmtpAddress = SubmissionItemUtils.GetPrimarySmtpAddress(result2.Data);
				if (string.IsNullOrEmpty(primarySmtpAddress))
				{
					TraceHelper.StoreDriverTracer.TracePass<string, string>(TraceHelper.MessageProbeActivityId, 0L, "Primary SMTP address for \"{0}:{1}\" is invalid or missing", address, type);
					return false;
				}
				TraceHelper.StoreDriverTracer.TracePass<string>(TraceHelper.MessageProbeActivityId, 0L, "Use primary smtp address {0}", primarySmtpAddress);
				result = new RoutingAddress(primarySmtpAddress);
				return true;
			}
			else
			{
				if (result2.Error != null && result2.Error != ProviderError.NotFound)
				{
					TraceHelper.StoreDriverTracer.TracePass<ProviderError>(TraceHelper.MessageProbeActivityId, 0L, "Failed to look up due to error :{0}", result2.Error);
					return false;
				}
				TraceHelper.StoreDriverTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "The address doesn't exist in AD");
				return false;
			}
		}

		private static void AddExchangeOrganizationBccHeader(List<MimeRecipient> bccRecipientList, TransportMailItem mailItem)
		{
			AddressHeader addressHeader = new AddressHeader("X-MS-Exchange-Organization-BCC");
			foreach (MimeRecipient newChild in bccRecipientList)
			{
				addressHeader.AppendChild(newChild);
			}
			HeaderList headers = mailItem.Message.MimeDocument.RootPart.Headers;
			headers.AppendChild(addressHeader);
		}

		private static void SaveP2Recipient(TransportMailItem mailItem, string recipientAddress, int recipientType, ref List<string> mapiToRecipients, ref List<string> mapiCcRecipients)
		{
			List<string> list;
			if (recipientType == 1)
			{
				if (mapiToRecipients == null)
				{
					mapiToRecipients = new List<string>();
				}
				list = mapiToRecipients;
			}
			else
			{
				if (recipientType != 2)
				{
					TraceHelper.StoreDriverTracer.TraceFail<int>(TraceHelper.MessageProbeActivityId, 0L, "Error, recipient type of MAPI_P2 recipient was not To or Cc: {0}", recipientType);
					return;
				}
				if (mapiCcRecipients == null)
				{
					mapiCcRecipients = new List<string>();
				}
				list = mapiCcRecipients;
			}
			int num = list.BinarySearch(recipientAddress, StringComparer.OrdinalIgnoreCase);
			if (num < 0)
			{
				num = ~num;
			}
			list.Insert(num, recipientAddress);
		}

		private static string GetPrimarySmtpAddress(ADRawEntry entry)
		{
			SmtpAddress value = (SmtpAddress)entry[ADRecipientSchema.PrimarySmtpAddress];
			if (value == SmtpAddress.Empty)
			{
				TraceHelper.StoreDriverTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "no primary SMTP address");
				return null;
			}
			if (!value.IsValidAddress)
			{
				TraceHelper.StoreDriverTracer.TracePass<string>(TraceHelper.MessageProbeActivityId, 0L, "Invalid Smtp Address {0}", value.ToString());
				return null;
			}
			return value.ToString();
		}

		public const int MapiP1 = 268435456;
	}
}
