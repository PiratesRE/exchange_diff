using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class InternalQueuedMessageSource
	{
		internal IMExSession MexSession
		{
			get
			{
				return this.mexSession;
			}
			set
			{
				this.mexSession = value;
			}
		}

		internal TaskContext TaskContext
		{
			get
			{
				return this.taskContext;
			}
			set
			{
				this.taskContext = value;
			}
		}

		public static MailRecipient RetrieveMailRecipient(EnvelopeRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			MailRecipientWrapper mailRecipientWrapper = recipient as MailRecipientWrapper;
			if (mailRecipientWrapper == null)
			{
				throw new ArgumentException("recipient");
			}
			mailRecipientWrapper.DisposeValidation();
			return mailRecipientWrapper.MailRecipient;
		}

		public void Initialize(IMExSession mexSession, TaskContext taskContext)
		{
			this.MexSession = mexSession;
			this.TaskContext = taskContext;
		}

		public void Defer(TimeSpan waitTime, string trackingContext)
		{
			this.Defer(waitTime, SmtpResponse.Empty, trackingContext);
		}

		public void Defer(TimeSpan waitTime, SmtpResponse deferReason, string trackingContext)
		{
			AgentQueuedMessageEventArgs agentQueuedMessageEventArgs = (AgentQueuedMessageEventArgs)this.MexSession.CurrentEventArgs;
			MailItem mailItem = agentQueuedMessageEventArgs.MailItem;
			if (mailItem.MimeWriteStreamOpen)
			{
				mailItem.RestoreLastSavedMime(this.MexSession.ExecutingAgentName, this.MexSession.OutstandingEventTopic);
				mailItem.Recipients.Clear();
				return;
			}
			TransportMailItemWrapper transportMailItemWrapper = mailItem as TransportMailItemWrapper;
			if (transportMailItemWrapper == null)
			{
				throw new ArgumentException("mailItem");
			}
			TransportMailItem transportMailItem = transportMailItemWrapper.TransportMailItem;
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.AgentDefer);
			transportMailItem.DeferReason = DeferReason.Agent;
			transportMailItemWrapper.CloseWrapper();
			int num = transportMailItem.ExtendedProperties.GetValue<int>("Microsoft.Exchange.Transport.AgentDeferCount", 0);
			num++;
			transportMailItem.ExtendedProperties.SetValue<int>("Microsoft.Exchange.Transport.AgentDeferCount", num);
			ExTraceGlobals.ExtensibilityTracer.TraceDebug<long, int>((long)this.GetHashCode(), "msgId {0} agent defer Count is now {1}", transportMailItem.RecordId, num);
			this.TaskContext.AgentLatencyTracker.EndTrackingCurrentEvent();
			LatencyTracker.EndTrackLatency(LatencyComponent.Categorizer, transportMailItem.LatencyTracker);
			((IQueueItem)transportMailItem).DeferUntil = DateTime.UtcNow + waitTime;
			if (waitTime != TimeSpan.Zero)
			{
				LatencyTracker.BeginTrackLatency(TransportMailItem.GetDeferLatencyComponent(transportMailItem.DeferReason), transportMailItem.LatencyTracker);
			}
			transportMailItem.ClearTransportSettings();
			foreach (MailRecipient mailRecipient in transportMailItem.Recipients)
			{
				if (!mailRecipient.IsProcessed)
				{
					mailRecipient.Ack(AckStatus.Retry, deferReason);
				}
			}
			trackingContext = this.EnforceAgentNameInTrackingContext(trackingContext);
			MessageTrackingLog.TrackAgentInfo(transportMailItem);
			SystemProbeHelper.SchedulerTracer.TracePass<string, string>(transportMailItem, 0L, "deferring message. reason: {0} period:{1}", deferReason.ToString(), waitTime.ToString());
			bool flag = Components.CategorizerComponent.TrackAndCheckForLocalResubmitLoop(transportMailItem, waitTime);
			if (flag)
			{
				MessageTrackingLog.TrackBadmail(MessageTrackingSource.AGENT, null, transportMailItem, string.Format("message dropped on Defer in agent {0} because it is causing a resubmit loop", this.MexSession.ExecutingAgentName));
			}
			else if (waitTime != TimeSpan.Zero)
			{
				MessageTrackingLog.TrackDefer(MessageTrackingSource.AGENT, transportMailItem, trackingContext);
			}
			else
			{
				MessageTrackingLog.TrackResubmit(MessageTrackingSource.AGENT, transportMailItem, transportMailItem, trackingContext);
			}
			this.TaskContext.MessageDeferred = true;
			this.TaskContext.DeferTime = waitTime;
			if (flag)
			{
				transportMailItem.ReleaseFromActiveMaterializedLazy();
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.MessageDroppedOnDeferInAgent);
			}
			else
			{
				Components.CategorizerComponent.EnqueueSubmittedMessage(transportMailItem, this.taskContext);
			}
			this.MexSession.HaltExecution();
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.DeferAgentCompleted);
		}

		public void Delete(string trackingContext = null)
		{
			AgentQueuedMessageEventArgs agentQueuedMessageEventArgs = (AgentQueuedMessageEventArgs)this.MexSession.CurrentEventArgs;
			MailItem mailItem = agentQueuedMessageEventArgs.MailItem;
			TransportMailItemWrapper transportMailItemWrapper = mailItem as TransportMailItemWrapper;
			if (transportMailItemWrapper == null)
			{
				throw new ArgumentException("mailItem");
			}
			transportMailItemWrapper.TransportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.AgentDelete);
			MessageTrackingLog.TrackAgentInfo(transportMailItemWrapper.TransportMailItem);
			LatencyFormatter latencyFormatter = new LatencyFormatter(transportMailItemWrapper.TransportMailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, true);
			trackingContext = this.EnforceAgentNameInTrackingContext(trackingContext);
			foreach (MailRecipient mailRecipient in transportMailItemWrapper.TransportMailItem.Recipients)
			{
				MessageTrackingLog.TrackRecipientFail(MessageTrackingSource.AGENT, transportMailItemWrapper.TransportMailItem, mailRecipient.Email, mailRecipient.SmtpResponse.Equals(SmtpResponse.Empty) ? AckReason.MessageDeletedByTransportAgent : mailRecipient.SmtpResponse, trackingContext, latencyFormatter);
			}
			TaskContext.ReleaseItem(transportMailItemWrapper.TransportMailItem);
			this.MexSession.HaltExecution();
			transportMailItemWrapper.TransportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.AgentDeleteCompleted);
		}

		public void Fork(IList<EnvelopeRecipient> envelopeRecipients)
		{
			AgentQueuedMessageEventArgs agentQueuedMessageEventArgs = (AgentQueuedMessageEventArgs)this.MexSession.CurrentEventArgs;
			MailItem mailItem = agentQueuedMessageEventArgs.MailItem;
			if (envelopeRecipients.Count < 1)
			{
				throw new ArgumentException();
			}
			List<MailRecipient> list = new List<MailRecipient>();
			foreach (EnvelopeRecipient envelopeRecipient in envelopeRecipients)
			{
				MailRecipientWrapper mailRecipientWrapper = envelopeRecipient as MailRecipientWrapper;
				if (mailRecipientWrapper == null)
				{
					throw new ArgumentException();
				}
				list.Add(mailRecipientWrapper.MailRecipient);
			}
			TransportMailItemWrapper transportMailItemWrapper = mailItem as TransportMailItemWrapper;
			if (transportMailItemWrapper == null)
			{
				throw new ArgumentException();
			}
			TransportMailItem transportMailItem = this.TaskContext.ForkItem(transportMailItemWrapper.TransportMailItem, list);
			if (transportMailItem != null)
			{
				transportMailItemWrapper.TransportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.AgentForkParent);
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.AgentForkChild);
				MessageTrackingLog.TrackTransfer(MessageTrackingSource.AGENT, transportMailItem, transportMailItemWrapper.TransportMailItem.RecordId, this.MexSession.ExecutingAgentName);
			}
		}

		public void ExpandRecipients(IList<RecipientExpansionInfo> recipientExpansionInfo)
		{
			AgentQueuedMessageEventArgs agentQueuedMessageEventArgs = (AgentQueuedMessageEventArgs)this.MexSession.CurrentEventArgs;
			MailItem mailItem = agentQueuedMessageEventArgs.MailItem;
			if (recipientExpansionInfo.Count == 0)
			{
				throw new ArgumentOutOfRangeException("Expand Data needs at least one entry");
			}
			TransportMailItemWrapper transportMailItemWrapper = mailItem as TransportMailItemWrapper;
			if (transportMailItemWrapper == null)
			{
				throw new ArgumentException("Can't map mailitem to a TransportMailItemWrapper");
			}
			TransportMailItem transportMailItem = transportMailItemWrapper.TransportMailItem;
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.AgentExpand);
			bool flag = false;
			List<MailRecipient> list = new List<MailRecipient>();
			for (int i = 0; i < recipientExpansionInfo.Count; i++)
			{
				MailRecipientWrapper mailRecipientWrapper = recipientExpansionInfo[i].RemoveRecipient as MailRecipientWrapper;
				if (mailRecipientWrapper == null)
				{
					throw new ArgumentException();
				}
				list.Add(mailRecipientWrapper.MailRecipient);
			}
			foreach (MailRecipient item in transportMailItem.Recipients.AllUnprocessed)
			{
				if (!list.Contains(item))
				{
					flag = true;
				}
			}
			TransportMailItem transportMailItem2;
			if (flag)
			{
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.AgentForkParent);
				transportMailItem2 = transportMailItem.NewCloneWithoutRecipients();
				transportMailItem2.DropCatBreadcrumb(CategorizerBreadcrumb.AgentForkChild);
				ExTraceGlobals.ExtensibilityTracer.TraceDebug<long, long>((long)this.GetHashCode(), "Original msgId {0} forked for Expand, new Forked msgId={1}", transportMailItem.RecordId, transportMailItem2.RecordId);
			}
			else
			{
				ExTraceGlobals.ExtensibilityTracer.TraceDebug((long)this.GetHashCode(), "No fork required for Expand.");
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.AgentExpandNoFork);
				transportMailItem2 = transportMailItem;
			}
			MailRecipientCollectionWrapper mailRecipientCollectionWrapper = (MailRecipientCollectionWrapper)mailItem.Recipients;
			for (int j = 0; j < recipientExpansionInfo.Count; j++)
			{
				if (recipientExpansionInfo[j].GenerateDSN)
				{
					mailRecipientCollectionWrapper.Remove(recipientExpansionInfo[j].RemoveRecipient, DsnType.Expanded, recipientExpansionInfo[j].SmtpResponse, false, null);
				}
				else
				{
					mailRecipientCollectionWrapper.Remove(recipientExpansionInfo[j].RemoveRecipient, false);
				}
				ExTraceGlobals.ExtensibilityTracer.TraceDebug<RoutingAddress, DsnType, SmtpResponse>((long)this.GetHashCode(), "Remove recipient {0} with DSN Report {1}, smtp response {2}", ((MailRecipientWrapper)recipientExpansionInfo[j].RemoveRecipient).MailRecipient.Email, DsnType.Expanded, recipientExpansionInfo[j].SmtpResponse);
			}
			if (flag)
			{
				MailRecipientCollection recipients = transportMailItem.Recipients;
				for (int k = recipients.Count - 1; k >= 0; k--)
				{
					MailRecipient mailRecipient = recipients[k];
					if (!mailRecipient.IsProcessed)
					{
						mailRecipient.MoveTo(transportMailItem2);
					}
				}
			}
			for (int l = 0; l < recipientExpansionInfo.Count; l++)
			{
				if (recipientExpansionInfo[l].Addresses == null)
				{
					throw new ArgumentOutOfRangeException("Address Array needs at least one entry");
				}
				for (int m = 0; m < recipientExpansionInfo[l].Addresses.Length; m++)
				{
					transportMailItem.Recipients.Add((string)recipientExpansionInfo[l].Addresses[m]);
				}
				ExTraceGlobals.ExtensibilityTracer.TraceDebug<int, int>((long)this.GetHashCode(), "Recipient {0} expanded to {1} new recipients", l, recipientExpansionInfo[l].Addresses.Length);
				MsgTrackExpandInfo msgTrackInfo = new MsgTrackExpandInfo(this.MexSession.ExecutingAgentName, ((MailRecipientWrapper)recipientExpansionInfo[l].RemoveRecipient).MailRecipient.Email, new long?(transportMailItem2.RecordId), SmtpResponse.RecipientAddressExpanded.ToString());
				MessageTrackingLog.TrackExpand<RoutingAddress>(MessageTrackingSource.AGENT, transportMailItem, msgTrackInfo, recipientExpansionInfo[l].Addresses);
			}
			if (flag)
			{
				ExTraceGlobals.ExtensibilityTracer.TraceDebug<long, int>((long)this.GetHashCode(), "Forked msgId {0} has {1} recipients", transportMailItem2.RecordId, transportMailItem2.Recipients.Count);
				transportMailItem2.CommitLazy();
				this.TaskContext.AgentLatencyTracker.EndTrackingCurrentEvent(transportMailItem2.LatencyTracker);
				this.TaskContext.ChainItemToSelf(transportMailItem2, this.mexSession);
				MessageTrackingLog.TrackTransfer(MessageTrackingSource.AGENT, transportMailItem2, transportMailItemWrapper.TransportMailItem.RecordId, this.MexSession.ExecutingAgentName);
			}
		}

		internal void TrackAgentInfo(string agentName, string groupName, List<KeyValuePair<string, string>> data)
		{
			if (data == null || data.Count == 0)
			{
				throw new InvalidOperationException("Cannot pass in null data to TrackAgentInfo");
			}
			TransportMailItem transportMailItem = this.GetTransportMailItem();
			if (!this.ValidateDataSize(data))
			{
				data = new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("ERROR", "MaxDataSizeExceeded")
				};
			}
			transportMailItem.AddAgentInfo(agentName, groupName, data);
		}

		internal void SetRiskLevel(RiskLevel level)
		{
			TransportMailItem transportMailItem = this.GetTransportMailItem();
			transportMailItem.RiskLevel = level;
		}

		internal RiskLevel GetRiskLevel()
		{
			TransportMailItem transportMailItem = this.GetTransportMailItem();
			return transportMailItem.RiskLevel;
		}

		internal void SetOutboundIPPool(EnvelopeRecipient recipient, int pool)
		{
			MailRecipient mailRecipient = InternalQueuedMessageSource.RetrieveMailRecipient(recipient);
			mailRecipient.OutboundIPPool = pool;
		}

		internal int GetOutboundIPPool(EnvelopeRecipient recipient)
		{
			MailRecipient mailRecipient = InternalQueuedMessageSource.RetrieveMailRecipient(recipient);
			return mailRecipient.OutboundIPPool;
		}

		public void SetComponentCost(string componentName, long cost)
		{
			if (string.IsNullOrEmpty(componentName))
			{
				throw new ArgumentNullException("componentName");
			}
			TransportMailItem transportMailItem = this.GetTransportMailItem();
			transportMailItem.ExtendedProperties.SetValue<long>("Microsoft.Exchange.Transport.TransportMailItem.NonforkingComponentCost." + componentName, cost);
		}

		public long GetComponentCost(string componentName)
		{
			if (string.IsNullOrEmpty(componentName))
			{
				throw new ArgumentNullException("componentName");
			}
			TransportMailItem transportMailItem = this.GetTransportMailItem();
			return transportMailItem.ExtendedProperties.GetValue<long>("Microsoft.Exchange.Transport.TransportMailItem.NonforkingComponentCost." + componentName, 0L);
		}

		private TransportMailItem GetTransportMailItem()
		{
			AgentQueuedMessageEventArgs agentQueuedMessageEventArgs = (AgentQueuedMessageEventArgs)this.MexSession.CurrentEventArgs;
			MailItem mailItem = agentQueuedMessageEventArgs.MailItem;
			TransportMailItemWrapper transportMailItemWrapper = mailItem as TransportMailItemWrapper;
			if (transportMailItemWrapper == null)
			{
				throw new ArgumentException("Can't map MailItem to a TransportMailItemWrapper");
			}
			if (transportMailItemWrapper.TransportMailItem == null)
			{
				throw new InvalidOperationException(string.Format("Agent {0} is calling an API on source after having called Defer. This is not allowed", this.MexSession.ExecutingAgentName));
			}
			return transportMailItemWrapper.TransportMailItem;
		}

		private bool ValidateDataSize(List<KeyValuePair<string, string>> data)
		{
			int num = 0;
			foreach (KeyValuePair<string, string> keyValuePair in data)
			{
				if (!string.IsNullOrEmpty(keyValuePair.Key))
				{
					num += keyValuePair.Key.Length;
				}
				if (!string.IsNullOrEmpty(keyValuePair.Value))
				{
					num += keyValuePair.Value.Length;
				}
			}
			return num <= Components.TransportAppConfig.Logging.MaxMsgTrkAgenInfoSize;
		}

		private string EnforceAgentNameInTrackingContext(string trackingContext)
		{
			string result;
			if (string.IsNullOrEmpty(trackingContext))
			{
				result = this.MexSession.ExecutingAgentName;
			}
			else if (!trackingContext.Contains(this.MexSession.ExecutingAgentName))
			{
				result = string.Format("{0}; {1}", this.MexSession.ExecutingAgentName, trackingContext);
			}
			else
			{
				result = trackingContext;
			}
			return result;
		}

		private const string AgentDeferCount = "Microsoft.Exchange.Transport.AgentDeferCount";

		private IMExSession mexSession;

		private TaskContext taskContext;
	}
}
