using System;

namespace Microsoft.Exchange.Data.Internal
{
	internal abstract class ReadableWritableDataStorage : ReadableDataStorage
	{
		public ReadableWritableDataStorage()
		{
		}

		public abstract void Write(long position, byte[] buffer, int offset, int count);

		public abstract void SetLength(long length);

		public virtual StreamOnDataStorage OpenWriteStream(bool append)
		{
			base.ThrowIfDisposed();
			if (append)
			{
				return new AppendStreamOnDataStorage(this);
			}
			return new ReadWriteStreamOnDataStorage(this);
		}
	}
}
