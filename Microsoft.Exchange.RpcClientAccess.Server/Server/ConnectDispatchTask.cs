using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class ConnectDispatchTask : ExchangeDispatchTask
	{
		public ConnectDispatchTask(IExchangeDispatch exchangeDispatch, CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, string userDn, int flags, int connectionModulus, int codePage, int stringLocale, int sortLocale, short[] clientVersion, ArraySegment<byte> segmentAuxIn, ArraySegment<byte> segmentAuxOut, IStandardBudget budget) : base("ConnectDispatchTask", exchangeDispatch, protocolRequestInfo, asyncCallback, asyncState)
		{
			this.clientBinding = clientBinding;
			this.userDn = userDn;
			this.flags = flags;
			this.connectionModulus = connectionModulus;
			this.codePage = codePage;
			this.stringLocale = stringLocale;
			this.sortLocale = sortLocale;
			this.clientVersion = clientVersion;
			this.segmentAuxIn = segmentAuxIn;
			this.segmentAuxOut = segmentAuxOut;
			this.auxOutData = Array<byte>.EmptySegment;
			this.budget = budget;
		}

		internal override IntPtr ContextHandle
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		internal override int? InternalExecute()
		{
			bool flag = false;
			int? result;
			try
			{
				int value = base.ExchangeDispatch.Connect(base.ProtocolRequestInfo, this.clientBinding, out this.contextHandle, this.userDn, this.flags, this.connectionModulus, this.codePage, this.stringLocale, this.sortLocale, out this.pollsMax, out this.retryCount, out this.retryDelay, out this.dnPrefix, out this.displayName, this.clientVersion, out this.serverVersion, this.segmentAuxIn, this.segmentAuxOut, out this.auxOutData, this.budget);
				flag = true;
				result = new int?(value);
			}
			finally
			{
				if (!flag)
				{
					this.auxOutData = Array<byte>.EmptySegment;
					this.serverVersion = Microsoft.Exchange.RpcClientAccess.Server.ExchangeDispatch.ExchangeServerVersion;
				}
			}
			return result;
		}

		public int End(out IntPtr contextHandle, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, out short[] serverVersion, out ArraySegment<byte> segmentAuxOut)
		{
			bool flag = true;
			int result;
			try
			{
				int num = base.CheckCompletion();
				contextHandle = this.contextHandle;
				pollsMax = this.pollsMax;
				retryCount = this.retryCount;
				retryDelay = this.retryDelay;
				dnPrefix = this.dnPrefix;
				displayName = this.displayName;
				serverVersion = this.serverVersion;
				segmentAuxOut = this.auxOutData;
				if (num == 0)
				{
					flag = false;
				}
				result = num;
			}
			finally
			{
				if (flag)
				{
					Util.DisposeIfPresent(this.budget);
				}
			}
			return result;
		}

		private readonly ClientBinding clientBinding;

		private readonly string userDn;

		private readonly int flags;

		private readonly int connectionModulus;

		private readonly int codePage;

		private readonly int stringLocale;

		private readonly int sortLocale;

		private readonly short[] clientVersion;

		private readonly ArraySegment<byte> segmentAuxIn;

		private readonly IStandardBudget budget;

		private readonly ArraySegment<byte> segmentAuxOut;

		private IntPtr contextHandle;

		private TimeSpan pollsMax;

		private int retryCount;

		private TimeSpan retryDelay;

		private string dnPrefix;

		private string displayName;

		private short[] serverVersion;

		private ArraySegment<byte> auxOutData;
	}
}
