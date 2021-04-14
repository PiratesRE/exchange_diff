using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class TestBridgeManager
	{
		internal static void SetBridge(ITestBridgeDelegateSessionCache bridge)
		{
			TestBridgeManager.bridge = bridge;
		}

		internal static int GetSize(int currentSize)
		{
			if (TestBridgeManager.bridge != null)
			{
				return TestBridgeManager.bridge.GetSize(currentSize);
			}
			return currentSize;
		}

		internal static void Removed(string mailboxLegacyDn)
		{
			if (TestBridgeManager.bridge != null)
			{
				TestBridgeManager.bridge.Removed(mailboxLegacyDn);
			}
		}

		internal static void Added(string mailboxLegacyDn)
		{
			if (TestBridgeManager.bridge != null)
			{
				TestBridgeManager.bridge.Added(mailboxLegacyDn);
			}
		}

		private static ITestBridgeDelegateSessionCache bridge;
	}
}
