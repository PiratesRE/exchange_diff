using System;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Inference
{
	public static class CounterNames
	{
		public const string PipelineObjectName = "MSExchangeInference Pipeline";

		public const string DeliveryStoreDriverObjectName = "MSExchange Delivery Store Driver";

		public const string ClassificationProcessingObjectName = "MSExchangeInference Classification Processing";

		public const string DeliveryStoreAgentsObjectName = "MSExchange Delivery Store Driver Agents";

		public const string NumberOfSucceededDocumentsCounterName = "Number Of Succeeded Documents";

		public const string NumberOfFailedDocumentsCounterName = "Number Of Failed Documents";

		public const string PipelineInstanceName = "classificationpipeline";

		public const string ClassificationAgentInstanceName = "inference classification agent";

		public const string SuccessfulDeliveriesCounterName = "SuccessfulDeliveries";

		public const string AgentFailureCounterName = "StoreDriverDelivery Agent Failure";

		public const string ItemSkippedCounterName = "Items Skipped";

		public const string NumberOfQuotaExceededExceptionsCounterName = "Number of Quota Exceeded Exceptions in Pipeline";

		public const string NumberOfTransientExceptionsCounterName = "Number of Transient Exceptions in Pipeline";
	}
}
