using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetUsersOperationBehavior : Attribute, IOperationBehavior
	{
		public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
		{
			clientOperation.Formatter = new GetUsersMessageFormatter(clientOperation.Formatter);
		}

		public void Validate(OperationDescription operationDescription)
		{
		}

		public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
		{
		}

		public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
		{
		}
	}
}
