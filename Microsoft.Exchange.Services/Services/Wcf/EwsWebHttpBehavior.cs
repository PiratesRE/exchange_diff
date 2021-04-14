using System;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EwsWebHttpBehavior : WebHttpBehavior
	{
		protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
		{
			return new EwsWebQueryStringConverter(base.GetQueryStringConverter(operationDescription));
		}
	}
}
