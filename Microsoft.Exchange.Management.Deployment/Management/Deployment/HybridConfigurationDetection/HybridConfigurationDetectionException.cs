using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection
{
	public class HybridConfigurationDetectionException : LocalizedException
	{
		public HybridConfigurationDetectionException(LocalizedString message) : base(message)
		{
		}

		public HybridConfigurationDetectionException(LocalizedString message, Exception exception) : base(message, exception)
		{
		}
	}
}
