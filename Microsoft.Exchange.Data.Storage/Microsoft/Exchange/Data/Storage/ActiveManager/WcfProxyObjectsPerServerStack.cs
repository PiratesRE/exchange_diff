using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Storage.ServerLocator;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WcfProxyObjectsPerServerStack
	{
		internal int Count
		{
			get
			{
				return this.m_stack.Count;
			}
		}

		internal void Push(ServerLocatorServiceClient client)
		{
			Interlocked.Exchange(ref this.m_lastAccessTicksUtc, DateTime.UtcNow.Ticks);
			this.m_stack.Push(client);
		}

		internal ServerLocatorServiceClient Pop()
		{
			return this.m_stack.Pop();
		}

		public DateTime LastAccessTimeUtc
		{
			get
			{
				return new DateTime(Interlocked.Read(ref this.m_lastAccessTicksUtc));
			}
		}

		private long m_lastAccessTicksUtc = long.MinValue;

		private Stack<ServerLocatorServiceClient> m_stack = new Stack<ServerLocatorServiceClient>(10);
	}
}
