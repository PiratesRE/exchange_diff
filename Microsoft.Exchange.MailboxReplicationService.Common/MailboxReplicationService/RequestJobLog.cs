using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RequestJobLog : ObjectLog<RequestJobLogData>
	{
		private RequestJobLog() : base(new RequestJobLog.RequestJobLogSchema(), new SimpleObjectLogConfiguration("Request", "RequestLogEnabled", "RequestLogMaxDirSize", "RequestLogMaxFileSize"))
		{
		}

		public static void Write(RequestJobBase request)
		{
			RequestJobLog.instance.LogObject(new RequestJobLogData(request));
		}

		public static void Write(RequestJobBase request, RequestState statusDetail)
		{
			RequestJobLog.instance.LogObject(new RequestJobLogData(request, statusDetail));
		}

		private static RequestJobLog instance = new RequestJobLog();

		private class RequestJobLogSchema : ConfigurableObjectLogSchema<RequestJobLogData, RequestJobSchema>
		{
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
					return "Request Log";
				}
			}

			public override HashSet<string> ExcludedProperties
			{
				get
				{
					return RequestJobLog.RequestJobLogSchema.excludedProperties;
				}
			}

			private static IEnumerable<IObjectLogPropertyDefinition<RequestJobLogData>> GetTimeTrackerTimestamps()
			{
				List<IObjectLogPropertyDefinition<RequestJobLogData>> list = new List<IObjectLogPropertyDefinition<RequestJobLogData>>();
				foreach (object obj in Enum.GetValues(typeof(RequestJobTimestamp)))
				{
					RequestJobTimestamp requestJobTimestamp = (RequestJobTimestamp)obj;
					if (RequestJobTimeTracker.SupportTimestampTracking(requestJobTimestamp))
					{
						list.Add(new RequestJobLog.RequestJobLogSchema.TimeTrackerTimeStampProperty(requestJobTimestamp));
					}
				}
				return list;
			}

			private static IEnumerable<IObjectLogPropertyDefinition<RequestJobLogData>> GetTimeTrackerDurations()
			{
				List<IObjectLogPropertyDefinition<RequestJobLogData>> list = new List<IObjectLogPropertyDefinition<RequestJobLogData>>();
				foreach (object obj in Enum.GetValues(typeof(RequestState)))
				{
					RequestState requestState = (RequestState)obj;
					if (RequestJobTimeTracker.SupportDurationTracking(requestState))
					{
						list.Add(new RequestJobLog.RequestJobLogSchema.TimeTrackerDurationProperty(requestState));
					}
				}
				return list;
			}

			private static HashSet<string> excludedProperties = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
			{
				RequestJobSchema.RetryCount.Name,
				RequestJobSchema.UserId.Name,
				RequestJobSchema.DisplayName.Name,
				RequestJobSchema.Alias.Name,
				RequestJobSchema.SourceAlias.Name,
				RequestJobSchema.TargetAlias.Name,
				RequestJobSchema.SourceRootFolder.Name,
				RequestJobSchema.TargetRootFolder.Name,
				RequestJobSchema.IncludeFolders.Name,
				RequestJobSchema.ExcludeFolders.Name,
				RequestJobSchema.EmailAddress.Name,
				RequestJobSchema.RemoteCredentialUsername.Name,
				RequestJobSchema.Name.Name,
				RequestJobSchema.SourceUserId.Name,
				RequestJobSchema.TargetUserId.Name,
				RequestJobSchema.RemoteUserName.Name,
				RequestJobSchema.RemoteMailboxLegacyDN.Name,
				RequestJobSchema.RemoteUserLegacyDN.Name,
				RequestJobSchema.RequestCreator.Name
			};

			public static readonly ObjectLogSimplePropertyDefinition<RequestJobLogData> ExternalDirectoryOrganizationId = new ObjectLogSimplePropertyDefinition<RequestJobLogData>("ExternalDirectoryOrganizationId", (RequestJobLogData rj) => rj.Request.ExternalDirectoryOrganizationId);

			public static readonly ObjectLogSimplePropertyDefinition<RequestJobLogData> StatusDetail = new ObjectLogSimplePropertyDefinition<RequestJobLogData>("StatusDetail", delegate(RequestJobLogData rj)
			{
				string result;
				if (!rj.TryGetOverride("StatusDetail", out result))
				{
					return rj.Request.TimeTracker.CurrentState.ToString();
				}
				return result;
			});

			public static readonly IEnumerable<IObjectLogPropertyDefinition<RequestJobLogData>> TimeTrackerTimestamps = RequestJobLog.RequestJobLogSchema.GetTimeTrackerTimestamps();

			public static readonly IEnumerable<IObjectLogPropertyDefinition<RequestJobLogData>> TimeTrackerDurations = RequestJobLog.RequestJobLogSchema.GetTimeTrackerDurations();

			private class TimeTrackerTimeStampProperty : IObjectLogPropertyDefinition<RequestJobLogData>
			{
				public TimeTrackerTimeStampProperty(RequestJobTimestamp rjts)
				{
					this.rjts = rjts;
				}

				string IObjectLogPropertyDefinition<RequestJobLogData>.FieldName
				{
					get
					{
						return string.Format("TS_{0}", this.rjts);
					}
				}

				object IObjectLogPropertyDefinition<RequestJobLogData>.GetValue(RequestJobLogData rj)
				{
					DateTime? timestamp = rj.Request.TimeTracker.GetTimestamp(this.rjts);
					if (timestamp != null)
					{
						return timestamp.Value;
					}
					return null;
				}

				private RequestJobTimestamp rjts;
			}

			private class TimeTrackerDurationProperty : IObjectLogPropertyDefinition<RequestJobLogData>
			{
				public TimeTrackerDurationProperty(RequestState rs)
				{
					this.rs = rs;
				}

				string IObjectLogPropertyDefinition<RequestJobLogData>.FieldName
				{
					get
					{
						return string.Format("Duration_{0}", this.rs);
					}
				}

				object IObjectLogPropertyDefinition<RequestJobLogData>.GetValue(RequestJobLogData rj)
				{
					RequestJobDurationData duration = rj.Request.TimeTracker.GetDuration(this.rs);
					if (duration != null)
					{
						return (long)duration.Duration.TotalMilliseconds;
					}
					return null;
				}

				private RequestState rs;
			}
		}
	}
}
