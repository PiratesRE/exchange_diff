using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core
{
	internal interface IMessageTrackingRequestLogInformation
	{
		void AddRequestDataForLogging(List<KeyValuePair<string, object>> requestData);
	}
}
