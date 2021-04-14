using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class SmtpInStreamBuilderBase : ISmtpInStreamBuilder
	{
		public bool IsDiscardingData { get; set; }

		public Stream BodyStream { get; set; }

		public long TotalBytesRead { get; protected set; }

		public long TotalBytesWritten { get; protected set; }

		public long EohPos { get; protected set; }

		public abstract bool IsEodSeen { get; }

		public virtual void Reset()
		{
			this.IsDiscardingData = false;
			this.TotalBytesRead = 0L;
			this.TotalBytesWritten = 0L;
			this.EohPos = -1L;
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
				this.BodyStream.Write(data, offset, count);
				this.TotalBytesWritten += (long)count;
			}
			catch (Exception)
			{
				this.IsDiscardingData = true;
				throw;
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
	}
}
