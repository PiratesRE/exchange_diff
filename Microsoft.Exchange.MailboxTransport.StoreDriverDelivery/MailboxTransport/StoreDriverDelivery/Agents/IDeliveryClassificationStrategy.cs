using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal interface IDeliveryClassificationStrategy
	{
		void ApplyClassification(StoreDriverDeliveryEventArgsImpl argsImpl, InferenceClassificationResult result);
	}
}
