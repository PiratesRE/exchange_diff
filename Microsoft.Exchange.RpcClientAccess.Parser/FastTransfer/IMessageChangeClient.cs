using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMessageChangeClient : IDisposable
	{
		IMessage Message { get; }

		void ReportMessageSize(int messageSize);

		void ReportIsAssociatedMessage(bool isAssociatedMessage);

		IPropertyBag MessageHeaderPropertyBag { get; }

		void SetPartial();

		void ScrubGroupProperties(PropertyGroupMapping mapping, int groupIndex);
	}
}
