using System;
using System.IO;
using System.Reflection;
using mppg;

namespace Microsoft.Exchange.Data.Generated
{
	public sealed class Scanner : ScanBase, IColorScan
	{
		private static int GetMaxParseToken()
		{
			FieldInfo field = typeof(Tokens).GetField("maxParseToken");
			if (!(field == null))
			{
				return (int)field.GetValue(null);
			}
			return int.MaxValue;
		}

		protected override int CurrentSc
		{
			get
			{
				return this.scState;
			}
			set
			{
				this.scState = value;
				this.currentStart = Scanner.startState[value];
			}
		}

		public Scanner(string query)
		{
			this.query = query;
			this.SetSource(query, 0);
		}

		public override void yyerror(string format, params object[] args)
		{
			throw new ParsingSyntaxException(this.query, this.yylloc.sCol + 1);
		}

		internal void LoadYylval()
		{
			this.yylval.Value = this.yytext;
			this.yylloc = new LexLocation(this.tokLin, this.tokCol, this.tokELin, this.tokECol);
		}

		private sbyte Map(int chr)
		{
			if (chr < 126)
			{
				return Scanner.map0[chr];
			}
			return 26;
		}

		static Scanner()
		{
			int[] array = new int[2];
			array[0] = 1;
			Scanner.startState = array;
			Scanner.map0 = new sbyte[]
			{
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				26,
				27,
				28,
				2,
				26,
				22,
				26,
				28,
				1,
				20,
				21,
				26,
				8,
				26,
				4,
				6,
				26,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				26,
				26,
				28,
				28,
				28,
				26,
				26,
				17,
				23,
				23,
				18,
				7,
				23,
				11,
				23,
				14,
				23,
				15,
				9,
				23,
				13,
				16,
				23,
				12,
				19,
				23,
				10,
				23,
				23,
				23,
				23,
				23,
				23,
				26,
				26,
				26,
				26,
				24,
				3,
				17,
				23,
				23,
				18,
				7,
				23,
				11,
				23,
				14,
				23,
				15,
				9,
				23,
				13,
				16,
				23,
				12,
				19,
				23,
				10,
				23,
				23,
				23,
				23,
				23,
				23,
				25,
				28,
				0
			};
			Scanner.NxS = new Scanner.Table[50];
			Scanner.NxS[0] = new Scanner.Table(0, 0, 0, null);
			Scanner.NxS[1] = new Scanner.Table(20, 18, 3, new sbyte[]
			{
				4,
				5,
				44,
				3,
				-1,
				-1,
				-1,
				6,
				7,
				-1,
				41,
				42,
				-1,
				43,
				2,
				-1,
				3,
				-1
			});
			Scanner.NxS[2] = new Scanner.Table(5, 3, -1, new sbyte[]
			{
				2,
				47,
				48
			});
			Scanner.NxS[3] = new Scanner.Table(20, 18, 3, new sbyte[]
			{
				-1,
				-1,
				-1,
				3,
				3,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				3,
				-1,
				3,
				-1
			});
			Scanner.NxS[4] = new Scanner.Table(0, 0, -1, null);
			Scanner.NxS[5] = new Scanner.Table(0, 0, -1, null);
			Scanner.NxS[6] = new Scanner.Table(27, 1, -1, new sbyte[]
			{
				6
			});
			Scanner.NxS[7] = new Scanner.Table(28, 1, -1, new sbyte[]
			{
				7
			});
			Scanner.NxS[8] = new Scanner.Table(20, 18, 8, new sbyte[]
			{
				-1,
				-1,
				-1,
				8,
				8,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				8,
				-1,
				8,
				-1
			});
			Scanner.NxS[9] = new Scanner.Table(0, 0, -1, null);
			Scanner.NxS[10] = new Scanner.Table(25, 8, 45, new sbyte[]
			{
				-1,
				45,
				45,
				45,
				9,
				45,
				45,
				46
			});
			Scanner.NxS[11] = new Scanner.Table(5, 1, -1, new sbyte[]
			{
				11
			});
			Scanner.NxS[12] = new Scanner.Table(5, 3, -1, new sbyte[]
			{
				12,
				-1,
				48
			});
			Scanner.NxS[13] = new Scanner.Table(5, 20, -1, new sbyte[]
			{
				15,
				-1,
				15,
				-1,
				15,
				15,
				15,
				37,
				15,
				15,
				15,
				15,
				15,
				15,
				15,
				-1,
				-1,
				-1,
				15,
				15
			});
			Scanner.NxS[14] = new Scanner.Table(5, 20, -1, new sbyte[]
			{
				15,
				-1,
				32,
				-1,
				15,
				33,
				15,
				15,
				15,
				34,
				15,
				15,
				15,
				15,
				15,
				-1,
				-1,
				-1,
				15,
				15
			});
			Scanner.NxS[15] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[16] = new Scanner.Table(20, 20, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				30,
				-1,
				15,
				31
			});
			Scanner.NxS[17] = new Scanner.Table(5, 20, -1, new sbyte[]
			{
				15,
				-1,
				23,
				-1,
				15,
				15,
				15,
				15,
				15,
				15,
				15,
				24,
				15,
				15,
				15,
				-1,
				-1,
				-1,
				15,
				15
			});
			Scanner.NxS[18] = new Scanner.Table(19, 19, 15, new sbyte[]
			{
				22,
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[19] = new Scanner.Table(5, 20, -1, new sbyte[]
			{
				15,
				-1,
				15,
				-1,
				15,
				15,
				15,
				15,
				20,
				15,
				15,
				15,
				15,
				15,
				15,
				-1,
				-1,
				-1,
				15,
				15
			});
			Scanner.NxS[20] = new Scanner.Table(18, 20, 15, new sbyte[]
			{
				21,
				15,
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[21] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[22] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[23] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[24] = new Scanner.Table(20, 20, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1,
				15,
				25
			});
			Scanner.NxS[25] = new Scanner.Table(20, 19, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1,
				26
			});
			Scanner.NxS[26] = new Scanner.Table(5, 20, -1, new sbyte[]
			{
				15,
				-1,
				15,
				-1,
				15,
				15,
				15,
				15,
				15,
				27,
				15,
				15,
				15,
				15,
				15,
				-1,
				-1,
				-1,
				15,
				15
			});
			Scanner.NxS[27] = new Scanner.Table(5, 20, -1, new sbyte[]
			{
				15,
				-1,
				15,
				-1,
				15,
				15,
				15,
				15,
				15,
				15,
				28,
				15,
				15,
				15,
				15,
				-1,
				-1,
				-1,
				15,
				15
			});
			Scanner.NxS[28] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				29,
				-1
			});
			Scanner.NxS[29] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[30] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[31] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[32] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[33] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[34] = new Scanner.Table(5, 20, -1, new sbyte[]
			{
				15,
				-1,
				15,
				-1,
				15,
				15,
				15,
				15,
				15,
				15,
				35,
				15,
				15,
				15,
				15,
				-1,
				-1,
				-1,
				15,
				15
			});
			Scanner.NxS[35] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				36,
				-1
			});
			Scanner.NxS[36] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[37] = new Scanner.Table(20, 18, 15, new sbyte[]
			{
				-1,
				-1,
				-1,
				15,
				15,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1,
				15,
				-1
			});
			Scanner.NxS[38] = new Scanner.Table(0, 0, -1, null);
			Scanner.NxS[39] = new Scanner.Table(2, 2, 42, new sbyte[]
			{
				38,
				49
			});
			Scanner.NxS[40] = new Scanner.Table(1, 1, -1, new sbyte[]
			{
				41
			});
			Scanner.NxS[41] = new Scanner.Table(1, 1, 41, new sbyte[]
			{
				40
			});
			Scanner.NxS[42] = new Scanner.Table(2, 2, 42, new sbyte[]
			{
				38,
				49
			});
			Scanner.NxS[43] = new Scanner.Table(5, 19, -1, new sbyte[]
			{
				2,
				-1,
				13,
				-1,
				14,
				15,
				16,
				15,
				17,
				15,
				15,
				18,
				19,
				15,
				15,
				-1,
				-1,
				-1,
				15
			});
			Scanner.NxS[44] = new Scanner.Table(20, 18, 8, new sbyte[]
			{
				-1,
				-1,
				-1,
				8,
				8,
				45,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				8,
				-1,
				8,
				-1
			});
			Scanner.NxS[45] = new Scanner.Table(25, 8, 45, new sbyte[]
			{
				-1,
				45,
				45,
				45,
				9,
				45,
				45,
				46
			});
			Scanner.NxS[46] = new Scanner.Table(0, 4, 45, new sbyte[]
			{
				10,
				45,
				45,
				46
			});
			Scanner.NxS[47] = new Scanner.Table(5, 1, -1, new sbyte[]
			{
				12
			});
			Scanner.NxS[48] = new Scanner.Table(4, 5, -1, new sbyte[]
			{
				47,
				11,
				-1,
				-1,
				47
			});
			Scanner.NxS[49] = new Scanner.Table(2, 2, 42, new sbyte[]
			{
				39,
				49
			});
		}

		private int NextState(int qStat)
		{
			if (this.chr == -1)
			{
				if (qStat > 40 || qStat == this.currentStart)
				{
					return 0;
				}
				return this.currentStart;
			}
			else
			{
				int num = (int)this.Map(this.chr) - Scanner.NxS[qStat].min;
				if (num < 0)
				{
					num += 29;
				}
				int num2;
				if (num >= Scanner.NxS[qStat].rng)
				{
					num2 = Scanner.NxS[qStat].dflt;
				}
				else
				{
					num2 = (int)Scanner.NxS[qStat].nxt[num];
				}
				if (num2 != -1)
				{
					return num2;
				}
				return this.currentStart;
			}
		}

		private int NextState()
		{
			if (this.chr == -1)
			{
				if (this.state > 40 || this.state == this.currentStart)
				{
					return 0;
				}
				return this.currentStart;
			}
			else
			{
				int num = (int)this.Map(this.chr) - Scanner.NxS[this.state].min;
				if (num < 0)
				{
					num += 29;
				}
				int num2;
				if (num >= Scanner.NxS[this.state].rng)
				{
					num2 = Scanner.NxS[this.state].dflt;
				}
				else
				{
					num2 = (int)Scanner.NxS[this.state].nxt[num];
				}
				if (num2 != -1)
				{
					return num2;
				}
				return this.currentStart;
			}
		}

		public Scanner(Stream file)
		{
			this.buffer = Scanner.TextBuff.NewTextBuff(file);
			this.cNum = -1;
			this.chr = 10;
			this.GetChr();
		}

		public Scanner()
		{
		}

		private void GetChr()
		{
			if (this.chr == 10)
			{
				this.lineStartNum = this.cNum + 1;
				this.lNum++;
			}
			this.chr = this.buffer.Read();
			this.cNum++;
		}

		private void MarkToken()
		{
			this.tokPos = this.buffer.ReadPos;
			this.tokNum = this.cNum;
			this.tokLin = this.lNum;
			this.tokCol = this.cNum - this.lineStartNum;
		}

		private void MarkEnd()
		{
			this.tokTxt = null;
			this.tokLen = this.cNum - this.tokNum;
			this.tokEPos = this.buffer.ReadPos;
			this.tokELin = this.lNum;
			this.tokECol = this.cNum - this.lineStartNum;
		}

		public void SetSource(string source, int offset)
		{
			this.buffer = new Scanner.StringBuff(source);
			this.buffer.Pos = offset;
			this.cNum = offset - 1;
			this.chr = 10;
			this.GetChr();
		}

		public int GetNext(ref int state, out int start, out int end)
		{
			this.EolState = state;
			Tokens result = (Tokens)this.Scan();
			state = this.EolState;
			start = this.tokPos;
			end = this.tokEPos - 1;
			return (int)result;
		}

		public override int yylex()
		{
			int num;
			do
			{
				num = this.Scan();
			}
			while (num >= Scanner.parserMax);
			return num;
		}

		private int yyleng
		{
			get
			{
				return this.tokLen;
			}
		}

		private int yypos
		{
			get
			{
				return this.tokPos;
			}
		}

		private int yyline
		{
			get
			{
				return this.tokLin;
			}
		}

		private int yycol
		{
			get
			{
				return this.tokCol;
			}
		}

		public string yytext
		{
			get
			{
				if (this.tokTxt == null)
				{
					this.tokTxt = this.buffer.GetString(this.tokPos, this.tokEPos);
				}
				return this.tokTxt;
			}
		}

		private void yyless(int n)
		{
			this.buffer.Pos = this.tokPos;
			this.cNum = this.tokNum;
			for (int i = 0; i <= n; i++)
			{
				this.GetChr();
			}
			this.MarkEnd();
		}

		public IErrorHandler Handler
		{
			get
			{
				return this.handler;
			}
			set
			{
				this.handler = value;
			}
		}

		internal int YY_START
		{
			get
			{
				return this.CurrentSc;
			}
			set
			{
				this.CurrentSc = value;
			}
		}

		private int Scan()
		{
			int result2;
			try
			{
				for (;;)
				{
					bool flag = false;
					this.state = this.currentStart;
					while (this.NextState() == this.state)
					{
						this.GetChr();
					}
					this.MarkToken();
					int num;
					while ((num = this.NextState()) != this.currentStart)
					{
						if (flag && num > 40)
						{
							Scanner.Context ctx = new Scanner.Context();
							Scanner.Result result = this.Recurse2(ctx, num);
							if (result == Scanner.Result.noMatch)
							{
								this.RestoreStateAndPos(ctx);
								break;
							}
							break;
						}
						else
						{
							this.state = num;
							this.GetChr();
							if (this.state <= 40)
							{
								flag = true;
							}
						}
					}
					if (this.state > 40)
					{
						this.state = this.currentStart;
					}
					else
					{
						this.MarkEnd();
						switch (this.state)
						{
						case 0:
							goto IL_154;
						case 1:
						case 7:
							goto IL_15C;
						case 2:
						case 11:
						case 12:
							goto IL_162;
						case 3:
							goto IL_167;
						case 4:
							goto IL_16C;
						case 5:
							goto IL_172;
						case 6:
							goto IL_178;
						case 8:
							goto IL_17E;
						case 9:
						case 10:
							goto IL_183;
						case 13:
						case 14:
						case 15:
						case 16:
						case 17:
						case 18:
						case 19:
						case 20:
						case 24:
						case 26:
						case 27:
						case 28:
						case 34:
						case 35:
							goto IL_188;
						case 21:
							goto IL_18E;
						case 22:
							goto IL_194;
						case 23:
							goto IL_19A;
						case 25:
							goto IL_1A0;
						case 29:
							goto IL_1A6;
						case 30:
							goto IL_1AC;
						case 31:
							goto IL_1B2;
						case 32:
							goto IL_1B8;
						case 33:
							goto IL_1BE;
						case 36:
							goto IL_1C4;
						case 37:
							goto IL_1CA;
						case 38:
						case 39:
							goto IL_1D0;
						case 40:
							goto IL_1D5;
						}
					}
				}
				IL_154:
				return 2;
				IL_15C:
				return 22;
				IL_162:
				return 6;
				IL_167:
				return 3;
				IL_16C:
				return 9;
				IL_172:
				return 10;
				IL_178:
				return 24;
				IL_17E:
				return 7;
				IL_183:
				return 8;
				IL_188:
				return 22;
				IL_18E:
				return 12;
				IL_194:
				return 11;
				IL_19A:
				return 14;
				IL_1A0:
				return 19;
				IL_1A6:
				return 21;
				IL_1AC:
				return 17;
				IL_1B2:
				return 15;
				IL_1B8:
				return 18;
				IL_1BE:
				return 16;
				IL_1C4:
				return 20;
				IL_1CA:
				return 13;
				IL_1D0:
				return 5;
				IL_1D5:
				result2 = 4;
			}
			finally
			{
				this.LoadYylval();
			}
			return result2;
		}

		private Scanner.Result Recurse2(Scanner.Context ctx, int next)
		{
			this.SaveStateAndPos(ctx);
			this.state = next;
			if (this.state == 0)
			{
				return Scanner.Result.accept;
			}
			this.GetChr();
			bool flag = false;
			while ((next = this.NextState()) != this.currentStart)
			{
				if (flag && next > 40)
				{
					this.SaveStateAndPos(ctx);
				}
				this.state = next;
				if (this.state == 0)
				{
					return Scanner.Result.accept;
				}
				this.GetChr();
				flag = (this.state <= 40);
			}
			if (flag)
			{
				return Scanner.Result.accept;
			}
			return Scanner.Result.noMatch;
		}

		private void SaveStateAndPos(Scanner.Context ctx)
		{
			ctx.bPos = this.buffer.Pos;
			ctx.cNum = this.cNum;
			ctx.state = this.state;
			ctx.cChr = this.chr;
		}

		private void RestoreStateAndPos(Scanner.Context ctx)
		{
			this.buffer.Pos = ctx.bPos;
			this.cNum = ctx.cNum;
			this.state = ctx.state;
			this.chr = ctx.cChr;
		}

		private void RestorePos(Scanner.Context ctx)
		{
			this.buffer.Pos = ctx.bPos;
			this.cNum = ctx.cNum;
		}

		internal void BEGIN(int next)
		{
			this.CurrentSc = next;
		}

		internal void ECHO()
		{
			Console.Out.Write(this.yytext);
		}

		private const int maxAccept = 40;

		private const int initial = 1;

		private const int eofNum = 0;

		private const int goStart = -1;

		private const int INITIAL = 0;

		public ScanBuff buffer;

		private IErrorHandler handler;

		private int scState;

		private static int parserMax = Scanner.GetMaxParseToken();

		private readonly string query;

		private int state;

		private int currentStart = 1;

		private int chr;

		private int cNum;

		private int lNum;

		private int lineStartNum;

		private int tokPos;

		private int tokNum;

		private int tokLen;

		private int tokCol;

		private int tokLin;

		private int tokEPos;

		private int tokECol;

		private int tokELin;

		private string tokTxt;

		private static int[] startState;

		private static sbyte[] map0;

		private static Scanner.Table[] NxS;

		private enum Result
		{
			accept,
			noMatch,
			contextFound
		}

		private struct Table
		{
			public Table(int m, int x, int d, sbyte[] n)
			{
				this.min = m;
				this.rng = x;
				this.dflt = d;
				this.nxt = n;
			}

			public int min;

			public int rng;

			public int dflt;

			public sbyte[] nxt;
		}

		internal class Context
		{
			public int bPos;

			public int cNum;

			public int state;

			public int cChr;
		}

		public sealed class StringBuff : ScanBuff
		{
			public StringBuff(string str)
			{
				this.str = str;
				this.sLen = str.Length;
			}

			public override int Read()
			{
				if (this.bPos < this.sLen)
				{
					return (int)this.str[this.bPos++];
				}
				if (this.bPos == this.sLen)
				{
					this.bPos++;
					return 10;
				}
				return -1;
			}

			public override int ReadPos
			{
				get
				{
					return this.bPos - 1;
				}
			}

			public override int Peek()
			{
				if (this.bPos < this.sLen)
				{
					return (int)this.str[this.bPos];
				}
				return 10;
			}

			public override string GetString(int beg, int end)
			{
				if (end > this.sLen)
				{
					end = this.sLen;
				}
				if (end <= beg)
				{
					return "";
				}
				return this.str.Substring(beg, end - beg);
			}

			public override int Pos
			{
				get
				{
					return this.bPos;
				}
				set
				{
					this.bPos = value;
				}
			}

			private readonly string str;

			private int bPos;

			private readonly int sLen;
		}

		public sealed class StreamBuff : ScanBuff
		{
			public StreamBuff(Stream str)
			{
				this.bStrm = new BufferedStream(str);
			}

			public override int Read()
			{
				return this.bStrm.ReadByte();
			}

			public override int ReadPos
			{
				get
				{
					return (int)this.bStrm.Position - 1;
				}
			}

			public override int Peek()
			{
				int result = this.bStrm.ReadByte();
				this.bStrm.Seek(-1L, SeekOrigin.Current);
				return result;
			}

			public override string GetString(int beg, int end)
			{
				if (end - beg <= 0)
				{
					return "";
				}
				long position = this.bStrm.Position;
				char[] array = new char[end - beg];
				this.bStrm.Position = (long)beg;
				for (int i = 0; i < end - beg; i++)
				{
					array[i] = (char)this.bStrm.ReadByte();
				}
				this.bStrm.Position = position;
				return new string(array);
			}

			public override int Pos
			{
				get
				{
					return (int)this.bStrm.Position;
				}
				set
				{
					this.bStrm.Position = (long)value;
				}
			}

			private const int delta = 1;

			private BufferedStream bStrm;
		}

		public class TextBuff : ScanBuff
		{
			private Exception BadUTF8()
			{
				return new Exception(string.Format("BadUTF8 Character", new object[0]));
			}

			public static Scanner.TextBuff NewTextBuff(Stream strm)
			{
				int num = strm.ReadByte();
				int num2 = strm.ReadByte();
				if (num == 254 && num2 == 255)
				{
					return new Scanner.BigEndTextBuff(strm);
				}
				if (num == 255 && num2 == 254)
				{
					return new Scanner.LittleEndTextBuff(strm);
				}
				int num3 = strm.ReadByte();
				if (num == 239 && num2 == 187 && num3 == 191)
				{
					return new Scanner.TextBuff(strm);
				}
				strm.Seek(0L, SeekOrigin.Begin);
				return new Scanner.TextBuff(strm);
			}

			protected TextBuff(Stream str)
			{
				this.bStrm = new BufferedStream(str);
			}

			public override int Read()
			{
				int num = this.bStrm.ReadByte();
				if (num < 127)
				{
					this.delta = ((num == -1) ? 0 : 1);
					return num;
				}
				if ((num & 224) == 192)
				{
					this.delta = 2;
					int num2 = this.bStrm.ReadByte();
					if ((num2 & 192) == 128)
					{
						return ((num & 31) << 6) + (num2 & 63);
					}
					throw this.BadUTF8();
				}
				else
				{
					if ((num & 240) != 224)
					{
						throw this.BadUTF8();
					}
					this.delta = 3;
					int num2 = this.bStrm.ReadByte();
					int num3 = this.bStrm.ReadByte();
					if ((num2 & num3 & 192) == 128)
					{
						return ((num & 15) << 12) + ((num2 & 63) << 6) + (num3 & 63);
					}
					throw this.BadUTF8();
				}
			}

			public sealed override int ReadPos
			{
				get
				{
					return (int)this.bStrm.Position - this.delta;
				}
			}

			public sealed override int Peek()
			{
				int result = this.Read();
				this.bStrm.Seek((long)(-(long)this.delta), SeekOrigin.Current);
				return result;
			}

			public sealed override string GetString(int beg, int end)
			{
				if (end - beg <= 0)
				{
					return "";
				}
				long position = this.bStrm.Position;
				char[] array = new char[end - beg];
				this.bStrm.Position = (long)beg;
				int num = 0;
				while (this.bStrm.Position < (long)end)
				{
					array[num] = (char)this.Read();
					num++;
				}
				this.bStrm.Position = position;
				return new string(array, 0, num);
			}

			public sealed override int Pos
			{
				get
				{
					return (int)this.bStrm.Position;
				}
				set
				{
					this.bStrm.Position = (long)value;
				}
			}

			protected BufferedStream bStrm;

			protected int delta = 1;
		}

		public sealed class BigEndTextBuff : Scanner.TextBuff
		{
			internal BigEndTextBuff(Stream str) : base(str)
			{
			}

			public override int Read()
			{
				int num = this.bStrm.ReadByte();
				int num2 = this.bStrm.ReadByte();
				return (num << 8) + num2;
			}
		}

		public sealed class LittleEndTextBuff : Scanner.TextBuff
		{
			internal LittleEndTextBuff(Stream str) : base(str)
			{
			}

			public override int Read()
			{
				int num = this.bStrm.ReadByte();
				int num2 = this.bStrm.ReadByte();
				return (num2 << 8) + num;
			}
		}
	}
}
