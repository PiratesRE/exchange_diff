using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyBudgetWrapper : BudgetWrapperBase<IMapiFxProxy>, IFxProxy, IMapiFxProxy, IDisposeTrackable, IDisposable
	{
		public FxProxyBudgetWrapper(IMapiFxProxy destProxy, bool ownsObject, Func<IDisposable> createCostHandleDelegate, Action<uint> chargeDelegate) : base(destProxy, ownsObject, createCostHandleDelegate, chargeDelegate)
		{
		}

		byte[] IMapiFxProxy.GetObjectData()
		{
			byte[] objectData;
			using (base.CreateCostHandle())
			{
				objectData = base.WrappedObject.GetObjectData();
			}
			return objectData;
		}

		void IMapiFxProxy.ProcessRequest(FxOpcodes opCode, byte[] request)
		{
			using (base.CreateCostHandle())
			{
				base.WrappedObject.ProcessRequest(opCode, request);
			}
			if (request != null)
			{
				base.Charge((uint)request.Length);
			}
		}

		void IFxProxy.Flush()
		{
			IFxProxy fxProxy = base.WrappedObject as IFxProxy;
			if (fxProxy != null)
			{
				using (base.CreateCostHandle())
				{
					fxProxy.Flush();
				}
			}
		}
	}
}
