using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.Logging.WorkloadManagement;
using Microsoft.Exchange.Transport.MessageDepot;
using Microsoft.Exchange.Transport.MessageResubmission;
using Microsoft.Exchange.Transport.Storage;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal sealed class CategorizerComponent : ICategorizerComponentFacade, IStartableTransportComponent, ITransportComponent, ICategorizer, IDiagnosable
	{
		public int TotalQueuedMessages
		{
			get
			{
				return this.submitMessageQueue.TotalCount;
			}
		}

		public string CurrentState
		{
			get
			{
				if (this.scheduler != null)
				{
					StringBuilder stringBuilder = new StringBuilder(256);
					stringBuilder.Append("Active job count=");
					stringBuilder.AppendLine(this.scheduler.JobList.ExecutingJobCount.ToString());
					stringBuilder.Append("Pending job count=");
					stringBuilder.AppendLine(this.scheduler.JobList.PendingJobCount.ToString());
					return stringBuilder.ToString();
				}
				return null;
			}
		}

		internal SubmitMessageQueue SubmitMessageQueue
		{
			get
			{
				return this.submitMessageQueue;
			}
		}

		public IList<StageInfo> Stages
		{
			get
			{
				return this.stages;
			}
		}

		public void BifurcateRecipientsAndDefer(TransportMailItem mailItem, ICollection<MailRecipient> recipientsToBeForked, TaskContext taskContext, SmtpResponse deferResponse, TimeSpan deferTime, DeferReason deferReason)
		{
			TransportMailItem transportMailItem = mailItem.NewCloneWithoutRecipients();
			foreach (MailRecipient mailRecipient in recipientsToBeForked)
			{
				mailRecipient.MoveTo(transportMailItem);
				mailRecipient.Ack(AckStatus.Retry, deferResponse);
				Resolver.ClearResolverProperties(mailRecipient);
			}
			Resolver.ClearResolverAndTransportSettings(transportMailItem);
			transportMailItem.CommitLazy();
			this.MailItemBifurcatedInCategorizer(transportMailItem);
			this.DeferMailItem(transportMailItem, taskContext, deferReason, deferTime);
		}

		public void Retire()
		{
			this.retired = true;
			ExTraceGlobals.SchedulerTracer.TraceDebug((long)this.GetHashCode(), "Retire Categorizer");
			this.scheduler.Retire();
		}

		public void Load()
		{
			if (this.isMessageDepotEnabled)
			{
				this.messageDepot = this.messageDepotComponent.MessageDepot;
			}
			this.submitMessageQueue = SubmitMessageQueue.Instance;
			this.agentLoopChecker = new AgentGeneratedMessageLoopChecker(new AgentGeneratedMessageLoopCheckerTransportConfig(Components.Configuration));
			this.stages = new StageInfo[]
			{
				new StageInfo(new StageHandler(this.Stage1OnSubmitted), LatencyComponent.None),
				new StageInfo(new StageHandler(this.Stage2ResolveEngine), LatencyComponent.CategorizerResolver),
				new StageInfo(new StageHandler(this.Stage3OnResolvedMessage), LatencyComponent.None),
				new StageInfo(new StageHandler(this.Stage4Routing), LatencyComponent.CategorizerRouting),
				new StageInfo(new StageHandler(this.Stage5OnRoutedMessage), LatencyComponent.None),
				new StageInfo(new StageHandler(this.Stage6ContentConversion), LatencyComponent.CategorizerContentConversion),
				new StageInfo(new StageHandler(this.Stage7BifurcateByRecipSettings), LatencyComponent.CategorizerBifurcation),
				new StageInfo(new StageHandler(this.Stage8OnCategorizedMessage), LatencyComponent.None),
				new StageInfo(new StageHandler(this.Stage9StuffAndDSN), LatencyComponent.CategorizerFinal)
			};
			this.scheduler = new CatScheduler(this.stages, this.submitMessageQueue, this.processingQuotaComponent);
			if (Components.IsBridgehead)
			{
				Resolver.InitializePerfCounters();
				bool shuttingDown = Components.ShuttingDown;
			}
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void Unload()
		{
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
		}

		public void Stop()
		{
			if (!this.retired)
			{
				this.Retire();
			}
			this.scheduler.Stop();
		}

		public void Pause()
		{
		}

		public void Continue()
		{
		}

		public void UpdateSubmitQueue()
		{
			if (!this.retired)
			{
				this.submitMessageQueue.TimedUpdate();
			}
		}

		public void DataAvail()
		{
			if (!this.retired)
			{
				this.scheduler.CheckAndScheduleJobThread();
			}
		}

		public void EnqueueSideEffectMessage(IReadOnlyMailItem originalMailItem, TransportMailItem sideEffectMailItem, string agentName)
		{
			this.EnqueueSideEffectMessage(originalMailItem.RootPart.Headers, sideEffectMailItem, agentName);
		}

		public void EnqueueSideEffectMessage(MailItem originalMailItem, TransportMailItem sideEffectMailItem, string agentName)
		{
			this.EnqueueSideEffectMessage(originalMailItem.Message.RootPart.Headers, sideEffectMailItem, agentName);
		}

		private void EnqueueSideEffectMessage(HeaderList headerList, TransportMailItem sideEffectMailItem, string agentName)
		{
			if (!sideEffectMailItem.IsActive)
			{
				throw new InvalidOperationException("Cannot enqueue deleted mailitem!");
			}
			if (sideEffectMailItem.IsReadOnly)
			{
				throw new InvalidOperationException("Cannot enqueue readonly mailitem!");
			}
			bool flag = this.agentLoopChecker.IsEnabledInSubmission();
			bool flag2 = this.agentLoopChecker.CheckAndStampInSubmission(headerList, sideEffectMailItem.RootPart.Headers, agentName);
			if (flag2)
			{
				MessageTrackingLog.TrackAgentGeneratedMessageRejected(MessageTrackingSource.ROUTING, flag, sideEffectMailItem);
			}
			if (flag2 && flag)
			{
				CategorizerComponent.AckAllRecipients(sideEffectMailItem, AckStatus.Fail, SmtpResponse.AgentGeneratedMessageDepthExceeded);
				sideEffectMailItem.ReleaseFromActiveMaterializedLazy();
				return;
			}
			this.EnqueueSubmittedMessage(sideEffectMailItem);
		}

		public SmtpResponse EnqueueSubmittedMessage(TransportMailItem mailItem)
		{
			return this.EnqueueSubmittedMessage(mailItem, null);
		}

		public SmtpResponse EnqueueSubmittedMessage(TransportMailItem mailItem, TaskContext taskContext)
		{
			if (!mailItem.IsActive)
			{
				throw new InvalidOperationException("Cannot enqueue deleted mailitem!");
			}
			if (mailItem.IsReadOnly)
			{
				throw new InvalidOperationException("Cannot enqueue readonly mailitem!");
			}
			mailItem.Recipients.RemoveDuplicates();
			this.EnqueueScopedSubmittedMessage(mailItem, taskContext);
			return SmtpResponse.Empty;
		}

		public void SetLoadTimeDependencies(IProcessingQuotaComponent processingQuotaComponent, IMessageDepotComponent messageDepotComponent)
		{
			this.processingQuotaComponent = processingQuotaComponent;
			this.messageDepotComponent = messageDepotComponent;
			if (messageDepotComponent != null)
			{
				this.isMessageDepotEnabled = messageDepotComponent.Enabled;
				this.messageDepot = messageDepotComponent.MessageDepot;
				return;
			}
			this.isMessageDepotEnabled = false;
			this.messageDepot = null;
		}

		public void MonitorJobs()
		{
			this.scheduler.MonitorJobs();
		}

		void ICategorizerComponentFacade.EnqueueSideEffectMessage(ITransportMailItemFacade originalMailItem, ITransportMailItemFacade sideEffectMailItem, string agentName)
		{
			this.EnqueueSideEffectMessage((TransportMailItem)originalMailItem, (TransportMailItem)sideEffectMailItem, agentName);
		}

		internal static void AckAllRecipients(TransportMailItem transportMailItem, AckStatus status, SmtpResponse response)
		{
			foreach (MailRecipient mailRecipient in transportMailItem.Recipients.AllUnprocessed)
			{
				mailRecipient.Ack(status, response);
			}
		}

		internal void DeferMailItem(TransportMailItem transportMailItem, TaskContext taskContext, DeferReason reason)
		{
			this.DeferMailItem(transportMailItem, taskContext, reason, TimeSpan.FromMinutes(Components.TransportAppConfig.Resolver.ResolverRetryInterval));
		}

		internal void DeferMailItem(TransportMailItem transportMailItem, TaskContext taskContext, DeferReason reason, TimeSpan deferTime)
		{
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Defer);
			transportMailItem.ClearTransportSettings();
			if (taskContext != null)
			{
				taskContext.EndTrackStageLatency(transportMailItem);
			}
			LatencyTracker.EndTrackLatency(LatencyComponent.Categorizer, transportMailItem.LatencyTracker);
			if (deferTime > TimeSpan.Zero)
			{
				((IQueueItem)transportMailItem).DeferUntil = DateTime.UtcNow.Add(deferTime);
				LatencyTracker.BeginTrackLatency(TransportMailItem.GetDeferLatencyComponent(reason), transportMailItem.LatencyTracker);
			}
			transportMailItem.DeferReason = reason;
			MessageTrackingLog.TrackAgentInfo(transportMailItem);
			SystemProbeHelper.SchedulerTracer.TracePass<string, string>(transportMailItem, 0L, "deferring message. reason:{0} period:{1}", reason.ToString(), deferTime.ToString());
			bool flag = this.TrackAndCheckForLocalResubmitLoop(transportMailItem, deferTime);
			bool flag2 = this.NdrIfTooManyDeferrals(transportMailItem, reason);
			if (flag)
			{
				MessageTrackingLog.TrackBadmail(MessageTrackingSource.ROUTING, null, transportMailItem, "message dropped on Defer in Categorizer because it is causing a resubmit loop");
			}
			else if (!flag2)
			{
				if (deferTime > TimeSpan.Zero)
				{
					MessageTrackingLog.TrackDefer(MessageTrackingSource.ROUTING, transportMailItem, null);
					Components.QueueManager.UpdatePerfCountersOnMessageDeferredFromCategorizer();
				}
				else
				{
					MessageTrackingLog.TrackResubmit(MessageTrackingSource.ROUTING, transportMailItem, transportMailItem, reason.ToString());
					Components.QueueManager.UpdatePerfCountersOnMessagesResubmittedFromCategorizer();
				}
			}
			this.FinishedCategorizingMailItem(transportMailItem);
			if (taskContext != null)
			{
				taskContext.MessageDeferred = true;
				taskContext.DeferTime = deferTime;
			}
			if (flag)
			{
				transportMailItem.ReleaseFromActiveMaterializedLazy();
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.MessageDroppedOnDeferInCategorizer);
				return;
			}
			if (flag2)
			{
				transportMailItem.ReleaseFromActiveMaterializedLazy();
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.MessageNDRedOnDeferInCategorizer);
				return;
			}
			this.EnqueueSubmittedMessage(transportMailItem, taskContext);
		}

		internal bool TrackAndCheckForLocalResubmitLoop(TransportMailItem transportMailItem, TimeSpan deferTime)
		{
			bool result = false;
			if (deferTime == TimeSpan.Zero)
			{
				int num = 1 + transportMailItem.ExtendedProperties.GetValue<int>("Microsoft.Exchange.Transport.Categorizer.CategorizerComponent.ResubmitCount", 0);
				transportMailItem.ExtendedProperties.SetValue<int>("Microsoft.Exchange.Transport.Categorizer.CategorizerComponent.ResubmitCount", num);
				result = (num > Components.TransportAppConfig.Routing.MaxAllowedCategorizerResubmits);
			}
			return result;
		}

		internal bool CatContainsMailItem(long recordId)
		{
			return null != this.scheduler.GetScheduledCategorizerItemById(recordId);
		}

		internal CategorizerItem GetCategorizerItemById(long mailItemId)
		{
			CategorizerItem scheduledCategorizerItemById = this.scheduler.GetScheduledCategorizerItemById(mailItemId);
			if (scheduledCategorizerItemById != null)
			{
				return scheduledCategorizerItemById;
			}
			TransportMailItem mailItemById = this.submitMessageQueue.GetMailItemById(mailItemId);
			if (mailItemById != null)
			{
				return new CategorizerItem(mailItemById, -2);
			}
			return null;
		}

		internal bool VisitCategorizerItems(Func<CategorizerItem, bool> visitor, bool recycleInstances)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			this.scheduler.VisitCategorizerItems(visitor);
			CategorizerItem categorizerItem = null;
			return this.submitMessageQueue.VisitMailItems(delegate(TransportMailItem mailItem)
			{
				if (categorizerItem == null || !recycleInstances)
				{
					categorizerItem = new CategorizerItem(mailItem, -1);
				}
				else
				{
					categorizerItem.Reset(mailItem, -1);
				}
				return visitor(categorizerItem);
			});
		}

		internal int GetMailItemCount()
		{
			return this.scheduler.GetMailItemCount() + this.submitMessageQueue.TotalCount;
		}

		internal bool HandleComponentException(Exception e, TaskContext taskContext)
		{
			bool result = false;
			TransportMailItem subjectMailItem = taskContext.SubjectMailItem;
			subjectMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.UnhandledException);
			bool flag = !subjectMailItem.IsActive;
			bool messageDeferred = taskContext.MessageDeferred;
			if (messageDeferred)
			{
				subjectMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ExceptionDeferred);
			}
			if (flag)
			{
				subjectMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ExceptionDeleted);
			}
			ExTraceGlobals.SchedulerTracer.TraceError<long, string, Exception>((long)this.GetHashCode(), "Leaked exception detected (msgId={0}{1}): {2}", subjectMailItem.RecordId, flag ? ", deleted" : (messageDeferred ? ", deferred" : string.Empty), e);
			if (ExceptionHelper.HandleLeakedException)
			{
				if (ExceptionHelper.IsHandleablePermanentException(e))
				{
					if (!flag && !messageDeferred)
					{
						byte[] bytes = Encoding.UTF7.GetBytes(e.Message);
						string @string = Encoding.ASCII.GetString(bytes);
						string text = string.Format(CultureInfo.InvariantCulture, "CAT.InvalidContent.Exception: {0}, {1}; cannot handle content of message with InternalId {2}, InternetMessageId {3}.", new object[]
						{
							e.GetType().Name,
							@string,
							subjectMailItem.RecordId,
							subjectMailItem.InternetMessageId ?? "not available"
						});
						CategorizerComponent.AckAllRecipients(subjectMailItem, AckStatus.Fail, new SmtpResponse("550", "5.6.0", new string[]
						{
							text
						}));
						ExTraceGlobals.SchedulerTracer.TraceError<long>((long)this.GetHashCode(), "Message (msgId={0}) rejected due to leaked exception", subjectMailItem.RecordId);
						subjectMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ExceptionHandledPermanent);
						this.BifurcateAndHandleFailedMessage(subjectMailItem, taskContext);
					}
					else
					{
						subjectMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ExceptionHandledDeletedDeferred);
					}
					result = true;
				}
				else if (ExceptionHelper.IsHandleableTransientException(e))
				{
					if (!flag && !messageDeferred)
					{
						subjectMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ExceptionHandledTransient);
						byte[] bytes2 = Encoding.UTF7.GetBytes(e.Message);
						string string2 = Encoding.ASCII.GetString(bytes2);
						string text2 = string.Format(CultureInfo.InvariantCulture, "CAT.Transient.Exception: {0}, {1}", new object[]
						{
							e.GetType().Name,
							string2
						});
						CategorizerComponent.AckAllRecipients(subjectMailItem, AckStatus.Retry, new SmtpResponse("420", "4.2.0", new string[]
						{
							text2
						}));
						Components.CategorizerComponent.DeferMailItem(subjectMailItem, taskContext, DeferReason.TransientFailure);
						ExTraceGlobals.SchedulerTracer.TraceError<long>((long)this.GetHashCode(), "Message (msgId={0}) deferred due to leaked exception", subjectMailItem.RecordId);
					}
					else
					{
						subjectMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ExceptionHandledDeletedDeferred);
					}
					result = true;
				}
			}
			return result;
		}

		internal void DeferBifurcatedMailItem(TransportMailItem mailItem, TaskContext taskContext, DeferReason reason, TimeSpan deferTime)
		{
			this.MailItemBifurcatedInCategorizer(mailItem);
			this.DeferMailItem(mailItem, taskContext, reason, deferTime);
		}

		internal void MailItemBifurcatedInCategorizer(TransportMailItem mailItem)
		{
			mailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ChildBifurcate);
			Components.QueueManager.UpdatePerfCountersOnMessageBifurcatedInCategorizer();
			CategorizerComponent.UpdateNewMailItemWithActivityContext(mailItem);
		}

		internal void FinishedCategorizingMailItem(TransportMailItem mailItem)
		{
			Components.QueueManager.UpdatePerfCountersOnLeavingCategorizer();
			CategorizerComponent.LogAndClearActivityScope(mailItem);
		}

		internal void TimedUpdate()
		{
			this.scheduler.TimedUpdate();
			Components.QueueManager.TimeUpdatePerfCounters();
		}

		private static bool HasTiRecipients(TransportMailItem mailItem)
		{
			foreach (MailRecipient mailRecipient in mailItem.Recipients.AllUnprocessed)
			{
				if (mailRecipient.NextHop.NextHopType.DeliveryType == DeliveryType.SmtpRelayToTiRg)
				{
					return true;
				}
			}
			return false;
		}

		private static void SetBanner(TransportMailItem transportMailItem)
		{
			List<string> list = ClassificationUtils.ExtractClassifications(transportMailItem.RootPart.Headers);
			ClassificationUtils.DropStoreLabels(transportMailItem.RootPart.Headers);
			if (list.Count > 0 && transportMailItem.Message.TnefPart != null && CategorizerComponent.HasTiRecipients(transportMailItem))
			{
				ClassificationSummary classificationSummary = Components.ClassificationConfig.Summarize(transportMailItem.OrganizationId, list, null);
				if (classificationSummary.IsValid)
				{
					if (classificationSummary.IsClassified)
					{
						Util.SetMessageClassificationTnef(transportMailItem, true, classificationSummary.DisplayName, classificationSummary.RecipientDescription, classificationSummary.ClassificationID.ToString(), classificationSummary.RetainClassificationEnabled);
						return;
					}
				}
				else
				{
					CategorizerComponent.AckAllRecipients(transportMailItem, AckStatus.Fail, AckReason.UnrecognizedClassification);
				}
			}
		}

		private static void UpdateNextHopWithRiskLevel(TransportMailItem mailItem)
		{
			foreach (MailRecipient mailRecipient in mailItem.Recipients.AllUnprocessed)
			{
				if (mailRecipient.NextHop.NextHopType.IsSmtpConnectorDeliveryType && (mailItem.RiskLevel != RiskLevel.Normal || mailRecipient.OutboundIPPool != 0))
				{
					mailRecipient.NextHop = new NextHopSolutionKey(mailRecipient.NextHop.NextHopType.DeliveryType, mailRecipient.NextHop.NextHopDomain, mailRecipient.NextHop.NextHopConnector, mailRecipient.NextHop.TlsAuthLevel, mailRecipient.NextHop.NextHopTlsDomain, mailItem.RiskLevel, mailRecipient.OutboundIPPool, mailRecipient.NextHop.OverrideSource);
				}
			}
		}

		private static void AddRecipientsToSolutionTable(TransportMailItem mailItem)
		{
			foreach (MailRecipient mailRecipient in mailItem.Recipients.AllUnprocessed)
			{
				if (!mailRecipient.NextHop.IsEmpty)
				{
					mailItem.UpdateNextHopSolutionTable(mailRecipient.NextHop, mailRecipient);
				}
			}
		}

		private static void AckSuppressedRecipients(TransportMailItem transportMailItem)
		{
			SmtpResponse ackReason = AckReason.SubmissionCancelledRecipientMdbNotTargetted;
			List<MailRecipient> list;
			if (CategorizerComponent.CheckDropProbeMessage(transportMailItem))
			{
				ackReason = AckReason.ProbeMessageDropped;
				list = new List<MailRecipient>(transportMailItem.Recipients.AllUnprocessed);
			}
			else
			{
				bool flag;
				list = MessageResubmissionComponent.GetSuppressedRecipients(transportMailItem, out flag);
				if (flag)
				{
					ackReason = AckReason.SubmissionCancelledProbeRequest;
				}
			}
			list.ForEach(delegate(MailRecipient cancelledRecipient)
			{
				cancelledRecipient.Ack(AckStatus.SuccessNoDsn, ackReason);
			});
			if (!list.Any<MailRecipient>())
			{
				return;
			}
			MessageTrackingSource source = (ackReason == AckReason.ProbeMessageDropped) ? MessageTrackingSource.ROUTING : MessageTrackingSource.SAFETYNET;
			LatencyFormatter latencyFormatter = new LatencyFormatter(transportMailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, true);
			MessageTrackingLog.TrackResubmitCancelled(source, transportMailItem, list, ackReason, latencyFormatter);
		}

		private static void LogAndClearActivityScope(TransportMailItem transportMailItem)
		{
			if (transportMailItem.ActivityScope == null)
			{
				throw new InvalidOperationException("Activity scope not set in mail item");
			}
			TransportWlmLog.LogActivity(transportMailItem.InternetMessageId, transportMailItem.ExternalOrganizationId, "CAT", transportMailItem.ActivityScope);
			transportMailItem.ActivityScope.End();
		}

		private static void UpdateNewMailItemWithActivityContext(TransportMailItem mailItem)
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			ActivityContext.ClearThreadScope();
			mailItem.ActivityScope = ActivityContext.Start(null);
			ActivityContext.ClearThreadScope();
			if (currentActivityScope != null)
			{
				ActivityContext.SetThreadScope(currentActivityScope);
			}
		}

		private static void StampTransmittableMailboxDeliveryProperties(TransportMailItem transportMailItem)
		{
			transportMailItem.ExtendedProperties.SetValue<ulong>("Microsoft.Exchange.Transport.MailboxTransport.InternalMessageId", (ulong)transportMailItem.RecordId);
			transportMailItem.ExtendedProperties.SetValue<string>(SmtpMessageContextBlob.ProcessTransportRoleKey, Components.Configuration.ProcessTransportRole.ToString());
		}

		private void EnqueueScopedSubmittedMessage(TransportMailItem mailItem, TaskContext taskContext)
		{
			mailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Enqueue);
			mailItem.PerfCounterAttribution = "Cat";
			ExTraceGlobals.SchedulerTracer.TracePfd<int, long>((long)this.GetHashCode(), "PFD CAT {0} Submit Enqueue (msgId={1})", 26103, mailItem.RecordId);
			if (Components.ResourceManager.ShouldShrinkDownMemoryCaches && (mailItem.DeferUntil != DateTime.MinValue || this.submitMessageQueue.Suspended || this.submitMessageQueue.ActiveCount > 20))
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), "Message {0} is dehydrated due to high memory pressure.", mailItem.RecordId);
				mailItem.CommitLazyAndDehydrate(Breadcrumb.DehydrateOnCategorizerMemoryPressure);
			}
			Components.ShadowRedundancyComponent.ShadowRedundancyManager.NotifyMailItemPreEnqueuing(mailItem, this.submitMessageQueue);
			if (!this.isMessageDepotEnabled)
			{
				this.submitMessageQueue.Enqueue(mailItem);
				return;
			}
			MessageDepotMailItem messageDepotMailItem = new MessageDepotMailItem(mailItem);
			AcquireToken acquireToken;
			if (taskContext != null && taskContext.TryGetDeferToken(mailItem, out acquireToken))
			{
				this.messageDepot.DeferMessage(messageDepotMailItem.Id, taskContext.DeferTime, acquireToken);
				return;
			}
			this.messageDepot.Add(messageDepotMailItem);
		}

		private IList<TransportMailItem> BifurcateByRecipSettings(TransportMailItem transportMailItem)
		{
			RecipSettingBifurcator bifurcationHelper = new RecipSettingBifurcator(transportMailItem);
			MailItemBifurcator<TemplateWithHistory> mailItemBifurcator = new MailItemBifurcator<TemplateWithHistory>(transportMailItem, bifurcationHelper);
			List<TransportMailItem> bifurcatedMailItems = mailItemBifurcator.GetBifurcatedMailItems();
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ParentBifurcate);
			foreach (TransportMailItem mailItem in bifurcatedMailItems)
			{
				this.MailItemBifurcatedInCategorizer(mailItem);
			}
			return bifurcatedMailItems;
		}

		private TaskCompletion Stage1OnSubmitted(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage1);
			ExTraceGlobals.SchedulerTracer.TracePfd<int, long>((long)this.GetHashCode(), " PFD CAT {0}  Stage1 OnSubmitted(msgId={1})", 18935, transportMailItem.RecordId);
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(4232457533U, transportMailItem.Subject);
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(2957389117U, transportMailItem.Subject);
			MailRecipientCollection recipients = transportMailItem.Recipients;
			foreach (MailRecipient mailRecipient in recipients)
			{
				mailRecipient.ExposeRoutingDomain = false;
			}
			transportMailItem.CacheTransportSettings();
			this.UpdateUMMessageClass(transportMailItem);
			Util.SetAsciiHeader(transportMailItem.RootPart.Headers, "X-MS-Exchange-Organization-HygienePolicy", transportMailItem.TransportSettings.HygieneSuite);
			TransportMailItemWrapper mailItem = taskContext.CreatePublicWrapper(true);
			transportMailItem.PerfCounterAttribution = "Cat event 1";
			if (!transportMailItem.ExtendedProperties.GetValue<bool>("Microsoft.Exchange.Transport.Categorizer.CategorizerComponent.MessageLatencyHeaderStamped", false))
			{
				if (TransportMailItemWrapper.GetInboundDeliveryMethod(transportMailItem) == DeliveryMethod.Smtp)
				{
					LatencyComponent previousHopDeliveryLatencyComponent = LatencyComponent.DeliveryQueueInternal;
					if (transportMailItem.ExtendedProperties.GetValue<uint>("Microsoft.Exchange.Transport.TransportMailItem.InboundProxySequenceNumber", 0U) != 0U)
					{
						previousHopDeliveryLatencyComponent = LatencyComponent.SmtpSend;
					}
					else if (transportMailItem.RootPart != null && transportMailItem.RootPart.Headers != null)
					{
						Header header = transportMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Processed-By-MBTSubmission");
						if (header != null)
						{
							previousHopDeliveryLatencyComponent = LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionSmtpOut;
							transportMailItem.RootPart.Headers.RemoveAll("X-MS-Exchange-Organization-Processed-By-MBTSubmission");
						}
					}
					LatencyHeaderManager.HandleLatencyHeaders(transportMailItem.TransportSettings.InternalSMTPServers, transportMailItem.RootPart.Headers, transportMailItem.DateReceived, previousHopDeliveryLatencyComponent);
				}
				transportMailItem.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.Categorizer.CategorizerComponent.MessageLatencyHeaderStamped", true);
			}
			taskContext.SaveMimeVersion(transportMailItem);
			taskContext.AgentLatencyTracker.BeginTrackLatency(LatencyComponent.CategorizerOnSubmittedMessage, transportMailItem.LatencyTracker);
			IAsyncResult result = MExEvents.RaiseOnSubmittedMessage(taskContext, new AsyncCallback(this.EventCallback), mailItem);
			return this.HandleMExResult(result, transportMailItem, taskContext);
		}

		private void UpdateUMMessageClass(TransportMailItem mailItem)
		{
			EmailMessage message = mailItem.Message;
			string mapiMessageClass = message.MapiMessageClass;
			if (string.IsNullOrEmpty(mapiMessageClass) || !ObjectClass.IsUMMessage(mapiMessageClass) || MultilevelAuth.IsAuthenticated(mailItem))
			{
				return;
			}
			if (message.TnefPart == null)
			{
				HeaderList headers = message.MimeDocument.RootPart.Headers;
				Header header = headers.FindFirst("Content-Class");
				if (header != null)
				{
					header.Value = "unauthenticated," + header.Value;
					ExTraceGlobals.SchedulerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Set MIME content-class to {0} for anonymous UM message with ID='{1}'", header.Value, message.MessageId);
					return;
				}
			}
			else
			{
				message.MapiProperties.SetProperty(TnefPropertyTag.MessageClassW, "IPM.Note");
				ExTraceGlobals.SchedulerTracer.TraceDebug<string>((long)this.GetHashCode(), "Set MessageClass MAPI property to IPM.Note for anonymous UM message with ID='{0}'", message.MessageId);
			}
		}

		private TaskCompletion Stage2ResolveEngine(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			Dictionary<string, List<RoutingAddress>> catchAllOriginalRecipientsMap = null;
			ExTraceGlobals.SchedulerTracer.TracePfd<int, long>((long)this.GetHashCode(), " PFD CAT {0} Stage2 ResolveEngine(msgId={1})", 27127, transportMailItem.RecordId);
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage2);
			if (Components.TransportAppConfig.Resolver.IsResolverEnabled)
			{
				Resolver resolver = null;
				try
				{
					resolver = new Resolver(transportMailItem, taskContext, new Resolver.DeferBifurcatedDelegate(this.DeferBifurcatedMailItem));
					resolver.ResolveAll();
					if (this.ResolveCatchAllRecipient(transportMailItem, resolver, taskContext, out catchAllOriginalRecipientsMap) == TaskCompletion.Completed)
					{
						return TaskCompletion.Completed;
					}
				}
				catch (ADTransientException)
				{
					this.HandleStage2TransientError(transportMailItem, resolver, taskContext, "Transient failure occurred while resolving; reloading mail item and deferring.", DeferReason.ADTransientFailureDuringResolve);
					return TaskCompletion.Completed;
				}
			}
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage2Completed);
			this.DeleteOrChainToNext(transportMailItem, taskContext);
			this.SendMailToCatchAllRecipient(transportMailItem, catchAllOriginalRecipientsMap);
			return TaskCompletion.Completed;
		}

		private TaskCompletion ResolveCatchAllRecipient(TransportMailItem transportMailItem, Resolver resolver, TaskContext taskContext, out Dictionary<string, List<RoutingAddress>> catchAllOriginalRecipientsMap)
		{
			Dictionary<string, List<RoutingAddress>> dictionary;
			catchAllOriginalRecipientsMap = (dictionary = new Dictionary<string, List<RoutingAddress>>());
			catchAllOriginalRecipientsMap = dictionary;
			bool flag = transportMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-CatchAll-OriginalRecipients") != null;
			bool flag2 = transportMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Sharing-Instance-Guid") != null;
			if (flag || flag2)
			{
				return TaskCompletion.Pending;
			}
			string text = string.Empty;
			PerTenantAcceptedDomainTable perTenantAcceptedDomainTable = null;
			foreach (MailRecipient mailRecipient in transportMailItem.Recipients)
			{
				if (mailRecipient.SmtpResponse.Equals(SmtpResponse.RcptNotFound))
				{
					if (perTenantAcceptedDomainTable == null && !Components.Configuration.TryGetAcceptedDomainTable(transportMailItem.OrganizationId, out perTenantAcceptedDomainTable))
					{
						this.HandleStage2TransientError(transportMailItem, resolver, taskContext, "Unable to get the accepted domain table; reloading mail item and deferring.", DeferReason.TransientAcceptedDomainsLoadFailure);
						return TaskCompletion.Completed;
					}
					AcceptedDomainEntry domainEntry = perTenantAcceptedDomainTable.AcceptedDomainTable.GetDomainEntry(SmtpDomain.GetDomainPart(mailRecipient.Email));
					if (domainEntry != null && domainEntry.CatchAllRecipientID != Guid.Empty)
					{
						Result<TransportMiniRecipient> result = transportMailItem.ADRecipientCache.FindAndCacheRecipient(new ADObjectId(domainEntry.CatchAllRecipientID));
						if (result.Data != null)
						{
							if (string.IsNullOrEmpty(text))
							{
								text = string.Join(",", (from r in transportMailItem.Recipients
								select r.Email.ToString()).ToArray<string>());
							}
							string text2 = result.Data.PrimarySmtpAddress.ToString();
							if (!text.Contains(text2))
							{
								List<RoutingAddress> list;
								if (!catchAllOriginalRecipientsMap.TryGetValue(text2, out list))
								{
									list = new List<RoutingAddress>();
									catchAllOriginalRecipientsMap.Add(text2, list);
								}
								list.Add(mailRecipient.Email);
							}
							mailRecipient.Ack(AckStatus.SuccessNoDsn, AckReason.ForwardedToAlternateRecipient);
						}
						else
						{
							ExTraceGlobals.SchedulerTracer.TraceWarning<Guid, long>((long)this.GetHashCode(), "Unable to find the catch-all recipient from AD. Guid: {0}", domainEntry.CatchAllRecipientID, transportMailItem.RecordId);
						}
					}
				}
			}
			return TaskCompletion.Pending;
		}

		private void SendMailToCatchAllRecipient(TransportMailItem transportMailItem, Dictionary<string, List<RoutingAddress>> catchAllOriginalRecipientsMap)
		{
			if (catchAllOriginalRecipientsMap == null)
			{
				return;
			}
			foreach (KeyValuePair<string, List<RoutingAddress>> keyValuePair in catchAllOriginalRecipientsMap)
			{
				TransportMailItem transportMailItem2 = transportMailItem.NewCloneWithoutRecipients();
				transportMailItem2.Recipients.Add(keyValuePair.Key);
				transportMailItem2.ResetToActive();
				transportMailItem2.PrioritizationReason = "CatchAllRecipient";
				transportMailItem2.Priority = DeliveryPriority.Low;
				string value = string.Join(";", (from r in keyValuePair.Value
				select r.ToString()).ToArray<string>());
				Header newChild = new AsciiTextHeader("X-MS-Exchange-Organization-CatchAll-OriginalRecipients", value);
				AutoResponseSuppress autoResponseSuppress = AutoResponseSuppress.DR | AutoResponseSuppress.RN | AutoResponseSuppress.NRN | AutoResponseSuppress.OOF | AutoResponseSuppress.AutoReply;
				Header newChild2 = new AsciiTextHeader("X-Auto-Response-Suppress", autoResponseSuppress.ToString());
				transportMailItem2.RootPart.Headers.AppendChild(newChild);
				transportMailItem2.RootPart.Headers.AppendChild(newChild2);
				MessageTrackingLog.TrackTransfer(MessageTrackingSource.ROUTING, transportMailItem2, transportMailItem.RecordId, "CatchAllRedirect");
				if (Resolver.PerfCounters != null)
				{
					Resolver.PerfCounters.CatchAllRecipientsTotal.Increment();
				}
				this.EnqueueSubmittedMessage(transportMailItem2);
			}
		}

		private void HandleStage2TransientError(TransportMailItem transportMailItem, Resolver resolver, TaskContext taskContext, string errorMessage, DeferReason deferReason)
		{
			ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), errorMessage, transportMailItem.RecordId);
			if (resolver != null)
			{
				resolver.CommitBackupMailItem();
			}
			this.DeferMailItem(transportMailItem, taskContext, deferReason);
			if (Resolver.PerfCounters != null)
			{
				Resolver.PerfCounters.MessagesRetriedTotal.Increment();
			}
		}

		private TaskCompletion Stage3OnResolvedMessage(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			ExTraceGlobals.SchedulerTracer.TracePfd<int, long>((long)this.GetHashCode(), " PFD CAT {0} Stage3 OnResolvedMessage(msgId={1})", 44521, transportMailItem.RecordId);
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage3);
			TransportMailItemWrapper mailItem = taskContext.CreatePublicWrapper(false);
			MailRecipientCollection recipients = transportMailItem.Recipients;
			foreach (MailRecipient mailRecipient in recipients)
			{
				mailRecipient.ExposeRoutingDomain = true;
			}
			transportMailItem.PerfCounterAttribution = "Cat event 3";
			taskContext.SaveMimeVersion(transportMailItem);
			taskContext.AgentLatencyTracker.BeginTrackLatency(LatencyComponent.CategorizerOnResolvedMessage, transportMailItem.LatencyTracker);
			IAsyncResult result = MExEvents.RaiseOnResolvedMessage(taskContext, new AsyncCallback(this.EventCallback), mailItem);
			return this.HandleMExResult(result, transportMailItem, taskContext);
		}

		private TaskCompletion Stage4Routing(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			ExTraceGlobals.SchedulerTracer.TracePfd<int, long>((long)this.GetHashCode(), " PFD CAT {0} Stage4 Routing(msgId={1})", 23031, transportMailItem.RecordId);
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage4);
			MailRecipientCollection recipients = transportMailItem.Recipients;
			foreach (MailRecipient mailRecipient in recipients)
			{
				mailRecipient.ExposeRoutingDomain = false;
			}
			this.RouteMessage(transportMailItem, taskContext);
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage4Completed);
			this.DeleteOrChainToNext(transportMailItem, taskContext);
			return TaskCompletion.Completed;
		}

		private void RouteMessage(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			Components.RoutingComponent.MailRouter.RouteToMultipleDestinations(transportMailItem, taskContext);
		}

		private TaskCompletion Stage5OnRoutedMessage(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			ExTraceGlobals.SchedulerTracer.TracePfd<int, long>((long)this.GetHashCode(), " PFD CAT {0} Stage5 OnRoutedMessage(msgId={1})", 16887, transportMailItem.RecordId);
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage5);
			TransportMailItemWrapper mailItem = taskContext.CreatePublicWrapper(false);
			transportMailItem.PerfCounterAttribution = "Cat event 6";
			taskContext.SaveMimeVersion(transportMailItem);
			taskContext.AgentLatencyTracker.BeginTrackLatency(LatencyComponent.CategorizerOnRoutedMessage, transportMailItem.LatencyTracker);
			IAsyncResult result = MExEvents.RaiseOnRoutedMessage(taskContext, new AsyncCallback(this.EventCallback), mailItem);
			return this.HandleMExResult(result, transportMailItem, taskContext);
		}

		private TaskCompletion Stage6ContentConversion(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			ExTraceGlobals.SchedulerTracer.TracePfd<int, long>((long)this.GetHashCode(), " PFD CAT {0} Stage6 ContentConversion(msgId={1})", 31223, transportMailItem.RecordId);
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage6);
			if (!Components.IsBridgehead || DatacenterRegistry.IsForefrontForOffice())
			{
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage6NoConversion);
				this.DeleteOrChainToNext(transportMailItem, taskContext);
				return TaskCompletion.Completed;
			}
			MessageTrackingLog.TrackAgentInfo(transportMailItem);
			return this.ConvertMailItemContent(transportMailItem, taskContext);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private TaskCompletion ConvertMailItemContent(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			try
			{
				MailItemBifurcator<RecipientEncoding> mailItemBifurcator = new MailItemBifurcator<RecipientEncoding>(transportMailItem, new ContentConverter(transportMailItem));
				List<TransportMailItem> bifurcatedMailItems = mailItemBifurcator.GetBifurcatedMailItems();
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ParentBifurcate);
				foreach (TransportMailItem transportMailItem2 in bifurcatedMailItems)
				{
					Components.CategorizerComponent.MailItemBifurcatedInCategorizer(transportMailItem2);
					this.DeleteOrChainToNext(transportMailItem2, taskContext);
				}
				if (bifurcatedMailItems.Count != 0)
				{
					transportMailItem.CommitLazy();
				}
			}
			catch (ADTransientException arg)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug<ADTransientException, long>((long)this.GetHashCode(), "Transient failure {0} occurred while doing content conversion for mail item {1}.", arg, transportMailItem.RecordId);
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ConversionException);
				this.DeferMailItem(transportMailItem, taskContext, DeferReason.StorageTransientFailureDuringContentConversion);
				return TaskCompletion.Completed;
			}
			catch (DataValidationException ex)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug<DataValidationException>((long)this.GetHashCode(), "Invalid data in AD: {0}", ex);
				this.LogMessage(transportMailItem, ex);
				byte[] bytes = Encoding.UTF7.GetBytes(ex.Message);
				string @string = Encoding.ASCII.GetString(bytes);
				string errorDetails = string.Format(CultureInfo.InvariantCulture, "M2MCVT.InvalidContent.Exception: {0}, {1}; invalid message content.", new object[]
				{
					ex.GetType().Name,
					@string
				});
				CategorizerComponent.AckAllRecipients(transportMailItem, AckStatus.Fail, AckReason.MimeToMimeInvalidContent(errorDetails));
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ConversionExceptionAck);
			}
			catch (StoragePermanentException ex2)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug<StoragePermanentException>((long)this.GetHashCode(), "XSO StorageException: {0}", ex2);
				this.LogMessage(transportMailItem, ex2);
				byte[] bytes2 = Encoding.UTF7.GetBytes(ex2.Message);
				string string2 = Encoding.ASCII.GetString(bytes2);
				string errorDetails2 = string.Format(CultureInfo.InvariantCulture, "M2MCVT.StorageError.Exception: {0}, {1}; storage error in content conversion.", new object[]
				{
					ex2.GetType().Name,
					string2
				});
				CategorizerComponent.AckAllRecipients(transportMailItem, AckStatus.Fail, AckReason.MimeToMimeStorageError(errorDetails2));
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ConversionExceptionAck);
			}
			catch (StorageTransientException arg2)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug<StorageTransientException>((long)this.GetHashCode(), "XSO Exception: {0}", arg2);
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ConversionException);
				this.DeferMailItem(transportMailItem, taskContext, DeferReason.StorageTransientFailureDuringContentConversion);
				return TaskCompletion.Completed;
			}
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ConversionCompleted);
			this.DeleteOrChainToNext(transportMailItem, taskContext);
			return TaskCompletion.Completed;
		}

		private TaskCompletion Stage7BifurcateByRecipSettings(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage7);
			IList<TransportMailItem> list = this.BifurcateByRecipSettings(transportMailItem);
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ParentBifurcate);
			foreach (TransportMailItem transportMailItem2 in list)
			{
				transportMailItem2.DropCatBreadcrumb(CategorizerBreadcrumb.ChildBifurcate);
				this.DeleteOrChainToNext(transportMailItem2, taskContext);
			}
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage7Completed);
			this.DeleteOrChainToNext(transportMailItem, taskContext);
			return TaskCompletion.Completed;
		}

		private TaskCompletion Stage8OnCategorizedMessage(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			TransportMailItemWrapper mailItem = taskContext.CreatePublicWrapper(false);
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage8);
			transportMailItem.PerfCounterAttribution = "Cat event 8";
			taskContext.SaveMimeVersion(transportMailItem);
			taskContext.AgentLatencyTracker.BeginTrackLatency(LatencyComponent.CategorizerOnCategorizedMessage, transportMailItem.LatencyTracker);
			IAsyncResult result = MExEvents.RaiseOnCategorizedMessage(taskContext, new AsyncCallback(this.EventCallback), mailItem);
			return this.HandleMExResult(result, transportMailItem, taskContext);
		}

		private TaskCompletion Stage9StuffAndDSN(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			ExTraceGlobals.SchedulerTracer.TracePfd<int, long>((long)this.GetHashCode(), " PFD CAT {0} Stage9 StuffAndDSN(msgId={1})", 25079, transportMailItem.RecordId);
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage9);
			if (transportMailItem.Message.ContentManager != null)
			{
				transportMailItem.Message.ContentManager.Dispose();
				transportMailItem.Message.ContentManager = null;
			}
			transportMailItem.Recipients.RemoveDuplicatesToSameRoute();
			transportMailItem.Recipients.Sort(Resolver.RecipientHomeMDBComparer);
			if (MultiTenantTransport.MultiTenancyEnabled)
			{
				transportMailItem.UpdateDirectionalityAndScopeHeaders();
			}
			CategorizerComponent.SetBanner(transportMailItem);
			CategorizerComponent.AckSuppressedRecipients(transportMailItem);
			Components.RoutingComponent.MailRouter.ApplyDelayedFanout(transportMailItem);
			CategorizerComponent.UpdateNextHopWithRiskLevel(transportMailItem);
			CategorizerComponent.AddRecipientsToSolutionTable(transportMailItem);
			Components.OrarGenerator.GenerateOrarMessage(transportMailItem);
			Components.DsnGenerator.GenerateDSNs(transportMailItem, DsnGenerator.CallerComponent.ResolverRouting);
			MessageTrackingLog.TrackAgentInfo(transportMailItem);
			MessageTrackingLog.TrackRelayedAndFailed(MessageTrackingSource.ROUTING, transportMailItem, transportMailItem.Recipients, null);
			this.ClearUnusedRecipientCache(transportMailItem);
			taskContext.EndTrackStageLatency(transportMailItem);
			LatencyTracker.EndTrackLatency(LatencyComponent.Categorizer, transportMailItem.LatencyTracker);
			LatencyHeaderManager.FinalizeLatencyHeadersOnHub(transportMailItem, Components.Configuration.LocalServer.TransportServer.Fqdn);
			CategorizerComponent.StampTransmittableMailboxDeliveryProperties(transportMailItem);
			this.FinishedCategorizingMailItem(transportMailItem);
			if (transportMailItem.NextHopSolutions == null || transportMailItem.NextHopSolutions.Count == 0)
			{
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Deleted);
				transportMailItem.ReleaseFromActiveMaterializedLazy();
			}
			else
			{
				taskContext.PromoteHeadersIfChanged(transportMailItem);
				transportMailItem.PerfCounterAttribution = "Queue";
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.EnqueueDelivery);
				Components.RemoteDeliveryComponent.QueueMessageForNextHop(transportMailItem);
			}
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Stage9Completed);
			return TaskCompletion.Completed;
		}

		private void EventCallback(IAsyncResult ar)
		{
			if (!ar.CompletedSynchronously)
			{
				TaskContext taskContext = (TaskContext)ar.AsyncState;
				TransportMailItem subjectMailItem = taskContext.SubjectMailItem;
				subjectMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.EventCallback);
				using (MailItemTraceFilter mailItemTraceFilter = default(MailItemTraceFilter))
				{
					bool flag = !subjectMailItem.IsActive || taskContext.MessageDeferred;
					if (!flag)
					{
						mailItemTraceFilter.SetMailItem(subjectMailItem);
					}
					IMExSession mexSession = taskContext.MexSession;
					PoisonMessage.Context = new MessageContext(subjectMailItem.RecordId, subjectMailItem.InternetMessageId, MessageProcessingSource.Categorizer);
					ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), "Async MEx Event Callback(msgId={0})", subjectMailItem.RecordId);
					try
					{
						MExEvents.EndEvent(mexSession, ar);
					}
					catch (Exception e)
					{
						subjectMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.EventException);
						bool flag2 = this.HandleComponentException(e, taskContext);
						flag = (!subjectMailItem.IsActive || taskContext.MessageDeferred);
						if (!flag2)
						{
							subjectMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.EventExceptionUnhandled);
							throw;
						}
					}
					finally
					{
						taskContext.AgentLatencyTracker.EndTrackLatency(!flag);
					}
					this.PostEventProcessing(subjectMailItem, taskContext);
					taskContext.TaskCompletedAsync();
				}
			}
		}

		private void DeleteOrChainToNext(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), "ProcessAnyDSNs (msgId={0})", transportMailItem.RecordId);
			if (transportMailItem.Recipients.CountUnprocessed() != 0)
			{
				taskContext.ChainItemToNext(transportMailItem);
				return;
			}
			this.BifurcateAndHandleFailedMessage(transportMailItem, taskContext);
		}

		private void BifurcateAndHandleFailedMessage(TransportMailItem mailItem, TaskContext taskContext)
		{
			if (taskContext.Stage >= 1 && taskContext.Stage < 6)
			{
				IList<TransportMailItem> list = this.BifurcateByRecipSettings(mailItem);
				foreach (TransportMailItem transportMailItem in list)
				{
					transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.BifurcateAndFail);
					this.HandleFailedMessage(transportMailItem, taskContext);
				}
			}
			mailItem.DropCatBreadcrumb(CategorizerBreadcrumb.Fail);
			this.HandleFailedMessage(mailItem, taskContext);
		}

		private void HandleFailedMessage(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			bool flag = false;
			if (transportMailItem.RetryDeliveryIfRejected)
			{
				foreach (MailRecipient mailRecipient in transportMailItem.Recipients.All)
				{
					if (mailRecipient.IsActive && (mailRecipient.AckStatus == AckStatus.Fail || mailRecipient.AckStatus == AckStatus.Retry))
					{
						Resolver.ClearResolverProperties(mailRecipient);
						if (mailRecipient.AckStatus == AckStatus.Fail)
						{
							SmtpResponse smtpResponse = TransportMailItem.ReplaceFailWithRetryResponse(mailRecipient.SmtpResponse);
							mailRecipient.Ack(AckStatus.Retry, smtpResponse);
						}
						flag = true;
					}
				}
			}
			if (flag)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), "Resubmit due to RetryDeliveryIfRejected (msgId={0})", transportMailItem.RecordId);
				CategorizerComponent.logger.LogEvent(TransportEventLogConstants.Tuple_RetryCategorizationIfFailed, "RetryCategorizationIfFailed", new object[]
				{
					transportMailItem.RecordId
				});
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "RetryCategorizationIfFailed", null, "Couldn't categorize a non-expirable message.", ResultSeverityLevel.Warning, false);
				Resolver.ClearResolverAndTransportSettings(transportMailItem);
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.ResubmitFail);
				this.DeferMailItem(transportMailItem, taskContext, DeferReason.MarkedAsRetryDeliveryIfRejected);
				return;
			}
			Components.OrarGenerator.GenerateOrarMessage(transportMailItem);
			if (taskContext.Stage != 1)
			{
				Components.DsnGenerator.GenerateDSNs(transportMailItem, DsnGenerator.CallerComponent.ResolverRouting);
			}
			else
			{
				Components.DsnGenerator.GenerateDSNs(transportMailItem, DsnGenerator.CallerComponent.Other);
			}
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.DsnGenerated);
			MessageTrackingLog.TrackAgentInfo(transportMailItem);
			MessageTrackingLog.TrackRelayedAndFailed(MessageTrackingSource.ROUTING, transportMailItem, transportMailItem.Recipients, null);
			this.FinishedCategorizingMailItem(transportMailItem);
			TaskContext.ReleaseItem(transportMailItem);
		}

		private void PostEventProcessing(TransportMailItem transportMailItem, TaskContext taskContext)
		{
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.PostEventProcessing);
			transportMailItem.PerfCounterAttribution = "Cat";
			taskContext.ClosePublicWrapper();
			if (taskContext.MessageDeferred)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), "Message was deferred earlier (msgId={0})", transportMailItem.RecordId);
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.PostEventDeferred);
				if (taskContext.DeferTime > TimeSpan.Zero)
				{
					Components.QueueManager.UpdatePerfCountersOnMessageDeferredFromCategorizer();
				}
				else
				{
					Components.QueueManager.UpdatePerfCountersOnMessagesResubmittedFromCategorizer();
				}
				this.FinishedCategorizingMailItem(transportMailItem);
				return;
			}
			if (transportMailItem.IsActive)
			{
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.PostEventNotDeferred);
				taskContext.PromoteHeadersIfChanged(transportMailItem);
				this.DeleteOrChainToNext(transportMailItem, taskContext);
				return;
			}
			if (transportMailItem.PendingDatabaseUpdates)
			{
				throw new InvalidOperationException("TransportMailItem released from active in CAT agent and was not committed to DB");
			}
			ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), "Message was released from active earlier (msgId={0})", transportMailItem.RecordId);
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.PostEventDeleted);
			this.FinishedCategorizingMailItem(transportMailItem);
		}

		private void ClearUnusedRecipientCache(TransportMailItem transportMailItem)
		{
			bool flag = true;
			foreach (NextHopSolutionKey nextHopSolutionKey in transportMailItem.NextHopSolutions.Keys)
			{
				if (nextHopSolutionKey.NextHopType.DeliveryType == DeliveryType.MapiDelivery)
				{
					flag = false;
					break;
				}
				if (Components.TransportAppConfig.MessageContextBlobConfiguration.TransferADRecipientCache && (nextHopSolutionKey.NextHopType.DeliveryType == DeliveryType.SmtpDeliveryToMailbox || nextHopSolutionKey.NextHopType.IsHubRelayDeliveryType))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), "msgId={0} no MapiDelivery recips - clear ADRecipient cache", transportMailItem.RecordId);
				transportMailItem.ADRecipientCache.Clear();
			}
		}

		private TaskCompletion HandleMExResult(IAsyncResult result, TransportMailItem transportMailItem, TaskContext taskContext)
		{
			if (!result.CompletedSynchronously)
			{
				return TaskCompletion.Pending;
			}
			transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.MexResult);
			ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), "Event returned sync (msgId={0})", transportMailItem.RecordId);
			try
			{
				MExEvents.EndEvent(taskContext.MexSession, result);
			}
			catch
			{
				transportMailItem.DropCatBreadcrumb(CategorizerBreadcrumb.MexResultException);
				throw;
			}
			finally
			{
				taskContext.AgentLatencyTracker.EndTrackLatency(!taskContext.MessageDeferred);
			}
			this.PostEventProcessing(transportMailItem, taskContext);
			return TaskCompletion.Completed;
		}

		private bool NdrIfTooManyDeferrals(TransportMailItem transportMailItem, DeferReason deferReason)
		{
			int num = (transportMailItem.Recipients.Count > 0) ? transportMailItem.Recipients[0].RetryCount : 0;
			if (deferReason == DeferReason.RecipientHasNoMdb && num > Components.TransportAppConfig.Routing.MaxDeferCountForRecipientHasNoMdb)
			{
				SmtpResponse smtpResponse = transportMailItem.Recipients[0].SmtpResponse;
				CategorizerComponent.AckAllRecipients(transportMailItem, AckStatus.Fail, smtpResponse);
				LatencyFormatter latencyFormatter = new LatencyFormatter(transportMailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, false);
				MessageTrackingLog.TrackFailedRecipients(MessageTrackingSource.ROUTING, "message deferred too many times: " + deferReason.ToString(), transportMailItem, null, transportMailItem.Recipients, smtpResponse, latencyFormatter);
				Components.DsnGenerator.GenerateDSNs(transportMailItem, DsnGenerator.CallerComponent.ResolverRouting);
				return true;
			}
			return false;
		}

		private static bool CheckDropProbeMessage(TransportMailItem transportMailItem)
		{
			Header header = transportMailItem.RootPart.Headers.FindFirst("X-Exchange-Probe-Drop-Message");
			if (header == null)
			{
				return false;
			}
			if (header.Value == "FrontEnd-CAT-250")
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug<string, string, string>(0L, "Email '{0}' will be dropped because header '{1}' with value '{2}' was detected", transportMailItem.InternetMessageId ?? "not available", "X-Exchange-Probe-Drop-Message", "FrontEnd-CAT-250");
				return true;
			}
			return false;
		}

		private void LogMessage(TransportMailItem mailItem, Exception exception)
		{
			string contentConversionTracingPath = Components.Configuration.LocalServer.ContentConversionTracingPath;
			if (contentConversionTracingPath != null && !Directory.Exists(contentConversionTracingPath))
			{
				try
				{
					Directory.CreateDirectory(contentConversionTracingPath);
				}
				catch (UnauthorizedAccessException arg)
				{
					ExTraceGlobals.SchedulerTracer.TraceDebug<UnauthorizedAccessException>((long)this.GetHashCode(), "Exception hit when accessing ContentConversion trace logging directory: {0}", arg);
				}
				catch (AccessDeniedException arg2)
				{
					ExTraceGlobals.SchedulerTracer.TraceDebug<AccessDeniedException>((long)this.GetHashCode(), "Exception hit when accessing ContentConversion trace logging directory: {0}", arg2);
				}
				catch (IOException arg3)
				{
					ExTraceGlobals.SchedulerTracer.TraceDebug<IOException>((long)this.GetHashCode(), "Exception hit when accessing ContentConversion trace logging directory: {0}", arg3);
				}
			}
			ItemConversion.SaveFailedMimeDocument(mailItem.Message, exception, contentConversionTracingPath);
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "Categorizer";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
			bool flag = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("conditionalQueuing", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag3 = parameters.Argument.IndexOf("config", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag4 = parameters.Argument.IndexOf("diversity", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag5 = (!flag3 && !flag4) || parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			if (flag3)
			{
				XElement xelement2 = new XElement("config");
				xelement2.Add(TransportAppConfig.GetDiagnosticInfoForType(Components.TransportAppConfig.QueueConfiguration));
				xelement2.Add(TransportAppConfig.GetDiagnosticInfoForType(Components.TransportAppConfig.Resolver));
				xelement2.Add(TransportAppConfig.GetDiagnosticInfoForType(Components.TransportAppConfig.Routing));
				xelement2.Add(TransportAppConfig.GetDiagnosticInfoForType(Components.TransportAppConfig.ThrottlingConfig));
				xelement.Add(xelement2);
			}
			if (flag2 || flag)
			{
				xelement.Add(this.scheduler.GetDiagnosticInfo(flag, flag2));
			}
			if (flag5)
			{
				xelement.Add(new XElement("help", "Supported arguments: config, conditionalQueuing, verbose, diversity:" + QueueDiversity.UsageString));
			}
			if (flag4)
			{
				string requestArgument = parameters.Argument.Substring(parameters.Argument.IndexOf("diversity", StringComparison.OrdinalIgnoreCase) + "diversity".Length);
				this.GetDiversityDiagnosticInfo(xelement, requestArgument);
			}
			return xelement;
		}

		private void GetDiversityDiagnosticInfo(XElement categorizerElement, string requestArgument)
		{
			QueueDiversity queueDiversity;
			string content;
			if (!QueueDiversity.TryParse(requestArgument, false, out queueDiversity, out content))
			{
				categorizerElement.Add(new XElement("Error", content));
				return;
			}
			if (queueDiversity.QueueId.Type == QueueType.Submission)
			{
				categorizerElement.Add(queueDiversity.GetDiagnosticInfo(this.submitMessageQueue));
				return;
			}
			categorizerElement.Add(queueDiversity.GetComponentAdvice());
		}

		internal const string MessageLatencyHeaderStamped = "Microsoft.Exchange.Transport.Categorizer.CategorizerComponent.MessageLatencyHeaderStamped";

		internal const string CategorizerResubmitCount = "Microsoft.Exchange.Transport.Categorizer.CategorizerComponent.ResubmitCount";

		private const string AttribCat = "Cat";

		private const string AttribCatEvent = "Cat event ";

		private const string AttribQueue = "Queue";

		private const string OrgHeaderFromMBTSubmission = "X-MS-Exchange-Organization-Processed-By-MBTSubmission";

		private static ExEventLog logger = new ExEventLog(ExTraceGlobals.SchedulerTracer.Category, TransportEventLog.GetEventSource());

		private CatScheduler scheduler;

		private StageInfo[] stages;

		private SubmitMessageQueue submitMessageQueue;

		private IProcessingQuotaComponent processingQuotaComponent;

		private IMessageDepot messageDepot;

		private IMessageDepotComponent messageDepotComponent;

		private bool isMessageDepotEnabled;

		private AgentGeneratedMessageLoopChecker agentLoopChecker;

		private volatile bool retired;
	}
}
