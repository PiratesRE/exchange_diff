using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IContentsSynchronizer : IDisposable
	{
		ProgressInformation ProgressInformation { get; }

		IEnumerator<IMessageChange> GetChanges();

		IPropertyBag GetDeletions();

		IPropertyBag GetReadUnreadStateChanges();

		IIcsState GetFinalState();
	}
}
