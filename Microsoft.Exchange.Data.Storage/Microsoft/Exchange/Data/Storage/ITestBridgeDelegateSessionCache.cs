using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ITestBridgeDelegateSessionCache
	{
		int GetSize(int currentSize);

		void Removed(string mailboxLegacyDn);

		void Added(string mailboxLegacyDn);
	}
}
