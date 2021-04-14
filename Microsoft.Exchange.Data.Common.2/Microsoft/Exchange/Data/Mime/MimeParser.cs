using System;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	internal class MimeParser
	{
		public MimeParser(bool expectBinaryContent)
		{
			this.parseInlineAttachments = true;
			this.expectBinaryContent = expectBinaryContent;
			this.Reset();
		}

		public MimeParser(bool parseEmbeddedMessages, bool parseInlineAttachments, bool expectBinaryContent)
		{
			this.parseEmbeddedMessages = parseEmbeddedMessages;
			this.parseInlineAttachments = parseInlineAttachments;
			this.expectBinaryContent = expectBinaryContent;
			this.Reset();
		}

		public bool IsEndOfFile
		{
			get
			{
				return this.state == MimeParser.ParseState.EndOfFile;
			}
		}

		public int Position
		{
			get
			{
				return this.position;
			}
		}

		public int Depth
		{
			get
			{
				return this.parseStackTop;
			}
		}

		public int PartDepth
		{
			get
			{
				if (this.parseStackTop != 0)
				{
					return this.parseStack[this.parseStackTop - 1].PartDepth;
				}
				return 0;
			}
		}

		public int HeaderNameLength
		{
			get
			{
				return this.headerNameLength;
			}
		}

		public int HeaderDataOffset
		{
			get
			{
				return this.headerDataOffset;
			}
		}

		public bool IsHeaderComplete
		{
			get
			{
				return this.headerComplete;
			}
		}

		public MimeComplianceStatus ComplianceStatus
		{
			get
			{
				return this.compliance;
			}
			set
			{
				this.compliance = value;
			}
		}

		public bool IsMime
		{
			get
			{
				return this.mime;
			}
		}

		public MajorContentType ContentType
		{
			get
			{
				return this.currentLevel.ContentType;
			}
		}

		public ContentTransferEncoding TransferEncoding
		{
			get
			{
				return this.currentLevel.TransferEncoding;
			}
		}

		public ContentTransferEncoding InlineFormat
		{
			get
			{
				return this.inlineFormat;
			}
		}

		public void Reset()
		{
			this.state = MimeParser.ParseState.Headers;
			this.currentOffset = 0;
			this.lineOffset = 0;
			this.mime = false;
			this.currentLevel.Reset(true);
			this.parseStackTop = 0;
			this.position = 0;
			this.lastTokenLength = 0;
			this.firstHeader = true;
			this.nextBoundaryLevel = -1;
			this.nextBoundaryEnd = false;
			this.headerNameLength = 0;
			this.headerDataOffset = 0;
			this.headerComplete = false;
			this.inlineFormat = ContentTransferEncoding.Unknown;
			this.compliance = MimeComplianceStatus.Compliant;
		}

		public void SetMIME()
		{
			if (this.parseStackTop == 0 || this.parseStack[this.parseStackTop - 1].ContentType == MajorContentType.MessageRfc822)
			{
				if (this.parseStackTop == 0)
				{
					this.mime = true;
				}
				this.currentLevel.IsMime = true;
			}
		}

		public void SetContentType(MajorContentType contentType, MimeString boundaryValue)
		{
			this.currentLevel.SetContentType(contentType, boundaryValue);
			if (contentType != MajorContentType.Multipart)
			{
				this.nextBoundaryLevel = -1;
			}
		}

		public void SetTransferEncoding(ContentTransferEncoding encoding)
		{
			this.currentLevel.TransferEncoding = encoding;
		}

		public void SetStreamMode()
		{
			this.state = MimeParser.ParseState.Body;
			this.currentLevel.StreamMode = true;
		}

		public void ReportConsumedData(int lengthConsumed)
		{
			this.lastTokenLength -= lengthConsumed;
			this.position += lengthConsumed;
		}

		public MimeToken Parse(byte[] data, int start, int end, bool flush)
		{
			int num = start + this.currentOffset;
			int num2 = start + this.lineOffset;
			switch (this.state)
			{
			case MimeParser.ParseState.Headers:
			{
				bool flag = false;
				int num3 = ByteString.IndexOf(data, 10, num, end - num);
				if (num3 == -1)
				{
					num3 = end;
				}
				if (num3 == end)
				{
					if ((end - start <= 998 && !flush) || (!flush && end - start <= 999 && data[end - 1] == 13))
					{
						this.currentOffset = end - start;
						return new MimeToken(MimeTokenId.None, 0);
					}
				}
				else if (num3 == start || data[num3 - 1] != 13)
				{
					this.compliance |= MimeComplianceStatus.BareLinefeedInHeader;
					flag = true;
				}
				else
				{
					num3--;
				}
				this.headerNameLength = 0;
				this.headerDataOffset = 0;
				int num4;
				if (num3 - start > 998)
				{
					this.compliance |= MimeComplianceStatus.InvalidWrapping;
					this.currentOffset = num3 - (start + 998);
					this.lineOffset = num2 - (start + 998);
					num3 = start + 998;
					num4 = 0;
				}
				else
				{
					this.currentOffset = 0;
					this.lineOffset = ((num3 == end) ? (num2 - num3) : 0);
					num4 = ((num3 == end) ? 0 : (flag ? 1 : 2));
				}
				if (num3 == start)
				{
					this.state = MimeParser.ParseState.EndOfHeaders;
					this.lastTokenLength = num4;
					return new MimeToken(MimeTokenId.EndOfHeaders, this.lastTokenLength);
				}
				if (num4 != 0 && num3 + num4 < end && data[num3 + num4] != 32 && data[num3 + num4] != 9)
				{
					this.headerComplete = true;
				}
				else
				{
					this.headerComplete = false;
				}
				if (!this.firstHeader && (num2 < start || data[num2] == 32 || data[num2] == 9))
				{
					this.lastTokenLength = num3 + num4 - start;
					return new MimeToken(MimeTokenId.HeaderContinuation, this.lastTokenLength);
				}
				this.firstHeader = false;
				int num5 = 0;
				this.headerNameLength = MimeScan.FindEndOf(MimeScan.Token.Field, data, start, num3 - start, out num5, false);
				if (this.headerNameLength == 0)
				{
					this.compliance |= MimeComplianceStatus.InvalidHeader;
					this.lastTokenLength = num3 + num4 - start;
					return new MimeToken(MimeTokenId.Header, this.lastTokenLength);
				}
				int num6 = start + this.headerNameLength;
				if (num6 == num3 || data[num6] != 58)
				{
					num6 += MimeScan.SkipLwsp(data, num6, num3 - num6);
					if (num6 == num3 || data[num6] != 58)
					{
						this.headerNameLength = 0;
						if (this.mime && (this.parseStackTop > 0 || this.currentLevel.ContentType == MajorContentType.Multipart) && num3 - num2 > 2 && data[num2] == 45 && data[num2 + 1] == 45 && this.FindBoundary(data, num2, num3, out this.nextBoundaryLevel, out this.nextBoundaryEnd))
						{
							this.compliance |= MimeComplianceStatus.MissingBodySeparator;
							if (this.nextBoundaryLevel != this.parseStackTop)
							{
								this.compliance |= MimeComplianceStatus.MissingBoundary;
							}
							this.lineOffset = 0;
							this.currentOffset = num3 - start;
							this.state = MimeParser.ParseState.EndOfHeaders;
							return new MimeToken(MimeTokenId.EndOfHeaders, 0);
						}
						this.compliance |= MimeComplianceStatus.InvalidHeader;
						this.lastTokenLength = num3 + num4 - start;
						return new MimeToken(MimeTokenId.Header, this.lastTokenLength);
					}
				}
				this.headerDataOffset = num6 + 1 - start;
				this.lastTokenLength = num3 + num4 - start;
				return new MimeToken(MimeTokenId.Header, this.lastTokenLength);
			}
			case MimeParser.ParseState.EndOfHeaders:
				this.CheckMimeConstraints();
				if (this.mime && this.parseEmbeddedMessages && this.currentLevel.ContentType == MajorContentType.MessageRfc822 && !this.currentLevel.StreamMode)
				{
					this.PushLevel(false);
					this.lastTokenLength = 0;
					return new MimeToken(MimeTokenId.EmbeddedStart, 0);
				}
				this.state = MimeParser.ParseState.Body;
				break;
			case MimeParser.ParseState.Body:
				break;
			default:
				return new MimeToken(MimeTokenId.EndOfFile, 0);
			}
			return this.ParseBody(data, start, end, flush, num2, num);
		}

		private static bool IsUUEncodeBegin(byte[] data, int line, int nextNL)
		{
			MimeString mimeString = new MimeString(data, line, nextNL - line);
			if (mimeString.Length < 13 || !mimeString.HasPrefixEq(MimeParser.UUBegin, 0, 6))
			{
				return false;
			}
			int num = 6;
			while (num < 10 && 48 <= mimeString[num] && 55 >= mimeString[num])
			{
				num++;
			}
			return num != 6 && 32 == mimeString[num];
		}

		private static bool IsUUEncodeEnd(byte[] data, int line, int nextNL)
		{
			MimeString mimeString = new MimeString(data, line, nextNL - line);
			return mimeString.Length >= 3 && mimeString.HasPrefixEq(MimeParser.UUEnd, 0, 3);
		}

		private MimeToken ParseBody(byte[] data, int start, int end, bool flush, int line, int current)
		{
			int num = (line <= start) ? 0 : (line - start);
			int num2;
			int num3;
			bool flag2;
			for (;;)
			{
				if (this.expectBinaryContent)
				{
					num2 = ByteString.IndexOf(data, 10, current, end - current);
				}
				else
				{
					bool flag;
					num2 = ByteString.IndexOf(data, 10, current, end - current, out flag);
					if (flag)
					{
						this.compliance |= MimeComplianceStatus.UnexpectedBinaryContent;
					}
				}
				if (num2 == -1)
				{
					num2 = end;
				}
				if (num2 == end)
				{
					num3 = 0;
					if (end - start != 0 && data[end - 1] == 13 && !flush)
					{
						num2--;
						end--;
					}
				}
				else if (num2 == start || data[num2 - 1] != 13)
				{
					if (this.currentLevel.TransferEncoding != ContentTransferEncoding.Binary)
					{
						this.compliance |= MimeComplianceStatus.BareLinefeedInBody;
					}
					num3 = 1;
				}
				else
				{
					num2--;
					num3 = 2;
				}
				if (num2 - line > 998 && this.currentLevel.TransferEncoding != ContentTransferEncoding.Binary)
				{
					this.compliance |= MimeComplianceStatus.InvalidWrapping;
				}
				if (this.nextBoundaryLevel != -1)
				{
					goto IL_2AF;
				}
				if (!this.mime || line < start || (num2 != end && num2 == line) || (num2 != line && data[line] != 45) || num2 - line > 998)
				{
					if (!this.parseInlineAttachments || this.currentLevel.IsMime || line < start || (num2 != end && num2 == line) || (num2 != line && ((this.inlineFormat == ContentTransferEncoding.Unknown && (data[line] | 32) != 98) || (this.inlineFormat == ContentTransferEncoding.UUEncode && (data[line] | 32) != 101))) || num2 - line > 998)
					{
						if (num2 == end)
						{
							break;
						}
						current = num2 + num3;
						line = current;
						num = num3;
						continue;
					}
					else
					{
						flag2 = false;
					}
				}
				else
				{
					flag2 = true;
				}
				if (num2 == end && !flush)
				{
					goto Block_27;
				}
				if (!flag2 || num2 - line <= 2 || (this.parseStackTop == 0 && this.currentLevel.ContentType != MajorContentType.Multipart) || data[line + 1] != 45 || !this.FindBoundary(data, line, num2, out this.nextBoundaryLevel, out this.nextBoundaryEnd))
				{
					if (!this.parseInlineAttachments || this.currentLevel.IsMime || !this.IsInlineBoundary(data, line, num2, end, out this.nextBoundaryLevel, out this.nextBoundaryEnd))
					{
						if (num2 == end)
						{
							goto Block_36;
						}
						current = num2 + num3;
						line = current;
						num = num3;
						continue;
					}
					else
					{
						flag2 = false;
					}
				}
				if (this.nextBoundaryLevel != this.parseStackTop || (!this.currentLevel.Epilogue && !this.nextBoundaryEnd))
				{
					goto IL_293;
				}
				this.compliance |= MimeComplianceStatus.MissingBoundary;
				this.nextBoundaryLevel = -1;
				this.currentLevel.Epilogue = true;
				if (num2 == end)
				{
					goto IL_28F;
				}
				current = num2 + num3;
				line = current;
				num = num3;
			}
			int num4 = end;
			goto IL_2BB;
			Block_27:
			num4 = ((line < start + num || !flag2) ? line : (line - num));
			goto IL_2BB;
			Block_36:
			num4 = end;
			goto IL_2BB;
			IL_28F:
			num4 = end;
			goto IL_2BB;
			IL_293:
			if (line - start > (flag2 ? num : 0))
			{
				num4 = line - (flag2 ? num : 0);
				goto IL_2BB;
			}
			IL_2AF:
			return this.ProcessBoundary(start, line, num2, num3);
			IL_2BB:
			this.lineOffset = line - num4;
			this.currentOffset = num2 - num4;
			if (num4 != start)
			{
				this.lastTokenLength = num4 - start;
				return new MimeToken(MimeTokenId.PartData, this.lastTokenLength);
			}
			if (!flush)
			{
				return new MimeToken(MimeTokenId.None, 0);
			}
			return this.ProcessEOF();
		}

		private void PushLevel(bool inheritMime)
		{
			if (this.parseStack == null || this.parseStackTop == this.parseStack.Length)
			{
				int num = (this.parseStack == null) ? 4 : (this.parseStack.Length * 2);
				MimeParser.ParseLevel[] destinationArray = new MimeParser.ParseLevel[num];
				if (this.parseStack != null)
				{
					Array.Copy(this.parseStack, 0, destinationArray, 0, this.parseStackTop);
				}
				for (int i = 0; i < this.parseStackTop; i++)
				{
					this.parseStack[i] = default(MimeParser.ParseLevel);
				}
				this.parseStack = destinationArray;
			}
			if (this.currentLevel.ContentType != MajorContentType.MessageRfc822)
			{
				this.currentLevel.PartDepth = ((this.parseStackTop == 0) ? 1 : (this.parseStack[this.parseStackTop - 1].PartDepth + 1));
			}
			this.parseStack[this.parseStackTop++] = this.currentLevel;
			this.currentLevel.Reset(!inheritMime);
			this.state = MimeParser.ParseState.Headers;
			this.firstHeader = true;
		}

		private void CheckMimeConstraints()
		{
			if (!this.mime)
			{
				this.currentLevel.SetContentType(MajorContentType.Other, default(MimeString));
				this.currentLevel.TransferEncoding = ContentTransferEncoding.SevenBit;
				return;
			}
			if ((this.currentLevel.ContentType == MajorContentType.Multipart || this.currentLevel.ContentType == MajorContentType.MessageRfc822 || this.currentLevel.ContentType == MajorContentType.Message) && this.currentLevel.TransferEncoding > ContentTransferEncoding.Binary)
			{
				this.compliance |= MimeComplianceStatus.InvalidTransferEncoding;
			}
			if (this.parseStackTop != 0 && this.currentLevel.TransferEncoding <= ContentTransferEncoding.Binary && this.currentLevel.TransferEncoding > this.parseStack[this.parseStackTop - 1].TransferEncoding)
			{
				this.compliance |= MimeComplianceStatus.InvalidTransferEncoding;
			}
		}

		private bool FindBoundary(byte[] data, int line, int nextNL, out int nextBoundaryLevel, out bool nextBoundaryEnd)
		{
			while (nextNL > line && MimeScan.IsLWSP(data[nextNL - 1]))
			{
				nextNL--;
			}
			uint num = ByteString.ComputeCrc(data, line, nextNL - line);
			bool flag;
			if (this.currentLevel.IsBoundary(data, line, nextNL - line, (long)((ulong)num), out flag))
			{
				nextBoundaryLevel = this.parseStackTop;
				nextBoundaryEnd = flag;
				return true;
			}
			for (int i = this.parseStackTop - 1; i >= 0; i--)
			{
				if (this.parseStack[i].IsBoundary(data, line, nextNL - line, (long)((ulong)num), out flag))
				{
					nextBoundaryLevel = i;
					nextBoundaryEnd = flag;
					return true;
				}
			}
			nextBoundaryLevel = -1;
			nextBoundaryEnd = false;
			return false;
		}

		private bool IsInlineBoundary(byte[] data, int line, int nextNL, int end, out int nextBoundaryLevel, out bool nextBoundaryEnd)
		{
			ContentTransferEncoding contentTransferEncoding = this.inlineFormat;
			if (contentTransferEncoding != ContentTransferEncoding.Unknown)
			{
				if (contentTransferEncoding == ContentTransferEncoding.UUEncode)
				{
					if ((data[line] | 32) == 101 && nextNL - line >= 3 && MimeParser.IsUUEncodeEnd(data, line, nextNL))
					{
						nextBoundaryLevel = -100;
						nextBoundaryEnd = true;
						return true;
					}
				}
			}
			else if ((data[line] | 32) == 98 && nextNL - line >= 11 && nextNL != end && MimeParser.IsUUEncodeBegin(data, line, nextNL))
			{
				nextBoundaryLevel = -100;
				nextBoundaryEnd = false;
				return true;
			}
			nextBoundaryLevel = -1;
			nextBoundaryEnd = false;
			return false;
		}

		private MimeToken ProcessBoundary(int start, int line, int nextNL, int sizeNL)
		{
			if (this.nextBoundaryLevel < 0)
			{
				this.lineOffset = 0;
				this.currentOffset = 0;
				if (!this.nextBoundaryEnd)
				{
					this.inlineFormat = ((this.nextBoundaryLevel == -100) ? ContentTransferEncoding.UUEncode : ContentTransferEncoding.BinHex);
					this.nextBoundaryLevel = -1;
					this.lastTokenLength = nextNL + sizeNL - start;
					return new MimeToken(MimeTokenId.InlineStart, this.lastTokenLength);
				}
				this.inlineFormat = ContentTransferEncoding.Unknown;
				this.nextBoundaryLevel = -1;
				this.lastTokenLength = nextNL + sizeNL - start;
				return new MimeToken(MimeTokenId.InlineEnd, this.lastTokenLength);
			}
			else
			{
				if (this.nextBoundaryLevel == this.parseStackTop)
				{
					this.lineOffset = 0;
					this.currentOffset = 0;
					this.nextBoundaryLevel = -1;
					this.PushLevel(true);
					this.lastTokenLength = nextNL + sizeNL - start;
					return new MimeToken(MimeTokenId.NestedStart, this.lastTokenLength);
				}
				if (this.nextBoundaryLevel == this.parseStackTop - 1)
				{
					if (this.currentLevel.ContentType == MajorContentType.Multipart && !this.currentLevel.Epilogue)
					{
						this.compliance |= MimeComplianceStatus.MissingBoundary;
					}
					this.lineOffset = 0;
					this.currentOffset = 0;
					this.nextBoundaryLevel = -1;
					if (this.nextBoundaryEnd)
					{
						this.currentLevel = this.parseStack[--this.parseStackTop];
						this.currentLevel.Epilogue = true;
						this.parseStack[this.parseStackTop].Reset(false);
						this.lastTokenLength = nextNL + sizeNL - start;
						return new MimeToken(MimeTokenId.NestedEnd, this.lastTokenLength);
					}
					this.currentLevel.Reset(false);
					this.state = MimeParser.ParseState.Headers;
					this.firstHeader = true;
					this.lastTokenLength = nextNL + sizeNL - start;
					return new MimeToken(MimeTokenId.NestedNext, this.lastTokenLength);
				}
				else
				{
					this.lineOffset = line - start;
					this.currentOffset = nextNL - start;
					if (this.inlineFormat != ContentTransferEncoding.Unknown)
					{
						this.compliance |= MimeComplianceStatus.MissingBoundary;
						this.inlineFormat = ContentTransferEncoding.Unknown;
						return new MimeToken(MimeTokenId.InlineEnd, 0);
					}
					this.currentLevel = this.parseStack[--this.parseStackTop];
					this.currentLevel.Epilogue = true;
					this.parseStack[this.parseStackTop].Reset(false);
					if (this.currentLevel.ContentType == MajorContentType.MessageRfc822)
					{
						return new MimeToken(MimeTokenId.EmbeddedEnd, 0);
					}
					this.compliance |= MimeComplianceStatus.MissingBoundary;
					return new MimeToken(MimeTokenId.NestedEnd, 0);
				}
			}
		}

		private MimeToken ProcessEOF()
		{
			if (this.inlineFormat != ContentTransferEncoding.Unknown)
			{
				this.compliance |= MimeComplianceStatus.MissingBoundary;
				this.inlineFormat = ContentTransferEncoding.Unknown;
				return new MimeToken(MimeTokenId.InlineEnd, 0);
			}
			if (this.parseStackTop == 0)
			{
				this.state = MimeParser.ParseState.EndOfFile;
				this.currentLevel.Reset(true);
				return new MimeToken(MimeTokenId.EndOfFile, 0);
			}
			this.currentLevel = this.parseStack[--this.parseStackTop];
			this.currentLevel.Epilogue = true;
			this.parseStack[this.parseStackTop].Reset(false);
			if (this.currentLevel.ContentType == MajorContentType.MessageRfc822)
			{
				return new MimeToken(MimeTokenId.EmbeddedEnd, 0);
			}
			this.compliance |= MimeComplianceStatus.MissingBoundary;
			return new MimeToken(MimeTokenId.NestedEnd, 0);
		}

		private static readonly byte[] UUBegin = ByteString.StringToBytes("begin ", true);

		private static readonly byte[] UUEnd = ByteString.StringToBytes("end", true);

		private MimeParser.ParseState state;

		private int currentOffset;

		private int lineOffset;

		private bool mime;

		private MimeParser.ParseLevel currentLevel;

		private MimeParser.ParseLevel[] parseStack;

		private int parseStackTop;

		private int position;

		private int lastTokenLength;

		private bool firstHeader;

		private bool parseEmbeddedMessages;

		private bool parseInlineAttachments;

		private int nextBoundaryLevel;

		private bool nextBoundaryEnd;

		private MimeComplianceStatus compliance;

		private bool expectBinaryContent;

		private int headerNameLength;

		private int headerDataOffset;

		private bool headerComplete;

		private ContentTransferEncoding inlineFormat;

		private enum ParseState
		{
			Headers,
			EndOfHeaders,
			Body,
			EndOfFile
		}

		private struct ParseLevel
		{
			public MajorContentType ContentType
			{
				get
				{
					return this.contentType;
				}
			}

			public ContentTransferEncoding TransferEncoding
			{
				get
				{
					return this.transferEncoding;
				}
				set
				{
					this.transferEncoding = value;
				}
			}

			public int PartDepth
			{
				get
				{
					return this.partDepth;
				}
				set
				{
					this.partDepth = value;
				}
			}

			public bool IsMime
			{
				get
				{
					return this.mime;
				}
				set
				{
					this.mime = value;
				}
			}

			public bool StreamMode
			{
				get
				{
					return this.streamMode;
				}
				set
				{
					this.streamMode = value;
				}
			}

			public void Reset(bool cleanMimeState)
			{
				this.streamMode = false;
				this.contentType = MajorContentType.Other;
				this.Epilogue = false;
				this.transferEncoding = ContentTransferEncoding.SevenBit;
				if (cleanMimeState)
				{
					this.mime = false;
				}
				this.boundaryValue = default(MimeString);
				this.boundaryCrc = 0U;
				this.endBoundaryCrc = 0U;
				this.partDepth = 0;
			}

			public void SetContentType(MajorContentType contentType, MimeString boundaryValue)
			{
				if (contentType == MajorContentType.Multipart)
				{
					int srcOffset;
					int num;
					byte[] data = boundaryValue.GetData(out srcOffset, out num);
					int num2 = MimeString.TwoDashes.Length + num + MimeString.TwoDashes.Length;
					byte[] array = new byte[num2];
					int num3 = MimeString.TwoDashes.Length;
					Buffer.BlockCopy(MimeString.TwoDashes, 0, array, 0, num3);
					Buffer.BlockCopy(data, srcOffset, array, num3, num);
					num3 += num;
					this.boundaryCrc = ByteString.ComputeCrc(array, 0, num3);
					Buffer.BlockCopy(MimeString.TwoDashes, 0, array, num3, MimeString.TwoDashes.Length);
					num3 += MimeString.TwoDashes.Length;
					this.endBoundaryCrc = ByteString.ComputeCrc(array, 0, num3);
					this.boundaryValue = new MimeString(array, 0, num3);
				}
				else
				{
					this.boundaryValue = default(MimeString);
					this.boundaryCrc = 0U;
					this.endBoundaryCrc = 0U;
				}
				this.contentType = contentType;
			}

			public bool IsBoundary(byte[] bytes, int offset, int length, long crc, out bool term)
			{
				if (crc == (long)((ulong)this.boundaryCrc) && this.boundaryValue.Length - 2 == length)
				{
					term = false;
					return this.boundaryValue.HasPrefixEq(bytes, offset, length);
				}
				if (crc == (long)((ulong)this.endBoundaryCrc) && this.boundaryValue.Length == length)
				{
					return term = this.boundaryValue.HasPrefixEq(bytes, offset, length);
				}
				return term = false;
			}

			private bool mime;

			private bool streamMode;

			private MajorContentType contentType;

			public bool Epilogue;

			private ContentTransferEncoding transferEncoding;

			private MimeString boundaryValue;

			private int partDepth;

			private uint boundaryCrc;

			private uint endBoundaryCrc;
		}
	}
}
