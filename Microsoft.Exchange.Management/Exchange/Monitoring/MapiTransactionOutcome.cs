using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class MapiTransactionOutcome : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MapiTransactionOutcome.schema;
			}
		}

		public string Server
		{
			get
			{
				return (string)this[MapiTransactionOutcomeSchema.Server];
			}
			internal set
			{
				this[MapiTransactionOutcomeSchema.Server] = value;
			}
		}

		public string Database
		{
			get
			{
				return (string)this[MapiTransactionOutcomeSchema.Database];
			}
			internal set
			{
				this[MapiTransactionOutcomeSchema.Database] = value;
			}
		}

		public string Mailbox
		{
			get
			{
				return (string)this[MapiTransactionOutcomeSchema.Mailbox];
			}
			internal set
			{
				this[MapiTransactionOutcomeSchema.Mailbox] = value;
			}
		}

		public Guid? MailboxGuid
		{
			get
			{
				return (Guid?)this[MapiTransactionOutcomeSchema.MailboxGuid];
			}
			internal set
			{
				this[MapiTransactionOutcomeSchema.MailboxGuid] = value;
			}
		}

		public bool? IsArchive
		{
			get
			{
				return (bool?)this[MapiTransactionOutcomeSchema.IsArchive];
			}
			internal set
			{
				this[MapiTransactionOutcomeSchema.IsArchive] = value;
			}
		}

		public bool IsDatabaseCopyActive
		{
			get
			{
				return (bool)this[MapiTransactionOutcomeSchema.IsDatabaseCopyActive];
			}
			internal set
			{
				this[MapiTransactionOutcomeSchema.IsDatabaseCopyActive] = value;
			}
		}

		public MapiTransactionResult Result
		{
			get
			{
				return (MapiTransactionResult)this[MapiTransactionOutcomeSchema.Result];
			}
			internal set
			{
				this[MapiTransactionOutcomeSchema.Result] = value;
			}
		}

		public TimeSpan Latency
		{
			get
			{
				return (TimeSpan)(this[MapiTransactionOutcomeSchema.Latency] ?? TimeSpan.Zero);
			}
			internal set
			{
				this[MapiTransactionOutcomeSchema.Latency] = value;
			}
		}

		public string Error
		{
			get
			{
				return (string)this.propertyBag[MapiTransactionOutcomeSchema.Error];
			}
			internal set
			{
				this.propertyBag[MapiTransactionOutcomeSchema.Error] = value;
			}
		}

		public MapiTransactionOutcome() : base(new SimpleProviderPropertyBag())
		{
		}

		public MapiTransactionOutcome(Server server, Database database, ADRecipient adRecipient) : base(new SimpleProviderPropertyBag())
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (database == null)
			{
				throw new ArgumentNullException("database");
			}
			this.Server = (server.Name ?? string.Empty);
			this.Database = (database.Name ?? string.Empty);
			this.Mailbox = ((adRecipient == null) ? null : (adRecipient.Name ?? null));
			this.Result = new MapiTransactionResult(MapiTransactionResultEnum.Undefined);
			this.transactionTarget = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", new object[]
			{
				this.Server,
				this.Database
			});
		}

		internal void Update(MapiTransactionResultEnum resultEnum, TimeSpan latency, string error, Guid? mailboxGuid, MailboxMiscFlags? mailboxMiscFlags, bool isDatabaseCopyActive)
		{
			lock (this.thisLock)
			{
				this.Result = new MapiTransactionResult(resultEnum);
				this.Latency = latency;
				this.MailboxGuid = mailboxGuid;
				this.IsArchive = ((mailboxMiscFlags != null) ? new bool?((mailboxMiscFlags & MailboxMiscFlags.ArchiveMailbox) == MailboxMiscFlags.ArchiveMailbox) : null);
				this.IsDatabaseCopyActive = isDatabaseCopyActive;
				this.Error = (error ?? string.Empty);
			}
		}

		internal string ShortTargetString()
		{
			return this.transactionTarget;
		}

		internal string LongTargetString()
		{
			return this.transactionTarget + "\\" + (this.Mailbox ?? string.Empty);
		}

		private const string targetItemSeparator = "\\";

		private const string transactionTargetFormatStr = "{0}\\{1}";

		private readonly string transactionTarget;

		[NonSerialized]
		private object thisLock = new object();

		private static MapiTransactionOutcomeSchema schema = ObjectSchema.GetInstance<MapiTransactionOutcomeSchema>();
	}
}
