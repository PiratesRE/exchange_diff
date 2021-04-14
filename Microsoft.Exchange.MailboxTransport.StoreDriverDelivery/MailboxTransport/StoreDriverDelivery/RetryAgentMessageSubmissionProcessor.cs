using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class RetryAgentMessageSubmissionProcessor : DeliveryProcessorBase
	{
		public RetryAgentMessageSubmissionProcessor(MailItemDeliver mailItemDeliver) : base(mailItemDeliver)
		{
			this.useDeliveryProcessorBase = false;
		}

		public override void CreateMessage(DeliverableItem item)
		{
			if (this.mailItemDeliver.IsPublicFolderRecipient)
			{
				this.useDeliveryProcessorBase = true;
				base.CreateMessage(item);
				return;
			}
			RetryAgentMessageSubmissionProcessor.Diag.TraceDebug<string>(0L, "RetryOnDuplicateDelivery set on incoming message {0}.", (this.mailItemDeliver.DeliveryItem != null && this.mailItemDeliver.DeliveryItem.Message != null) ? this.mailItemDeliver.DeliveryItem.Message.InternetMessageId : null);
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.CreateMessage);
			if (!this.mailItemDeliver.LoadMessageForAgentEventsRetry())
			{
				RetryAgentMessageSubmissionProcessor.Diag.TraceDebug(0L, "The attempt to load the message failed. Using normal delivery processing for message.");
				this.mailItemDeliver.ClearRetryOnDuplicateDelivery();
				ExTraceGlobals.FaultInjectionTracer.TraceTest(43648U);
				this.useDeliveryProcessorBase = true;
				base.CreateMessage(item);
				return;
			}
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.PromoteProperties);
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.OnCreatedEvent);
			this.mailItemDeliver.RaiseEvent("OnCreatedMessage", LatencyComponent.StoreDriverOnCreatedMessage);
		}

		public override void DeliverMessage()
		{
			if (this.useDeliveryProcessorBase)
			{
				base.DeliverMessage();
				return;
			}
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.PostCreate);
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.Delivery);
			this.mailItemDeliver.RaiseOnDeliveredEvent();
			ExTraceGlobals.FaultInjectionTracer.TraceTest(39552U);
			throw new SmtpResponseException(SmtpResponse.NoopOk, MessageAction.LogDuplicate);
		}

		private static readonly Trace Diag = ExTraceGlobals.MapiDeliverTracer;

		private bool useDeliveryProcessorBase;
	}
}
