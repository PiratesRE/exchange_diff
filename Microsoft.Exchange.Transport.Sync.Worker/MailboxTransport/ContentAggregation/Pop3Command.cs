using System;
using System.Globalization;
using System.Security;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Sync.Worker.Framework;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Pop3Command
	{
		private Pop3Command(Pop3CommandType commandType, bool listings, string formatString, params object[] formatStringArguments) : this(commandType, listings, string.Format(CultureInfo.InvariantCulture, formatString, formatStringArguments))
		{
		}

		private Pop3Command(Pop3CommandType commandType, bool listings, string commandText)
		{
			this.type = commandType;
			BufferBuilder bufferBuilder = new BufferBuilder(commandText.Length);
			try
			{
				bufferBuilder.Append(commandText);
			}
			catch (ArgumentException innerException)
			{
				throw new Pop3InvalidCommandException("All characters in the string must be in the range 0x00 - 0xff.", innerException);
			}
			this.commandBytes = bufferBuilder.GetBuffer();
			this.listings = listings;
		}

		private Pop3Command(SecureString password)
		{
			this.type = Pop3CommandType.Pass;
			BufferBuilder bufferBuilder = new BufferBuilder(Pop3Command.PassBytes.Length + password.Length + Pop3Command.CrLfBytes.Length);
			bufferBuilder.Append(Pop3Command.PassBytes);
			try
			{
				bufferBuilder.Append(password);
			}
			catch (ArgumentException innerException)
			{
				throw new Pop3InvalidCommandException("All characters in the string must be in the range 0x00 - 0xff.", innerException);
			}
			bufferBuilder.Append(Pop3Command.CrLfBytes);
			this.commandBytes = bufferBuilder.GetBuffer();
		}

		internal Pop3CommandType Type
		{
			get
			{
				return this.type;
			}
		}

		internal byte[] Buffer
		{
			get
			{
				return this.commandBytes;
			}
		}

		internal bool Listings
		{
			get
			{
				return this.listings;
			}
		}

		public override string ToString()
		{
			if (this.type == Pop3CommandType.Pass)
			{
				return "PASS *****";
			}
			if (this.type == Pop3CommandType.Blob)
			{
				return "<BLOB>";
			}
			return Encoding.ASCII.GetString(this.commandBytes).TrimEnd(new char[0]);
		}

		internal static Pop3Command Blob(byte[] blob)
		{
			return new Pop3Command(Pop3CommandType.Blob, false, "{0}\r\n", new object[]
			{
				Encoding.ASCII.GetString(blob)
			});
		}

		internal static Pop3Command List()
		{
			return Pop3Command.list;
		}

		internal static Pop3Command Uidl()
		{
			return Pop3Command.uidl;
		}

		internal static Pop3Command Retr(int messageNumber)
		{
			return new Pop3Command(Pop3CommandType.Retr, true, "retr {0}\r\n", new object[]
			{
				messageNumber
			});
		}

		internal static Pop3Command Top(int messageNumber, int lineCount)
		{
			return new Pop3Command(Pop3CommandType.Top, true, "top {0} {1}\r\n", new object[]
			{
				messageNumber,
				lineCount
			});
		}

		internal static Pop3Command Dele(int messageNumber)
		{
			return new Pop3Command(Pop3CommandType.Dele, false, "dele {0}\r\n", new object[]
			{
				messageNumber
			});
		}

		internal static Pop3Command User(string userName)
		{
			return new Pop3Command(Pop3CommandType.User, false, "user {0}\r\n", new object[]
			{
				userName
			});
		}

		internal static Pop3Command Pass(SecureString password)
		{
			return new Pop3Command(password);
		}

		internal static Pop3Command Auth(string mechanism)
		{
			return new Pop3Command(Pop3CommandType.Auth, false, "auth {0}\r\n", new object[]
			{
				mechanism
			});
		}

		internal void ClearBuffer()
		{
			if (this.type == Pop3CommandType.Pass)
			{
				Array.Clear(this.commandBytes, 5, this.commandBytes.Length - 7);
			}
		}

		private const string PassString = "PASS *****";

		private const string BlobString = "<BLOB>";

		internal static readonly Pop3Command AuthNtlm = new Pop3Command(Pop3CommandType.Auth, false, "auth ntlm\r\n");

		internal static readonly Pop3Command Capa = new Pop3Command(Pop3CommandType.Capa, true, "capa\r\n");

		internal static readonly Pop3Command Stls = new Pop3Command(Pop3CommandType.Stls, false, "stls\r\n");

		internal static readonly Pop3Command Stat = new Pop3Command(Pop3CommandType.Stat, false, "stat\r\n");

		internal static readonly Pop3Command Quit = new Pop3Command(Pop3CommandType.Quit, false, "quit\r\n");

		private static readonly Pop3Command list = new Pop3Command(Pop3CommandType.List, true, "list\r\n");

		private static readonly Pop3Command uidl = new Pop3Command(Pop3CommandType.Uidl, true, "uidl\r\n");

		private static readonly byte[] PassBytes = Encoding.ASCII.GetBytes("pass ");

		private static readonly byte[] CrLfBytes = Encoding.ASCII.GetBytes("\r\n");

		private readonly Pop3CommandType type;

		private readonly byte[] commandBytes;

		private readonly bool listings;
	}
}
