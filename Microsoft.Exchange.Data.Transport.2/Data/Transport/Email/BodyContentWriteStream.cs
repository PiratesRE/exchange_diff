using System;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class BodyContentWriteStream : AppendStreamOnDataStorage
	{
		public BodyContentWriteStream(IBody body) : base(new TemporaryDataStorage())
		{
			this.body = body;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.body != null)
			{
				ReadableDataStorage readableWritableStorage = base.ReadableWritableStorage;
				readableWritableStorage.AddRef();
				this.body.SetNewContent(readableWritableStorage, 0L, readableWritableStorage.Length);
				readableWritableStorage.Release();
				this.body = null;
			}
			base.Dispose(disposing);
		}

		private IBody body;
	}
}
