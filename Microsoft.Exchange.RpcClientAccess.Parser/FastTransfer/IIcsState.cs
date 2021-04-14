using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IIcsState : IDisposable
	{
		IPropertyBag PropertyBag { get; }

		void Flush();
	}
}
