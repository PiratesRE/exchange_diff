using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SessionStatisticsLog : ObjectLog<SessionStatisticsLogData>
	{
		private SessionStatisticsLog() : base(new SessionStatisticsLog.SessionStatisticsLogSchema(), new SimpleObjectLogConfiguration("SessionStatistic", "SessionStatisticsLogEnabled", "SessionStatisticsLogMaxDirSize", "SessionStatisticsLogMaxFileSize"))
		{
		}

		public static void Write(Guid requestGuid, SessionStatistics sessionStatistics, SessionStatistics archiveSessionStatistics)
		{
			if (sessionStatistics != null || archiveSessionStatistics != null)
			{
				SessionStatisticsLogData objectToLog = new SessionStatisticsLogData(requestGuid, sessionStatistics, archiveSessionStatistics);
				SessionStatisticsLog.instance.LogObject(objectToLog);
			}
		}

		private static SessionStatisticsLog instance = new SessionStatisticsLog();

		private class SessionStatisticsLogSchema : ObjectLogSchema
		{
			private static DurationInfo FindMaxDurationInfoFromStats(SessionStatistics stats, bool isArchive)
			{
				string str = isArchive ? "_Archive" : string.Empty;
				DurationInfo durationInfo = new DurationInfo
				{
					Name = string.Empty,
					Duration = TimeSpan.Zero
				};
				if (stats != null)
				{
					if (stats.SourceProviderInfo.Durations.Count > 0)
					{
						DurationInfo durationInfo2 = stats.SourceProviderInfo.Durations[0];
						durationInfo.Name = "SourceProvider_" + durationInfo2.Name + str;
						durationInfo.Duration = durationInfo2.Duration;
					}
					if (stats.DestinationProviderInfo.Durations.Count > 0)
					{
						DurationInfo durationInfo2 = stats.DestinationProviderInfo.Durations[0];
						if (durationInfo2.Duration > durationInfo.Duration)
						{
							durationInfo.Name = "DestinationProvider_" + durationInfo2.Name + str;
							durationInfo.Duration = durationInfo2.Duration;
						}
					}
				}
				return durationInfo;
			}

			private static IEnumerable<IObjectLogPropertyDefinition<SessionStatisticsLogData>> GetMaximumProviderDurations()
			{
				List<IObjectLogPropertyDefinition<SessionStatisticsLogData>> list = new List<IObjectLogPropertyDefinition<SessionStatisticsLogData>>();
				Func<SessionStatisticsLogData, DurationInfo> findMaxDurationInfo = delegate(SessionStatisticsLogData logData)
				{
					DurationInfo durationInfo = SessionStatisticsLog.SessionStatisticsLogSchema.FindMaxDurationInfoFromStats(logData.SessionStatistics, false);
					DurationInfo durationInfo2 = SessionStatisticsLog.SessionStatisticsLogSchema.FindMaxDurationInfoFromStats(logData.ArchiveSessionStatistics, true);
					if (!(durationInfo.Duration > durationInfo2.Duration))
					{
						return durationInfo2;
					}
					return durationInfo;
				};
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("MaxProviderDurationMethodName", (SessionStatisticsLogData s) => findMaxDurationInfo(s).Name));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("MaxProviderDurationInMilliseconds", (SessionStatisticsLogData s) => (long)findMaxDurationInfo(s).Duration.TotalMilliseconds));
				return list;
			}

			private static IEnumerable<IObjectLogPropertyDefinition<SessionStatisticsLogData>> GetProviderDurations(bool isArchive = false)
			{
				List<IObjectLogPropertyDefinition<SessionStatisticsLogData>> list = new List<IObjectLogPropertyDefinition<SessionStatisticsLogData>>();
				string str = isArchive ? "_Archive" : string.Empty;
				Func<SessionStatisticsLogData, SessionStatistics> getStatsFunc = delegate(SessionStatisticsLogData logData)
				{
					if (!isArchive)
					{
						return logData.SessionStatistics;
					}
					return logData.ArchiveSessionStatistics;
				};
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("SourceProvider_TotalDurationInMilliseconds" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0L : ((long)getStatsFunc(s).SourceProviderInfo.TotalDuration.TotalMilliseconds)));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("DestinationProvider_TotalDurationInMilliseconds" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0L : ((long)getStatsFunc(s).DestinationProviderInfo.TotalDuration.TotalMilliseconds)));
				foreach (string name in SessionStatisticsLog.SessionStatisticsLogSchema.SourceProviderMethods)
				{
					list.Add(new SessionStatisticsLog.SessionStatisticsLogSchema.ProviderDurationProperty(name, false, isArchive));
				}
				foreach (string name2 in SessionStatisticsLog.SessionStatisticsLogSchema.DestinationProviderMethods)
				{
					list.Add(new SessionStatisticsLog.SessionStatisticsLogSchema.ProviderDurationProperty(name2, true, isArchive));
				}
				return list;
			}

			private static IEnumerable<IObjectLogPropertyDefinition<SessionStatisticsLogData>> GetLatencyInfo(bool isArchive = false)
			{
				List<IObjectLogPropertyDefinition<SessionStatisticsLogData>> list = new List<IObjectLogPropertyDefinition<SessionStatisticsLogData>>();
				string str = isArchive ? "_Archive" : string.Empty;
				Func<SessionStatisticsLogData, SessionStatistics> getStatsFunc = delegate(SessionStatisticsLogData logData)
				{
					if (!isArchive)
					{
						return logData.SessionStatistics;
					}
					return logData.ArchiveSessionStatistics;
				};
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("SourceLatencyInMillisecondsCurrent" + str, delegate(SessionStatisticsLogData s)
				{
					SessionStatistics sessionStatistics = getStatsFunc(s);
					return (sessionStatistics == null) ? 0 : sessionStatistics.SourceLatencyInfo.Current;
				}));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("SourceLatencyInMillisecondsAverage" + str, delegate(SessionStatisticsLogData s)
				{
					SessionStatistics sessionStatistics = getStatsFunc(s);
					return (sessionStatistics == null) ? 0 : sessionStatistics.SourceLatencyInfo.Average;
				}));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("SourceLatencyInMillisecondsMin" + str, delegate(SessionStatisticsLogData s)
				{
					SessionStatistics sessionStatistics = getStatsFunc(s);
					return (sessionStatistics == null || sessionStatistics.SourceLatencyInfo.Min == int.MaxValue) ? 0 : sessionStatistics.SourceLatencyInfo.Min;
				}));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("SourceLatencyInMillisecondsMax" + str, delegate(SessionStatisticsLogData s)
				{
					SessionStatistics sessionStatistics = getStatsFunc(s);
					return (sessionStatistics == null || sessionStatistics.SourceLatencyInfo.Max == int.MinValue) ? 0 : sessionStatistics.SourceLatencyInfo.Max;
				}));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("SourceLatencyNumberOfSamples" + str, delegate(SessionStatisticsLogData s)
				{
					SessionStatistics sessionStatistics = getStatsFunc(s);
					return (sessionStatistics == null) ? 0 : sessionStatistics.SourceLatencyInfo.NumberOfLatencySamplingCalls;
				}));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("SourceLatencyTotalNumberOfRemoteCalls" + str, delegate(SessionStatisticsLogData s)
				{
					SessionStatistics sessionStatistics = getStatsFunc(s);
					return (sessionStatistics == null) ? 0 : sessionStatistics.SourceLatencyInfo.TotalNumberOfRemoteCalls;
				}));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("DestinationLatencyInMillisecondsCurrent" + str, delegate(SessionStatisticsLogData s)
				{
					SessionStatistics sessionStatistics = getStatsFunc(s);
					return (sessionStatistics == null) ? 0 : sessionStatistics.DestinationLatencyInfo.Current;
				}));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("DestinationLatencyInMillisecondsAverage" + str, delegate(SessionStatisticsLogData s)
				{
					SessionStatistics sessionStatistics = getStatsFunc(s);
					return (sessionStatistics == null) ? 0 : sessionStatistics.DestinationLatencyInfo.Average;
				}));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("DestinationLatencyInMillisecondsMin" + str, delegate(SessionStatisticsLogData s)
				{
					SessionStatistics sessionStatistics = getStatsFunc(s);
					return (sessionStatistics == null) ? 0 : sessionStatistics.DestinationLatencyInfo.Min;
				}));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("DestinationLatencyInMillisecondsMax" + str, delegate(SessionStatisticsLogData s)
				{
					SessionStatistics sessionStatistics = getStatsFunc(s);
					return (sessionStatistics == null) ? 0 : sessionStatistics.DestinationLatencyInfo.Max;
				}));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("DestinationLatencyNumberOfSamples" + str, delegate(SessionStatisticsLogData s)
				{
					SessionStatistics sessionStatistics = getStatsFunc(s);
					return (sessionStatistics == null) ? 0 : sessionStatistics.DestinationLatencyInfo.NumberOfLatencySamplingCalls;
				}));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("DestinationLatencyTotalNumberOfRemoteCalls" + str, delegate(SessionStatisticsLogData s)
				{
					SessionStatistics sessionStatistics = getStatsFunc(s);
					return (sessionStatistics == null) ? 0 : sessionStatistics.DestinationLatencyInfo.TotalNumberOfRemoteCalls;
				}));
				return list;
			}

			private static IEnumerable<IObjectLogPropertyDefinition<SessionStatisticsLogData>> GetWordBreakingStats(bool isArchive = false)
			{
				List<IObjectLogPropertyDefinition<SessionStatisticsLogData>> list = new List<IObjectLogPropertyDefinition<SessionStatisticsLogData>>();
				string str = isArchive ? "_Archive" : string.Empty;
				Func<SessionStatisticsLogData, SessionStatistics> getStatsFunc = delegate(SessionStatisticsLogData logData)
				{
					if (!isArchive)
					{
						return logData.SessionStatistics;
					}
					return logData.ArchiveSessionStatistics;
				};
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("CI_TotalTimeInMilliseconds" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0L : ((long)getStatsFunc(s).TotalTimeProcessingMessages.TotalMilliseconds)));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("CI_TimeInWordBreakerInMilliseconds" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0L : ((long)getStatsFunc(s).TimeInWordbreaker.TotalMilliseconds)));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("CI_TimeInQueueInMilliseconds" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0L : ((long)getStatsFunc(s).TimeInQueue.TotalMilliseconds)));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("CI_TimeProcessingFailedMessagesInMilliseconds" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0L : ((long)getStatsFunc(s).TimeProcessingFailedMessages.TotalMilliseconds)));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("CI_TimeInTransportRetrieverInMilliseconds" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0L : ((long)getStatsFunc(s).TimeInTransportRetriever.TotalMilliseconds)));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("CI_TimeInDocParserInMilliseconds" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0L : ((long)getStatsFunc(s).TimeInDocParser.TotalMilliseconds)));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("CI_TimeInNLGSubflowInMilliseconds" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0L : ((long)getStatsFunc(s).TimeInNLGSubflow.TotalMilliseconds)));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("CI_TotalMessagesProcessed" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0 : getStatsFunc(s).TotalMessagesProcessed));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("CI_MessageLevelFailures" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0 : getStatsFunc(s).MessageLevelFailures));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("CI_MessagesSuccessfullyAnnotated" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0 : getStatsFunc(s).MessagesSuccessfullyAnnotated));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("CI_AnnotationSkipped" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0 : getStatsFunc(s).AnnotationSkipped));
				list.Add(new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("CI_ConnectionLevelFailures" + str, (SessionStatisticsLogData s) => (getStatsFunc(s) == null) ? 0 : getStatsFunc(s).ConnectionLevelFailures));
				return list;
			}

			public override string Software
			{
				get
				{
					return "Microsoft Exchange Mailbox Replication Service";
				}
			}

			public override string LogType
			{
				get
				{
					return "SessionStatistics Log";
				}
			}

			private static readonly string[] SourceProviderMethods = new string[]
			{
				"ISourceMailbox.ExportMessages",
				"ISourceMailbox.GetFolder",
				"WrapperBase.Dispose",
				"ISourceFolder.CopyTo",
				"IFolder.EnumerateMessages",
				"IFolder.GetSecurityDescriptor",
				"IFolder.GetFolderRec"
			};

			private static readonly string[] DestinationProviderMethods = new string[]
			{
				"IMapiFxProxy.ProcessRequest",
				"IDestinationFolder.SetSecurityDescriptor",
				"IFxProxyPool.GetFolderProxy",
				"IMailbox.SaveSyncState",
				"IDestinationMailbox.CreateFolder",
				"IFxProxyPool.GetFolderData",
				"IDestinationFolder.SetRules"
			};

			public static readonly ObjectLogSimplePropertyDefinition<SessionStatisticsLogData> RequestGuid = new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("RequestGuid", (SessionStatisticsLogData s) => s.RequestGuid);

			public static readonly IEnumerable<IObjectLogPropertyDefinition<SessionStatisticsLogData>> MaximumProviderDurations = SessionStatisticsLog.SessionStatisticsLogSchema.GetMaximumProviderDurations();

			public static readonly ObjectLogSimplePropertyDefinition<SessionStatisticsLogData> SessionId = new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("SessionId", delegate(SessionStatisticsLogData s)
			{
				if (s.SessionStatistics != null)
				{
					return s.SessionStatistics.SessionId;
				}
				return null;
			});

			public static readonly IEnumerable<IObjectLogPropertyDefinition<SessionStatisticsLogData>> WordBreakingStats = SessionStatisticsLog.SessionStatisticsLogSchema.GetWordBreakingStats(false);

			public static readonly IEnumerable<IObjectLogPropertyDefinition<SessionStatisticsLogData>> ProviderDurations = SessionStatisticsLog.SessionStatisticsLogSchema.GetProviderDurations(false);

			public static readonly IEnumerable<IObjectLogPropertyDefinition<SessionStatisticsLogData>> LatencyInfo = SessionStatisticsLog.SessionStatisticsLogSchema.GetLatencyInfo(false);

			public static readonly ObjectLogSimplePropertyDefinition<SessionStatisticsLogData> PreFinalSyncDataProcessingDurationInMilliseconds = new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("PreFinalSyncDataProcessingDurationInMilliseconds", (SessionStatisticsLogData s) => (s.SessionStatistics == null) ? 0.0 : s.SessionStatistics.PreFinalSyncDataProcessingDuration.TotalMilliseconds);

			public static readonly ObjectLogSimplePropertyDefinition<SessionStatisticsLogData> SessionId_Archive = new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("SessionId_Archive", delegate(SessionStatisticsLogData s)
			{
				if (s.ArchiveSessionStatistics != null)
				{
					return s.ArchiveSessionStatistics.SessionId;
				}
				return null;
			});

			public static readonly IEnumerable<IObjectLogPropertyDefinition<SessionStatisticsLogData>> WordBreakingStats_Archive = SessionStatisticsLog.SessionStatisticsLogSchema.GetWordBreakingStats(true);

			public static readonly IEnumerable<IObjectLogPropertyDefinition<SessionStatisticsLogData>> ProviderDurations_Archive = SessionStatisticsLog.SessionStatisticsLogSchema.GetProviderDurations(true);

			public static readonly IEnumerable<IObjectLogPropertyDefinition<SessionStatisticsLogData>> LatencyInfo_Archive = SessionStatisticsLog.SessionStatisticsLogSchema.GetLatencyInfo(true);

			public static readonly ObjectLogSimplePropertyDefinition<SessionStatisticsLogData> PreFinalSyncDataProcessingDurationInMilliseconds_Archive = new ObjectLogSimplePropertyDefinition<SessionStatisticsLogData>("PreFinalSyncDataProcessingDurationInMilliseconds_Archive", (SessionStatisticsLogData s) => (s.ArchiveSessionStatistics == null) ? 0.0 : s.ArchiveSessionStatistics.PreFinalSyncDataProcessingDuration.TotalMilliseconds);

			private class ProviderDurationProperty : IObjectLogPropertyDefinition<SessionStatisticsLogData>
			{
				public ProviderDurationProperty(string name, bool isDestinationSide = false, bool isArchive = false)
				{
					this.name = name;
					this.isArchive = isArchive;
					this.isDestinationSide = isDestinationSide;
				}

				string IObjectLogPropertyDefinition<SessionStatisticsLogData>.FieldName
				{
					get
					{
						return string.Format("{0}Duration_{1}{2}", this.isDestinationSide ? "Destination" : "Source", this.name, this.isArchive ? "_Archive" : string.Empty);
					}
				}

				object IObjectLogPropertyDefinition<SessionStatisticsLogData>.GetValue(SessionStatisticsLogData logData)
				{
					List<DurationInfo> durations;
					if (this.isArchive)
					{
						if (logData.ArchiveSessionStatistics == null)
						{
							return 0L;
						}
						if (this.isDestinationSide)
						{
							durations = logData.ArchiveSessionStatistics.DestinationProviderInfo.Durations;
						}
						else
						{
							durations = logData.ArchiveSessionStatistics.SourceProviderInfo.Durations;
						}
					}
					else
					{
						if (logData.SessionStatistics == null)
						{
							return 0L;
						}
						if (this.isDestinationSide)
						{
							durations = logData.SessionStatistics.DestinationProviderInfo.Durations;
						}
						else
						{
							durations = logData.SessionStatistics.SourceProviderInfo.Durations;
						}
					}
					DurationInfo durationInfo = durations.Find((DurationInfo d) => d.Name.Equals(this.name));
					if (durationInfo != null)
					{
						return (long)durationInfo.Duration.TotalMilliseconds;
					}
					return 0L;
				}

				private readonly string name;

				private readonly bool isArchive;

				private readonly bool isDestinationSide;
			}
		}
	}
}
