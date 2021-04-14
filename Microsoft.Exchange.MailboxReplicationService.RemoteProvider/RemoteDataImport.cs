using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RemoteDataImport : RemoteObject, IDataImport, IDisposable
	{
		public RemoteDataImport(IMailboxReplicationProxyService mrsProxy, long handle, IDataMessage getDataResponseMsg) : base(mrsProxy, handle)
		{
			this.getDataResponseMsg = getDataResponseMsg;
		}

		IDataMessage IDataImport.SendMessageAndWaitForReply(IDataMessage message)
		{
			if (message is FlushMessage)
			{
				base.MrsProxy.IDataImport_Flush(base.Handle);
				return null;
			}
			if (message is FxProxyGetObjectDataRequestMessage && this.getDataResponseMsg is FxProxyGetObjectDataResponseMessage)
			{
				return this.getDataResponseMsg;
			}
			if (message is FxProxyPoolGetFolderDataRequestMessage && this.getDataResponseMsg is FxProxyPoolGetFolderDataResponseMessage)
			{
				return this.getDataResponseMsg;
			}
			throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IDataImport_GetObjectData");
		}

		void IDataImport.SendMessage(IDataMessage message)
		{
			DataMessageOpcode opcode;
			byte[] data;
			message.Serialize(base.MrsProxyClient.UseCompression, out opcode, out data);
			base.MrsProxy.IDataImport_ImportBuffer(base.Handle, (int)opcode, data);
		}

		private IDataMessage getDataResponseMsg;
	}
}
