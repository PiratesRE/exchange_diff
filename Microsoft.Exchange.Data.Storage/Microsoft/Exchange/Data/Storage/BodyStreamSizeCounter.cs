using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BodyStreamSizeCounter : StreamWrapper
	{
		internal BodyStreamSizeCounter(Stream stream) : base(stream, true, StreamBase.Capabilities.Writable)
		{
			this.byteCount = 0;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (base.InternalStream != null)
			{
				base.InternalStream.Write(buffer, offset, count);
			}
			this.byteCount += count;
		}

		public override void Flush()
		{
			if (base.InternalStream != null)
			{
				base.InternalStream.Flush();
			}
		}

		private int byteCount;
	}
}
