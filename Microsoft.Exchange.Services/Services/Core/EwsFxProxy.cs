using System;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services.Core
{
	internal class EwsFxProxy : DisposeTrackableBase, IMapiFxProxy, IDisposeTrackable, IDisposable
	{
		public EwsFxProxy(XmlWriter writer)
		{
			Guid objectType = InterfaceIds.IMessageGuid;
			byte[] serverVersion = new byte[]
			{
				8,
				0,
				130,
				140,
				0,
				0
			};
			this.cachedObjectData = BinarySerializer.Serialize(delegate(BinarySerializer serializer)
			{
				serializer.Write(objectType);
				serializer.Write(serverVersion);
			});
			this.writer = writer;
		}

		byte[] IMapiFxProxy.GetObjectData()
		{
			return this.cachedObjectData;
		}

		void IMapiFxProxy.ProcessRequest(FxOpcodes opcode, byte[] request)
		{
			this.intBuffer = BitConverter.GetBytes((int)opcode);
			this.writer.WriteBase64(this.intBuffer, 0, this.intBuffer.GetLength(0));
			int length = request.GetLength(0);
			this.intBuffer = BitConverter.GetBytes(length);
			this.writer.WriteBase64(this.intBuffer, 0, this.intBuffer.GetLength(0));
			if (length != 0)
			{
				this.writer.WriteBase64(request, 0, request.GetLength(0));
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EwsFxProxy>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		private byte[] cachedObjectData;

		private XmlWriter writer;

		private byte[] intBuffer;
	}
}
