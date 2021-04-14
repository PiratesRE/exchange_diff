using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class IllegalCrossServerConnectionExceptionMapping : StaticExceptionMapping
	{
		public IllegalCrossServerConnectionExceptionMapping() : base(typeof(IllegalCrossServerConnectionException), ResponseCodeType.ErrorInternalServerTransientError, CoreResources.IDs.ErrorIllegalCrossServerConnection)
		{
		}

		protected override void DoServiceErrorPostProcessing(LocalizedException exception, ServiceError error)
		{
			EWSSettings.ExceptionType = exception.GetType().ToString();
		}
	}
}
