using System;
using System.Text;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal abstract class DavCommand
	{
		protected DavCommand(byte[] commandBytes)
		{
			this.commandBytes = commandBytes;
			this.CheckExpectedToken(this.FirstToken);
			this.CheckExpectedToken(this.SecondToken);
			this.CheckExpectedToken(DavCommand.Colon);
			this.GetAddress();
			this.ParseArguments();
		}

		public int CurrentOffset
		{
			get
			{
				return this.currentOffset;
			}
		}

		public byte[] CommandBytes
		{
			get
			{
				return this.commandBytes;
			}
		}

		public RoutingAddress Address
		{
			get
			{
				return this.address;
			}
		}

		protected abstract byte[] FirstToken { get; }

		protected abstract byte[] SecondToken { get; }

		private int RemainingBufferLength
		{
			get
			{
				return this.commandBytes.Length - this.currentOffset;
			}
		}

		private bool EndOfCommand
		{
			get
			{
				return this.currentOffset == this.commandBytes.Length;
			}
		}

		protected abstract void ParseArguments();

		private static bool IsWhiteSpace(byte ch)
		{
			return ch == 9 || ch == 32;
		}

		private void CheckExpectedToken(byte[] token)
		{
			this.TrimStart();
			if (token.Length >= this.RemainingBufferLength)
			{
				throw new FormatException("Expected token is missing");
			}
			if (!SmtpCommand.CompareArg(token, this.commandBytes, this.currentOffset, token.Length))
			{
				throw new FormatException("Expected token is missing");
			}
			this.currentOffset += token.Length;
		}

		private void TrimStart()
		{
			if (this.EndOfCommand)
			{
				return;
			}
			while (this.currentOffset < this.commandBytes.Length && DavCommand.IsWhiteSpace(this.commandBytes[this.currentOffset]))
			{
				this.currentOffset++;
			}
		}

		private void GetAddress()
		{
			this.TrimStart();
			if (this.EndOfCommand)
			{
				throw new FormatException("No Address");
			}
			string @string = Encoding.ASCII.GetString(this.commandBytes, this.currentOffset, this.commandBytes.Length - this.currentOffset);
			string text = null;
			this.address = Parse821.ParseAddressLine(@string, out text);
			if (!this.address.IsValid)
			{
				throw new FormatException("Invalid Address");
			}
			this.currentOffset = this.commandBytes.Length - ((text != null) ? text.Length : 0);
		}

		private static readonly byte[] Colon = Util.AsciiStringToBytes(":");

		private RoutingAddress address;

		private int currentOffset;

		private byte[] commandBytes;
	}
}
