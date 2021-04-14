using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IDelegateUserCollectionBridge
	{
		IList<Exception> CreateDelegateForwardingRule();

		IList<Exception> UpdateSendOnBehalfOfPermissions();

		IList<Exception> SetFolderPermissions();

		IList<Exception> SetOulookLocalFreeBusyData();

		IList<Exception> RollbackDelegateState();
	}
}
