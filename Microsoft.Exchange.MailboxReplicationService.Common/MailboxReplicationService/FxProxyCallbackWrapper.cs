using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyCallbackWrapper : DisposableWrapper<IFxProxy>, IFxProxy, IMapiFxProxy, IDisposeTrackable, IDisposable
	{
		public FxProxyCallbackWrapper(IFxProxy destProxy, bool ownsObject, Action<TimeSpan> updateDuration) : base(destProxy, ownsObject)
		{
			this.updateDuration = updateDuration;
		}

		byte[] IMapiFxProxy.GetObjectData()
		{
			return base.WrappedObject.GetObjectData();
		}

		void IMapiFxProxy.ProcessRequest(FxOpcodes opCode, byte[] request)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				base.WrappedObject.ProcessRequest(opCode, request);
			}
			finally
			{
				this.updateDuration(stopwatch.Elapsed);
				stopwatch.Stop();
			}
		}

		void IFxProxy.Flush()
		{
			base.WrappedObject.Flush();
		}

		private Action<TimeSpan> updateDuration;
	}
}
