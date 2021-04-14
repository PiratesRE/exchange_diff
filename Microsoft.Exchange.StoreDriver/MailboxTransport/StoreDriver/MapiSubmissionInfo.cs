using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	[Serializable]
	internal class MapiSubmissionInfo : SubmissionInfo
	{
		public MapiSubmissionInfo(string serverDN, Guid mailboxGuid, byte[] entryId, byte[] parentEntryId, long eventCounter, string serverFqdn, IPAddress networkAddress, Guid mdbGuid, bool isShadowSupported, DateTime originalCreateTime, string mailboxHopLatency) : base(serverDN, serverFqdn, networkAddress, mdbGuid, isShadowSupported, originalCreateTime, mailboxHopLatency)
		{
			this.mailboxGuid = mailboxGuid;
			this.entryId = entryId;
			this.parentEntryId = parentEntryId;
			this.eventCounter = eventCounter;
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

		public override bool IsShadowSubmission
		{
			get
			{
				return false;
			}
		}

		public TransportMiniRecipient SenderAdEntry
		{
			get
			{
				return this.senderAdEntry;
			}
		}

		public override SubmissionItem CreateSubmissionItem(MailItemSubmitter context)
		{
			throw new NotImplementedException();
		}

		public override OrganizationId GetOrganizationId()
		{
			if (this.MailboxGuid.Equals(Guid.Empty) || this.senderAdEntry == null)
			{
				SubmissionInfo.Diag.TraceDebug(0L, "Using ForestWideOrgId scope for PF replication mail");
				return OrganizationId.ForestWideOrgId;
			}
			return this.senderAdEntry.OrganizationId;
		}

		public override SenderGuidTraceFilter GetTraceFilter()
		{
			return new SenderGuidTraceFilter(base.MdbGuid, this.mailboxGuid);
		}

		public override string GetPoisonId()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				this.mailboxGuid.Equals(Guid.Empty) ? base.MdbGuid : this.mailboxGuid,
				this.eventCounter
			});
		}

		public void LoadAdRawEntry(TenantPartitionHint tenantPartitionHint)
		{
			if (Guid.Empty.Equals(this.mailboxGuid))
			{
				SubmissionInfo.Diag.TraceError(0L, "Mailbox GUID was empty, unable to load AD entry.");
				return;
			}
			try
			{
				IRecipientSession adrecipientSession = this.GetADRecipientSession(tenantPartitionHint);
				this.senderAdEntry = adrecipientSession.FindByExchangeGuidIncludingAlternate<TransportMiniRecipient>(this.mailboxGuid);
				if (this.senderAdEntry != null)
				{
					ADObjectId adobjectId = (ADObjectId)this.senderAdEntry[IADMailStorageSchema.Database];
					if (adobjectId != null)
					{
						ADObjectId adobjectId2 = ADObjectIdResolutionHelper.ResolveDN(adobjectId);
						base.DatabaseName = adobjectId2.Name;
					}
				}
			}
			catch (NonUniqueRecipientException)
			{
				SubmissionInfo.Diag.TraceError<Guid>(0L, "Multiple objects with Mailbox Guid {0} were found.", this.mailboxGuid);
			}
		}

		public string GetSenderEmailAddress()
		{
			if (this.senderAdEntry == null)
			{
				return string.Empty;
			}
			SmtpAddress smtpAddress = (SmtpAddress)this.senderAdEntry[ADRecipientSchema.PrimarySmtpAddress];
			if (smtpAddress.IsValidAddress)
			{
				return smtpAddress.ToString();
			}
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)this.senderAdEntry[ADRecipientSchema.EmailAddresses];
			if (proxyAddressCollection == null || 0 >= proxyAddressCollection.Count)
			{
				return string.Empty;
			}
			ProxyAddress proxyAddress = proxyAddressCollection.Find(new Predicate<ProxyAddress>(this.IsSmtpAddress));
			if (null != proxyAddress)
			{
				return proxyAddress.ToString();
			}
			return proxyAddressCollection[0].ToString();
		}

		public MultiValuedProperty<CultureInfo> GetSenderLocales()
		{
			if (this.senderAdEntry != null)
			{
				return (MultiValuedProperty<CultureInfo>)this.senderAdEntry[ADUserSchema.Languages];
			}
			return new MultiValuedProperty<CultureInfo>();
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

		private IRecipientSession GetADRecipientSession(TenantPartitionHint tenantPartitionHint)
		{
			if (this.recipientSession == null)
			{
				if (tenantPartitionHint == null)
				{
					this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 439, "GetADRecipientSession", "f:\\15.00.1497\\sources\\dev\\MailboxTransport\\src\\StoreDriver\\SubmissionInfo.cs");
				}
				else
				{
					this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantPartitionHint(tenantPartitionHint), 445, "GetADRecipientSession", "f:\\15.00.1497\\sources\\dev\\MailboxTransport\\src\\StoreDriver\\SubmissionInfo.cs");
				}
			}
			return this.recipientSession;
		}

		private bool IsSmtpAddress(ProxyAddress address)
		{
			return address.Prefix.Equals(ProxyAddressPrefix.Smtp);
		}

		protected TransportMiniRecipient senderAdEntry;

		private readonly Guid mailboxGuid;

		private readonly byte[] entryId;

		private readonly byte[] parentEntryId;

		private readonly long eventCounter;

		private IRecipientSession recipientSession;
	}
}
