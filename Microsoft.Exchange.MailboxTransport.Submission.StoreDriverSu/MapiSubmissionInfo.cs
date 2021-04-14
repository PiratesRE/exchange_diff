using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	[Serializable]
	internal class MapiSubmissionInfo : SubmissionInfo
	{
		public MapiSubmissionInfo(string serverDN, Guid mailboxGuid, byte[] entryId, byte[] parentEntryId, long eventCounter, string serverFqdn, IPAddress networkAddress, Guid mdbGuid, string databaseName, DateTime originalCreateTime, bool isPublicFolder, TenantPartitionHint tenantHint, string mailboxHopLatency, LatencyTracker latencyTracker, bool shouldDeprioritize, bool shouldThrottle, IStoreDriverTracer storeDriverTracer) : base(serverDN, serverFqdn, networkAddress, mdbGuid, databaseName, originalCreateTime, tenantHint, mailboxHopLatency, latencyTracker, shouldDeprioritize, shouldThrottle)
		{
			this.mailboxGuid = mailboxGuid;
			this.entryId = entryId;
			this.parentEntryId = parentEntryId;
			this.eventCounter = eventCounter;
			this.isPublicFolder = isPublicFolder;
			this.storeDriverTracer = storeDriverTracer;
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public long EventCounter
		{
			get
			{
				return this.eventCounter;
			}
		}

		public byte[] EntryId
		{
			get
			{
				return this.entryId;
			}
		}

		public byte[] ParentEntryId
		{
			get
			{
				return this.parentEntryId;
			}
		}

		public bool IsPublicFolder
		{
			get
			{
				return this.isPublicFolder;
			}
		}

		public TransportMiniRecipient SenderAdEntry
		{
			get
			{
				return this.senderAdEntry;
			}
			internal set
			{
				this.senderAdEntry = value;
			}
		}

		public override SubmissionItem CreateSubmissionItem(MailItemSubmitter context)
		{
			return new MapiSubmissionItem(this, context, this.storeDriverTracer);
		}

		public override OrganizationId GetOrganizationId()
		{
			if (this.senderAdEntry == null)
			{
				this.storeDriverTracer.MapiStoreDriverSubmissionTracer.TracePass(this.storeDriverTracer.MessageProbeActivityId, 0L, "Using ForestWideOrgId scope for PF replication mail");
				return OrganizationId.ForestWideOrgId;
			}
			return this.senderAdEntry.OrganizationId;
		}

		public override SenderGuidTraceFilter GetTraceFilter()
		{
			return new SenderGuidTraceFilter(base.MdbGuid, this.mailboxGuid);
		}

		public override SubmissionPoisonContext GetPoisonContext()
		{
			return new SubmissionPoisonContext(this.mailboxGuid.Equals(Guid.Empty) ? base.MdbGuid : this.mailboxGuid, this.eventCounter);
		}

		public override void LogEvent(SubmissionInfo.Event submissionInfoEvent)
		{
			switch (submissionInfoEvent)
			{
			case SubmissionInfo.Event.StoreDriverSubmissionPoisonMessageInSubmission:
				this.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_StoreDriverSubmissionPoisonMessageInMapiSubmit);
				return;
			case SubmissionInfo.Event.FailedToGenerateNdrInSubmission:
				break;
			case SubmissionInfo.Event.InvalidSender:
				this.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_InvalidSender);
				break;
			default:
				return;
			}
		}

		public override void LogEvent(SubmissionInfo.Event submissionInfoEvent, Exception exception)
		{
			switch (submissionInfoEvent)
			{
			case SubmissionInfo.Event.StoreDriverSubmissionPoisonMessage:
				this.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_StoreDriverSubmissionPoisonMessage, exception);
				return;
			case SubmissionInfo.Event.StoreDriverSubmissionPoisonMessageInSubmission:
				break;
			case SubmissionInfo.Event.FailedToGenerateNdrInSubmission:
				this.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_FailedToGenerateNDRInMapiSubmit, exception);
				break;
			default:
				return;
			}
		}

		public void LoadAdRawEntry()
		{
			if (Guid.Empty.Equals(this.mailboxGuid))
			{
				this.storeDriverTracer.MapiStoreDriverSubmissionTracer.TraceFail(this.storeDriverTracer.MessageProbeActivityId, 0L, "Mailbox GUID was empty, unable to load AD entry.");
				return;
			}
			try
			{
				this.senderAdEntry = StoreProvider.FindByExchangeGuidIncludingAlternate<TransportMiniRecipient>(this.mailboxGuid, base.TenantHint);
			}
			catch (NonUniqueRecipientException)
			{
				this.storeDriverTracer.MapiStoreDriverSubmissionTracer.TracePass<Guid>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Multiple objects with Mailbox Guid {0} were found.", this.mailboxGuid);
			}
		}

		public string GetSenderEmailAddress()
		{
			if (this.senderAdEntry == null)
			{
				return string.Empty;
			}
			SmtpAddress primarySmtpAddress = this.senderAdEntry.PrimarySmtpAddress;
			if (primarySmtpAddress.IsValidAddress)
			{
				return primarySmtpAddress.ToString();
			}
			ProxyAddressCollection emailAddresses = this.senderAdEntry.EmailAddresses;
			if (emailAddresses == null || 0 >= emailAddresses.Count)
			{
				return string.Empty;
			}
			ProxyAddress proxyAddress = emailAddresses.Find(new Predicate<ProxyAddress>(this.IsSmtpAddress));
			if (null != proxyAddress)
			{
				return proxyAddress.ToString();
			}
			return emailAddresses[0].ToString();
		}

		public MultiValuedProperty<Guid> GetAggregatedMailboxGuids()
		{
			if (this.senderAdEntry != null)
			{
				return this.senderAdEntry.AggregatedMailboxGuids;
			}
			return MultiValuedProperty<Guid>.Empty;
		}

		public MultiValuedProperty<CultureInfo> GetSenderLocales()
		{
			if (this.senderAdEntry != null)
			{
				return this.senderAdEntry.Languages;
			}
			return MultiValuedProperty<CultureInfo>.Empty;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Event {0}, mailbox {1}, mdb {2}", new object[]
			{
				this.eventCounter,
				this.mailboxGuid,
				base.MdbGuid
			});
		}

		private bool IsSmtpAddress(ProxyAddress address)
		{
			return address.Prefix.Equals(ProxyAddressPrefix.Smtp);
		}

		private void LogEvent(ExEventLog.EventTuple eventTuple)
		{
			StoreDriverSubmission.LogEvent(eventTuple, null, new object[]
			{
				this.EventCounter,
				this.MailboxGuid,
				base.MdbGuid
			});
		}

		private void LogEvent(ExEventLog.EventTuple eventTuple, Exception exception)
		{
			StoreDriverSubmission.LogEvent(eventTuple, null, new object[]
			{
				this.EventCounter,
				this.MailboxGuid,
				base.MdbGuid,
				exception
			});
		}

		private readonly Guid mailboxGuid;

		private readonly byte[] entryId;

		private readonly byte[] parentEntryId;

		private readonly long eventCounter;

		private readonly bool isPublicFolder;

		private TransportMiniRecipient senderAdEntry;

		private IStoreDriverTracer storeDriverTracer;
	}
}
