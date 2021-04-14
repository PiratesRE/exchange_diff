using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.MessageDepot;
using Microsoft.Exchange.Transport.Scheduler.Contracts;
using Microsoft.Exchange.Transport.Scheduler.Processing;

namespace Microsoft.Exchange.Transport
{
	internal class SchedulerAdapterComponent : ITransportComponent
	{
		public SchedulerAdapterComponent(IMessageDepotComponent messageDepotComponent, IProcessingSchedulerComponent processingSchedulerComponent)
		{
			ArgumentValidator.ThrowIfNull("messageDepotComponent", messageDepotComponent);
			ArgumentValidator.ThrowIfNull("processingSchedulerComponent", processingSchedulerComponent);
			this.messageDepotComponent = messageDepotComponent;
			this.processingSchedulerComponent = processingSchedulerComponent;
		}

		private void HandleActiveMessage(MessageEventArgs args)
		{
			if (args.ItemWrapper.State == MessageDepotItemState.Ready)
			{
				IEnumerable<IMessageScope> messageScopes = this.GetMessageScopes(args.ItemWrapper.Item.MessageEnvelope);
				SchedulableMailItem message = new SchedulableMailItem(args.ItemWrapper.Item.Id, args.ItemWrapper.Item.MessageEnvelope, messageScopes, args.ItemWrapper.Item.ArrivalTime);
				this.scheduler.Submit(message);
			}
		}

		private IEnumerable<IMessageScope> GetMessageScopes(MessageEnvelope messageEnvelope)
		{
			return new List<IMessageScope>
			{
				new PriorityScope(messageEnvelope.DeliveryPriority),
				new TenantScope(messageEnvelope.ExternalOrganizationId)
			};
		}

		public void Load()
		{
			if (this.messageDepotComponent.Enabled)
			{
				this.scheduler = this.processingSchedulerComponent.ProcessingScheduler;
				this.messageDepotComponent.MessageDepot.SubscribeToActivatedEvent(MessageDepotItemStage.Submission, new MessageActivatedEventHandler(this.HandleActiveMessage));
			}
		}

		public void Unload()
		{
			if (this.messageDepotComponent.Enabled)
			{
				this.messageDepotComponent.MessageDepot.UnsubscribeFromActivatedEvent(MessageDepotItemStage.Submission, new MessageActivatedEventHandler(this.HandleActiveMessage));
			}
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		private readonly IProcessingSchedulerComponent processingSchedulerComponent;

		private readonly IMessageDepotComponent messageDepotComponent;

		private IProcessingScheduler scheduler;
	}
}
