using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common.Logging
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WatsonReporter
	{
		internal WatsonReporter()
		{
		}

		public void ReportWatson(SyncLogSession syncLogSession, string message, Exception exception)
		{
			string text = null;
			this.GetTransportSyncWatsonException(exception, message, out text);
			int hashCode = text.GetHashCode();
			bool flag = true;
			lock (this.syncRoot)
			{
				if (!this.watsonInstances.ContainsKey(hashCode))
				{
					this.watsonInstances.Add(hashCode, new WatsonReporter.WatsonCallstack(message, text, () => this.GetCurrentTime()));
				}
				else if (this.watsonInstances[hashCode].FirstSeenTime + WatsonReporter.ThrottlingPeriod < this.GetCurrentTime())
				{
					this.watsonInstances[hashCode].Reset(syncLogSession);
				}
				else
				{
					this.watsonInstances[hashCode].IncrementCount();
					if (this.watsonInstances[hashCode].CountSinceFirstSeen > WatsonReporter.MaxWatsonsPerCallstackPerThrottlingPeriod)
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				syncLogSession.LogError((TSLID)229UL, "ReportException: {0} with hash {1} and inner exception {2}.", new object[]
				{
					message,
					hashCode,
					exception
				});
			}
		}

		protected virtual void SendNonTerminatingWatsonReport(ReportTransportSyncWatsonException reportException, string extraData)
		{
			ExWatson.SendReport(reportException, ReportOptions.None, extraData);
		}

		protected virtual ExDateTime GetCurrentTime()
		{
			return ExDateTime.UtcNow;
		}

		private ReportTransportSyncWatsonException GetTransportSyncWatsonException(Exception exception, string message, out string callstack)
		{
			string text = null;
			string stackTrace = null;
			for (Exception ex = exception; ex != null; ex = ex.InnerException)
			{
				if (ex.StackTrace != null)
				{
					text = ex.StackTrace;
					break;
				}
			}
			if (text == null)
			{
				stackTrace = (text = Environment.StackTrace);
			}
			callstack = text;
			return new ReportTransportSyncWatsonException(message, exception, stackTrace);
		}

		internal static readonly TimeSpan ThrottlingPeriod = TimeSpan.FromHours(1.0);

		internal static readonly int MaxWatsonsPerCallstackPerThrottlingPeriod = 2;

		private object syncRoot = new object();

		private Dictionary<int, WatsonReporter.WatsonCallstack> watsonInstances = new Dictionary<int, WatsonReporter.WatsonCallstack>();

		private class WatsonCallstack
		{
			internal WatsonCallstack(string message, string callStack, Func<ExDateTime> getCurrentTime)
			{
				this.Message = message;
				this.CallStack = callStack;
				this.countSinceFirstSeen = 1;
				this.getCurrentTime = getCurrentTime;
				this.firstSeenTime = this.getCurrentTime();
			}

			public ExDateTime FirstSeenTime
			{
				get
				{
					return this.firstSeenTime;
				}
			}

			public int CountSinceFirstSeen
			{
				get
				{
					return this.countSinceFirstSeen;
				}
			}

			internal void Reset(SyncLogSession synclogSession)
			{
				SyncUtilities.ThrowIfArgumentNull("synclogSession", synclogSession);
				if (this.CountSinceFirstSeen > WatsonReporter.MaxWatsonsPerCallstackPerThrottlingPeriod)
				{
					synclogSession.LogError((TSLID)230UL, "Watson: {0} with callstack {1} skipped {2} times in last throttling period {3} for being overactive.", new object[]
					{
						this.Message,
						this.CallStack,
						this.CountSinceFirstSeen - WatsonReporter.MaxWatsonsPerCallstackPerThrottlingPeriod,
						WatsonReporter.ThrottlingPeriod
					});
				}
				this.countSinceFirstSeen = 1;
				this.firstSeenTime = this.getCurrentTime();
			}

			internal void IncrementCount()
			{
				this.countSinceFirstSeen++;
			}

			private readonly string CallStack;

			private readonly string Message;

			private readonly Func<ExDateTime> getCurrentTime;

			private ExDateTime firstSeenTime;

			private int countSinceFirstSeen;
		}
	}
}
