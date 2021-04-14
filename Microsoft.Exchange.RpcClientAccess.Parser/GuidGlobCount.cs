using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	public struct GuidGlobCount
	{
		public GuidGlobCount(Guid guid, ulong globCount)
		{
			GlobCountSet.VerifyGlobCountArgument(globCount, "globCount");
			this.guid = guid;
			this.globCount = globCount;
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public ulong GlobCount
		{
			get
			{
				return this.globCount;
			}
		}

		private Guid guid;

		private ulong globCount;
	}
}
