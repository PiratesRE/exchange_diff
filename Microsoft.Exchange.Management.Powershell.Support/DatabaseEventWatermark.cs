using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public sealed class DatabaseEventWatermark : IConfigurable
	{
		ObjectId IConfigurable.Identity
		{
			get
			{
				return null;
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			return new ValidationError[0];
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.New;
			}
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
			DatabaseEventWatermark databaseEventWatermark = source as DatabaseEventWatermark;
			if (databaseEventWatermark == null)
			{
				throw new NotImplementedException(string.Format("Cannot copy changes from type {0}.", source.GetType()));
			}
			this.watermark = databaseEventWatermark.watermark;
			this.consumerGuid = databaseEventWatermark.consumerGuid;
			this.databaseId = databaseEventWatermark.databaseId;
			this.server = databaseEventWatermark.server;
			this.isDatabaseCopyActive = databaseEventWatermark.isDatabaseCopyActive;
		}

		void IConfigurable.ResetChangeTracking()
		{
		}

		public DatabaseEventWatermark()
		{
		}

		internal DatabaseEventWatermark(Watermark watermark, DatabaseId databaseId, long lastDatabaseCounter, Server server, bool isDatabaseCopyActive)
		{
			this.Instantiate(watermark, databaseId, lastDatabaseCounter, server, isDatabaseCopyActive);
		}

		internal void Instantiate(Watermark watermark, DatabaseId databaseId, long lastDatabaseCounter, Server server, bool isDatabaseCopyActive)
		{
			if (watermark == null)
			{
				throw new ArgumentNullException("watermark");
			}
			if (null == databaseId)
			{
				throw new ArgumentNullException("databaseId");
			}
			this.watermark = watermark;
			this.databaseId = databaseId;
			this.lastDatabaseCounter = lastDatabaseCounter;
			this.server = server;
			this.isDatabaseCopyActive = isDatabaseCopyActive;
		}

		public long Counter
		{
			get
			{
				return this.watermark.EventCounter;
			}
		}

		public long LastDatabaseCounter
		{
			get
			{
				return this.lastDatabaseCounter;
			}
		}

		public Guid ConsumerGuid
		{
			get
			{
				return this.watermark.ConsumerGuid;
			}
		}

		public Guid? MailboxGuid
		{
			get
			{
				if (Guid.Empty != this.watermark.MailboxGuid)
				{
					return new Guid?(this.watermark.MailboxGuid);
				}
				return null;
			}
		}

		public DatabaseId Database
		{
			get
			{
				return this.databaseId;
			}
		}

		public Server Server
		{
			get
			{
				return this.server;
			}
		}

		public bool IsDatabaseCopyActive
		{
			get
			{
				return this.isDatabaseCopyActive;
			}
		}

		private Watermark watermark;

		private Guid consumerGuid;

		private DatabaseId databaseId;

		private long lastDatabaseCounter;

		private Server server;

		private bool isDatabaseCopyActive;
	}
}
