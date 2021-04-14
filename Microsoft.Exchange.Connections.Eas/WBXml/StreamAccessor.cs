using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.WBXml
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class StreamAccessor
	{
		internal StreamAccessor(Stream stream)
		{
			this.internalStream = stream;
		}

		internal virtual bool CanSeek
		{
			get
			{
				return this.internalStream.CanSeek;
			}
		}

		internal abstract long Length { get; }

		internal abstract long Position { get; set; }

		protected Stream InternalStream
		{
			get
			{
				return this.internalStream;
			}
		}

		internal abstract long Seek(long offset, SeekOrigin origin);

		internal abstract int Read(byte[] buffer, int offset, int count);

		private readonly Stream internalStream;
	}
}
