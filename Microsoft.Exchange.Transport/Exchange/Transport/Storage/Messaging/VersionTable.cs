using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal class VersionTable : DataTable
	{
		public VersionTable(DatabaseAutoRecovery databaseAutoRecovery, long requiredVersion, long desiredRevision = 0L)
		{
			this.databaseAutoRecovery = databaseAutoRecovery;
			this.requiredVersion = requiredVersion;
			this.desiredRevision = desiredRevision;
			this.isAttached = false;
		}

		public long DesiredRevision
		{
			get
			{
				return this.desiredRevision;
			}
		}

		public long CurrentVersion
		{
			get
			{
				if (!this.isAttached)
				{
					throw new InvalidOperationException("Version table not attached yet.");
				}
				return this.currentVersion;
			}
		}

		public long CurrentRevision
		{
			get
			{
				if (!this.isAttached)
				{
					throw new InvalidOperationException("Version table not attached yet.");
				}
				return this.currentRevision;
			}
		}

		public void UpgradeRevision(Transaction transaction, long newRevision)
		{
			DataColumn<long> dataColumn = (DataColumn<long>)base.Schemas[1];
			lock (this)
			{
				using (DataTableCursor dataTableCursor = this.OpenCursor(transaction.Connection))
				{
					dataTableCursor.MoveFirst();
					dataTableCursor.PrepareUpdate(true);
					dataColumn.WriteToCursor(dataTableCursor, newRevision);
					dataTableCursor.Update();
					this.currentRevision = newRevision;
				}
			}
			ExTraceGlobals.StorageTracer.TraceDebug<long, long>((long)this.GetHashCode(), "Database upgraded to version: {0} revision:{1}", this.currentVersion, this.currentRevision);
		}

		protected override void AttachLoadInitValues(Transaction transaction, DataTableCursor cursor)
		{
			DataColumn<long> dataColumn = (DataColumn<long>)base.Schemas[0];
			DataColumn<long> dataColumn2 = (DataColumn<long>)base.Schemas[1];
			if (base.IsNewTable)
			{
				cursor.PrepareInsert(false, false);
				dataColumn.WriteToCursor(cursor, this.requiredVersion);
				dataColumn2.WriteToCursor(cursor, this.desiredRevision);
				cursor.Update();
				this.currentVersion = this.requiredVersion;
				this.currentRevision = this.desiredRevision;
				ExTraceGlobals.StorageTracer.TraceDebug<long>((long)this.GetHashCode(), "Database created with version: {0}", this.requiredVersion);
			}
			else
			{
				if (cursor.TryMoveFirst())
				{
					this.currentVersion = dataColumn.ReadFromCursor(cursor);
					this.currentRevision = dataColumn2.ReadFromCursor(cursor);
				}
				ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "Database opened with version: {0} required: {1} revision:{2} desired:{3}", new object[]
				{
					this.currentVersion,
					this.requiredVersion,
					this.currentRevision,
					this.desiredRevision
				});
				if (this.currentVersion != this.requiredVersion)
				{
					string text = string.Empty;
					string text2 = string.Empty;
					try
					{
						text = cursor.Connection.Source.DatabasePath;
						text2 = cursor.Connection.Source.LogFilePath;
						if (this.databaseAutoRecovery != null)
						{
							this.databaseAutoRecovery.SetDatabaseCorruptionFlag();
						}
					}
					catch (ObjectDisposedException)
					{
					}
					Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseSchemaNotSupported, null, new object[]
					{
						Strings.MessagingDatabaseInstanceName,
						this.currentVersion,
						this.requiredVersion,
						text,
						text2
					});
					throw new TransportComponentLoadFailedException(Strings.DatabaseSchemaNotSupported(Strings.MessagingDatabaseInstanceName), new TransientException(Strings.DatabaseSchemaNotSupported(Strings.MessagingDatabaseInstanceName)));
				}
			}
			this.isAttached = true;
		}

		[DataColumnDefinition(typeof(long), ColumnAccess.CachedProp, Required = true)]
		public const int Version = 0;

		[DataColumnDefinition(typeof(long), ColumnAccess.CachedProp, Required = false)]
		public const int Revision = 1;

		private readonly long requiredVersion;

		private readonly long desiredRevision;

		private readonly DatabaseAutoRecovery databaseAutoRecovery;

		private long currentVersion;

		private long currentRevision;

		private bool isAttached;
	}
}
