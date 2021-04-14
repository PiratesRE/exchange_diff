using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal sealed class InterceptorStoreDriverDeliveryAgent : StoreDriverDeliveryAgent
	{
		public InterceptorStoreDriverDeliveryAgent(FilteredRuleCache filteredRuleCache)
		{
			this.filteredRuleCache = filteredRuleCache;
			base.OnInitializedMessage += this.InitMsgEventHandler;
			base.OnPromotedMessage += this.PromotedMessageEventHandler;
			base.OnCreatedMessage += this.CreatedMessageEventHandler;
		}

		private void InitMsgEventHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs e)
		{
			this.HandleMessage(source, e, InterceptorAgentEvent.OnInitMsg);
		}

		private void PromotedMessageEventHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs e)
		{
			this.HandleMessage(source, e, InterceptorAgentEvent.OnPromotedMessage);
		}

		private void CreatedMessageEventHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs e)
		{
			this.HandleMessage(source, e, InterceptorAgentEvent.OnCreatedMessage);
		}

		private void HandleMessage(StoreDriverEventSource source, StoreDriverDeliveryEventArgs e, InterceptorAgentEvent evt)
		{
			InterceptorAgentRule interceptorAgentRule = InterceptorAgentRuleEvaluator.Evaluate(this.filteredRuleCache.Rules, e, evt);
			if (interceptorAgentRule == null)
			{
				return;
			}
			interceptorAgentRule.PerformAction(new DeliverableMailItemWrapper(e.MailItem), delegate
			{
				throw new SmtpResponseException(new SmtpResponse("250", "2.7.0", new string[]
				{
					"STOREDRV.Deliver; message deleted by administrative rule"
				}), "Interceptor Store Driver Delivery Agent");
			}, delegate(SmtpResponse response)
			{
				throw new SmtpResponseException(response);
			}, null);
		}

		private readonly FilteredRuleCache filteredRuleCache;
	}
}
