using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.SharePointSignalStore
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISharePointSender<T>
	{
		void SetData(T data);

		void Send();
	}
}
