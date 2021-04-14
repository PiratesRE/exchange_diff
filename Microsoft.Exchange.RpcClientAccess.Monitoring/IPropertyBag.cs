using System;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal interface IPropertyBag
	{
		bool TryGet(ContextProperty property, out object value);

		void Set(ContextProperty property, object value);
	}
}
