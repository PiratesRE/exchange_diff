using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class LogPipelineStatisticsStage : SynchronousPipelineStageBase
	{
		internal override PipelineDispatcher.PipelineResourceType ResourceType
		{
			get
			{
				return PipelineDispatcher.PipelineResourceType.CpuBound;
			}
		}

		internal override TimeSpan ExpectedRunTime
		{
			get
			{
				return TimeSpan.FromMinutes(1.0);
			}
		}

		protected override void InternalDoSynchronousWork()
		{
			IUMResolveCaller iumresolveCaller = base.WorkItem.Message as IUMResolveCaller;
			if (iumresolveCaller != null)
			{
				base.WorkItem.PipelineStatisticsLogRow.CallerName = iumresolveCaller.ContactInfo.DisplayName;
			}
			IUMCAMessage iumcamessage = base.WorkItem.Message as IUMCAMessage;
			if (iumcamessage != null)
			{
				base.WorkItem.PipelineStatisticsLogRow.CalleeAlias = iumcamessage.CAMessageRecipient.ADRecipient.Alias;
				base.WorkItem.PipelineStatisticsLogRow.OrganizationId = Util.GetTenantName(iumcamessage.CAMessageRecipient.ADRecipient);
			}
			PipelineStatisticsLogger.Instance.Append(base.WorkItem.PipelineStatisticsLogRow);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LogPipelineStatisticsStage>(this);
		}
	}
}
