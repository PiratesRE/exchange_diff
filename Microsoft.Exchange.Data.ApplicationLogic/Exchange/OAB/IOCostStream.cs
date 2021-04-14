using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OAB;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class IOCostStream : BaseStream
	{
		public IOCostStream(Stream stream) : base(stream)
		{
			this.writing = new Stopwatch();
			this.reading = new Stopwatch();
		}

		public TimeSpan Writing
		{
			get
			{
				return this.writing.Elapsed;
			}
		}

		public TimeSpan Reading
		{
			get
			{
				return this.reading.Elapsed;
			}
		}

		public long BytesRead { get; private set; }

		public long BytesWritten { get; private set; }

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.reading.Start();
			int result;
			try
			{
				int num = base.Read(buffer, offset, count);
				this.BytesRead += (long)num;
				result = num;
			}
			finally
			{
				this.reading.Stop();
			}
			return result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.writing.Start();
			try
			{
				base.Write(buffer, offset, count);
				this.BytesWritten += (long)count;
			}
			finally
			{
				this.writing.Stop();
			}
		}

		public override void Close()
		{
			base.Close();
			if (IOCostStream.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				IOCostStream.Tracer.TraceDebug((long)this.GetHashCode(), "IOCostStream: time spent writing: {0}ms, time spent reading: {1}ms. Bytes read: {2} bytes, bytes written: {3} bytes. File: {4}", new object[]
				{
					this.writing.Elapsed.TotalMilliseconds,
					this.reading.Elapsed.TotalMilliseconds,
					this.BytesRead,
					this.BytesWritten,
					IOCostStream.GetBaseStreamFileName(base.InnerStream)
				});
			}
		}

		private static string GetBaseStreamFileName(Stream stream)
		{
			FileStream fileStream;
			for (;;)
			{
				fileStream = (stream as FileStream);
				if (fileStream != null)
				{
					break;
				}
				BaseStream baseStream = stream as BaseStream;
				if (baseStream == null)
				{
					goto IL_25;
				}
				stream = baseStream.InnerStream;
			}
			return fileStream.Name;
			IL_25:
			return "unknown";
		}

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.DataTracer;

		private Stopwatch writing;

		private Stopwatch reading;
	}
}
