using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.Protocols.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncReportObject : ISyncReportObject
	{
		internal DeltaSyncReportObject(DeltaSyncMail deltaSyncMail)
		{
			SyncUtilities.ThrowIfArgumentNull("deltaSyncMail", deltaSyncMail);
			this.deltaSyncMail = deltaSyncMail;
		}

		public string FolderName
		{
			get
			{
				return null;
			}
		}

		public string Sender
		{
			get
			{
				return this.deltaSyncMail.From;
			}
		}

		public string Subject
		{
			get
			{
				return this.deltaSyncMail.Subject;
			}
		}

		public string MessageClass
		{
			get
			{
				return this.deltaSyncMail.MessageClass;
			}
		}

		public int? MessageSize
		{
			get
			{
				return new int?(this.deltaSyncMail.Size);
			}
		}

		public ExDateTime? DateSent
		{
			get
			{
				return null;
			}
		}

		public ExDateTime? DateReceived
		{
			get
			{
				return new ExDateTime?(this.deltaSyncMail.DateReceived);
			}
		}

		private DeltaSyncMail deltaSyncMail;
	}
}
