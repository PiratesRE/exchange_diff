using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.MailboxTransport.Shared.SubmissionItem;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission.Agents
{
	internal class MfnSubmitter : IDisposable
	{
		public MfnSubmitter(SubmissionItem item, MailItemSubmitter submitter)
		{
			this.item = item;
			this.submitter = submitter;
		}

		internal CultureInfo CultureInfo
		{
			get
			{
				if (this.cultureInfo == null)
				{
					this.cultureInfo = this.GetMesssageCultureInfo();
				}
				return this.cultureInfo;
			}
		}

		private ADRecipient OrganizerRecipient
		{
			get
			{
				if (this.organizerRecipient == null)
				{
					MailboxSession mailboxSession = this.item.Item.Session as MailboxSession;
					if (mailboxSession != null)
					{
						MeetingMessage meetingMessage = null;
						try
						{
							meetingMessage = (Item.ConvertFrom(this.item.Item, mailboxSession) as MeetingMessage);
							if (meetingMessage != null)
							{
								string text = MfnSubmitter.SafeGetProperty<string>(meetingMessage, CalendarItemBaseSchema.OrganizerEmailAddress, string.Empty);
								string text2 = MfnSubmitter.SafeGetProperty<string>(meetingMessage, CalendarItemBaseSchema.OrganizerType, string.Empty);
								if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
								{
									ProxyAddress proxyAddress = ProxyAddress.Parse(text2, text);
									this.organizerRecipient = this.RecipientSession.FindByProxyAddress(proxyAddress);
								}
							}
						}
						finally
						{
							if (meetingMessage != null)
							{
								Item.SafeDisposeConvertedItem(this.item.Item, meetingMessage);
							}
						}
					}
				}
				return this.organizerRecipient;
			}
		}

		private IRecipientSession RecipientSession
		{
			get
			{
				if (this.recipientSession == null)
				{
					if (this.submitter.SubmissionInfo.TenantHint == null)
					{
						this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 173, "RecipientSession", "f:\\15.00.1497\\sources\\dev\\MailboxTransport\\src\\MailboxTransportSubmission\\StoreDriverSubmission\\agents\\MfnSubmitter\\MfnSubmitter.cs");
					}
					else
					{
						this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantPartitionHint(this.submitter.SubmissionInfo.TenantHint), 179, "RecipientSession", "f:\\15.00.1497\\sources\\dev\\MailboxTransport\\src\\MailboxTransportSubmission\\StoreDriverSubmission\\agents\\MfnSubmitter\\MfnSubmitter.cs");
					}
				}
				return this.recipientSession;
			}
		}

		public void Dispose()
		{
		}

		internal void CheckAndSubmitMfn(TransportMailItem originalMailItem)
		{
			if (!this.ShouldGenerateMfn())
			{
				return;
			}
			using (MeetingRequest meetingRequest = MeetingRequest.Bind(this.item.Session, this.item.Item.Id))
			{
				using (MeetingForwardNotification meetingForwardNotification = meetingRequest.CreateNotification())
				{
					TransportMailItem transportMailItem = TransportMailItem.NewSideEffectMailItem(originalMailItem, this.submitter.OrganizationId, LatencyComponent.Agent, MailDirectionality.Originating, this.submitter.ExternalOrganizationId);
					transportMailItem.From = RoutingAddress.NullReversePath;
					if (this.TryCreateMfnSubjectAndBody(meetingForwardNotification))
					{
						Stream stream = transportMailItem.OpenMimeWriteStream();
						stream.Close();
						this.CopyMfnRecipientsTo(meetingForwardNotification, transportMailItem);
						this.CopyMfnContentTo(meetingForwardNotification, transportMailItem);
						MailboxSession mailboxSession = this.item.Item.Session as MailboxSession;
						Participant participant = Participant.TryConvertTo(this.item.Sender, "SMTP", mailboxSession);
						if (participant == null)
						{
							participant = this.item.Sender;
						}
						if (mailboxSession != null)
						{
							MfnLog.LogEntry(mailboxSession, string.Format("Decorating MFN with the address - Name {0}, EmailAddress : {1}, RoutingType : {2}", participant.DisplayName, participant.EmailAddress, participant.RoutingType));
						}
						Components.DsnGenerator.DecorateMfn(transportMailItem, participant.DisplayName, participant.EmailAddress);
						this.item.DecorateMessage(transportMailItem);
						this.item.ApplySecurityAttributesTo(transportMailItem);
						ClassificationUtils.PromoteStoreClassifications(transportMailItem.RootPart.Headers);
						this.submitter.StoreDriverSubmission.ThrowIfStopped();
						transportMailItem.UpdateCachedHeaders();
						this.submitter.SendMailItem(new SubmissionReadOnlyMailItem(transportMailItem, MailItemType.OtherMessage), null);
					}
				}
			}
		}

		private static T SafeGetProperty<T>(StoreObject message, PropertyDefinition propertyDefinition, T defaultValue)
		{
			if (message == null)
			{
				return defaultValue;
			}
			object obj = message.TryGetProperty(propertyDefinition);
			if (obj == null || obj is PropertyError)
			{
				return defaultValue;
			}
			if (!(obj is T))
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		private bool ShouldGenerateMfn()
		{
			MessageItem messageItem = this.item.Item;
			MailboxSession mailboxSession = messageItem.Session as MailboxSession;
			if (mailboxSession == null)
			{
				TraceHelper.MeetingForwardNotificationTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "Skipping MFN generation as the session is not a mailboxsession.");
				return false;
			}
			if (!ObjectClass.IsMeetingRequest(messageItem.ClassName) && !ObjectClass.IsMeetingRequestSeries(messageItem.ClassName))
			{
				TraceHelper.MeetingForwardNotificationTracer.TracePass<string>(TraceHelper.MessageProbeActivityId, 0L, "Not a meeting request {0} - skipping MFN generation.", messageItem.ClassName);
				return false;
			}
			string text = null;
			bool result;
			try
			{
				string text2 = MfnSubmitter.SafeGetProperty<string>(messageItem, MessageItemSchema.ReceivedRepresentingEmailAddress, string.Empty);
				if (!string.IsNullOrEmpty(text2))
				{
					text = string.Format("Skipping MFN generation as the update/forward was made by a delegate {0}. From: {1}, Sender {2}, Subject: {3}", new object[]
					{
						text2,
						messageItem.From,
						messageItem.Sender,
						messageItem.Subject ?? "<No subject found>"
					});
					result = false;
				}
				else
				{
					int num = MfnSubmitter.SafeGetProperty<int>(messageItem, MeetingMessageSchema.AppointmentAuxiliaryFlags, 0);
					if ((num & 32) != 0)
					{
						text = string.Format("Skipping MFN generation for RUMs.  From: {0}, Sender {1} , Subject: {2}", messageItem.From, messageItem.Sender, messageItem.Subject ?? "<No subject found>");
						result = false;
					}
					else if (messageItem.IsResponseRequested && (num & 256) != 0)
					{
						text = string.Format("Skipping MFN generation for events added from group calendar.  From: {0}, Sender {1} , Subject: {2}", messageItem.From, messageItem.Sender, messageItem.Subject ?? "<No subject found>");
						result = false;
					}
					else
					{
						if ((num & 4) == 0)
						{
							if (mailboxSession.IsGroupMailbox())
							{
								TraceHelper.MeetingForwardNotificationTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "Skipping MFN generation as the meeting was created/updated on the group calendar. From: {0}, Sender {1}, Auxilliary Flags {2}, Subject {3}", new object[]
								{
									messageItem.From,
									messageItem.Sender,
									num,
									messageItem.Subject ?? "<No subject found>"
								});
								return false;
							}
							if (Participant.HasSameEmail(messageItem.From, messageItem.Sender, mailboxSession, true))
							{
								TraceHelper.MeetingForwardNotificationTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "Skipping MFN generation as the update was made by the organizer. From: {0}, Sender {1}, Auxilliary Flags {2}, Subject {3}", new object[]
								{
									messageItem.From,
									messageItem.Sender,
									num,
									messageItem.Subject ?? "<No subject found>"
								});
								return false;
							}
							bool? flag = this.IsDelegateOfOrganizer();
							if (flag != null && flag.Value)
							{
								text = string.Format("Skipping MFN generation as the update was made by a delegate. From: {0}, Sender {1}. Auxilliary Flags {2}, Subject {3}", new object[]
								{
									messageItem.From,
									messageItem.Sender,
									num,
									messageItem.Subject ?? "<No subject found>"
								});
								return false;
							}
						}
						if (MeetingMessage.IsFromExternalParticipant(messageItem.From.RoutingType))
						{
							text = string.Format("Skipping MFN generation as the organizer is external. From: {0}, RoutingType {1}, Sender {2}, Auxilliary Flags {3} Subject : {4}", new object[]
							{
								messageItem.From,
								messageItem.From.RoutingType,
								messageItem.Sender,
								num,
								messageItem.Subject ?? "<No subject found>"
							});
							result = false;
						}
						else
						{
							text = string.Format("Generating MFN as the meeting was forwarded. From: {0}, RoutingType {1}, Sender {2}, Auxilliary Flags {3}, Subject {4}", new object[]
							{
								messageItem.From,
								messageItem.From.RoutingType,
								messageItem.Sender,
								num,
								messageItem.Subject ?? "<No subject found>"
							});
							result = true;
						}
					}
				}
			}
			finally
			{
				if (text != null)
				{
					TraceHelper.MeetingForwardNotificationTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, text);
					MfnLog.LogEntry(mailboxSession, text);
				}
			}
			return result;
		}

		private bool? IsDelegateOfOrganizer()
		{
			bool? result = null;
			try
			{
				List<ADObjectId> list = new List<ADObjectId>();
				if (this.OrganizerRecipient != null && this.OrganizerRecipient.GrantSendOnBehalfTo != null)
				{
					foreach (ADObjectId adobjectId in this.OrganizerRecipient.GrantSendOnBehalfTo)
					{
						list.Add(adobjectId);
					}
					if (list.Count > 0)
					{
						Result<ADRecipient>[] array = this.RecipientSession.ReadMultiple(list.ToArray());
						foreach (Result<ADRecipient> result2 in array)
						{
							if (result2.Error == null && string.Equals(this.item.Item.Sender.RoutingType, "EX", StringComparison.OrdinalIgnoreCase) && string.Equals(this.item.Item.Sender.EmailAddress, result2.Data.LegacyExchangeDN, StringComparison.OrdinalIgnoreCase))
							{
								TraceHelper.MeetingForwardNotificationTracer.TracePass<string>(TraceHelper.MessageProbeActivityId, 0L, "MFN generation: Found a delegate match {0}", this.item.Item.Sender.EmailAddress);
								result = new bool?(true);
								return result;
							}
						}
					}
					TraceHelper.MeetingForwardNotificationTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "Failed to find a delegate match");
					result = new bool?(false);
					return result;
				}
			}
			catch (LocalizedException arg)
			{
				TraceHelper.MeetingForwardNotificationTracer.TraceFail<LocalizedException>(TraceHelper.MessageProbeActivityId, 0L, "No Meeting Forward Notification will be sent. Exception: {0} ", arg);
			}
			catch (ArgumentException arg2)
			{
				TraceHelper.MeetingForwardNotificationTracer.TraceFail<ArgumentException>(TraceHelper.MessageProbeActivityId, 0L, "No Meeting Forward Notification will be sent. Exception: {0} ", arg2);
			}
			return result;
		}

		private void CopyMfnRecipientsTo(MeetingForwardNotification meetingForwardNotification, TransportMailItem mailItem)
		{
			bool resubmittedMessage = this.item.ResubmittedMessage;
			foreach (Recipient recipient in meetingForwardNotification.Recipients)
			{
				RoutingAddress address;
				int? recipientType;
				if (SubmissionItemUtils.TryGetRoutingAddressFromParticipant(mailItem.ADRecipientCache, recipient.Participant, "Recipient", out address) && SubmissionItemUtils.GetP2RecipientType(resubmittedMessage, recipient, out recipientType))
				{
					MailRecipient transportRecipient = mailItem.Recipients.Add((string)address);
					string refTypePropValue = SubmissionItemBase.GetRefTypePropValue<string>(recipient, StoreObjectSchema.DisplayName);
					SubmissionItemUtils.CopyRecipientPropsFromXSOToTransport(this.item, recipient, transportRecipient, recipientType, refTypePropValue);
					MSExchangeStoreDriverSubmission.TotalRecipients.Increment();
				}
			}
		}

		private void CopyMfnContentTo(MeetingForwardNotification meetingForwardNotification, TransportMailItem mailItem)
		{
			OutboundConversionOptions outboundConversionOptions = new OutboundConversionOptions(Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName);
			outboundConversionOptions.DsnMdnOptions = DsnMdnOptions.PropagateUserSettings;
			outboundConversionOptions.DsnHumanReadableWriter = Components.DsnGenerator.DsnHumanReadableWriter;
			outboundConversionOptions.RecipientCache = mailItem.ADRecipientCache;
			outboundConversionOptions.UserADSession = mailItem.ADRecipientCache.ADSession;
			mailItem.CacheTransportSettings();
			outboundConversionOptions.ClearCategories = mailItem.TransportSettings.ClearCategories;
			outboundConversionOptions.UseRFC2231Encoding = mailItem.TransportSettings.Rfc2231EncodingEnabled;
			outboundConversionOptions.Limits.MimeLimits = MimeLimits.Unlimited;
			outboundConversionOptions.AllowDlpHeadersToPenetrateFirewall = true;
			using (Stream stream = mailItem.OpenMimeWriteStream())
			{
				if (Components.Configuration.LocalServer.TransportServer.ContentConversionTracingEnabled && Components.Configuration.LocalServer.TransportServer.PipelineTracingPath != null)
				{
					outboundConversionOptions.LogDirectoryPath = Components.Configuration.LocalServer.TransportServer.PipelineTracingPath.PathName;
				}
				ItemConversion.ConvertItemToSummaryTnef(meetingForwardNotification, stream, outboundConversionOptions);
			}
		}

		private bool TryCreateMfnSubjectAndBody(MeetingForwardNotification meetingForwardNotification)
		{
			if (this.item.Sender != null)
			{
				ExTimeZone exTimeZoneFromItem = TimeZoneHelper.GetExTimeZoneFromItem(this.item.Item);
				string value = string.Concat(new string[]
				{
					Strings.descTitle.ToString(this.CultureInfo),
					this.GenerateExplanation(),
					this.GenerateMeetingSubject(meetingForwardNotification),
					this.GenerateMeetingTime(meetingForwardNotification, exTimeZoneFromItem),
					this.GenerateRecipientsTable(meetingForwardNotification),
					this.GenerateTimeZoneInfo(exTimeZoneFromItem)
				});
				using (TextWriter textWriter = meetingForwardNotification.Body.OpenTextWriter(BodyFormat.TextHtml))
				{
					textWriter.Write(this.GenerateHeader());
					textWriter.Write(value);
					textWriter.Write(this.GenerateFooter());
				}
				meetingForwardNotification.Subject = string.Format(Strings.MeetingForwardNotificationSubject.ToString(this.CultureInfo), (meetingForwardNotification.Subject != null || meetingForwardNotification.Subject != string.Empty) ? meetingForwardNotification.Subject : Strings.NoSubject.ToString(this.CultureInfo));
				return true;
			}
			return false;
		}

		private string GenerateHeader()
		{
			return "<html><head></head><body>" + Strings.descTahomaBlackMediumFontTag.ToString(this.CultureInfo);
		}

		private string GenerateFooter()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<HR/>");
			stringBuilder.Append(Strings.descArialGreySmallFontTag.ToString(this.CultureInfo));
			stringBuilder.Append(Strings.descCredit.ToString(this.CultureInfo));
			stringBuilder.Append("</font></body></html>");
			return stringBuilder.ToString();
		}

		private string GenerateExplanation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			MailboxSession session = this.item.Item.Session as MailboxSession;
			Participant participant = Participant.TryConvertTo(this.item.Sender, "SMTP", session);
			if (participant == null)
			{
				participant = this.item.Sender;
			}
			stringBuilder.Append("<a href=\"mailto:");
			stringBuilder.Append(participant.EmailAddress);
			stringBuilder.Append("\">");
			stringBuilder.Append(participant.DisplayName + "</a>&nbsp;");
			return string.Format(Strings.descExplination.ToString(this.CultureInfo), stringBuilder.ToString());
		}

		private string GenerateRecipientsTable(MeetingForwardNotification meetingForwardNotification)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<table border=\"0\" cellpadding=\"2\"><tr><td width=\"29\">&nbsp;</td><td><b>" + Strings.descTahomaGreyMediumFontTag.ToString(this.CultureInfo) + Strings.descRecipientsLabel.ToString(this.CultureInfo) + "</font></b></td></tr>");
			List<Participant> participantCollection = meetingForwardNotification.GetParticipantCollection();
			foreach (Participant participant in participantCollection)
			{
				if (participant != null)
				{
					MailboxSession session = this.item.Item.Session as MailboxSession;
					Participant participant2 = Participant.TryConvertTo(participant, "SMTP", session);
					if (participant2 == null)
					{
						participant2 = participant;
					}
					stringBuilder.Append("<tr><td width=\"29\">&nbsp;</td><td>");
					stringBuilder.Append(Strings.descTahomaBlackMediumFontTag.ToString(this.CultureInfo));
					stringBuilder.Append("<a href=\"mailto:");
					stringBuilder.Append(participant2.EmailAddress);
					stringBuilder.Append("\">");
					stringBuilder.Append(participant2.DisplayName + "</a>&nbsp;");
					stringBuilder.Append("</font></td></tr>");
				}
			}
			stringBuilder.Append("</table><br/>");
			return stringBuilder.ToString();
		}

		private string GenerateMeetingTime(MeetingForwardNotification meetingForwardNotification, ExTimeZone tz)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = meetingForwardNotification.GenerateWhen(this.CultureInfo, tz);
			if (string.IsNullOrEmpty(value))
			{
				value = Strings.NoStartTime.ToString(this.CultureInfo);
			}
			stringBuilder.Append("<table border=\"0\" cellpadding=\"2\"><tr><td width=\"29\">&nbsp;</td><td><b>" + Strings.descTahomaGreyMediumFontTag.ToString(this.CultureInfo) + Strings.descMeetingTimeLabel.ToString(this.CultureInfo) + "</font></b></td></tr>");
			stringBuilder.Append("<tr><td width=\"29\">&nbsp;</td><td>");
			stringBuilder.Append(Strings.descTahomaBlackMediumFontTag.ToString(this.CultureInfo));
			stringBuilder.Append(value);
			stringBuilder.Append("</font></td></tr>");
			stringBuilder.Append("</table><br/>");
			return stringBuilder.ToString();
		}

		private string GenerateMeetingSubject(MeetingForwardNotification meetingForwardNotification)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = meetingForwardNotification.Subject;
			if (text == null || text == string.Empty)
			{
				text = Strings.NoSubject.ToString(this.CultureInfo);
			}
			stringBuilder.Append("<table border=\"0\" cellpadding=\"2\"><tr><td width=\"29\">&nbsp;</td><td><b>" + Strings.descTahomaGreyMediumFontTag.ToString(this.CultureInfo) + Strings.descMeetingSubjectLabel.ToString(this.CultureInfo) + "</font></b></td></tr>");
			stringBuilder.Append("<tr><td width=\"29\">&nbsp;</td><td>");
			stringBuilder.Append(Strings.descTahomaBlackMediumFontTag.ToString(this.CultureInfo));
			stringBuilder.Append(text);
			stringBuilder.Append("</font></td></tr>");
			stringBuilder.Append("</table><br/>");
			return stringBuilder.ToString();
		}

		private string GenerateTimeZoneInfo(ExTimeZone tz)
		{
			return string.Concat(new string[]
			{
				Strings.descTahomaGreyMediumFontTag.ToString(this.CultureInfo),
				Strings.descTimeZoneInfo.ToString(this.CultureInfo),
				"&nbsp;",
				tz.LocalizableDisplayName.ToString(this.CultureInfo),
				"</font>"
			});
		}

		private CultureInfo GetMesssageCultureInfo()
		{
			MessageItem messageItem = this.item.Item;
			CultureInfo cultureInfo = messageItem.GetPreferredAcceptLanguage();
			if (cultureInfo == null)
			{
				try
				{
					string formatString = "GetPreferredAcceptLanguage returned null AcceptLanguage. Attempting to look up AD for the organizer's culture. Item Id: {0}";
					TraceHelper.MeetingForwardNotificationTracer.TracePass<string>(TraceHelper.MessageProbeActivityId, 0L, formatString, messageItem.InternetMessageId);
					IADOrgPerson iadorgPerson = this.OrganizerRecipient as IADOrgPerson;
					if (iadorgPerson != null)
					{
						MultiValuedProperty<CultureInfo> languages = iadorgPerson.Languages;
						if (languages != null && languages.Count > 0)
						{
							cultureInfo = languages[0];
						}
					}
				}
				catch (LocalizedException arg)
				{
					TraceHelper.MeetingForwardNotificationTracer.TraceFail<LocalizedException>(TraceHelper.MessageProbeActivityId, 0L, "Error while looking up organizer's culture in AD. Exception: {0} ", arg);
				}
				catch (ArgumentException arg2)
				{
					TraceHelper.MeetingForwardNotificationTracer.TraceFail<ArgumentException>(TraceHelper.MessageProbeActivityId, 0L, "Error while looking up organizer's culture in AD. Exception: {0} ", arg2);
				}
			}
			if (cultureInfo != null)
			{
				try
				{
					cultureInfo = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
				}
				catch (ArgumentException)
				{
					cultureInfo = null;
				}
			}
			if (cultureInfo == null)
			{
				string formatString2 = "GetPreferredAcceptLanguage returned null AcceptLanguage. Organizer's preferred culture in AD could not be found. Using default culture. Item Id: {0}";
				TraceHelper.MeetingForwardNotificationTracer.TracePass<string>(TraceHelper.MessageProbeActivityId, 0L, formatString2, messageItem.InternetMessageId);
				cultureInfo = CultureInfo.InvariantCulture;
			}
			return cultureInfo;
		}

		private static readonly Trace diag = ExTraceGlobals.MeetingForwardNotificationTracer;

		private SubmissionItem item;

		private MailItemSubmitter submitter;

		private ADRecipient organizerRecipient;

		private IRecipientSession recipientSession;

		private CultureInfo cultureInfo;
	}
}
