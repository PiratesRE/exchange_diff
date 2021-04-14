using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal sealed class NullCategorizer : ICategorizer, ICategorizerComponentFacade
	{
		public SmtpResponse EnqueueSubmittedMessage(TransportMailItem mailItem)
		{
			throw new NotImplementedException();
		}

		public void SetLoadTimeDependencies(IProcessingQuotaComponent processingQuotaComponent, IMessageDepotComponent messageDepotComponent)
		{
			throw new NotImplementedException();
		}

		void ICategorizerComponentFacade.EnqueueSideEffectMessage(ITransportMailItemFacade originalMailItem, ITransportMailItemFacade sideEffectMailItem, string agentName)
		{
			throw new NotImplementedException();
		}
	}
}
