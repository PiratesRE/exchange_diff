using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal interface ICriticalFeature
	{
		bool IsCriticalException(Exception ex);
	}
}
