using System;
using Microsoft.Exchange.UnifiedContent.Exchange;

namespace Microsoft.Filtering
{
	internal interface IExtendedMapiFilteringContext : IMapiFilteringContext
	{
		void SetFipsRecoveryOptions(RecoveryOptions options);

		RecoveryOptions GetFipsRecoveryOptions();
	}
}
