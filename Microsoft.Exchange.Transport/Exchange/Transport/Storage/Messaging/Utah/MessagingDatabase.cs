using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.MessageResubmission;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Utah
{
	internal class MessagingDatabase : IMessagingDatabase
	{
		public MessagingDatabase()
		{
			this.generationTable = new DataGenerationTable(this);
		}

		public void Start()
		{
			this.useAllGenerations = true;
		}

		public DataSource DataSource
		{
			get
			{
				return this.dataSource;
			}
		}

		public QueueTable QueueTable
		{
			get
			{
				return this.queueTable;
			}
		}

		public ServerInfoTable ServerInfoTable
		{
			get
			{
				return this.serverInfoTable;
			}
		}

		internal IMessagingDatabaseConfig Config
		{
			get
			{
				return this.config;
			}
		}

		internal DatabaseAutoRecovery DatabaseAutoRecovery
		{
			get
			{
				return this.databaseAutoRecovery;
			}
		}

		public DataGenerationManager<MessagingGeneration> GenerationManager
		{
			get
			{
				return this.generationManager;
			}
		}

		public string CurrentState
		{
			get
			{
				if (this.databaseOpenTime != TimeSpan.Zero)
				{
					return string.Format("Messaging Database open time {0}", this.databaseOpenTime);
				}
				return "Messaging Database is not open";
			}
		}

		public void Attach(IMessagingDatabaseConfig config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			this.config = config;
			this.useAllGenerations = false;
			MailItemStorage.DefaultAsyncCommitTimeout = config.DefaultAsyncCommitTimeout;
			int i = 10;
			while (i > 0)
			{
				i--;
				try
				{
					this.AttachInternal();
					break;
				}
				catch (EsentFileAccessDeniedException ex)
				{
					if (i <= 0)
					{
						Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseInUse, null, new object[]
						{
							Strings.MessagingDatabaseInstanceName,
							ex
						});
						string notificationReason = string.Format("Database {0} is already in use. The service will be stopped. Exception details: {1}", Strings.MessagingDatabaseInstanceName, ex);
						EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, notificationReason, ResultSeverityLevel.Error, false);
						throw new TransportComponentLoadFailedException(Strings.DataBaseInUse("Messaging Database"), ex);
					}
					Thread.Sleep(1000);
				}
				catch (SchemaException inner)
				{
					throw new TransportComponentLoadFailedException(Strings.DatabaseAttachFailed("Messaging Database"), inner);
				}
			}
		}

		public void Detach()
		{
			this.QueueTable.Detach();
			this.ServerInfoTable.Detach();
			this.GenerationManager.Unload();
			this.generationTable.Detach();
			this.replayRequestTable.Detach();
			this.dataSource.CloseDatabase(false);
			this.dataSource = null;
			if (this.delayedBootloaderComplete != null)
			{
				this.delayedBootloaderComplete.Dispose();
				this.delayedBootloaderComplete = null;
			}
		}

		private void AttachInternal()
		{
			ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, " db path {0}", this.config.DatabasePath);
			ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, " logfile path {0}", this.config.LogFilePath);
			ExTraceGlobals.StorageTracer.TraceDebug<int>(0L, " max connections {0}", this.config.MaxConnections);
			this.databaseAutoRecovery = new DatabaseAutoRecovery(this.config.DatabaseRecoveryAction, "MessagingDatabase", this.config.DatabasePath, this.config.LogFilePath, Strings.MessagingDatabaseInstanceName, "Messaging", 3, null);
			this.databaseAutoRecovery.PerformDatabaseAutoRecoveryIfNeccessary();
			this.dataSource = new DataSource(Strings.MessagingDatabaseInstanceName, this.config.DatabasePath, "mail.que", this.config.MaxConnections, "mail", this.config.LogFilePath, this.databaseAutoRecovery);
			this.dataSource.LogBuffers = this.config.LogBufferSize;
			this.dataSource.LogFileSize = this.config.LogFileSize;
			this.dataSource.MaxBackgroundCleanupTasks = this.config.MaxBackgroundCleanupTasks;
			this.dataSource.ExtensionSize = this.config.ExtensionSize;
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.dataSource.OpenDatabase();
			this.databaseOpenTime = stopwatch.Elapsed;
			stopwatch.Stop();
			if (this.dataSource.NewDatabase)
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, "New database created");
				Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_NewDatabaseCreated, null, new object[]
				{
					"mail.que"
				});
			}
			VersionTable versionTable = new VersionTable(this.DatabaseAutoRecovery, 15L, 1L);
			ExTraceGlobals.StorageTracer.TraceDebug(0L, "attaching tables");
			using (DataConnection dataConnection = this.dataSource.DemandNewConnection())
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, " mail version");
				versionTable.Attach(this.dataSource, dataConnection);
				ExTraceGlobals.StorageTracer.TraceDebug(0L, " generation table");
				this.generationTable.Attach(this.dataSource, dataConnection);
				ExTraceGlobals.StorageTracer.TraceDebug(0L, " replay request table");
				this.replayRequestTable.Attach(this.dataSource, dataConnection);
				ExTraceGlobals.StorageTracer.TraceDebug(0L, " server info");
				this.ServerInfoTable.Attach(this.dataSource, dataConnection);
				ExTraceGlobals.StorageTracer.TraceDebug(0L, " mail queue");
				this.QueueTable.Attach(this.dataSource, dataConnection);
				ExTraceGlobals.StorageTracer.TraceDebug(0L, " done.");
			}
			TimeSpan messagingGenerationLength = this.config.MessagingGenerationLength;
			Func<TimeSpan> ageToCleanup = () => this.config.MessagingGenerationCleanupAge;
			DataGenerationCategory category = DataGenerationCategory.Messaging;
			DataGenerationTable table = this.generationTable;
			int recentGenerationDepth = this.config.RecentGenerationDepth;
			bool autoCreateEnabled = true;
			this.generationManager = new DataGenerationManager<MessagingGeneration>(messagingGenerationLength, ageToCleanup, category, table, recentGenerationDepth, true, autoCreateEnabled, true);
			this.generationManager.GetRecentGenerations();
			this.UpgradeRevision(versionTable);
			versionTable.Detach();
		}

		private void UpgradeRevision(VersionTable version)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			Stopwatch stopwatch2 = new Stopwatch();
			if (version.CurrentRevision < 1L)
			{
				using (DataConnection dataConnection = this.DataSource.DemandNewConnection())
				{
					using (Transaction transaction = dataConnection.BeginTransaction())
					{
						foreach (MessagingGeneration messagingGeneration in this.GenerationManager.GetGenerations(DateTime.MinValue, DateTime.MaxValue))
						{
							using (DataTableCursor dataTableCursor = messagingGeneration.RecipientTable.OpenCursor(dataConnection))
							{
								using (DataTableCursor dataTableCursor2 = messagingGeneration.MessageTable.OpenCursor(dataConnection))
								{
									int num = 0;
									stopwatch2.Restart();
									dataTableCursor2.TryCreateIndex("NdxMessage_DiscardState", "+DiscardState\0\0");
									dataTableCursor.SetCurrentIndex("NdxRecipient_UndeliveredMessageRowId");
									dataTableCursor.MoveBeforeFirst();
									dataTableCursor2.SetCurrentIndex(null);
									DataColumn<byte> dataColumn = (DataColumn<byte>)messagingGeneration.MessageTable.Schemas[22];
									while (dataTableCursor.TryMoveNext(true))
									{
										if (dataTableCursor2.TrySeek(new byte[][]
										{
											BitConverter.GetBytes(messagingGeneration.RecipientTable.Schemas[9].Int32FromIndex(dataTableCursor).Value)
										}))
										{
											dataTableCursor2.PrepareUpdate(true);
											dataColumn.WriteToCursor(dataTableCursor2, 2);
											dataTableCursor2.Update();
											num++;
										}
										transaction.RestartIfStale(100);
									}
									transaction.Checkpoint(TransactionCommitMode.Lazy, 100);
									ExTraceGlobals.StorageTracer.TracePerformance<string, int, TimeSpan>((long)this.GetHashCode(), "Generation {0} upgraded {1} active messages to revision: 1 in {2}", messagingGeneration.Name, num, stopwatch2.Elapsed);
								}
							}
						}
						version.UpgradeRevision(transaction, 1L);
						transaction.Commit(TransactionCommitMode.Lazy);
						ExTraceGlobals.StorageTracer.TracePerformance<TimeSpan>((long)this.GetHashCode(), "Database upgraded to revision: 1 in {0}", stopwatch.Elapsed);
					}
				}
			}
		}

		public IMailRecipientStorage NewRecipientStorage(long messageId)
		{
			MessagingGeneration messagingGeneration = (messageId != 0L) ? this.GetGenerationFromId(messageId) : this.GenerationManager.GetCurrentGeneration();
			if (messagingGeneration == null)
			{
				throw new ArgumentException("The generation of this message no longer exists.", "messageId");
			}
			return new MailRecipientStorage(messagingGeneration.RecipientTable, messageId);
		}

		public IMailItemStorage NewMailItemStorage(bool loadDefaults)
		{
			return new MailItemStorage(this.GenerationManager.GetCurrentGeneration().MessageTable, loadDefaults);
		}

		public IMailItemStorage LoadMailItemFromId(long messageId)
		{
			MessagingGeneration generationFromId = this.GetGenerationFromId(messageId);
			if (generationFromId == null)
			{
				return null;
			}
			IMailItemStorage result;
			using (DataTableCursor cursor = generationFromId.MessageTable.GetCursor())
			{
				using (cursor.BeginTransaction())
				{
					cursor.SetCurrentIndex(null);
					result = (cursor.TrySeek(new byte[][]
					{
						BitConverter.GetBytes((int)messageId)
					}) ? new MailItemStorage(cursor) : null);
				}
			}
			return result;
		}

		public IMailRecipientStorage LoadMailRecipientFromId(long recipientId)
		{
			MessagingGeneration generationFromId = this.GetGenerationFromId(recipientId);
			if (generationFromId == null)
			{
				return null;
			}
			IMailRecipientStorage result;
			using (DataTableCursor cursor = generationFromId.RecipientTable.GetCursor())
			{
				using (cursor.BeginTransaction())
				{
					cursor.SetCurrentIndex(null);
					result = this.LoadMailRecipientFromRowId(MessagingGeneration.GetRowId(recipientId), cursor);
				}
			}
			return result;
		}

		public IEnumerable<IMailRecipientStorage> LoadMailRecipientsFromMessageId(long messageId)
		{
			MessagingGeneration generation = this.GetGenerationFromId(messageId);
			if (generation != null)
			{
				using (DataTableCursor cursor = generation.RecipientTable.GetCursor())
				{
					using (cursor.BeginTransaction())
					{
						int messageRowId = MessagingGeneration.GetRowId(messageId);
						cursor.SetCurrentIndex("NdxRecipient_UndeliveredMessageRowId");
						if (cursor.TrySeek(new byte[][]
						{
							BitConverter.GetBytes(messageRowId)
						}) && cursor.TrySetIndexUpperRange(new byte[][]
						{
							BitConverter.GetBytes(messageRowId)
						}))
						{
							do
							{
								yield return new MailRecipientStorage(cursor);
							}
							while (cursor.TryMoveNext(false));
							cursor.TryRemoveIndexRange();
						}
					}
				}
			}
			yield break;
		}

		public IEnumerable<IMailRecipientStorage> LoadMailRecipientsFromCursor(DataTableCursor cursor)
		{
			if (cursor == null)
			{
				throw new ArgumentNullException("cursor");
			}
			if (cursor.HasData())
			{
				do
				{
					yield return new MailRecipientStorage(cursor);
				}
				while (cursor.TryMoveNext(false));
			}
			yield break;
		}

		public IMailRecipientStorage LoadMailRecipientFromRowId(int recipientRowId, DataTableCursor cursor)
		{
			if (cursor == null)
			{
				throw new ArgumentNullException("cursor");
			}
			if (!cursor.TrySeek(new byte[][]
			{
				BitConverter.GetBytes(recipientRowId)
			}))
			{
				return null;
			}
			return new MailRecipientStorage(cursor);
		}

		public Transaction BeginNewTransaction()
		{
			DataConnection dataConnection = this.DataSource.DemandNewConnection();
			Transaction result = dataConnection.BeginTransaction();
			dataConnection.Release();
			return result;
		}

		public MessagingDatabaseResultStatus ReadUnprocessedMessageIds(out Dictionary<byte, List<long>> unprocessedMessageIds)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			MessagingDatabaseResultStatus result;
			if (this.useAllGenerations)
			{
				unprocessedMessageIds = this.GetUnprocessedMessageIdsInGenerations(this.generationManager.GetGenerations(DateTime.MinValue, DateTime.MaxValue));
				result = MessagingDatabaseResultStatus.Complete;
			}
			else
			{
				unprocessedMessageIds = this.GetUnprocessedMessageIdsInGenerations(this.generationManager.GetRecentGenerations());
				result = MessagingDatabaseResultStatus.Partial;
			}
			stopwatch.Stop();
			ExTraceGlobals.StorageTracer.TracePerformance<int, TimeSpan>(0L, "ReadUnprocessedMessageIds retrieved {0} bookmarks in {1}", unprocessedMessageIds.Values.Sum((List<long> i) => i.Count<long>()), stopwatch.Elapsed);
			return result;
		}

		public IEnumerable<MailItemAndRecipients> GetMessages(List<long> messageIds)
		{
			Transaction transaction = null;
			DataTableCursor messageCursor = null;
			DataTableCursor recipientCursor = null;
			Stopwatch stopwatch = Stopwatch.StartNew();
			int messageCount = 0;
			int generationCount = 0;
			try
			{
				transaction = this.BeginNewTransaction();
				MessagingGeneration currentGeneration = null;
				for (int i = 0; i < messageIds.Count; i++)
				{
					if (messageIds[i] != 0L)
					{
						int generationId = MessagingGeneration.GetGenerationId(messageIds[i]);
						int messageId = MessagingGeneration.GetRowId(messageIds[i]);
						if (currentGeneration == null || currentGeneration.GenerationId != generationId)
						{
							if (messageCursor != null)
							{
								messageCursor.Close();
							}
							if (recipientCursor != null)
							{
								recipientCursor.Close();
							}
							currentGeneration = this.GenerationManager.GetGeneration(generationId);
							messageCursor = currentGeneration.MessageTable.OpenCursor(transaction.Connection);
							messageCursor.SetCurrentIndex(null);
							recipientCursor = currentGeneration.RecipientTable.OpenCursor(transaction.Connection);
							recipientCursor.SetCurrentIndex("NdxRecipient_UndeliveredMessageRowId");
							generationCount++;
						}
						if (messageCursor.TrySeek(new byte[][]
						{
							BitConverter.GetBytes(messageId)
						}))
						{
							bool foundRecipients = recipientCursor.TrySeek(new byte[][]
							{
								BitConverter.GetBytes(messageId)
							}) && recipientCursor.TrySetIndexUpperRange(new byte[][]
							{
								BitConverter.GetBytes(messageId)
							});
							stopwatch.Stop();
							yield return new MailItemAndRecipients(new MailItemStorage(messageCursor), foundRecipients ? this.LoadMailRecipientsFromCursor(recipientCursor) : Enumerable.Empty<IMailRecipientStorage>());
							stopwatch.Start();
							if (foundRecipients)
							{
								recipientCursor.TryRemoveIndexRange();
							}
							messageIds[i] = 0L;
							messageCount++;
							transaction.RestartIfStale(this.Config.MaxMessageLoadTimePercentage);
						}
					}
				}
			}
			finally
			{
				if (recipientCursor != null)
				{
					recipientCursor.Dispose();
				}
				if (messageCursor != null)
				{
					messageCursor.Dispose();
				}
				if (transaction != null)
				{
					transaction.Dispose();
				}
				stopwatch.Stop();
				ExTraceGlobals.StorageTracer.TraceDebug<int, int, TimeSpan>(0L, "GetMessages returned {0} messages across {1} generations in {2}", messageCount, generationCount, stopwatch.Elapsed);
				ExTraceGlobals.StorageTracer.TracePerformance<int, int, TimeSpan>(0L, "GetMessages returned {0} messages across {1} generations in {2}", messageCount, generationCount, stopwatch.Elapsed);
			}
			yield break;
		}

		public IReplayRequest NewReplayRequest(Guid correlationId, Destination destination, DateTime startTime, DateTime endTime, bool isTestRequest = false)
		{
			return new ReplayRequest(this, new ReplayRequestStorage(this.replayRequestTable, destination, startTime, endTime, correlationId, isTestRequest));
		}

		public IEnumerable<IReplayRequest> GetAllReplayRequests()
		{
			return (from storage in this.replayRequestTable.GetAllRows()
			select new ReplayRequest(this, storage)).ToArray<ReplayRequest>();
		}

		public List<MailItemAndRecipients> GetDeliveredMessages(IEnumerable<Tuple<MessagingGeneration, IGrouping<int, int>>> bookmarks, ref long continuationKey, string conditions = null)
		{
			List<MailItemAndRecipients> list = new List<MailItemAndRecipients>();
			ResubmitFilter resubmitFilter = null;
			if (!string.IsNullOrEmpty(conditions) && !ResubmitFilter.TryBuild(conditions, out resubmitFilter))
			{
				return new List<MailItemAndRecipients>();
			}
			using (Transaction transaction = this.BeginNewTransaction())
			{
				foreach (IGrouping<MessagingGeneration, IGrouping<int, int>> grouping in from i in bookmarks
				group i.Item2 by i.Item1)
				{
					if (grouping.Key.TryEnterReplay())
					{
						try
						{
							using (DataTableCursor dataTableCursor = grouping.Key.MessageTable.OpenCursor(transaction.Connection))
							{
								using (DataTableCursor dataTableCursor2 = grouping.Key.RecipientTable.OpenCursor(transaction.Connection))
								{
									foreach (IGrouping<int, int> grouping2 in grouping)
									{
										dataTableCursor.Seek(new byte[][]
										{
											BitConverter.GetBytes(grouping2.Key)
										});
										if (resubmitFilter == null || !resubmitFilter.FromAddressChecking || resubmitFilter.ValidateStringParam(ResubmitFilter.FilterParameterType.FromAddress, dataTableCursor.Table.Schemas[15].StringFromCursor(dataTableCursor)))
										{
											List<IMailRecipientStorage> list2 = new List<IMailRecipientStorage>();
											foreach (int recipientRowId in grouping2)
											{
												list2.Add(this.LoadMailRecipientFromRowId(recipientRowId, dataTableCursor2));
											}
											MailItemAndRecipients item = new MailItemAndRecipients(new MailItemStorage(dataTableCursor), list2);
											list.Add(item);
										}
										int? num = dataTableCursor.Table.Schemas[0].Int32FromCursor(dataTableCursor);
										if (num != null)
										{
											continuationKey = grouping.Key.MessageTable.Generation.CombineIds(num.Value);
										}
										transaction.RestartIfStale(100);
									}
								}
							}
						}
						finally
						{
							grouping.Key.TryExitReplay();
						}
					}
				}
			}
			return list;
		}

		public IEnumerable<Tuple<MessagingGeneration, IGrouping<int, int>>> GetDeliveredBookmarks(Destination destination, DateTime startDate, DateTime endDate, long continuationKey)
		{
			foreach (MessagingGeneration gen2 in (from gen in this.GenerationManager.GetGenerations(this.AdjustStartDateForMaxDeliveryLag(startDate), endDate)
			where gen.GenerationId <= MessagingGeneration.GetGenerationId(continuationKey)
			select gen).Reverse<MessagingGeneration>())
			{
				SortedSet<Tuple<int, int>> deliveryBookmarks = new SortedSet<Tuple<int, int>>();
				using (Transaction transaction = this.BeginNewTransaction())
				{
					using (DataTableCursor dataTableCursor = gen2.RecipientTable.OpenCursor(transaction.Connection))
					{
						Stopwatch stopwatch = Stopwatch.StartNew();
						int num = 0;
						dataTableCursor.SetCurrentIndex("NdxRecipient_DestinationHash_DeliveryTimeOffset");
						if (gen2.TryEnterReplay())
						{
							if (dataTableCursor.TrySeekGE(new byte[][]
							{
								BitConverter.GetBytes(destination.GetHashCode()),
								BitConverter.GetBytes(MailRecipientStorage.GetTimeOffset(startDate)),
								BitConverter.GetBytes(-2147483648)
							}) && dataTableCursor.TrySetIndexUpperRange(new byte[][]
							{
								BitConverter.GetBytes(destination.GetHashCode()),
								BitConverter.GetBytes(MailRecipientStorage.GetTimeOffset(endDate)),
								BitConverter.GetBytes(2147483647)
							}))
							{
								do
								{
									int value = gen2.RecipientTable.Schemas[1].Int32FromIndex(dataTableCursor).Value;
									int value2 = gen2.RecipientTable.Schemas[0].Int32FromBookmark(dataTableCursor).Value;
									if (gen2.GenerationId != MessagingGeneration.GetGenerationId(continuationKey) || value > MessagingGeneration.GetRowId(continuationKey))
									{
										deliveryBookmarks.Add(Tuple.Create<int, int>(value, value2));
										num++;
									}
								}
								while (dataTableCursor.TryMoveNext(false));
							}
							gen2.TryExitReplay();
							stopwatch.Stop();
							ExTraceGlobals.StorageTracer.TracePerformance<int, int, TimeSpan>((long)gen2.GenerationId, "GetDelivered bookmarks for this generation returned {0} messages, {1} recipients and took {2}", deliveryBookmarks.Count, num, stopwatch.Elapsed);
							this.perfCounters.ReplayBookmarkAverageLatency.IncrementBy(stopwatch.ElapsedTicks);
							this.perfCounters.ReplayBookmarkAverageLatencyBase.IncrementBy((long)num);
						}
					}
				}
				foreach (IGrouping<int, int> messageRecipGroup in from i in deliveryBookmarks
				group i.Item2 by i.Item1)
				{
					yield return Tuple.Create<MessagingGeneration, IGrouping<int, int>>(gen2, messageRecipGroup);
				}
			}
			yield break;
		}

		public IEnumerable<Tuple<MessagingGeneration, IGrouping<int, int>>> GetConditionalDeliveredBookmarks(DateTime startDate, DateTime endDate, Destination destination, long continuationKey, string conditions)
		{
			ResubmitFilter filter = null;
			if (!string.IsNullOrEmpty(conditions) && ResubmitFilter.TryBuild(conditions, out filter))
			{
				foreach (MessagingGeneration gen2 in (from gen in this.GenerationManager.GetGenerations(this.AdjustStartDateForMaxDeliveryLag(startDate), endDate)
				where gen.GenerationId <= MessagingGeneration.GetGenerationId(continuationKey)
				select gen).Reverse<MessagingGeneration>())
				{
					SortedSet<Tuple<int, int>> deliveryBookmarks = new SortedSet<Tuple<int, int>>();
					using (Transaction transaction = this.BeginNewTransaction())
					{
						using (DataTableCursor dataTableCursor = gen2.RecipientTable.OpenCursor(transaction.Connection))
						{
							Stopwatch stopwatch = Stopwatch.StartNew();
							int num = 0;
							dataTableCursor.SetCurrentIndex(null);
							if (gen2.TryEnterReplay())
							{
								if (dataTableCursor.TryMoveFirst())
								{
									do
									{
										int value = gen2.RecipientTable.Schemas[1].Int32FromCursor(dataTableCursor).Value;
										int value2 = gen2.RecipientTable.Schemas[0].Int32FromIndex(dataTableCursor).Value;
										if (gen2.GenerationId != MessagingGeneration.GetGenerationId(continuationKey) || value > MessagingGeneration.GetRowId(continuationKey))
										{
											byte[] array = gen2.RecipientTable.Schemas[10].BytesFromCursor(dataTableCursor, false, 1);
											if (array != null && array[0] == 1 && (!filter.ToAddressChecking || filter.ValidateStringParam(ResubmitFilter.FilterParameterType.ToAddress, gen2.RecipientTable.Schemas[12].StringFromCursor(dataTableCursor))))
											{
												deliveryBookmarks.Add(Tuple.Create<int, int>(value, value2));
												num++;
											}
										}
									}
									while (dataTableCursor.TryMoveNext(false));
								}
								gen2.TryExitReplay();
								stopwatch.Stop();
								ExTraceGlobals.StorageTracer.TracePerformance<int, int, TimeSpan>((long)gen2.GenerationId, "GetDelivered bookmarks for this generation returned {0} messages, {1} recipients and took {2}", deliveryBookmarks.Count, num, stopwatch.Elapsed);
								this.perfCounters.ReplayBookmarkAverageLatency.IncrementBy(stopwatch.ElapsedTicks);
								this.perfCounters.ReplayBookmarkAverageLatencyBase.IncrementBy((long)num);
							}
						}
					}
					foreach (IGrouping<int, int> messageRecipGroup in from i in deliveryBookmarks
					group i.Item2 by i.Item1)
					{
						yield return Tuple.Create<MessagingGeneration, IGrouping<int, int>>(gen2, messageRecipGroup);
					}
				}
			}
			yield break;
		}

		public XElement GetDiagnosticInfo(string argument)
		{
			if (argument.Equals("Database", StringComparison.InvariantCultureIgnoreCase))
			{
				return new XElement("DatabaseOpenTime", this.databaseOpenTime);
			}
			if (argument.Equals("Generations", StringComparison.InvariantCultureIgnoreCase))
			{
				return this.GenerationManager.GetDiagnosticInfo(argument);
			}
			if (argument.StartsWith("Transaction", StringComparison.InvariantCultureIgnoreCase))
			{
				return Transaction.GetDiagnosticInfo(argument);
			}
			return new XElement("UtahMessagingDatabase", new XElement("help", "Supported arguments: config, Database, Generations, TransactionsOpen, TransactionStartTrace, TransactionTrace, TransactionPercentile"));
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return this.GetDiagnosticInfo(parameters.Argument);
		}

		internal MessagingGeneration GetGenerationFromId(long id)
		{
			return this.GenerationManager.GetGeneration(MessagingGeneration.GetGenerationId(id));
		}

		public void SuspendDataCleanup()
		{
			this.generationManager.SuspendDataCleanup();
		}

		public void SuspendDataCleanup(DateTime startDate, DateTime endDate)
		{
			this.generationManager.SuspendDataCleanup(this.AdjustStartDateForMaxDeliveryLag(startDate), endDate);
		}

		public void ResumeDataCleanup()
		{
			this.generationManager.ResumeDataCleanup();
		}

		public void ResumeDataCleanup(DateTime startDate, DateTime endDate)
		{
			this.generationManager.ResumeDataCleanup(this.AdjustStartDateForMaxDeliveryLag(startDate), endDate);
		}

		public void BootLoadCompleted()
		{
			this.ResumeDataCleanup();
			if (this.databaseAutoRecovery.GetDatabaseCorruptionCount() > 0)
			{
				this.delayedBootloaderComplete = new Timer(delegate(object o)
				{
					this.databaseAutoRecovery.ResetDatabaseCorruptionFlag();
				}, null, TimeSpan.FromMinutes(5.0), TimeSpan.FromMilliseconds(-1.0));
			}
		}

		internal DateTime AdjustStartDateForMaxDeliveryLag(DateTime originalStartDate)
		{
			TimeSpan t = this.config.MessagingGenerationExpirationAge - this.config.MessagingGenerationCleanupAge;
			if (!(originalStartDate <= DateTime.MinValue + t))
			{
				return originalStartDate - t;
			}
			return DateTime.MinValue;
		}

		private Dictionary<byte, List<long>> GetUnprocessedMessageIdsInGenerations(IEnumerable<MessagingGeneration> generations)
		{
			Dictionary<byte, List<long>> dictionary = new Dictionary<byte, List<long>>();
			foreach (MessageTable.MailPriorityAndId mailPriorityAndId in generations.SelectMany((MessagingGeneration gen) => gen.MessageTable.GetLeftoverPendingMessageIds()))
			{
				List<long> list;
				if (!dictionary.TryGetValue(mailPriorityAndId.Priority, out list))
				{
					list = (dictionary[mailPriorityAndId.Priority] = new List<long>());
				}
				list.Add(mailPriorityAndId.MessageId);
			}
			return dictionary;
		}

		public const string PerfCounterInstanceName = "mail";

		private const string DefaultDatabaseFileName = "mail.que";

		private readonly QueueTable queueTable = new QueueTable();

		private readonly ServerInfoTable serverInfoTable = new ServerInfoTable();

		private readonly DataGenerationTable generationTable;

		private readonly ReplayRequestTable replayRequestTable = new ReplayRequestTable();

		private readonly DatabasePerfCountersInstance perfCounters = DatabasePerfCounters.GetInstance("other");

		private Timer delayedBootloaderComplete;

		private DataGenerationManager<MessagingGeneration> generationManager;

		private IMessagingDatabaseConfig config;

		private DataSource dataSource;

		private DatabaseAutoRecovery databaseAutoRecovery;

		private TimeSpan databaseOpenTime;

		private bool useAllGenerations;
	}
}
