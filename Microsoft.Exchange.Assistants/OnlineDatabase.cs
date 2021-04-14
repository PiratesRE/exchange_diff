using System;
using Microsoft.Exchange.Diagnostics.Components.Assistants;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class OnlineDatabase : Base, IDisposable
	{
		public DatabaseInfo DatabaseInfo
		{
			get
			{
				return this.databaseInfo;
			}
		}

		public bool RestartRequired
		{
			get
			{
				return this.eventController != null && this.eventController.RestartRequired;
			}
		}

		public EventController EventController
		{
			get
			{
				return this.eventController;
			}
		}

		public OnlineDatabase(DatabaseInfo databaseInfo, DatabaseManager databaseManager)
		{
			this.databaseInfo = databaseInfo;
			this.databaseManager = databaseManager;
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = "OnlineDatabase for database '" + this.databaseInfo.DisplayName + "'";
			}
			return this.toString;
		}

		public void Dispose()
		{
			if (this.eventController != null)
			{
				this.eventController.Dispose();
				this.eventController = null;
			}
			this.DisposePerformanceCounters();
		}

		public void Start()
		{
			ExTraceGlobals.OnlineDatabaseTracer.TraceDebug<OnlineDatabase>((long)this.GetHashCode(), "{0}: Starting", this);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			EventBasedAssistantCollection eventBasedAssistantCollection = null;
			try
			{
				PoisonEventControl poisonControl = new PoisonEventControl(this.databaseManager.PoisonControlMaster, this.DatabaseInfo);
				PoisonMailboxControl poisonControl2 = new PoisonMailboxControl(this.databaseManager.PoisonControlMaster, this.DatabaseInfo);
				string text = this.databaseManager.ServiceName + "-" + this.DatabaseInfo.DatabaseName;
				ExTraceGlobals.OnlineDatabaseTracer.TraceDebug<OnlineDatabase, string>((long)this.GetHashCode(), "{0}: Creating performance counters instance {1}", this, text);
				this.performanceCounters = new PerformanceCountersPerDatabaseInstance(text, this.databaseManager.PerformanceCountersTotal);
				this.performanceCounters.Reset();
				if (this.databaseManager.AssistantTypes != null)
				{
					eventBasedAssistantCollection = EventBasedAssistantCollection.Create(this.databaseInfo, this.databaseManager.AssistantTypes);
					if (eventBasedAssistantCollection != null)
					{
						if (this.databaseInfo.IsPublic)
						{
							this.eventController = new EventControllerPublic(this.databaseInfo, eventBasedAssistantCollection, poisonControl, this.performanceCounters, this.databaseManager.EventGovernor);
						}
						else
						{
							this.eventController = new EventControllerPrivate(this.databaseInfo, eventBasedAssistantCollection, poisonControl, this.performanceCounters, this.databaseManager.EventGovernor);
						}
						eventBasedAssistantCollection = null;
						this.eventController.Start();
						flag2 = true;
					}
				}
				if (!this.databaseInfo.IsPublic && this.databaseManager.TimeBasedDriverManager != null)
				{
					this.databaseManager.TimeBasedDriverManager.StartDatabase(this.databaseInfo, poisonControl2, this.performanceCounters);
					flag3 = true;
				}
				flag = true;
			}
			finally
			{
				if (eventBasedAssistantCollection != null)
				{
					eventBasedAssistantCollection.Dispose();
				}
				if (!flag)
				{
					ExTraceGlobals.OnlineDatabaseTracer.TraceError<OnlineDatabase>((long)this.GetHashCode(), "{0}: unable to start", this);
					if (this.eventController != null)
					{
						if (flag2)
						{
							this.eventController.Stop();
						}
						this.eventController.Dispose();
						this.eventController = null;
					}
				}
				if (!flag3 && !flag2)
				{
					this.DisposePerformanceCounters();
				}
			}
			base.TracePfd("PFS AIS {0} {1}: Started", new object[]
			{
				20567,
				this
			});
		}

		public void RequestStop(HangDetector hangDetector)
		{
			ExTraceGlobals.OnlineDatabaseTracer.TraceDebug<OnlineDatabase>((long)this.GetHashCode(), "{0}: Requesting stop", this);
			if (this.eventController != null)
			{
				this.eventController.RequestStop(hangDetector);
			}
			if (this.databaseManager.TimeBasedDriverManager != null)
			{
				this.databaseManager.TimeBasedDriverManager.RequestStopDatabase(this.databaseInfo.Guid, hangDetector);
			}
			base.TracePfd("PFD AIS {0} {1}: Stop requested", new object[]
			{
				28759,
				this
			});
		}

		public void WaitUntilStopped()
		{
			ExTraceGlobals.OnlineDatabaseTracer.TraceDebug<OnlineDatabase>((long)this.GetHashCode(), "{0}: Waiting until stopped", this);
			if (this.eventController != null)
			{
				this.eventController.WaitUntilStopped();
			}
			if (this.databaseManager.TimeBasedDriverManager != null)
			{
				this.databaseManager.TimeBasedDriverManager.WaitUntilStoppedDatabase(this.databaseInfo.Guid);
			}
			this.DisposePerformanceCounters();
			base.TracePfd("PFD AIS {0} {1}: Stopped", new object[]
			{
				20055,
				this
			});
		}

		public void Stop(HangDetector hangDetector)
		{
			this.RequestStop(hangDetector);
			this.WaitUntilStopped();
		}

		public override void ExportToQueryableObject(QueryableObject queryableObject)
		{
			base.ExportToQueryableObject(queryableObject);
			QueryableOnlineDatabase queryableOnlineDatabase = queryableObject as QueryableOnlineDatabase;
			if (queryableOnlineDatabase != null)
			{
				queryableOnlineDatabase.DatabaseName = this.databaseInfo.DatabaseName;
				queryableOnlineDatabase.DatabaseGuid = this.databaseInfo.Guid;
				queryableOnlineDatabase.RestartRequired = this.RestartRequired;
				QueryableEventController queryableObject2 = new QueryableEventController();
				this.eventController.ExportToQueryableObject(queryableObject2);
				queryableOnlineDatabase.EventController = queryableObject2;
			}
		}

		private void DisposePerformanceCounters()
		{
			if (this.performanceCounters != null)
			{
				ExTraceGlobals.OnlineDatabaseTracer.TraceDebug<OnlineDatabase, string>((long)this.GetHashCode(), "{0}: Removing performance counters instance {1}", this, this.performanceCounters.Name);
				this.performanceCounters.Reset();
				this.performanceCounters.Close();
				this.performanceCounters.Remove();
				this.performanceCounters = null;
			}
		}

		private EventController eventController;

		private string toString;

		private DatabaseInfo databaseInfo;

		private DatabaseManager databaseManager;

		private PerformanceCountersPerDatabaseInstance performanceCounters;
	}
}
