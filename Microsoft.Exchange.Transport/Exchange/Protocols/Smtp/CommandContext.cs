using System;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class CommandContext
	{
		public static CommandContext FromAsyncResult(NetworkConnection.LazyAsyncResultWithTimeout asyncResult)
		{
			ArgumentValidator.ThrowIfNull("asyncResult", asyncResult);
			return new CommandContext(asyncResult.Buffer, asyncResult.Size, asyncResult.Offset);
		}

		public static CommandContext FromSmtpCommand(ILegacySmtpCommand command)
		{
			ArgumentValidator.ThrowIfNull("command", command);
			CommandContext commandContext = new CommandContext(command.ProtocolCommand, command.ProtocolCommandLength, 0);
			string text;
			commandContext.GetNextArgument(out text);
			return commandContext;
		}

		public static CommandContext FromByteArrayLegacyCodeOnly(byte[] bytes, int offset)
		{
			ArgumentValidator.ThrowIfNull("bytes", bytes);
			ArgumentValidator.ThrowIfOutOfRange<int>("offset", offset, 0, bytes.Length);
			return new CommandContext(bytes, bytes.Length - offset, offset);
		}

		public byte[] Command { get; private set; }

		public int OriginalLength
		{
			get
			{
				return this.originalLength;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
			private set
			{
				this.length = Math.Min(this.originalLength, value);
			}
		}

		public int OriginalOffset
		{
			get
			{
				return this.originalOffset;
			}
		}

		public int Offset
		{
			get
			{
				return this.offset;
			}
			private set
			{
				ArgumentValidator.ThrowIfOutOfRange<int>("value", value, this.originalOffset, this.originalOffset + this.originalLength + 1);
				this.Length = Math.Abs(value - (this.offset + this.length));
				this.offset = value;
			}
		}

		public bool HasArguments
		{
			get
			{
				Offset offset;
				return this.GetNextArgumentOffset(out offset, false);
			}
		}

		public bool IsEndOfCommand
		{
			get
			{
				return this.length == 0;
			}
		}

		public override string ToString()
		{
			return string.Format("Command.Length: {0}, offset: {1}, originalOffset: {2}, Length: {3}, originalLength: {4}, Command: '{5}'", new object[]
			{
				this.Command.Length,
				this.offset,
				this.originalOffset,
				this.Length,
				this.originalLength,
				ByteString.BytesToString(this.Command, this.originalOffset, this.originalLength, true)
			});
		}

		public SmtpInCommand IdentifySmtpCommand()
		{
			int num;
			SmtpInCommand result = SmtpInSessionUtils.IdentifySmtpCommand(this.Command, this.Offset, this.Length, out num);
			this.Offset = num;
			return result;
		}

		public void LogReceivedCommand(IProtocolLogSession protocolLogSession)
		{
			ArgumentValidator.ThrowIfNull("protocolLogSession", protocolLogSession);
			if (this.originalLength != 0)
			{
				byte[] array = new byte[this.originalLength];
				Buffer.BlockCopy(this.Command, this.originalOffset, array, 0, this.originalLength);
				protocolLogSession.LogReceive(array);
			}
		}

		public void TrimLeadingWhitespace()
		{
			if (this.IsEndOfCommand)
			{
				return;
			}
			int i;
			for (i = this.Offset; i < this.Offset + this.Length; i++)
			{
				byte b = this.Command[i];
				bool flag = 9 == b || 32 == b;
				if (!flag)
				{
					break;
				}
			}
			this.Offset = i;
		}

		public bool GetNextArgumentOffset(out Offset nextTokenOffset)
		{
			return this.GetNextArgumentOffset(out nextTokenOffset, true);
		}

		public bool GetNextArgument(out string argument)
		{
			argument = string.Empty;
			if (!this.HasArguments)
			{
				return false;
			}
			Offset offset;
			this.GetNextArgumentOffset(out offset);
			argument = ByteString.BytesToString(this.Command, offset.Start, offset.Length, true);
			this.Offset = offset.End;
			return true;
		}

		public void PushBackOffset(int howManyChars)
		{
			ArgumentValidator.ThrowIfOutOfRange<int>("howManyChars", howManyChars, 0, this.Offset - this.originalOffset);
			this.Offset -= howManyChars;
		}

		public bool GetCommandArguments(out string args)
		{
			if (this.IsEndOfCommand)
			{
				args = string.Empty;
				return false;
			}
			args = ByteString.BytesToString(this.Command, this.Offset, this.Length, true);
			this.Offset += this.Length;
			return true;
		}

		public bool ParseTokenAndVerifyCommand(byte[] cmdPart, byte delimToken)
		{
			ArgumentValidator.ThrowIfNull("cmdPart", cmdPart);
			this.TrimLeadingWhitespace();
			if (this.IsEndOfCommand)
			{
				return false;
			}
			int num = cmdPart.Length;
			if (num > this.length)
			{
				return false;
			}
			if (!BufferParser.CompareArg(cmdPart, this.Command, this.Offset, num))
			{
				this.Offset += num;
				return false;
			}
			this.Offset += num;
			bool flag = false;
			int i;
			for (i = this.Offset; i < this.Offset + this.length; i++)
			{
				byte b = this.Command[i];
				if (9 != b && 32 != b)
				{
					if (delimToken != b || flag)
					{
						break;
					}
					flag = true;
				}
			}
			this.Offset = i;
			return flag;
		}

		private CommandContext(byte[] command, int length, int offset)
		{
			ArgumentValidator.ThrowIfNull("command", command);
			ArgumentValidator.ThrowIfOutOfRange<int>("length", length, 0, command.Length - offset);
			ArgumentValidator.ThrowIfOutOfRange<int>("offset", offset, 0, command.Length);
			this.originalOffset = offset;
			this.originalLength = length;
			this.Command = command;
			this.length = length;
			this.offset = offset;
		}

		private bool GetNextArgumentOffset(out Offset nextTokenOffset, bool updateOffset)
		{
			if (this.IsEndOfCommand)
			{
				nextTokenOffset = new Offset(0, 0);
				return false;
			}
			int end;
			int nextToken = BufferParser.GetNextToken(this.Command, this.Offset, this.Length, out end);
			if (nextToken >= this.Offset + this.Length)
			{
				nextTokenOffset = new Offset(0, 0);
				return false;
			}
			if (updateOffset)
			{
				this.Offset = end;
			}
			nextTokenOffset = new Offset(nextToken, end);
			return true;
		}

		private readonly int originalOffset;

		private readonly int originalLength;

		private int offset;

		private int length;
	}
}
