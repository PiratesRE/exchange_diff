using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.UnifiedPolicy
{
	internal interface IUnifiedPolicyStatusPublisher
	{
		void PublishStatus(IEnumerable<object> statuses, bool deleteConfiguration);
	}
}
