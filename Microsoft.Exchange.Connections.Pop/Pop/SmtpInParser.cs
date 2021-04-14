using System;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Connections.Pop
{
	internal abstract class SmtpInParser
	{
		public bool DiscardingData
		{
			get
			{
				return this.discardingData;
			}
			set
			{
				this.discardingData = value;
			}
		}

		public Stream BodyStream
		{
			get
			{
				return this.bodyStream;
			}
			set
			{
				if (value == null && !this.discardingData)
				{
					throw new ArgumentException("BodyStream");
				}
				this.bodyStream = value;
			}
		}

		public long TotalBytesRead
		{
			get
			{
				return this.totalBytesRead;
			}
			internal set
			{
				this.totalBytesRead = value;
			}
		}

		public long TotalBytesWritten
		{
			get
			{
				return this.totalBytesWritten;
			}
			internal set
			{
				this.totalBytesWritten = value;
			}
		}

		public long EohPos
		{
			get
			{
				return this.eohPos;
			}
			internal set
			{
				this.eohPos = value;
			}
		}

		public SmtpInParser.ExceptionFilterDelegate ExceptionFilter
		{
			set
			{
				this.exceptionFilterDelegate = value;
			}
		}

		public virtual void Reset()
		{
			this.discardingData = false;
			this.totalBytesRead = 0L;
			this.totalBytesWritten = 0L;
			this.eohPos = -1L;
			this.exceptionFilterDelegate = null;
		}

		public abstract bool ParseAndWrite(byte[] data, int offset, int numBytes, out int numBytesConsumed);

		internal void Write(byte[] data, int offset, int count)
		{
			try
			{
				this.bodyStream.Write(data, offset, count);
				this.totalBytesWritten += (long)count;
			}
			catch (IOException e)
			{
				this.FilterException(e);
				this.discardingData = true;
			}
			catch (ExchangeDataException e2)
			{
				this.FilterException(e2);
				this.discardingData = true;
			}
		}

		internal void FilterException(Exception e)
		{
			if (this.exceptionFilterDelegate != null)
			{
				this.exceptionFilterDelegate(e);
			}
		}

		public const byte CR = 13;

		public const byte LF = 10;

		public const byte DOT = 46;

		public static readonly byte[] EodSequence = new byte[]
		{
			13,
			10,
			46,
			13,
			10
		};

		private bool discardingData;

		private long totalBytesRead;

		private long totalBytesWritten;

		private long eohPos = -1L;

		private Stream bodyStream;

		private SmtpInParser.ExceptionFilterDelegate exceptionFilterDelegate;

		public delegate void ExceptionFilterDelegate(Exception e);
	}
}
