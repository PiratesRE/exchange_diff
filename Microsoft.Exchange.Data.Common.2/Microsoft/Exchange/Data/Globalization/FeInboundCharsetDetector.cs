using System;

namespace Microsoft.Exchange.Data.Globalization
{
	internal struct FeInboundCharsetDetector
	{
		public FeInboundCharsetDetector(int defaultCodePage, bool strongDefault, bool enableIsoDetection, bool enableUtf8Detection, bool enableDbcsDetection)
		{
			this.defaultCodePage = (ushort)defaultCodePage;
			this.strongDefault = strongDefault;
			this.stateIso = FEData.ST.ERR;
			this.stateUtf8 = FEData.ST.ERR;
			this.stateGbkWan = FEData.ST.ERR;
			this.stateEucKrCn = FEData.ST.ERR;
			this.stateEucJp = FEData.ST.ERR;
			this.stateSJis = FEData.ST.ERR;
			this.stateBig5 = FEData.ST.ERR;
			this.stateJisEsc = FEData.JS.S0;
			this.countJapaneseEsc = 0;
			this.countKoreanDesignator = 0;
			this.countSo = 0;
			this.count8bit = 0;
			if (enableDbcsDetection)
			{
				if (defaultCodePage <= 20936)
				{
					if (defaultCodePage <= 936)
					{
						if (defaultCodePage == 0)
						{
							this.stateSJis = FEData.ST.ST0;
							this.stateEucJp = FEData.ST.ST0;
							this.stateGbkWan = FEData.ST.ST0;
							this.stateEucKrCn = FEData.ST.ST0;
							this.stateBig5 = FEData.ST.ST0;
							goto IL_1C7;
						}
						if (defaultCodePage != 932)
						{
							if (defaultCodePage != 936)
							{
								goto IL_1C7;
							}
							goto IL_1A9;
						}
					}
					else if (defaultCodePage <= 1361)
					{
						switch (defaultCodePage)
						{
						case 949:
							goto IL_199;
						case 950:
							this.stateBig5 = FEData.ST.ST0;
							this.stateGbkWan = FEData.ST.ST0;
							goto IL_1C7;
						default:
							if (defaultCodePage != 1361)
							{
								goto IL_1C7;
							}
							goto IL_199;
						}
					}
					else if (defaultCodePage != 20932)
					{
						if (defaultCodePage != 20936)
						{
							goto IL_1C7;
						}
						goto IL_1A9;
					}
				}
				else if (defaultCodePage <= 51932)
				{
					if (defaultCodePage == 20949)
					{
						goto IL_199;
					}
					switch (defaultCodePage)
					{
					case 50220:
					case 50221:
					case 50222:
						break;
					case 50223:
					case 50224:
					case 50226:
						goto IL_1C7;
					case 50225:
						goto IL_199;
					case 50227:
						goto IL_1A9;
					default:
						if (defaultCodePage != 51932)
						{
							goto IL_1C7;
						}
						break;
					}
				}
				else if (defaultCodePage <= 51949)
				{
					if (defaultCodePage == 51936)
					{
						goto IL_1A9;
					}
					if (defaultCodePage != 51949)
					{
						goto IL_1C7;
					}
					goto IL_199;
				}
				else
				{
					if (defaultCodePage != 52936 && defaultCodePage != 54936)
					{
						goto IL_1C7;
					}
					goto IL_1A9;
				}
				this.stateSJis = FEData.ST.ST0;
				this.stateEucJp = FEData.ST.ST0;
				goto IL_1C7;
				IL_199:
				this.stateGbkWan = FEData.ST.ST0;
				this.stateEucKrCn = FEData.ST.ST0;
				goto IL_1C7;
				IL_1A9:
				this.stateGbkWan = FEData.ST.ST0;
				this.stateEucKrCn = FEData.ST.ST0;
			}
			IL_1C7:
			if (enableIsoDetection)
			{
				this.stateIso = FEData.ST.ST0;
			}
			if (enableUtf8Detection)
			{
				this.stateUtf8 = FEData.ST.ST0;
			}
		}

		public static bool IsSupportedFarEastCharset(Charset charset)
		{
			int codePage = charset.CodePage;
			if (codePage <= 20936)
			{
				if (codePage <= 950)
				{
					if (codePage != 932 && codePage != 936)
					{
						switch (codePage)
						{
						case 949:
						case 950:
							break;
						default:
							return false;
						}
					}
				}
				else if (codePage != 1361 && codePage != 20932 && codePage != 20936)
				{
					return false;
				}
			}
			else if (codePage <= 51932)
			{
				if (codePage != 20949)
				{
					switch (codePage)
					{
					case 50220:
					case 50221:
					case 50222:
					case 50225:
					case 50227:
						break;
					case 50223:
					case 50224:
					case 50226:
						return false;
					default:
						if (codePage != 51932)
						{
							return false;
						}
						break;
					}
				}
			}
			else if (codePage <= 51949)
			{
				if (codePage != 51936 && codePage != 51949)
				{
					return false;
				}
			}
			else if (codePage != 52936 && codePage != 54936)
			{
				return false;
			}
			return true;
		}

		public void Reset(int codePage, bool strongDefault, bool enableIsoDetection, bool enableUtf8Detection, bool enableDbcsDetection)
		{
			this = new FeInboundCharsetDetector(codePage, strongDefault, enableIsoDetection, enableUtf8Detection, enableDbcsDetection);
		}

		public void Reset()
		{
			this = new FeInboundCharsetDetector(0, false, true, true, true);
		}

		public void RunJisStateMachine(byte bt)
		{
			FEData.JC jc = FEData.JisCharClass[(int)bt];
			this.stateJisEsc = FEData.JisEscNextState[(int)this.stateJisEsc, (int)jc];
			if ((this.stateJisEsc & FEData.JS.CNTA) != FEData.JS.S0)
			{
				if (this.stateJisEsc == FEData.JS.CNTA)
				{
					if (this.countSo != 255)
					{
						this.countSo += 1;
					}
				}
				else if (this.stateJisEsc == FEData.JS.CNTJ)
				{
					if (this.countJapaneseEsc != 255)
					{
						this.countJapaneseEsc += 1;
					}
				}
				else if (this.stateJisEsc == FEData.JS.CNTK && this.countKoreanDesignator != 255)
				{
					this.countKoreanDesignator += 1;
				}
				this.stateJisEsc = FEData.JS.S0;
			}
		}

		public void RunDbcsStateMachines(FEData.CC cc)
		{
			if (this.stateSJis != FEData.ST.ERR)
			{
				this.stateSJis = FEData.SJisNextState[(int)this.stateSJis, (int)cc];
			}
			if (this.stateEucJp != FEData.ST.ERR)
			{
				this.stateEucJp = FEData.EucJpNextState[(int)this.stateEucJp, (int)cc];
			}
			if (this.stateUtf8 != FEData.ST.ERR)
			{
				this.stateUtf8 = FEData.Utf8NextState[(int)this.stateUtf8, (int)cc];
			}
			if (this.stateGbkWan != FEData.ST.ERR)
			{
				this.stateGbkWan = FEData.GbkWanNextState[(int)this.stateGbkWan, (int)cc];
			}
			if (this.stateEucKrCn != FEData.ST.ERR)
			{
				this.stateEucKrCn = FEData.EucKrCnNextState[(int)this.stateEucKrCn, (int)cc];
			}
			if (this.stateBig5 != FEData.ST.ERR)
			{
				this.stateBig5 = FEData.Big5NextState[(int)this.stateBig5, (int)cc];
			}
		}

		public void AddBytes(byte[] bytes, int offset, int length, bool eof)
		{
			if (this.stateSJis != FEData.ST.ERR || this.stateEucJp != FEData.ST.ERR || this.stateIso != FEData.ST.ERR || this.stateGbkWan != FEData.ST.ERR || this.stateEucKrCn != FEData.ST.ERR || this.stateBig5 != FEData.ST.ERR || this.stateUtf8 != FEData.ST.ERR)
			{
				int num = offset + length;
				while (offset < num)
				{
					byte b = bytes[offset++];
					if (b > 127 && this.count8bit != 255)
					{
						this.count8bit += 1;
					}
					if (this.stateIso != FEData.ST.ERR && b <= 127)
					{
						this.RunJisStateMachine(b);
					}
					FEData.CC cc = FEData.CharClass[(int)b];
					this.RunDbcsStateMachines(cc);
				}
				if (eof)
				{
					this.RunDbcsStateMachines(FEData.CC.eof);
				}
			}
		}

		public bool SJisPossible
		{
			get
			{
				return this.stateSJis != FEData.ST.ERR;
			}
		}

		public bool EucJpPossible
		{
			get
			{
				return this.stateEucJp != FEData.ST.ERR;
			}
		}

		public bool Iso2022JpPossible
		{
			get
			{
				return this.stateIso != FEData.ST.ERR;
			}
		}

		public bool Iso2022KrPossible
		{
			get
			{
				return this.stateIso != FEData.ST.ERR;
			}
		}

		public bool Utf8Possible
		{
			get
			{
				return this.stateUtf8 != FEData.ST.ERR;
			}
		}

		public bool GbkPossible
		{
			get
			{
				return this.stateGbkWan != FEData.ST.ERR;
			}
		}

		public bool WansungPossible
		{
			get
			{
				return this.stateGbkWan != FEData.ST.ERR;
			}
		}

		public bool EucKrPossible
		{
			get
			{
				return this.stateEucKrCn != FEData.ST.ERR;
			}
		}

		public bool EucCnPossible
		{
			get
			{
				return this.stateEucKrCn != FEData.ST.ERR;
			}
		}

		public bool Big5Possible
		{
			get
			{
				return this.stateBig5 != FEData.ST.ERR;
			}
		}

		public bool PureAscii
		{
			get
			{
				return this.count8bit + this.countKoreanDesignator + this.countJapaneseEsc + this.countSo == 0;
			}
		}

		public bool Iso2022JpVeryLikely
		{
			get
			{
				return this.Iso2022JpPossible && this.countJapaneseEsc > 0 && this.countKoreanDesignator == 0;
			}
		}

		public bool Iso2022JpLikely
		{
			get
			{
				return this.Iso2022JpPossible && (this.countJapaneseEsc > this.countKoreanDesignator || (this.countKoreanDesignator == 0 && this.countSo != 0));
			}
		}

		public bool Iso2022KrVeryLikely
		{
			get
			{
				return this.Iso2022KrPossible && this.countKoreanDesignator > 0 && this.countJapaneseEsc == 0;
			}
		}

		public bool Iso2022KrLikely
		{
			get
			{
				return this.Iso2022KrPossible && (this.countKoreanDesignator > this.countJapaneseEsc || (this.countJapaneseEsc == 0 && this.countSo != 0));
			}
		}

		public bool SJisLikely
		{
			get
			{
				return this.SJisPossible && this.count8bit >= 6;
			}
		}

		public bool EucJpLikely
		{
			get
			{
				return this.EucJpPossible && this.count8bit >= 6;
			}
		}

		public bool Utf8Likely
		{
			get
			{
				return this.Utf8Possible && this.count8bit >= 6;
			}
		}

		public bool Utf8VeryLikely
		{
			get
			{
				return this.Utf8Possible && this.count8bit >= 18;
			}
		}

		public bool GbkLikely
		{
			get
			{
				return this.GbkPossible && this.count8bit >= 6;
			}
		}

		public bool WansungLikely
		{
			get
			{
				return this.WansungPossible && this.count8bit >= 6;
			}
		}

		public bool EucKrLikely
		{
			get
			{
				return this.EucKrPossible && this.count8bit >= 6;
			}
		}

		public bool EucCnLikely
		{
			get
			{
				return this.EucCnPossible && this.count8bit >= 6;
			}
		}

		public bool Big5Likely
		{
			get
			{
				return this.Big5Possible && this.count8bit >= 6;
			}
		}

		public int GetCodePageChoice()
		{
			return this.GetCodePageChoice((int)this.defaultCodePage, this.strongDefault);
		}

		public int GetCodePageChoice(int defaultCodePage, bool strongDefault)
		{
			if (this.PureAscii)
			{
				return defaultCodePage;
			}
			if (this.Iso2022JpVeryLikely)
			{
				return 50220;
			}
			if (this.Iso2022KrVeryLikely)
			{
				return 50225;
			}
			if (this.Utf8VeryLikely)
			{
				return 65001;
			}
			if (defaultCodePage == 50220 || defaultCodePage == 50221 || defaultCodePage == 50222)
			{
				defaultCodePage = 932;
			}
			else if (defaultCodePage == 50225)
			{
				defaultCodePage = 949;
			}
			else if (defaultCodePage == 20932)
			{
				defaultCodePage = 51932;
			}
			else if (defaultCodePage == 20949)
			{
				defaultCodePage = 51949;
			}
			if (defaultCodePage == 932 || defaultCodePage == 51932)
			{
				if (this.Iso2022JpLikely)
				{
					return 50222;
				}
				if (this.SJisPossible && defaultCodePage == 932 && strongDefault)
				{
					return 932;
				}
				if (this.EucJpPossible && defaultCodePage == 51932 && strongDefault)
				{
					return 51932;
				}
				if (!this.Utf8Likely)
				{
					if (this.EucJpPossible)
					{
						return 51932;
					}
					if (this.SJisPossible)
					{
						return 932;
					}
				}
			}
			else if (defaultCodePage == 949 || defaultCodePage == 51949)
			{
				if (this.Iso2022KrLikely)
				{
					return 50225;
				}
				if (this.WansungPossible && defaultCodePage == 949 && strongDefault)
				{
					return 949;
				}
				if (this.EucKrPossible && defaultCodePage == 51949 && strongDefault)
				{
					return 51949;
				}
				if (!this.Utf8Likely)
				{
					if (this.WansungPossible)
					{
						return 949;
					}
					if (this.EucKrPossible)
					{
						return 51949;
					}
				}
			}
			else if (defaultCodePage == 936 || defaultCodePage == 51936)
			{
				if (this.GbkPossible && defaultCodePage == 936 && strongDefault)
				{
					return 936;
				}
				if (this.EucCnPossible && defaultCodePage == 51936 && strongDefault)
				{
					return 51936;
				}
				if (!this.Utf8Likely)
				{
					if (this.GbkPossible)
					{
						return 936;
					}
					if (this.EucCnPossible)
					{
						return 51936;
					}
				}
			}
			else if (defaultCodePage == 950)
			{
				if (this.Big5Possible && strongDefault)
				{
					return 950;
				}
				if (!this.Utf8Likely)
				{
					if (this.Big5Possible)
					{
						return 950;
					}
					if (this.GbkLikely)
					{
						return 932;
					}
				}
			}
			if (this.Utf8Likely && !strongDefault)
			{
				return 65001;
			}
			return defaultCodePage;
		}

		private ushort defaultCodePage;

		private bool strongDefault;

		private FEData.ST stateIso;

		private FEData.ST stateUtf8;

		private FEData.ST stateSJis;

		private FEData.ST stateEucJp;

		private FEData.ST stateGbkWan;

		private FEData.ST stateEucKrCn;

		private FEData.ST stateBig5;

		private FEData.JS stateJisEsc;

		private byte countJapaneseEsc;

		private byte countKoreanDesignator;

		private byte countSo;

		private byte count8bit;
	}
}
