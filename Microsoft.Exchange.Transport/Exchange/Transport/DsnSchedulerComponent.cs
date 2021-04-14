using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.MessageDepot;

namespace Microsoft.Exchange.Transport
{
	internal sealed class DsnSchedulerComponent : ITransportComponent
	{
		public void SetLoadTimeDependencies(IMessageDepotComponent msgDepotComponent, IDsnGeneratorComponent dsnGeneratorComponent, IOrarGeneratorComponent orarGeneratorComponent, IMessageTrackingLog msgTrackingLog, ITransportConfiguration transportConfiguration)
		{
			ArgumentValidator.ThrowIfNull("msgDepotComponent", msgDepotComponent);
			ArgumentValidator.ThrowIfNull("dsnGeneratorComponent", dsnGeneratorComponent);
			ArgumentValidator.ThrowIfNull("orarGeneratorComponent", orarGeneratorComponent);
			ArgumentValidator.ThrowIfNull("msgTrackingLog", msgTrackingLog);
			ArgumentValidator.ThrowIfNull("transportConfiguration", transportConfiguration);
			this.messageDepotComponent = msgDepotComponent;
			this.dsnGenerator = dsnGeneratorComponent;
			this.orarGenerator = orarGeneratorComponent;
			this.msgTrackingLog = msgTrackingLog;
			this.transportConfiguration = transportConfiguration;
		}

		public void Load()
		{
			if (!this.messageDepotComponent.Enabled)
			{
				return;
			}
			IMessageDepot messageDepot = this.messageDepotComponent.MessageDepot;
			messageDepot.SubscribeToDelayedEvent(MessageDepotItemStage.Submission, new MessageEventHandler(this.AddToDelayDsnQueue));
			messageDepot.SubscribeToExpiredEvent(MessageDepotItemStage.Submission, new MessageEventHandler(this.AddToExpiredNdrQueue));
			messageDepot.SubscribeToRemovedEvent(MessageDepotItemStage.Submission, new MessageRemovedEventHandler(this.AddToDeletedNdrQueue));
			this.refreshTimer = new GuardedTimer(new TimerCallback(this.TimedUpdate), null, DsnSchedulerComponent.RefreshTimeInterval);
		}

		public void Unload()
		{
			if (!this.messageDepotComponent.Enabled)
			{
				return;
			}
			IMessageDepot messageDepot = this.messageDepotComponent.MessageDepot;
			messageDepot.UnsubscribeFromDelayedEvent(MessageDepotItemStage.Submission, new MessageEventHandler(this.AddToDelayDsnQueue));
			messageDepot.UnsubscribeFromExpiredEvent(MessageDepotItemStage.Submission, new MessageEventHandler(this.AddToExpiredNdrQueue));
			messageDepot.UnsubscribeFromRemovedEvent(MessageDepotItemStage.Submission, new MessageRemovedEventHandler(this.AddToDeletedNdrQueue));
			this.refreshTimer.Dispose(false);
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		private static void AddPendingWork(MessageEventArgs args, ConcurrentQueue<IMessageDepotItem> queue)
		{
			ArgumentValidator.ThrowIfNull("args", args);
			ArgumentValidator.ThrowIfNull("args.ItemWrapper", args.ItemWrapper);
			ArgumentValidator.ThrowIfNull("args.ItemWrapper.Item", args.ItemWrapper.Item);
			queue.Enqueue(args.ItemWrapper.Item);
		}

		private void TimedUpdate(object state)
		{
			if (!this.messageDepotComponent.Enabled)
			{
				return;
			}
			IMessageDepotItem msgDepotItem;
			while (this.itemsForDelayDsn.TryDequeue(out msgDepotItem))
			{
				this.GenerateDelayDsn(msgDepotItem);
			}
			while (this.itemsForExpiredNdr.TryDequeue(out msgDepotItem))
			{
				this.GenerateExpiredNdr(msgDepotItem);
			}
			MessageRemovedEventArgs messageRemovedEventArgs;
			while (this.itemsForDeletedNdr.TryDequeue(out messageRemovedEventArgs))
			{
				this.GenerateDeletedDsn(messageRemovedEventArgs);
			}
		}

		private void AddToDelayDsnQueue(MessageEventArgs args)
		{
			DsnSchedulerComponent.AddPendingWork(args, this.itemsForDelayDsn);
		}

		private void AddToExpiredNdrQueue(MessageEventArgs args)
		{
			DsnSchedulerComponent.AddPendingWork(args, this.itemsForExpiredNdr);
		}

		private void AddToDeletedNdrQueue(MessageRemovedEventArgs args)
		{
			if (args.Reason == MessageRemovalReason.Deleted)
			{
				this.itemsForDeletedNdr.Enqueue(args);
			}
		}

		private void GenerateDeletedDsn(MessageRemovedEventArgs messageRemovedEventArgs)
		{
			IMessageDepotItem item = messageRemovedEventArgs.ItemWrapper.Item;
			TransportMailItem transportMailItem = (TransportMailItem)item.MessageObject;
			DsnSchedulerComponent.ValidateRecipientCache(transportMailItem);
			if (messageRemovedEventArgs.GenerateNdr)
			{
				foreach (MailRecipient mailRecipient in transportMailItem.Recipients.AllUnprocessed)
				{
					mailRecipient.DsnNeeded = DsnFlags.Failure;
				}
				this.dsnGenerator.GenerateDSNs(transportMailItem, transportMailItem.Recipients);
			}
			MessageTrackingLog.TrackRelayedAndFailed(MessageTrackingSource.ADMIN, transportMailItem, transportMailItem.Recipients, null);
			transportMailItem.Ack(AckStatus.Fail, AckReason.MessageDeletedByAdmin, transportMailItem.Recipients, null);
			transportMailItem.ReleaseFromActive();
			transportMailItem.CommitLazy();
		}

		private void GenerateDelayDsn(IMessageDepotItem msgDepotItem)
		{
			TransportMailItem transportMailItem = (TransportMailItem)msgDepotItem.MessageObject;
			DsnSchedulerComponent.ValidateRecipientCache(transportMailItem);
			bool flag = false;
			foreach (MailRecipient mailRecipient in transportMailItem.Recipients)
			{
				if (mailRecipient.IsDelayDsnNeeded)
				{
					flag = true;
					mailRecipient.DsnNeeded = DsnFlags.Delay;
					mailRecipient.SmtpResponse = SmtpResponse.MessageDelayed;
				}
			}
			if (flag)
			{
				transportMailItem.CommitLazy();
				this.dsnGenerator.GenerateDSNs(transportMailItem, transportMailItem.Recipients);
			}
		}

		private static void ValidateRecipientCache(TransportMailItem mailItem)
		{
			if (mailItem.ADRecipientCache == null)
			{
				ADOperationResult adoperationResult = MultiTenantTransport.TryCreateADRecipientCache(mailItem);
				if (!adoperationResult.Succeeded)
				{
					MultiTenantTransport.TraceAttributionError(string.Format("Error {0} when creating recipient cache for message {1}. Falling back to first org", adoperationResult.Exception, MultiTenantTransport.ToString(mailItem)), new object[0]);
					MultiTenantTransport.UpdateADRecipientCacheAndOrganizationScope(mailItem, OrganizationId.ForestWideOrgId);
				}
			}
		}

		private void GenerateExpiredNdr(IMessageDepotItem msgDepotItem)
		{
			TransportMailItem transportMailItem = (TransportMailItem)msgDepotItem.MessageObject;
			IMessageDepot messageDepot = this.messageDepotComponent.MessageDepot;
			AcquireResult acquireResult;
			if (!messageDepot.TryAcquire(msgDepotItem.Id, out acquireResult))
			{
				return;
			}
			foreach (MailRecipient mailRecipient in transportMailItem.Recipients.AllUnprocessed)
			{
				mailRecipient.Ack(AckStatus.Fail, AckReason.MessageExpired);
			}
			this.orarGenerator.GenerateOrarMessage(transportMailItem, true);
			this.dsnGenerator.GenerateDSNs(transportMailItem);
			LatencyFormatter latencyFormatter = new LatencyFormatter(transportMailItem, this.transportConfiguration.LocalServer.TransportServer.Fqdn, true);
			this.msgTrackingLog.TrackRelayedAndFailed(MessageTrackingSource.QUEUE, "Queue=Submission", transportMailItem, transportMailItem.Recipients, null, SmtpResponse.Empty, latencyFormatter);
			transportMailItem.ReleaseFromActiveMaterializedLazy();
			messageDepot.Release(msgDepotItem.Id, acquireResult.Token);
		}

		private static readonly TimeSpan RefreshTimeInterval = TimeSpan.FromMinutes(1.0);

		private readonly ConcurrentQueue<IMessageDepotItem> itemsForDelayDsn = new ConcurrentQueue<IMessageDepotItem>();

		private readonly ConcurrentQueue<IMessageDepotItem> itemsForExpiredNdr = new ConcurrentQueue<IMessageDepotItem>();

		private readonly ConcurrentQueue<MessageRemovedEventArgs> itemsForDeletedNdr = new ConcurrentQueue<MessageRemovedEventArgs>();

		private IMessageTrackingLog msgTrackingLog;

		private IDsnGeneratorComponent dsnGenerator;

		private IOrarGeneratorComponent orarGenerator;

		private IMessageDepotComponent messageDepotComponent;

		private ITransportConfiguration transportConfiguration;

		private GuardedTimer refreshTimer;
	}
}
