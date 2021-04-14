using System;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	public class ReceivedHeader : Header
	{
		internal ReceivedHeader() : base("Received", HeaderId.Received)
		{
		}

		public ReceivedHeader(string from, string fromTcpInfo, string by, string byTcpInfo, string forMailbox, string with, string id, string via, string date) : base("Received", HeaderId.Received)
		{
			int num = -1;
			int num2 = ByteString.StringToBytesCount(from, true);
			int num3 = ByteString.StringToBytesCount(fromTcpInfo, true);
			int num4 = ByteString.StringToBytesCount(by, true);
			int num5 = ByteString.StringToBytesCount(byTcpInfo, true);
			int num6 = ByteString.StringToBytesCount(forMailbox, true);
			int num7 = ByteString.StringToBytesCount(with, true);
			int num8 = ByteString.StringToBytesCount(id, true);
			int num9 = ByteString.StringToBytesCount(via, true);
			int num10 = ByteString.StringToBytesCount(date, false);
			num += ((from != null) ? (num2 + ReceivedHeader.ParamFrom.Length + 2) : 0);
			num += ((fromTcpInfo != null) ? (num3 + ((from == null) ? (ReceivedHeader.ParamFrom.Length + 1) : 0) + 3) : 0);
			num += ((by != null) ? (num4 + ReceivedHeader.ParamBy.Length + 2) : 0);
			num += ((byTcpInfo != null) ? (num5 + ((by == null) ? (ReceivedHeader.ParamBy.Length + 1) : 0) + 3) : 0);
			num += ((forMailbox != null) ? (num6 + ReceivedHeader.ParamFor.Length + 2) : 0);
			num += ((with != null) ? (num7 + ReceivedHeader.ParamWith.Length + 2) : 0);
			num += ((id != null) ? (num8 + ReceivedHeader.ParamId.Length + 2) : 0);
			num += ((via != null) ? (num9 + ReceivedHeader.ParamVia.Length + 2) : 0);
			num += ((date != null) ? (num10 + ((-1 == num) ? 3 : 2)) : 0);
			if (-1 == num)
			{
				return;
			}
			byte[] array = new byte[num];
			int num11 = 0;
			this.AppendNameValue(ReceivedHeader.ParamFrom, from, array, ref num11);
			if (fromTcpInfo != null)
			{
				if (from == null)
				{
					this.AppendName(ReceivedHeader.ParamFrom, array, ref num11);
				}
				array[num11++] = 32;
				array[num11++] = 40;
				ByteString.StringToBytes(fromTcpInfo, array, num11, true);
				num11 += num3;
				array[num11++] = 41;
			}
			this.AppendNameValue(ReceivedHeader.ParamBy, by, array, ref num11);
			if (byTcpInfo != null)
			{
				if (by == null)
				{
					this.AppendName(ReceivedHeader.ParamBy, array, ref num11);
				}
				array[num11++] = 32;
				array[num11++] = 40;
				ByteString.StringToBytes(byTcpInfo, array, num11, true);
				num11 += num5;
				array[num11++] = 41;
			}
			this.AppendNameValue(ReceivedHeader.ParamFor, forMailbox, array, ref num11);
			this.AppendNameValue(ReceivedHeader.ParamWith, with, array, ref num11);
			this.AppendNameValue(ReceivedHeader.ParamId, id, array, ref num11);
			this.AppendNameValue(ReceivedHeader.ParamVia, via, array, ref num11);
			if (date != null)
			{
				ByteString.ValidateStringArgument(date, false);
				array[num11++] = 59;
				array[num11++] = 32;
				ByteString.StringToBytes(date, array, num11, false);
				num11 += num10;
			}
			this.RawValue = array;
		}

		public string From
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				return this.fromValue;
			}
		}

		public string FromTcpInfo
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				return this.fromTcpInfoValue;
			}
		}

		public string By
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				return this.byValue;
			}
		}

		public string ByTcpInfo
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				return this.byTcpInfoValue;
			}
		}

		public string Via
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				return this.viaValue;
			}
		}

		public string With
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				return this.withValue;
			}
		}

		public string Id
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				return this.idValue;
			}
		}

		public string For
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				return this.forValue;
			}
		}

		public string Date
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				return this.dateValue;
			}
		}

		public sealed override string Value
		{
			get
			{
				return base.GetRawValue(true);
			}
			set
			{
				throw new NotSupportedException(Strings.UnicodeMimeHeaderReceivedNotSupported);
			}
		}

		public sealed override bool RequiresSMTPUTF8
		{
			get
			{
				return !MimeString.IsPureASCII(base.Lines);
			}
		}

		internal override void RawValueAboutToChange()
		{
			this.parsed = false;
		}

		public sealed override MimeNode Clone()
		{
			ReceivedHeader receivedHeader = new ReceivedHeader();
			this.CopyTo(receivedHeader);
			return receivedHeader;
		}

		public sealed override void CopyTo(object destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (destination == this)
			{
				return;
			}
			ReceivedHeader receivedHeader = destination as ReceivedHeader;
			if (receivedHeader == null)
			{
				throw new ArgumentException(Strings.CantCopyToDifferentObjectType);
			}
			base.CopyTo(destination);
			receivedHeader.parsed = this.parsed;
			receivedHeader.fromValue = this.fromValue;
			receivedHeader.fromTcpInfoValue = this.fromTcpInfoValue;
			receivedHeader.byValue = this.byValue;
			receivedHeader.byTcpInfoValue = this.byTcpInfoValue;
			receivedHeader.viaValue = this.viaValue;
			receivedHeader.withValue = this.withValue;
			receivedHeader.idValue = this.idValue;
			receivedHeader.forValue = this.forValue;
			receivedHeader.dateValue = this.dateValue;
		}

		public sealed override bool IsValueValid(string value)
		{
			return false;
		}

		internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
		{
			throw new NotSupportedException(Strings.ChildrenCannotBeAddedToReceivedHeader);
		}

		private void AppendNameValue(byte[] name, string value, byte[] array, ref int offset)
		{
			if (value != null)
			{
				this.AppendName(name, array, ref offset);
				array[offset++] = 32;
				int num = ByteString.StringToBytes(value, array, offset, true);
				offset += num;
			}
		}

		private void AppendName(byte[] name, byte[] array, ref int offset)
		{
			if (offset != 0)
			{
				array[offset++] = 32;
			}
			Buffer.BlockCopy(name, 0, array, offset, name.Length);
			offset += name.Length;
		}

		private void Reset()
		{
			this.parsed = false;
			this.fromValue = null;
			this.fromTcpInfoValue = null;
			this.byValue = null;
			this.byTcpInfoValue = null;
			this.viaValue = null;
			this.withValue = null;
			this.idValue = null;
			this.forValue = null;
			this.dateValue = null;
		}

		private void Parse()
		{
			this.Reset();
			this.parsed = true;
			DecodingOptions headerDecodingOptions = base.GetHeaderDecodingOptions();
			ValueParser valueParser = new ValueParser(base.Lines, headerDecodingOptions.AllowUTF8);
			MimeStringList mimeStringList = default(MimeStringList);
			MimeStringList mimeStringList2 = default(MimeStringList);
			MimeString mimeString = default(MimeString);
			MimeString mimeString2 = MimeString.Empty;
			ReceivedHeader.ParseState parseState = ReceivedHeader.ParseState.None;
			for (;;)
			{
				valueParser.ParseWhitespace(true, ref mimeStringList);
				byte b = valueParser.ParseGet();
				if (b == 0)
				{
					break;
				}
				if (59 == b)
				{
					parseState = ReceivedHeader.ParseState.Date;
				}
				else if (40 == b && parseState == ReceivedHeader.ParseState.DomainValue)
				{
					parseState = ReceivedHeader.ParseState.DomainAddress;
				}
				else
				{
					valueParser.ParseUnget();
					mimeString = valueParser.ParseToken();
					if (mimeString.Length == 0)
					{
						valueParser.ParseGet();
						mimeStringList2.TakeOverAppend(ref mimeStringList);
						valueParser.ParseAppendLastByte(ref mimeStringList2);
						continue;
					}
					ReceivedHeader.ParseState parseState2 = this.StateFromToken(mimeString);
					if (ReceivedHeader.ParseState.None != parseState2)
					{
						parseState = parseState2;
					}
				}
				switch (parseState)
				{
				case ReceivedHeader.ParseState.Domain:
				case ReceivedHeader.ParseState.OptInfo:
					if (mimeString2.Length != 0)
					{
						this.FinishClause(ref mimeString2, ref mimeStringList2, headerDecodingOptions.AllowUTF8);
					}
					else
					{
						mimeStringList2.Reset();
					}
					mimeString2 = mimeString;
					valueParser.ParseWhitespace(false, ref mimeStringList);
					mimeStringList.Reset();
					parseState++;
					break;
				case ReceivedHeader.ParseState.DomainValue:
					mimeStringList2.TakeOverAppend(ref mimeStringList);
					mimeStringList2.AppendFragment(mimeString);
					break;
				case ReceivedHeader.ParseState.DomainAddress:
				{
					bool flag = mimeString2.CompareEqI(ReceivedHeader.ParamFrom);
					this.FinishClause(ref mimeString2, ref mimeStringList2, headerDecodingOptions.AllowUTF8);
					mimeStringList.Reset();
					valueParser.ParseUnget();
					valueParser.ParseComment(true, false, ref mimeStringList2, true);
					byte[] sz = mimeStringList2.GetSz();
					string text = (sz == null) ? null : ByteString.BytesToString(sz, headerDecodingOptions.AllowUTF8);
					if (flag)
					{
						this.fromTcpInfoValue = text;
					}
					else
					{
						this.byTcpInfoValue = text;
					}
					mimeStringList2.Reset();
					parseState = ReceivedHeader.ParseState.None;
					break;
				}
				case ReceivedHeader.ParseState.OptInfoValue:
					mimeStringList2.TakeOverAppend(ref mimeStringList);
					mimeStringList2.AppendFragment(mimeString);
					break;
				case ReceivedHeader.ParseState.Date:
				{
					this.FinishClause(ref mimeString2, ref mimeStringList2, headerDecodingOptions.AllowUTF8);
					mimeStringList.Reset();
					valueParser.ParseWhitespace(false, ref mimeStringList);
					valueParser.ParseToEnd(ref mimeStringList2);
					byte[] sz2 = mimeStringList2.GetSz();
					this.dateValue = ((sz2 == null) ? null : ByteString.BytesToString(sz2, false));
					break;
				}
				case ReceivedHeader.ParseState.None:
					mimeStringList2.Reset();
					break;
				}
			}
			this.FinishClause(ref mimeString2, ref mimeStringList2, headerDecodingOptions.AllowUTF8);
		}

		internal override void ForceParse()
		{
			if (!this.parsed)
			{
				this.Parse();
			}
		}

		private void FinishClause(ref MimeString param, ref MimeStringList value, bool allowUTF8)
		{
			if (param.Length != 0)
			{
				byte[] sz = value.GetSz();
				string text = (sz == null) ? null : ByteString.BytesToString(sz, allowUTF8);
				uint num = param.ComputeCrcI();
				if (num <= 2556329580U)
				{
					if (num != 271896810U)
					{
						if (num != 2115158205U)
						{
							if (num == 2556329580U)
							{
								this.fromValue = text;
							}
						}
						else
						{
							this.byValue = text;
						}
					}
					else
					{
						this.forValue = text;
					}
				}
				else if (num != 3117694226U)
				{
					if (num != 3740702146U)
					{
						if (num == 4276123055U)
						{
							this.idValue = text;
						}
					}
					else
					{
						this.viaValue = text;
					}
				}
				else
				{
					this.withValue = text;
				}
				value.Reset();
				param = MimeString.Empty;
			}
		}

		private ReceivedHeader.ParseState StateFromToken(MimeString token)
		{
			uint num = token.ComputeCrcI();
			if (num <= 2556329580U)
			{
				if (num == 271896810U)
				{
					return ReceivedHeader.ParseState.OptInfo;
				}
				if (num == 2115158205U)
				{
					return ReceivedHeader.ParseState.Domain;
				}
				if (num == 2556329580U)
				{
					return ReceivedHeader.ParseState.Domain;
				}
			}
			else
			{
				if (num == 3117694226U)
				{
					return ReceivedHeader.ParseState.OptInfo;
				}
				if (num == 3740702146U)
				{
					return ReceivedHeader.ParseState.OptInfo;
				}
				if (num == 4276123055U)
				{
					return ReceivedHeader.ParseState.OptInfo;
				}
			}
			return ReceivedHeader.ParseState.None;
		}

		internal const bool AllowUTF8Value = true;

		private const uint ParamFromCRC = 2556329580U;

		private const uint ParamByCRC = 2115158205U;

		private const uint ParamViaCRC = 3740702146U;

		private const uint ParamWithCRC = 3117694226U;

		private const uint ParamIdCRC = 4276123055U;

		private const uint ParamForCRC = 271896810U;

		private static readonly byte[] ParamFrom = ByteString.StringToBytes("from", true);

		private static readonly byte[] ParamBy = ByteString.StringToBytes("by", true);

		private static readonly byte[] ParamVia = ByteString.StringToBytes("via", true);

		private static readonly byte[] ParamWith = ByteString.StringToBytes("with", true);

		private static readonly byte[] ParamId = ByteString.StringToBytes("id", true);

		private static readonly byte[] ParamFor = ByteString.StringToBytes("for", true);

		private static readonly byte[] ParamDate = ByteString.StringToBytes("date", true);

		private static readonly byte[] ParamFromTcpInfo = ByteString.StringToBytes("x-from-tcp-info", true);

		private static readonly byte[] ParamByTcpInfo = ByteString.StringToBytes("x-by-tcp-info", true);

		private string fromValue;

		private string fromTcpInfoValue;

		private string byValue;

		private string byTcpInfoValue;

		private string viaValue;

		private string withValue;

		private string idValue;

		private string forValue;

		private string dateValue;

		private bool parsed;

		private enum ParseState
		{
			Domain,
			DomainValue,
			DomainAddress,
			OptInfo,
			OptInfoValue,
			Date,
			None
		}
	}
}
