using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class FailureItemManager : TimerComponent, IServiceComponent
	{
		internal FailureItemManager(IADConfig adConfig) : base(TimeSpan.Zero, TimeSpan.FromSeconds((double)RegistryParameters.FailureItemManagerDatabaseListUpdaterIntervalInSec), "FailureItemManager")
		{
			this.adConfig = adConfig;
		}

		public string Name
		{
			get
			{
				return "Failure Item Manager";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.FailureItemManager;
			}
		}

		public bool IsCritical
		{
			get
			{
				return true;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public bool IsRetriableOnError
		{
			get
			{
				return false;
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		public void Invoke(Action toInvoke)
		{
			toInvoke();
		}

		private IADConfig adConfig { get; set; }

		public new bool Start()
		{
			this.Trace("Starting", new object[0]);
			base.Start();
			this.Trace("Started", new object[0]);
			return true;
		}

		protected override void StopInternal()
		{
			this.Trace("Stopping", new object[0]);
			base.StopInternal();
			foreach (FailureItemWatcher failureItemWatcher in this.m_watcherMap.Values)
			{
				failureItemWatcher.Stop();
				failureItemWatcher.Dispose();
			}
			this.Trace("Stopped", new object[0]);
		}

		protected override void TimerCallbackInternal()
		{
			ExTraceGlobals.FailureItemTracer.TraceFunction(0L, "[FIM] Entering TimerCallbackInternal()");
			IEnumerable<IADDatabase> databasesOnLocalServer = this.adConfig.GetDatabasesOnLocalServer();
			if (databasesOnLocalServer != null)
			{
				Dictionary<Guid, bool> dictionary = new Dictionary<Guid, bool>();
				foreach (Guid key in this.m_watcherMap.Keys)
				{
					dictionary[key] = false;
				}
				foreach (IADDatabase iaddatabase in databasesOnLocalServer)
				{
					Guid guid = iaddatabase.Guid;
					FailureItemWatcher value;
					if (this.m_watcherMap.TryGetValue(guid, out value))
					{
						dictionary[guid] = true;
					}
					else
					{
						value = new FailureItemWatcher(iaddatabase);
						this.m_watcherMap[guid] = value;
						dictionary[guid] = true;
					}
				}
				foreach (Guid guid2 in dictionary.Keys)
				{
					if (!dictionary[guid2])
					{
						this.Trace("Removing database entry for {0} since it no more has copies on this server", new object[]
						{
							guid2
						});
						FailureItemWatcher failureItemWatcher = this.m_watcherMap[guid2];
						failureItemWatcher.Stop(true);
						failureItemWatcher.Dispose();
						this.m_watcherMap.Remove(guid2);
					}
				}
				foreach (Guid key2 in this.m_watcherMap.Keys)
				{
					if (base.PrepareToStopCalled)
					{
						this.Trace("Skipping to start the failure item watchers since failure item manager is stopping", new object[0]);
						break;
					}
					this.m_watcherMap[key2].Start();
				}
			}
			ExTraceGlobals.FailureItemTracer.TraceFunction(0L, "[FIM] Exiting TimerCallbackInternal()");
		}

		private void Trace(string formatString, params object[] args)
		{
			if (ExTraceGlobals.FailureItemTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string formatString2 = "[FIM] " + formatString;
				ExTraceGlobals.FailureItemTracer.TraceDebug(0L, formatString2, args);
			}
		}

		private Dictionary<Guid, FailureItemWatcher> m_watcherMap = new Dictionary<Guid, FailureItemWatcher>();
	}
}
