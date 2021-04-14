using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GroupsLogger : IGroupsLogger
	{
		public GroupsLogger(GroupTaskName taskName, Guid activityId)
		{
			ArgumentValidator.ThrowIfNull("activityId", activityId);
			this.taskName = taskName;
			this.activityId = activityId;
		}

		public Enum CurrentAction { get; set; }

		public void LogTrace(string formatString, params object[] args)
		{
			string text = string.Format(formatString, args);
			GroupsLogger.Tracer.TraceDebug((long)this.GetHashCode(), "ActivityId={0}. TaskName={1}. CurrentAction={2}. Message={3}.", new object[]
			{
				this.activityId,
				this.taskName,
				this.CurrentAction,
				text
			});
			FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.TraceTag>
			{
				{
					FederatedDirectoryLogSchema.TraceTag.TaskName,
					this.taskName
				},
				{
					FederatedDirectoryLogSchema.TraceTag.ActivityId,
					this.activityId
				},
				{
					FederatedDirectoryLogSchema.TraceTag.CurrentAction,
					this.CurrentAction
				},
				{
					FederatedDirectoryLogSchema.TraceTag.Message,
					text
				}
			});
		}

		public void LogException(Exception exception, string formatString, params object[] args)
		{
			string text = string.Format(formatString, args);
			GroupsLogger.Tracer.TraceError((long)this.GetHashCode(), "ActivityId={0}. TaskName={1}. CurrentAction={2}. Message={3}. Exception={4}", new object[]
			{
				this.activityId,
				this.taskName,
				this.CurrentAction,
				text,
				exception
			});
			FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.ExceptionTag>
			{
				{
					FederatedDirectoryLogSchema.ExceptionTag.TaskName,
					this.taskName
				},
				{
					FederatedDirectoryLogSchema.ExceptionTag.ActivityId,
					this.activityId
				},
				{
					FederatedDirectoryLogSchema.ExceptionTag.ExceptionType,
					exception.GetType()
				},
				{
					FederatedDirectoryLogSchema.ExceptionTag.ExceptionDetail,
					exception
				},
				{
					FederatedDirectoryLogSchema.ExceptionTag.CurrentAction,
					this.CurrentAction
				},
				{
					FederatedDirectoryLogSchema.ExceptionTag.Message,
					text
				}
			});
		}

		private static readonly Trace Tracer = ExTraceGlobals.ModernGroupsTracer;

		private readonly GroupTaskName taskName;

		private readonly Guid activityId;
	}
}
