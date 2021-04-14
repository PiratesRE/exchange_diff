using System;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class FailureItemWatcher : IDisposeTrackable, IDisposable
	{
		internal FailureItemWatcher(IADDatabase database)
		{
			this.m_disposeTracker = this.GetDisposeTracker();
			this.m_database = database;
			this.m_tracePrefix = string.Format("[FIW] ({0}):", database.Guid);
		}

		internal Bookmarker Bookmarker
		{
			get
			{
				return this.m_bookmarker;
			}
		}

		internal IADDatabase Database
		{
			get
			{
				return this.m_database;
			}
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FailureItemWatcher>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.m_disposeTracker != null)
			{
				this.m_disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.TraceEntryExit("Entering Dispose()", new object[0]);
			if (!this.m_fDisposed)
			{
				lock (this)
				{
					if (this.m_disposeTracker != null)
					{
						this.m_disposeTracker.Dispose();
					}
				}
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
			this.TraceEntryExit("Exiting Dispose()", new object[0]);
		}

		public void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_fDisposed)
				{
					if (disposing && this.m_watcher != null)
					{
						this.m_watcher.Dispose();
					}
					this.m_watcher = null;
					this.m_fDisposed = true;
				}
			}
		}

		internal static EventLogQuery CreateEventLogQueryByDatabaseGuid(Guid databaseGuid)
		{
			string query = string.Format("*[UserData/EventXML/DatabaseGuid='{{{0}}}']", databaseGuid);
			return new EventLogQuery(DatabaseFailureItem.ChannelName, PathType.LogName, query);
		}

		internal static DatabaseFailureItem FindMostRecentIdenticalFailureItem(DatabaseFailureItem referenceDbfi, out int countDupliateItems, out EventRecord latestRecord)
		{
			EventLogQuery eventQuery = FailureItemWatcher.CreateEventLogQueryByDatabaseGuid(referenceDbfi.Guid);
			DatabaseFailureItem result;
			using (EventLogReader eventLogReader = new EventLogReader(eventQuery, referenceDbfi.Bookmark))
			{
				latestRecord = null;
				DatabaseFailureItem databaseFailureItem = null;
				countDupliateItems = 0;
				for (;;)
				{
					EventRecord eventRecord2;
					EventRecord eventRecord = eventRecord2 = eventLogReader.ReadEvent();
					try
					{
						if (eventRecord != null)
						{
							DatabaseFailureItem databaseFailureItem2 = DatabaseFailureItem.Parse(eventRecord);
							if (referenceDbfi.Equals(databaseFailureItem2))
							{
								databaseFailureItem = databaseFailureItem2;
								latestRecord = eventRecord;
								countDupliateItems++;
								continue;
							}
						}
					}
					finally
					{
						if (eventRecord2 != null)
						{
							((IDisposable)eventRecord2).Dispose();
						}
					}
					break;
				}
				result = databaseFailureItem;
			}
			return result;
		}

		internal void Stop()
		{
			lock (this.m_locker)
			{
				this.InternalStop(false);
			}
		}

		internal void Stop(bool deleteBookmark)
		{
			lock (this.m_locker)
			{
				this.InternalStop(deleteBookmark);
			}
		}

		private void InternalStop(bool deleteBookmark)
		{
			EventLogWatcher eventLogWatcher = null;
			this.TraceEntryExit("Entering Stop()", new object[0]);
			lock (this.m_locker)
			{
				if (!this.m_isWatcherStarted)
				{
					this.TraceEntryExit("Stop(): Watcher is already stopped", new object[0]);
					return;
				}
				this.m_isEventProcessingEnabled = false;
				this.InternalEnable(false);
				if (this.m_bookmarker != null)
				{
					if (deleteBookmark)
					{
						this.m_bookmarker.Delete();
					}
					this.m_bookmarker.Close();
					this.m_bookmarker = null;
				}
				if (this.m_watcher != null)
				{
					eventLogWatcher = this.m_watcher;
					this.m_watcher = null;
				}
				this.m_isWatcherStarted = false;
			}
			if (eventLogWatcher != null)
			{
				eventLogWatcher.Dispose();
			}
			this.TraceEntryExit("Exiting Stop()", new object[0]);
		}

		internal void Start()
		{
			lock (this.m_locker)
			{
				this.InternalStart();
			}
		}

		private void InternalStart()
		{
			this.TraceEntryExit("Entering Start()", new object[0]);
			if (this.m_isWatcherStarted)
			{
				this.TraceEntryExit("Start(): Watcher is already started", new object[0]);
				return;
			}
			Exception ex = null;
			this.Initialize();
			try
			{
				this.m_watcher = this.CreateWatcher();
				this.m_isEventProcessingEnabled = true;
				this.m_isWatcherStarted = true;
				ThreadPool.QueueUserWorkItem(delegate(object param0)
				{
					this.Enable();
				});
			}
			catch (EventLogException ex2)
			{
				ex = ex2;
				this.Trace("EventLogException() while starting event watcher (error={0}). Will retry again", new object[]
				{
					ex2
				});
			}
			catch (Win32Exception ex3)
			{
				ex = ex3;
				this.Trace("Win32Exception() while starting event watcher (error={0}). Will retry again", new object[]
				{
					ex3
				});
			}
			if (!this.m_isWatcherStarted)
			{
				ReplayCrimsonEvents.FailureItemRegistrationFailed.Log<string, string>(this.m_database.Name, ex.ToString());
			}
			this.TraceEntryExit("Exiting Start()", new object[0]);
		}

		private bool InternalEnable(bool enable)
		{
			bool result = false;
			try
			{
				this.m_watcher.Enabled = enable;
				result = true;
			}
			catch (EventLogException ex)
			{
				this.Trace("Failed to enable watcher. (exception: {0})", new object[]
				{
					ex
				});
			}
			catch (UnauthorizedAccessException ex2)
			{
				this.Trace("Failed to enable watcher. (exception: {0})", new object[]
				{
					ex2
				});
			}
			return result;
		}

		private void Enable()
		{
			this.TraceEntryExit("Entering Enable()", new object[0]);
			lock (this.m_locker)
			{
				if (this.m_isWatcherStarted && this.m_isEventProcessingEnabled)
				{
					if (!this.InternalEnable(true))
					{
						ThreadPool.QueueUserWorkItem(delegate(object param0)
						{
							this.Trace("Failed to enable, so stopping the failure item watcher", new object[0]);
							this.Stop();
						});
					}
				}
				else
				{
					this.Trace("Ignored Enable() (isWatcherStarted={0} isEventProcessingEnabled={1})", new object[]
					{
						this.m_isWatcherStarted,
						this.m_isEventProcessingEnabled
					});
				}
			}
			this.TraceEntryExit("Exiting Enable()", new object[0]);
		}

		private void Restart()
		{
			this.Restart(0);
		}

		private void Restart(int sleepMs)
		{
			lock (this.m_locker)
			{
				this.TraceEntryExit("Entering Restart({0})", new object[]
				{
					sleepMs
				});
				this.InternalStop(false);
				if (sleepMs > 0)
				{
					Thread.Sleep(sleepMs);
				}
				this.InternalStart();
				this.TraceEntryExit("Exiting Restart()", new object[0]);
			}
		}

		private void Initialize()
		{
			if (this.m_bookmarker == null)
			{
				this.m_bookmarker = new Bookmarker(this.m_database.Guid);
			}
			this.m_isWatcherStarted = false;
			this.m_isEventProcessingEnabled = false;
			this.m_watcher = null;
		}

		private EventBookmark SetBookmarkForLatestFailureItem()
		{
			EventBookmark eventBookmark = null;
			EventLogQuery eventLogQuery = FailureItemWatcher.CreateEventLogQueryByDatabaseGuid(this.m_database.Guid);
			eventLogQuery.ReverseDirection = true;
			using (EventLogReader eventLogReader = new EventLogReader(eventLogQuery))
			{
				using (EventRecord eventRecord = eventLogReader.ReadEvent())
				{
					if (eventRecord != null)
					{
						DatabaseFailureItem databaseFailureItem = DatabaseFailureItem.Parse(eventRecord);
						eventBookmark = databaseFailureItem.Bookmark;
						this.Bookmarker.Write(eventBookmark);
					}
				}
			}
			return eventBookmark;
		}

		private EventLogWatcher CreateWatcher()
		{
			EventBookmark eventBookmark = this.Bookmarker.Read();
			if (eventBookmark != null)
			{
				this.Trace("Previous bookmark found", new object[0]);
			}
			else
			{
				this.Trace("No previous book mark found at the first attempt", new object[0]);
				eventBookmark = this.SetBookmarkForLatestFailureItem();
				if (eventBookmark != null)
				{
					this.Trace("Updated the bookmark with the last failure item raised", new object[0]);
				}
				else
				{
					this.Trace("There is no bookmark since there are no failure items in the channel", new object[0]);
				}
			}
			bool readExistingEvents = eventBookmark != null;
			EventLogQuery eventQuery = FailureItemWatcher.CreateEventLogQueryByDatabaseGuid(this.m_database.Guid);
			EventLogWatcher eventLogWatcher = new EventLogWatcher(eventQuery, eventBookmark, readExistingEvents);
			eventLogWatcher.EventRecordWritten += this.EventArrivedHandler;
			return eventLogWatcher;
		}

		private void EventArrivedHandler(object sender, EventRecordWrittenEventArgs arg)
		{
			this.TraceEntryExit("Entering EventArrived handler", new object[0]);
			EventRecord eventRecord = arg.EventRecord;
			bool flag = false;
			do
			{
				if (!this.m_isEventProcessingEnabled)
				{
					if (!ExTraceGlobals.FailureItemTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						break;
					}
					try
					{
						this.Trace("Ignoring Record# {0} since event processing not enabled!!", new object[]
						{
							(eventRecord != null) ? eventRecord.RecordId.ToString() : "<null>"
						});
						break;
					}
					catch (EventLogException)
					{
						break;
					}
				}
				try
				{
					flag = Monitor.TryEnter(this.m_locker, 1000);
					if (flag)
					{
						this.ProcessEvent(eventRecord, arg.EventException);
					}
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(this.m_locker);
					}
					else
					{
						this.Trace("Unable to acquire lock in EventArrivedHandler - retrying", new object[0]);
					}
				}
			}
			while (!flag);
			this.TraceEntryExit("Exiting EventArrived handler", new object[0]);
		}

		private bool IsSkipRecordForEventSuppression(EventRecord rec)
		{
			if (this.m_suppressUntilRecord != null && rec.RecordId <= this.m_suppressUntilRecord.RecordId && rec.TimeCreated <= this.m_suppressUntilRecord.TimeCreated)
			{
				this.Trace("Skipping Record# {0} (Until record is > {1} or creation time is > {2}", new object[]
				{
					rec.RecordId,
					this.m_suppressUntilRecord.RecordId,
					this.m_suppressUntilRecord.TimeCreated
				});
				return true;
			}
			return false;
		}

		private void ProcessEvent(EventRecord rec, Exception eventException)
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3487968573U);
			if (RegistryParameters.FailureItemProcessingDelayInMSec > 0)
			{
				this.Trace("Sleeping for {0}ms before running failure item handler", new object[]
				{
					RegistryParameters.FailureItemProcessingDelayInMSec
				});
				Thread.Sleep(RegistryParameters.FailureItemProcessingDelayInMSec);
			}
			if (rec == null || eventException != null)
			{
				string text = (eventException != null) ? eventException.ToString() : "<no exception>";
				ReplayCrimsonEvents.InconsistentFailureItemEventRecord.Log<string, bool, string>(this.m_database.Name, rec != null, text);
				this.Trace("EventArrivedHandler: Event record might be invalid (isRecordNull={0}, Exception={1})", new object[]
				{
					rec == null,
					text
				});
				this.m_isEventProcessingEnabled = false;
				ThreadPool.QueueUserWorkItem(delegate(object param0)
				{
					this.Restart(3000);
				});
				return;
			}
			bool flag = false;
			try
			{
				if (!this.IsSkipRecordForEventSuppression(rec) && !this.ProcessRecord(rec))
				{
					flag = true;
				}
			}
			catch (EventLogException ex)
			{
				this.Trace("Encountered exception while processing failure item. Exception: {0}", new object[]
				{
					ex.ToString()
				});
				flag = true;
			}
			if (flag)
			{
				this.Trace("Stopping the FailureItemWatcher.", new object[0]);
				this.m_isEventProcessingEnabled = false;
				ThreadPool.QueueUserWorkItem(delegate(object param0)
				{
					this.Stop();
				});
			}
		}

		private bool ProcessRecord(EventRecord rec)
		{
			bool flag = false;
			int num = 0;
			EventRecord suppressUntilRecord = null;
			EventBookmark bookmark = null;
			TagHandler tagHandler = null;
			this.m_suppressUntilRecord = null;
			try
			{
				this.Trace("Processing Record# {0}", new object[]
				{
					rec.RecordId
				});
				bookmark = rec.Bookmark;
				DatabaseFailureItem databaseFailureItem = DatabaseFailureItem.Parse(rec);
				if (databaseFailureItem != null)
				{
					if (databaseFailureItem.Tag == FailureTag.Remount)
					{
						DatabaseFailureItem databaseFailureItem2 = FailureItemWatcher.FindMostRecentIdenticalFailureItem(databaseFailureItem, out num, out suppressUntilRecord);
						if (num > 0)
						{
							this.Trace("Skipped {0} identical failure items of tag {1}", new object[]
							{
								num,
								databaseFailureItem.Tag
							});
							databaseFailureItem = databaseFailureItem2;
						}
					}
					bookmark = databaseFailureItem.Bookmark;
					this.Trace("Starting to process failure item: {0}", new object[]
					{
						databaseFailureItem
					});
					tagHandler = TagHandler.GetInstance(this, databaseFailureItem);
					if (tagHandler != null)
					{
						ExTraceGlobals.FaultInjectionTracer.TraceTest(2640719165U);
						flag = tagHandler.Execute();
						if (flag)
						{
							this.Trace("Finished processing failure item for database '{0}'", new object[]
							{
								this.m_database.Name
							});
						}
						else
						{
							this.Trace("Could not process failure item for database '{0}'", new object[]
							{
								this.m_database.Name
							});
						}
					}
					else
					{
						this.Trace("Failed to process failure item since there is no handler found (failureitem:{0})", new object[]
						{
							databaseFailureItem
						});
						flag = true;
					}
					this.Trace("Finished processing failure item", new object[0]);
				}
			}
			catch (InvalidFailureItemException ex)
			{
				this.Trace("Ignoring the invalid failure item. (error={0}, eventxml={1})", new object[]
				{
					ex,
					rec.ToXml()
				});
				flag = true;
			}
			if (flag)
			{
				this.Bookmarker.Write(bookmark);
				if (num > 0)
				{
					this.m_suppressUntilRecord = suppressUntilRecord;
				}
				if (tagHandler != null && tagHandler.PostProcessingAction != null)
				{
					tagHandler.PostProcessingAction();
				}
			}
			return flag;
		}

		private void Trace(string formatString, params object[] args)
		{
			this.Trace(false, formatString, args);
		}

		private void TraceEntryExit(string formatString, params object[] args)
		{
			this.Trace(true, formatString, args);
		}

		private void Trace(bool isEntryExit, string formatString, params object[] args)
		{
			bool flag = isEntryExit ? ExTraceGlobals.FailureItemTracer.IsTraceEnabled(TraceType.FunctionTrace) : ExTraceGlobals.FailureItemTracer.IsTraceEnabled(TraceType.DebugTrace);
			if (flag)
			{
				string formatString2 = this.m_tracePrefix + formatString;
				if (isEntryExit)
				{
					ExTraceGlobals.FailureItemTracer.TraceFunction(0L, formatString2, args);
					return;
				}
				ExTraceGlobals.FailureItemTracer.TraceDebug(0L, formatString2, args);
			}
		}

		private const int WatcherRestartIntervalInMs = 3000;

		private object m_locker = new object();

		private IADDatabase m_database;

		private string m_tracePrefix;

		private bool m_isWatcherStarted;

		private Bookmarker m_bookmarker;

		private EventLogWatcher m_watcher;

		private bool m_isEventProcessingEnabled;

		private DisposeTracker m_disposeTracker;

		private bool m_fDisposed;

		private EventRecord m_suppressUntilRecord;
	}
}
