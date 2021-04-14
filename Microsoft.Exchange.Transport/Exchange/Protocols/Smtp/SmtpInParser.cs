using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class SmtpInParser : ISmtpInStreamBuilder
	{
		public bool IsDiscardingData
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
					throw new ArgumentException(Strings.DiscardingDataFalse, "BodyStream");
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
			protected set
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
			protected set
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
			protected set
			{
				this.eohPos = value;
			}
		}

		public abstract bool IsEodSeen { get; }

		public ExceptionFilter ExceptionFilter
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

		public abstract bool Write(byte[] data, int offset, int numBytes, out int numBytesConsumed);

		public bool Write(CommandContext commandContext, out int numBytesConsumed)
		{
			ArgumentValidator.ThrowIfNull("commandContext", commandContext);
			return this.Write(commandContext.Command, commandContext.Offset, commandContext.Length, out numBytesConsumed);
		}

		protected void Write(byte[] data, int offset, int count)
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

		private ExceptionFilter exceptionFilterDelegate;
	}
}
