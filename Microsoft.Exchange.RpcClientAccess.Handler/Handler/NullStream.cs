using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NullStream : Stream
	{
		internal NullStream(Logon logon) : base(logon)
		{
		}

		public override void Commit()
		{
			base.CheckDisposed();
		}

		public override uint GetSize()
		{
			base.CheckDisposed();
			return 0U;
		}

		public override ArraySegment<byte> Read(ushort requestedSize)
		{
			base.CheckDisposed();
			return Array<byte>.EmptySegment;
		}

		public override long Seek(StreamSeekOrigin streamSeekOrigin, long offset)
		{
			base.CheckDisposed();
			return 0L;
		}

		public override void SetSize(ulong size)
		{
			base.CheckDisposed();
		}

		public override ushort Write(ArraySegment<byte> data)
		{
			base.CheckDisposed();
			return (ushort)data.Count;
		}

		public override ulong CopyToStream(Stream destinationStream, ulong bytesToCopy)
		{
			base.CheckDisposed();
			return 0UL;
		}

		public override void CheckCanWrite()
		{
			base.CheckDisposed();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NullStream>(this);
		}

		protected override void InternalDispose()
		{
			base.InternalDispose();
		}
	}
}
