using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IProcessingQuotaComponent : ITransportComponent
	{
		ProcessingQuotaComponent.ProcessingData GetQuotaOverride(Guid externalOrgId);

		ProcessingQuotaComponent.ProcessingData GetQuotaOverride(WaitCondition condition);

		void SetLoadTimeDependencies(TransportAppConfig.IProcessingQuotaConfig processingQuota);

		void TimedUpdate();
	}
}
