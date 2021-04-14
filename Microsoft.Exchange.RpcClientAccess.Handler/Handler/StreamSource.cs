using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal abstract class StreamSource : BaseObject
	{
		public abstract ICorePropertyBag PropertyBag { get; }

		public abstract void OnAccess();

		public abstract StreamSource Duplicate();
	}
}
