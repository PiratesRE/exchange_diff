using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyWrapper : WrapperBase<IMapiFxProxy>, IFxProxy, IMapiFxProxy, IDisposeTrackable, IDisposable
	{
		public FxProxyWrapper(IMapiFxProxy proxy, CommonUtils.CreateContextDelegate createContext) : base(proxy, createContext)
		{
		}

		byte[] IMapiFxProxy.GetObjectData()
		{
			byte[] result = null;
			base.CreateContext("IMapiFxProxy.GetObjectData", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetObjectData();
			}, true);
			return result;
		}

		void IMapiFxProxy.ProcessRequest(FxOpcodes opCode, byte[] request)
		{
			base.CreateContext("IMapiFxProxy.ProcessRequest", new DataContext[]
			{
				new SimpleValueDataContext("OpCode", opCode),
				new SimpleValueDataContext("DataLength", (request != null) ? request.Length : 0)
			}).Execute(delegate
			{
				this.WrappedObject.ProcessRequest(opCode, request);
			}, true);
		}

		void IFxProxy.Flush()
		{
			base.CreateContext("IFxProxy.Flush", new DataContext[0]).Execute(delegate
			{
				IFxProxy fxProxy = base.WrappedObject as IFxProxy;
				if (fxProxy != null)
				{
					fxProxy.Flush();
				}
			}, true);
		}
	}
}
