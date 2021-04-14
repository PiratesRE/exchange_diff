using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal interface ICategorizer
	{
		SmtpResponse EnqueueSubmittedMessage(TransportMailItem mailItem);

		void SetLoadTimeDependencies(IProcessingQuotaComponent processingQuotaComponent, IMessageDepotComponent messageDepotComponent);
	}
}
