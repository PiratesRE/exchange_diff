using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace Microsoft.Exchange.Services.DispatchPipe.Ews
{
	public abstract class EwsOperationContextBase
	{
		public static EwsOperationContextBase Current
		{
			get
			{
				if (OperationContext.Current != null)
				{
					return new WrappedWcfOperationContext(OperationContext.Current);
				}
				return EwsOperationContextBase.currentEwsOperationContext;
			}
		}

		public static implicit operator EwsOperationContextBase(OperationContext operationContext)
		{
			return new WrappedWcfOperationContext(operationContext);
		}

		public abstract Message RequestMessage { get; }

		public abstract MessageProperties IncomingMessageProperties { get; }

		public abstract MessageHeaders IncomingMessageHeaders { get; }

		public abstract MessageVersion IncomingMessageVersion { get; }

		public abstract IEnumerable<SupportingTokenSpecification> SupportingTokens { get; }

		public abstract MessageProperties OutgoingMessageProperties { get; }

		public abstract Uri LocalAddressUri { get; }

		internal static void SetCurrent(EwsOperationContextBase operationContext)
		{
			EwsOperationContextBase.currentEwsOperationContext = operationContext;
		}

		internal abstract OperationContext BackingOperationContext { get; }

		[ThreadStatic]
		private static EwsOperationContextBase currentEwsOperationContext;
	}
}
