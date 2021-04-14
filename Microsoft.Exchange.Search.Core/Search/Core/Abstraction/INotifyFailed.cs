using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface INotifyFailed
	{
		event EventHandler<FailedEventArgs> Failed;
	}
}
