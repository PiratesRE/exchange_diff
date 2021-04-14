using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.MessageDepot;
using Microsoft.Exchange.Transport.Scheduler.Contracts;
using Microsoft.Exchange.Transport.Scheduler.Processing;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class CategorizerAdapterComponent : ITransportComponent, IMessageProcessor
	{
		public CategorizerAdapterComponent(CategorizerComponent categorizerComponent, IMessageDepotComponent messageDepotComponent)
		{
			this.categorizerComponent = categorizerComponent;
			this.messageDepotComponent = messageDepotComponent;
		}

		public void Load()
		{
		}

		public void Unload()
		{
		}

		public string OnUnhandledException(Exception e)
		{
			return string.Empty;
		}

		public void Process(ISchedulableMessage message)
		{
			AcquireResult acquireResult;
			if (this.messageDepotComponent.MessageDepot.TryAcquire(message.Id, out acquireResult))
			{
				MessageEnvelope messageEnvelope = acquireResult.ItemWrapper.Item.MessageEnvelope;
				TransportMailItem mailItem = TransportMailItem.FromMessageEnvelope(messageEnvelope, LatencyComponent.CategorizerOnSubmittedMessage);
				ThrottlingContext throttlingContext;
				StandaloneJob standaloneJob = (StandaloneJob)CategorizerJobsUtil.SetupNewJob(mailItem, this.categorizerComponent.Stages, (QueuedRecipientsByAgeToken recipientByAgeToken, ThrottlingContext context, IList<StageInfo> stages) => StandaloneJob.NewJob(mailItem, acquireResult.Token, context, recipientByAgeToken, stages), out throttlingContext);
				if (standaloneJob != null)
				{
					standaloneJob.RunToCompletion();
					CategorizerJobsUtil.DoneProcessing(standaloneJob.GetQueuedRecipientsByAgeToken());
				}
				if (!standaloneJob.RootMailItemDeferred)
				{
					this.messageDepotComponent.MessageDepot.Release(message.Id, acquireResult.Token);
				}
			}
		}

		private readonly CategorizerComponent categorizerComponent;

		private readonly IMessageDepotComponent messageDepotComponent;
	}
}
