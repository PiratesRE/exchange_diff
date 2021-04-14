using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Exchange.Services.Wcf
{
	[AttributeUsage(AttributeTargets.Method)]
	public class OfflineClientAttribute : Attribute, IOperationBehavior
	{
		public bool Queued { get; set; }

		void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
		{
		}

		void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
		{
		}

		void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
		{
		}

		void IOperationBehavior.Validate(OperationDescription operationDescription)
		{
		}
	}
}
