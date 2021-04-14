using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyTransmitter : DisposableWrapper<IDataImport>, IFxProxy, IMapiFxProxy, IDisposeTrackable, IDisposable
	{
		public FxProxyTransmitter(IDataImport destination, bool ownsDestination) : base(destination, ownsDestination)
		{
		}

		byte[] IMapiFxProxy.GetObjectData()
		{
			IDataMessage dataMessage = base.WrappedObject.SendMessageAndWaitForReply(FxProxyGetObjectDataRequestMessage.Instance);
			FxProxyGetObjectDataResponseMessage fxProxyGetObjectDataResponseMessage = dataMessage as FxProxyGetObjectDataResponseMessage;
			if (fxProxyGetObjectDataResponseMessage == null)
			{
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			return fxProxyGetObjectDataResponseMessage.Buffer;
		}

		void IMapiFxProxy.ProcessRequest(FxOpcodes opcode, byte[] request)
		{
			base.WrappedObject.SendMessage(new FxProxyImportBufferMessage(opcode, request));
		}

		void IFxProxy.Flush()
		{
			base.WrappedObject.SendMessageAndWaitForReply(FlushMessage.Instance);
		}
	}
}
