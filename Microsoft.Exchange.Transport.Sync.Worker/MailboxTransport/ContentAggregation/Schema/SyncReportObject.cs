using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.Protocols.DeltaSync;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SyncReportObject : ISyncReportObject
	{
		internal SyncReportObject(ISyncObject syncObject, SchemaType schema)
		{
			if (syncObject != null)
			{
				this.Initialize(syncObject, schema);
			}
		}

		internal SyncReportObject(DeltaSyncObject deltaSyncObject, SchemaType schema)
		{
			if (deltaSyncObject != null)
			{
				this.Initialize(deltaSyncObject, schema);
			}
		}

		public string FolderName
		{
			get
			{
				return this.folderName;
			}
		}

		public string Sender
		{
			get
			{
				return this.sender;
			}
		}

		public string Subject
		{
			get
			{
				return this.subject;
			}
		}

		public string MessageClass
		{
			get
			{
				return this.messageClass;
			}
		}

		public int? MessageSize
		{
			get
			{
				return this.messageSize;
			}
		}

		public ExDateTime? DateSent
		{
			get
			{
				return this.dateSent;
			}
		}

		public ExDateTime? DateReceived
		{
			get
			{
				return this.dateReceived;
			}
		}

		private void Initialize(ISyncObject syncObject, SchemaType schema)
		{
			switch (schema)
			{
			case SchemaType.Email:
			{
				ISyncEmail syncEmail = (ISyncEmail)syncObject;
				if (syncEmail != null)
				{
					this.sender = syncEmail.From;
					this.subject = syncEmail.Subject;
					this.messageClass = syncEmail.MessageClass;
					this.messageSize = syncEmail.Size;
					this.dateReceived = syncEmail.ReceivedTime;
					return;
				}
				break;
			}
			case SchemaType.Contact:
			{
				ISyncContact syncContact = (ISyncContact)syncObject;
				if (syncContact != null)
				{
					this.subject = string.Format("{0}_{1}_{2}", syncContact.FirstName, syncContact.MiddleName, syncContact.LastName);
					return;
				}
				break;
			}
			case SchemaType.Folder:
			{
				SyncFolder syncFolder = (SyncFolder)syncObject;
				if (syncFolder != null)
				{
					this.folderName = syncFolder.DisplayName;
					return;
				}
				break;
			}
			default:
				throw new ArgumentException("Unknown schema {0} detected ", schema.ToString());
			}
		}

		private void Initialize(DeltaSyncObject deltaSyncObject, SchemaType schema)
		{
			switch (schema)
			{
			case SchemaType.Email:
			{
				DeltaSyncMail deltaSyncMail = (DeltaSyncMail)deltaSyncObject;
				if (deltaSyncMail != null)
				{
					this.sender = deltaSyncMail.From;
					this.subject = deltaSyncMail.Subject;
					this.messageClass = deltaSyncMail.MessageClass;
					this.messageSize = new int?(deltaSyncMail.Size);
					this.dateReceived = new ExDateTime?(deltaSyncMail.DateReceived);
					return;
				}
				return;
			}
			case SchemaType.Folder:
			{
				DeltaSyncFolder deltaSyncFolder = (DeltaSyncFolder)deltaSyncObject;
				if (deltaSyncFolder != null)
				{
					this.folderName = deltaSyncFolder.DisplayName;
					return;
				}
				return;
			}
			}
			throw new ArgumentException("Unknown schema {0} detected ", schema.ToString());
		}

		private string folderName;

		private string sender;

		private string subject;

		private string messageClass;

		private int? messageSize;

		private ExDateTime? dateSent = null;

		private ExDateTime? dateReceived;
	}
}
