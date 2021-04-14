using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyReceiver : DisposableWrapper<IFxProxy>, IDataImport, IDisposable
	{
		public FxProxyReceiver(IFxProxy destination, bool ownsDestination) : base(destination, ownsDestination)
		{
		}

		IDataMessage IDataImport.SendMessageAndWaitForReply(IDataMessage message)
		{
			if (message is FxProxyGetObjectDataRequestMessage)
			{
				return new FxProxyGetObjectDataResponseMessage(base.WrappedObject.GetObjectData());
			}
			if (message is FlushMessage)
			{
				base.WrappedObject.Flush();
				return null;
			}
			throw new UnexpectedErrorPermanentException(-2147024809);
		}

		void IDataImport.SendMessage(IDataMessage message)
		{
			FxProxyImportBufferMessage fxProxyImportBufferMessage = message as FxProxyImportBufferMessage;
			if (fxProxyImportBufferMessage != null)
			{
				base.WrappedObject.ProcessRequest(fxProxyImportBufferMessage.Opcode, fxProxyImportBufferMessage.Buffer);
				return;
			}
			throw new UnexpectedErrorPermanentException(-2147024809);
		}
	}
}
