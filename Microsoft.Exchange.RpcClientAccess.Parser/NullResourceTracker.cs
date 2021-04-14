using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NullResourceTracker : IResourceTracker
	{
		private NullResourceTracker()
		{
		}

		public bool TryReserveMemory(int size)
		{
			return true;
		}

		public int StreamSizeLimit
		{
			get
			{
				return int.MaxValue;
			}
		}

		public static IResourceTracker Instance
		{
			get
			{
				return NullResourceTracker.instance;
			}
		}

		private static readonly IResourceTracker instance = new NullResourceTracker();
	}
}
