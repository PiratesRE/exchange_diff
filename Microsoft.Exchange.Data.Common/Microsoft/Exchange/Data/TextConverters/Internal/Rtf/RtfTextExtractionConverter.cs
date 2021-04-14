using System;
using System.IO;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal class RtfTextExtractionConverter : IProducerConsumer, IDisposable
	{
		public RtfTextExtractionConverter(RtfParser parser, ConverterOutput output, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
		{
			this.output = output;
			this.parser = parser;
		}

		void IDisposable.Dispose()
		{
			if (this.parser != null && this.parser is IDisposable)
			{
				((IDisposable)this.parser).Dispose();
			}
			if (this.output != null && this.output != null)
			{
				((IDisposable)this.output).Dispose();
			}
			this.parser = null;
			this.output = null;
			GC.SuppressFinalize(this);
		}

		public void Run()
		{
			if (!this.endOfFile)
			{
				RtfTokenId rtfTokenId = this.parser.Parse();
				if (rtfTokenId != RtfTokenId.None)
				{
					this.Process(rtfTokenId);
				}
			}
		}

		public bool Flush()
		{
			if (!this.endOfFile)
			{
				this.Run();
			}
			return this.endOfFile;
		}

		private void Process(RtfTokenId tokenId)
		{
			switch (tokenId)
			{
			case RtfTokenId.EndOfFile:
				this.ProcessEOF();
				break;
			case RtfTokenId.Begin:
				this.ProcessBeginGroup();
				return;
			case RtfTokenId.End:
				this.ProcessEndGroup();
				return;
			case RtfTokenId.Binary:
			case (RtfTokenId)6:
				break;
			case RtfTokenId.Keywords:
				this.ProcessKeywords(this.parser.Token);
				return;
			case RtfTokenId.Text:
				this.ProcessText(this.parser.Token);
				return;
			default:
				return;
			}
		}

		private void ProcessBeginGroup()
		{
			this.level++;
			this.firstKeyword = true;
			this.ignorableDestination = false;
		}

		private void ProcessEndGroup()
		{
			if (this.skipLevel != 0)
			{
				if (this.level != this.skipLevel)
				{
					this.level--;
					return;
				}
				this.skipLevel = 0;
			}
			this.firstKeyword = false;
			if (this.level > 1)
			{
				this.level--;
				if (this.invisibleLevel != 2147483647 && this.level < this.invisibleLevel)
				{
					this.invisibleLevel = int.MaxValue;
				}
			}
		}

		private void ProcessKeywords(RtfToken token)
		{
			if (this.skipLevel != 0 && this.level >= this.skipLevel)
			{
				return;
			}
			foreach (RtfKeyword rtfKeyword in token.Keywords)
			{
				if (this.firstKeyword)
				{
					if (rtfKeyword.Id == 1)
					{
						this.ignorableDestination = true;
						continue;
					}
					this.firstKeyword = false;
					short id = rtfKeyword.Id;
					if (id <= 201)
					{
						if (id <= 24)
						{
							if (id != 8 && id != 15 && id != 24)
							{
								goto IL_118;
							}
						}
						else if (id != 50 && id != 175 && id != 201)
						{
							goto IL_118;
						}
					}
					else if (id <= 252)
					{
						if (id != 210 && id != 246 && id != 252)
						{
							goto IL_118;
						}
					}
					else
					{
						switch (id)
						{
						case 255:
						case 257:
							break;
						case 256:
							goto IL_118;
						default:
							if (id != 268)
							{
								switch (id)
								{
								case 315:
								case 316:
								case 319:
									break;
								case 317:
								case 318:
									goto IL_118;
								default:
									goto IL_118;
								}
							}
							break;
						}
					}
					this.skipLevel = this.level;
					break;
					IL_118:
					if (this.ignorableDestination)
					{
						this.skipLevel = this.level;
						break;
					}
				}
				short id2 = rtfKeyword.Id;
				if (id2 <= 68)
				{
					if (id2 == 40 || id2 == 48 || id2 == 68)
					{
						this.output.Write("\r\n");
					}
				}
				else if (id2 != 119 && id2 != 126)
				{
					if (id2 == 136)
					{
						this.invisibleLevel = ((rtfKeyword.Value != 0) ? this.level : int.MaxValue);
					}
				}
				else
				{
					this.output.Write("\t");
				}
			}
		}

		private void ProcessText(RtfToken token)
		{
			if (this.skipLevel != 0 && this.level >= this.skipLevel)
			{
				return;
			}
			this.firstKeyword = false;
			if (this.level < this.invisibleLevel)
			{
				token.Text.WriteTo(this.output);
			}
		}

		private void ProcessEOF()
		{
			this.output.Write("\r\n");
			this.output.Flush();
			this.endOfFile = true;
		}

		private RtfParser parser;

		private bool endOfFile;

		private ConverterOutput output;

		private bool firstKeyword;

		private bool ignorableDestination;

		private int level;

		private int skipLevel;

		private int invisibleLevel = int.MaxValue;
	}
}
