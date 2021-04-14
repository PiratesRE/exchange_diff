using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ExRpcPerf
	{
		static ExRpcPerf()
		{
			ExRpcModule.Bind();
			ExRpcPerf.pPerfData = ExRpcModule.pPerfData;
		}

		internal unsafe static void ConnectionCacheBirth(int maxConnections)
		{
			Interlocked.Increment(ref ((ExRpcPerfData*)((void*)ExRpcPerf.pPerfData))->ulNumConnectionCaches);
			ExRpcPerf.InterlockedAdd(ref ((ExRpcPerfData*)((void*)ExRpcPerf.pPerfData))->ulCacheTotalCapacity, maxConnections);
		}

		internal unsafe static void ConnectionCacheGone(int maxConnections)
		{
			Interlocked.Decrement(ref ((ExRpcPerfData*)((void*)ExRpcPerf.pPerfData))->ulNumConnectionCaches);
			ExRpcPerf.InterlockedAdd(ref ((ExRpcPerfData*)((void*)ExRpcPerf.pPerfData))->ulCacheTotalCapacity, -maxConnections);
		}

		internal unsafe static void ConnectionCacheActiveAdd(int numConnections)
		{
			ExRpcPerf.InterlockedAdd(ref ((ExRpcPerfData*)((void*)ExRpcPerf.pPerfData))->ulCacheConnectionsActive, numConnections);
		}

		internal unsafe static void ConnectionCacheIdleAdd(int numConnections)
		{
			ExRpcPerf.InterlockedAdd(ref ((ExRpcPerfData*)((void*)ExRpcPerf.pPerfData))->ulCacheConnectionsIdle, numConnections);
		}

		internal unsafe static void ConnectionCacheNewOutOfLimitConnection()
		{
			Interlocked.Increment(ref ((ExRpcPerfData*)((void*)ExRpcPerf.pPerfData))->ulOutOfLimitCreations);
		}

		internal unsafe static void ExRpcConnectionBirth()
		{
			Interlocked.Increment(ref ((ExRpcPerfData*)((void*)ExRpcPerf.pPerfData))->ulExRpcConnectionCreations);
		}

		internal unsafe static void ExRpcConnectionGone()
		{
			Interlocked.Increment(ref ((ExRpcPerfData*)((void*)ExRpcPerf.pPerfData))->ulExRpcConnectionDisposals);
		}

		private static void InterlockedAdd(ref int totalValue, int valueToAdd)
		{
			int num;
			int value;
			do
			{
				num = totalValue;
				value = num + valueToAdd;
			}
			while (Interlocked.CompareExchange(ref totalValue, value, num) != num);
		}

		internal static IntPtr pPerfData;
	}
}
