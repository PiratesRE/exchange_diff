using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class CategorizerJobsUtil
	{
		private CategorizerJobsUtil()
		{
		}

		public static Job SetupNewJob(TransportMailItem mailItem, IList<StageInfo> stages, Func<QueuedRecipientsByAgeToken, ThrottlingContext, IList<StageInfo>, Job> jobCreator, out ThrottlingContext throttlingContext)
		{
			throttlingContext = mailItem.ThrottlingContext;
			QueuedRecipientsByAgeToken queuedRecipientsByAgeToken = Components.QueueManager.GetQueuedRecipientsByAge().TrackEnteringCategorizer(mailItem);
			LatencyTracker.BeginTrackLatency(LatencyComponent.Categorizer, mailItem.LatencyTracker);
			ActivityContext.ClearThreadScope();
			mailItem.ActivityScope = ActivityContext.Start(null);
			AcceptedDomainTable acceptedDomains = null;
			DeferReason deferReason = CategorizerJobsUtil.TryPrepareForNewJob(mailItem, out acceptedDomains);
			Job job = null;
			if (deferReason == DeferReason.None)
			{
				job = jobCreator(queuedRecipientsByAgeToken, throttlingContext, stages);
				try
				{
					job.EnqueuePendingTask(0, mailItem, acceptedDomains);
				}
				catch (DataSourceTransientException)
				{
					deferReason = DeferReason.TransientFailure;
				}
			}
			if (deferReason == DeferReason.None)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug<Job, long>(0L, "Create new job {0} for mailitem {1}", job, mailItem.RecordId);
				return job;
			}
			Components.CategorizerComponent.DeferMailItem(mailItem, null, deferReason);
			CategorizerJobsUtil.DoneProcessing(queuedRecipientsByAgeToken);
			return null;
		}

		public static void DoneProcessing(QueuedRecipientsByAgeToken token)
		{
			Components.QueueManager.GetQueuedRecipientsByAge().TrackExitingCategorizer(token);
		}

		private static DeferReason TryPrepareForNewJob(TransportMailItem mailItem, out AcceptedDomainTable acceptedDomains)
		{
			acceptedDomains = null;
			if (!CategorizerJobsUtil.CheckAttribution(mailItem))
			{
				return DeferReason.TransientAttributionFailure;
			}
			if (mailItem.ADRecipientCache == null)
			{
				ADOperationResult adoperationResult = MultiTenantTransport.TryCreateADRecipientCache(mailItem);
				if (!adoperationResult.Succeeded)
				{
					return DeferReason.TransientAttributionFailure;
				}
			}
			PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
			if (!Components.Configuration.TryGetAcceptedDomainTable(mailItem.OrganizationId, out perTenantAcceptedDomainTable))
			{
				return DeferReason.TransientAcceptedDomainsLoadFailure;
			}
			acceptedDomains = perTenantAcceptedDomainTable.AcceptedDomainTable;
			return DeferReason.None;
		}

		private static bool CheckAttribution(TransportMailItem mailItem)
		{
			if (!MultiTenantTransport.MultiTenancyEnabled)
			{
				return true;
			}
			if (mailItem.Directionality == MailDirectionality.Undefined)
			{
				MultiTenantTransport.TraceAttributionError("Directionality is undefined, reading from header of mail {0}", new object[]
				{
					MultiTenantTransport.ToString(mailItem)
				});
				mailItem.Directionality = MultiTenantTransport.GetDirectionalityFromHeader(mailItem);
			}
			if (mailItem.ExternalOrganizationId == Guid.Empty)
			{
				MultiTenantTransport.TraceAttributionError("External org id is undefined, resolving from domain of mail {0}", new object[]
				{
					MultiTenantTransport.ToString(mailItem)
				});
				ADOperationResult adoperationResult = MultiTenantTransport.TryAttributeFromDomain(mailItem);
				switch (adoperationResult.ErrorCode)
				{
				case ADOperationErrorCode.RetryableError:
					MultiTenantTransport.TraceAttributionError("Check attribution encountered retriable failure {0} for mail item {1}", new object[]
					{
						adoperationResult.Exception,
						MultiTenantTransport.ToString(mailItem)
					});
					return false;
				case ADOperationErrorCode.PermanentError:
					mailItem.ExternalOrganizationId = MultiTenantTransport.SafeTenantId;
					break;
				}
			}
			return true;
		}
	}
}
