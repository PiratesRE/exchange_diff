using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal sealed class MessageChecker
	{
		internal MessageChecker(MailItem mailItem, Configuration configuration)
		{
			this.mailItem = mailItem;
			this.headers = mailItem.Message.MimeDocument.RootPart.Headers;
			this.configuration = configuration;
		}

		private static bool IsUMMessage(string mapiMessageClass)
		{
			return "IPM.Note.Microsoft.Voicemail.UM".Equals(mapiMessageClass, StringComparison.OrdinalIgnoreCase) || "IPM.Note.Microsoft.Voicemail.UM.CA".Equals(mapiMessageClass, StringComparison.OrdinalIgnoreCase) || "IPM.Note.Microsoft.Missed.Voice".Equals(mapiMessageClass, StringComparison.OrdinalIgnoreCase) || "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA".Equals(mapiMessageClass, StringComparison.OrdinalIgnoreCase) || "IPM.Note.rpmsg.Microsoft.Voicemail.UM".Equals(mapiMessageClass, StringComparison.OrdinalIgnoreCase);
		}

		internal static IComparer<RoutingAddress> AddressComparer
		{
			get
			{
				return MessageChecker.routingAddressComparer;
			}
		}

		internal CheckStatus CheckJournalReport()
		{
			if (((ITransportMailItemWrapperFacade)this.mailItem).TransportMailItem.IsJournalReport())
			{
				return CheckStatus.TransportJournalReport;
			}
			Header header = this.headers.FindFirst("X-MS-Exchange-Organization-AuthMechanism");
			int num = -1;
			if (header == null || !int.TryParse(header.Value, NumberStyles.HexNumber, null, out num))
			{
				ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Unable to determine authentication mechanism, will not patch report. Header: {0}", (header == null) ? null : header.Value);
				return CheckStatus.NotJournalReport;
			}
			if (num != 3 && num != 4 && num != 5)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "No need to patch this journal-report as it did not originate from store. It is either a regular journal-report or this is not the first (mailbox) hop");
				return CheckStatus.NotJournalReport;
			}
			if (this.headers.FindFirst("X-MS-Journal-Report") == null)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "No need to patch this message, it is not marked X-MS-Journal-Report");
				return CheckStatus.NotJournalReport;
			}
			ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "The message is from MAPI and has the X-MS-Journal-Report-Header");
			string mailboxSmtpAddress = this.mailItem.FromAddress.ToString();
			object obj = null;
			if (this.configuration.JournalReportNdrTo != this.mailItem.FromAddress && (!this.mailItem.Properties.TryGetValue("Microsoft.Exchange.Transport.ElcJournalReport", out obj) || !(bool)obj) && !this.configuration.ReconcileConfig.IsReconcileMailbox(mailboxSmtpAddress))
			{
				ExTraceGlobals.JournalingTracer.TraceError((long)this.GetHashCode(), "This message is marked as a journal-report, but the P1 sender is not authorized to submit or resend journal reports, and it is not marked as an ELC report (ElcReport-Property: {0}). We will NDR it.", new object[]
				{
					obj
				});
				throw new UnauthorizedSubmitterException();
			}
			AttachmentCollection attachments = this.mailItem.Message.Attachments;
			if (attachments == null || attachments.Count != 1)
			{
				ExTraceGlobals.JournalingTracer.TraceError((long)this.GetHashCode(), "This message looks like it may have been a resent/ELC journal-report, but does not have the structure expected. Journal reports are expected to have a single message attachment. We will NDR it.");
				throw new UnauthorizedSubmitterException();
			}
			ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "This is an ELC-journal-report or journal-report-NDR-to-resubmission");
			return CheckStatus.MailboxJournalReport;
		}

		internal bool CheckResubmittedMessage()
		{
			object obj;
			return this.mailItem.Properties.TryGetValue("Microsoft.Exchange.Transport.ResentMapiMessage", out obj) && obj is bool && (bool)obj;
		}

		internal bool CheckNdrOfJournalReport()
		{
			if (Utils.IsNdr(this.mailItem))
			{
				foreach (EnvelopeRecipient envelopeRecipient in this.mailItem.Recipients)
				{
					if (envelopeRecipient.Address == this.configuration.JournalReportNdrTo)
					{
						return true;
					}
				}
				EmailRecipientCollection to = this.mailItem.Message.To;
				if (to != null)
				{
					foreach (EmailRecipient emailRecipient in to)
					{
						if ((RoutingAddress)emailRecipient.SmtpAddress == this.configuration.MSExchangeRecipient)
						{
							return true;
						}
					}
					return false;
				}
				return false;
			}
			return false;
		}

		internal bool CheckFirstExchangeHop()
		{
			return null == this.headers.FindFirst("X-MS-Exchange-Organization-Processed-By-Journaling");
		}

		internal bool CheckFirstExchangeHopForGccProcessing()
		{
			return null == this.headers.FindFirst("X-MS-Exchange-Organization-Processed-By-Gcc-Journaling");
		}

		private bool CheckQuarantineMessage()
		{
			return null != this.headers.FindFirst("X-MS-Exchange-Organization-Quarantine");
		}

		private bool CheckPfReplicationMessage()
		{
			if (!EmailMessageHelpers.IsPublicFolderReplicationMessage(this.mailItem.Message))
			{
				return false;
			}
			ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Message is marked as PF-replication-message, checking submitter identity");
			bool result;
			try
			{
				ProxyAddress proxyAddress = Utils.RoutingAddressToProxyAddress(this.mailItem.FromAddress.ToString());
				object[] array = Utils.ADLookupUser(this.mailItem, proxyAddress, MessageChecker.recipientTypeProperty);
				if (array == null)
				{
					ExTraceGlobals.JournalingTracer.TraceError((long)this.GetHashCode(), "The sender of the message was not found in AD. This mesage will not be regarded as a system-message");
					result = false;
				}
				else
				{
					Microsoft.Exchange.Data.Directory.Recipient.RecipientType? recipientType = array[0] as Microsoft.Exchange.Data.Directory.Recipient.RecipientType?;
					ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "Recipient-type: {0}", (recipientType == null) ? "null" : ((int)recipientType.Value).ToString());
					result = (recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicDatabase);
				}
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Error looking up sender in AD, message will not be treated as a system message: {0}", ex.ToString());
				result = false;
			}
			return result;
		}

		private bool CheckSkipUmVoicemail()
		{
			if (this.configuration.SkipUMVoicemailMessages && MessageChecker.IsUMMessage(this.mailItem.Message.MapiMessageClass))
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Message had UM message-class and we are configured to skip UM messages");
				if (this.mailItem.FromAddress == this.configuration.MSExchangeRecipient)
				{
					ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Sender of message was Microsoft Exchange which is authorized to submit UM messages");
					return true;
				}
				if (this.IsSubmittedBySystem())
				{
					ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Message was authenticated as a system submission");
					return true;
				}
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Message could not be authenticated as a UM message, we will not skip journaling it");
			}
			return false;
		}

		private bool CheckExemptMessage()
		{
			Header header = this.headers.FindFirst("X-MS-Exchange-Organization-Do-Not-Journal");
			if (header != null)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "No need to journal this message as it is a system-message. Debug-Context: \"{0}\"", header.Value);
				return true;
			}
			return false;
		}

		private bool IsSubmittedBySystem()
		{
			return null != this.headers.FindFirst("X-MS-Exchange-Organization-Mapi-Admin-Submission");
		}

		internal bool CheckMuaSubmission()
		{
			if (this.mailItem.InboundDeliveryMethod != DeliveryMethod.Smtp)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Non-SMTP message, skipping MUA check");
				return false;
			}
			object obj = null;
			if (!this.mailItem.Properties.TryGetValue("Microsoft.Exchange.SmtpMuaSubmission", out obj))
			{
				ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Unexpected error: the property {0} does not exist on an SMTP submitted message. Assuming this is not an MUA", "Microsoft.Exchange.SmtpMuaSubmission");
				return false;
			}
			if (!(obj is bool))
			{
				ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Unexpected error: the property {0} was expected to be a UINT, but it is not. Assuming this is not an MUA", "Microsoft.Exchange.SmtpMuaSubmission");
				return false;
			}
			return (bool)obj;
		}

		internal bool CheckRecipientsChanged()
		{
			object obj;
			if (!this.mailItem.Properties.TryGetValue("Microsoft.Exchange.Journaling.OriginalRecipientInfo", out obj))
			{
				return true;
			}
			List<RoutingAddress> list = obj as List<RoutingAddress>;
			if (list == null)
			{
				ExTraceGlobals.JournalingTracer.TraceError((long)this.GetHashCode(), "Original recipient-info property could not be retrieved (was null). Possible interference by another agent. Assuming that the original recipients have changed");
				return true;
			}
			foreach (EnvelopeRecipient envelopeRecipient in this.mailItem.Recipients)
			{
				if (list.BinarySearch(envelopeRecipient.Address, MessageChecker.routingAddressComparer) < 0)
				{
					return true;
				}
			}
			return false;
		}

		internal bool ShouldSkipJournaling()
		{
			return this.ShouldSkipJournaling(false);
		}

		internal bool ShouldSkipGccJournaling()
		{
			return this.ShouldSkipJournaling(true);
		}

		internal bool ShouldSkipJournaling(bool isGccCheck)
		{
			if (this.mailItem.Recipients.Count == 0)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Not journaling this message as it has no envelope recipient");
				return true;
			}
			if (((ITransportMailItemWrapperFacade)this.mailItem).TransportMailItem.IsJournalReport())
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Not journaling this message as it is already a journal-report");
				return true;
			}
			if (this.CheckNdrOfJournalReport())
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Not journaling this message as it is an NDR of a journal report");
				return true;
			}
			if (this.CheckSkipUmVoicemail())
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Not journaling this message as it is a UM voicemail");
				return true;
			}
			if (this.CheckPfReplicationMessage())
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Not journaling this message because it is a public-folder replication message");
				return true;
			}
			if (this.CheckQuarantineMessage())
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Not journaling this message as it is a quarantine Dsn");
				return true;
			}
			if (!isGccCheck && this.CheckExemptMessage())
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Not journaling this message as it is exempt");
				return true;
			}
			if (isGccCheck)
			{
				if (!this.CheckFirstExchangeHopForGccProcessing())
				{
					ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "This isn't the first hop for the message, only run gcc journaling if any recipients were added");
					if (!this.CheckRecipientsChanged())
					{
						ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "No recipients added, no need to re-run gcc journaling");
						return true;
					}
					ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Some recipients were added, gcc journaling rules need to be re-run");
				}
			}
			else
			{
				if (!this.CheckFirstExchangeHop())
				{
					ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "This isn't the first hop for the message, only journal if any recipients were added");
					if (!this.CheckRecipientsChanged())
					{
						ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "No recipients added, no need to re-run journaling");
						return true;
					}
					ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Some recipients were added to the, journaling rules need to be re-run");
				}
				if (this.configuration.OrganizationId == OrganizationId.ForestWideOrgId && !VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.ProcessForestWideOrgJournal.Enabled)
				{
					ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Skip journaling for safe tenant in datacenter");
					return true;
				}
			}
			return false;
		}

		internal bool IsLegacyJournalReport()
		{
			object obj;
			if (this.mailItem.Properties.TryGetValue("Microsoft.Exchange.LegacyJournalReport", out obj))
			{
				LegacyJournalReportType legacyJournalReportType = (LegacyJournalReportType)obj;
				if (legacyJournalReportType == LegacyJournalReportType.Bcc || legacyJournalReportType == LegacyJournalReportType.Envelope || legacyJournalReportType == LegacyJournalReportType.EnvelopeV2)
				{
					return true;
				}
			}
			return false;
		}

		private const string MuaSubmission = "X-Smtp-Mua-Submission";

		private static IComparer<RoutingAddress> routingAddressComparer = new MessageChecker.RoutingAddressComparer();

		private static PropertyDefinition[] recipientTypeProperty = new PropertyDefinition[]
		{
			ADRecipientSchema.RecipientType
		};

		private MailItem mailItem;

		private HeaderList headers;

		private Configuration configuration;

		internal sealed class RoutingAddressComparer : IComparer<RoutingAddress>
		{
			public int Compare(RoutingAddress x, RoutingAddress y)
			{
				return x.CompareTo(y);
			}
		}
	}
}
