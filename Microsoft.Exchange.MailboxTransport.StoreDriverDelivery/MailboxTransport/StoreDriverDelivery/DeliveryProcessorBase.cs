using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class DeliveryProcessorBase : IDeliveryProcessor
	{
		public DeliveryProcessorBase(MailItemDeliver mailItemDeliver)
		{
			this.mailItemDeliver = mailItemDeliver;
		}

		public virtual void Initialize()
		{
			this.mailItemDeliver.RaiseEvent("OnInitializedMessage", LatencyComponent.StoreDriverOnInitializedMessage);
		}

		public virtual DeliverableItem CreateSession()
		{
			if (this.mailItemDeliver.Recipient.ExtendedProperties.Count == 0)
			{
				throw new SmtpResponseException(AckReason.ExtendedPropertiesNotAvailable, MessageAction.Reroute);
			}
			DeliverableItem deliverableItem = RecipientItem.Create(this.mailItemDeliver.Recipient) as DeliverableItem;
			if (deliverableItem == null)
			{
				throw new SmtpResponseException(AckReason.NotResolvedRecipient, MessageAction.Reroute);
			}
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.CreateReplay);
			this.mailItemDeliver.CreateReplay();
			this.mailItemDeliver.IsPublicFolderRecipient = (deliverableItem.RecipientType == RecipientType.PublicFolder);
			int value;
			if (DeliveryThrottling.Instance.TryGetDatabaseHealth(this.mailItemDeliver.MbxTransportMailItem.DatabaseGuid, out value))
			{
				this.mailItemDeliver.DatabaseHealthMeasureToLog = new int?(value);
			}
			if (!this.mailItemDeliver.IsPublicFolderRecipient)
			{
				this.mailItemDeliver.ExtractCulture();
			}
			MailboxItem mailboxItem = deliverableItem as MailboxItem;
			if (mailboxItem != null)
			{
				this.mailItemDeliver.RecipientMailboxGuid = mailboxItem.MailboxGuid;
			}
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.CreateSession);
			this.mailItemDeliver.CreateSession(deliverableItem);
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.OnPromotedEvent);
			this.mailItemDeliver.RaiseEvent("OnPromotedMessage", LatencyComponent.StoreDriverOnPromotedMessage);
			return deliverableItem;
		}

		public virtual void CreateMessage(DeliverableItem item)
		{
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.CreateMessage);
			this.mailItemDeliver.CreateMessage(item);
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.PromoteProperties);
			this.mailItemDeliver.PromotePropertiesToItem();
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.OnCreatedEvent);
			this.mailItemDeliver.RaiseEvent("OnCreatedMessage", LatencyComponent.StoreDriverOnCreatedMessage);
		}

		public virtual void DeliverMessage()
		{
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.PostCreate);
			if (this.mailItemDeliver.EventArguments.ShouldCreateItemForDelete)
			{
				this.mailItemDeliver.EventArguments.ShouldCreateItemForDelete = false;
				SmtpResponse bounceSmtpResponse = this.mailItemDeliver.EventArguments.BounceSmtpResponse;
				this.mailItemDeliver.EventArguments.BounceSmtpResponse = SmtpResponse.Empty;
				throw new SmtpResponseException(bounceSmtpResponse, this.mailItemDeliver.EventArguments.BounceSource);
			}
			this.mailItemDeliver.Stage = this.mailItemDeliver.DeliveryBreadcrumb.SetStage(MailItemDeliver.DeliveryStage.Delivery);
			this.mailItemDeliver.DeliverItem();
			this.mailItemDeliver.AddRecipientInfoForDeliveredEvent();
			this.mailItemDeliver.RaiseOnDeliveredEvent();
		}

		private static readonly Trace Diag = ExTraceGlobals.MapiDeliverTracer;

		protected MailItemDeliver mailItemDeliver;
	}
}
