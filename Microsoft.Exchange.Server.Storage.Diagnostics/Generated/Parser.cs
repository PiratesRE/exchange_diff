using System;
using System.Collections.Generic;
using mppg;

namespace Microsoft.Exchange.Server.Storage.Diagnostics.Generated
{
	public class Parser : ShiftReduceParser<Token, LexLocation>
	{
		protected override void Initialize()
		{
			this.errToken = 1;
			this.eofToken = 2;
			this.states = new State[133];
			base.AddState(0, new State(new int[]
			{
				3,
				53,
				4,
				103,
				5,
				124,
				6,
				131
			}, new int[]
			{
				-1,
				1,
				-3,
				3,
				-6,
				100,
				-7,
				123,
				-8,
				128
			}));
			base.AddState(1, new State(new int[]
			{
				2,
				2
			}));
			base.AddState(2, new State(-1));
			base.AddState(3, new State(new int[]
			{
				11,
				4,
				12,
				51,
				2,
				-2
			}));
			base.AddState(4, new State(new int[]
			{
				17,
				13,
				34,
				46,
				37,
				48
			}, new int[]
			{
				-4,
				5,
				-25,
				19,
				-26,
				20
			}));
			base.AddState(5, new State(new int[]
			{
				12,
				6,
				27,
				15,
				26,
				17,
				2,
				-3
			}));
			base.AddState(6, new State(new int[]
			{
				17,
				13
			}, new int[]
			{
				-5,
				7,
				-29,
				14,
				-26,
				10
			}));
			base.AddState(7, new State(new int[]
			{
				25,
				8,
				2,
				-5
			}));
			base.AddState(8, new State(new int[]
			{
				17,
				13
			}, new int[]
			{
				-29,
				9,
				-26,
				10
			}));
			base.AddState(9, new State(-75));
			base.AddState(10, new State(new int[]
			{
				13,
				11,
				14,
				12,
				25,
				-76,
				2,
				-76
			}));
			base.AddState(11, new State(-77));
			base.AddState(12, new State(-78));
			base.AddState(13, new State(-64));
			base.AddState(14, new State(-74));
			base.AddState(15, new State(new int[]
			{
				17,
				13,
				34,
				46,
				37,
				48
			}, new int[]
			{
				-4,
				16,
				-25,
				19,
				-26,
				20
			}));
			base.AddState(16, new State(-51));
			base.AddState(17, new State(new int[]
			{
				17,
				13,
				34,
				46,
				37,
				48
			}, new int[]
			{
				-4,
				18,
				-25,
				19,
				-26,
				20
			}));
			base.AddState(18, new State(new int[]
			{
				27,
				15,
				26,
				-52,
				12,
				-52,
				2,
				-52,
				38,
				-52
			}));
			base.AddState(19, new State(-50));
			base.AddState(20, new State(new int[]
			{
				28,
				21,
				29,
				30,
				30,
				32,
				31,
				34,
				32,
				36,
				33,
				38,
				35,
				40,
				34,
				43
			}));
			base.AddState(21, new State(new int[]
			{
				24,
				23,
				22,
				25,
				19,
				26,
				20,
				27,
				23,
				28,
				21,
				29
			}, new int[]
			{
				-23,
				22,
				-27,
				24
			}));
			base.AddState(22, new State(-55));
			base.AddState(23, new State(-65));
			base.AddState(24, new State(-66));
			base.AddState(25, new State(-67));
			base.AddState(26, new State(-68));
			base.AddState(27, new State(-69));
			base.AddState(28, new State(-70));
			base.AddState(29, new State(-71));
			base.AddState(30, new State(new int[]
			{
				24,
				23,
				22,
				25,
				19,
				26,
				20,
				27,
				23,
				28,
				21,
				29
			}, new int[]
			{
				-23,
				31,
				-27,
				24
			}));
			base.AddState(31, new State(-56));
			base.AddState(32, new State(new int[]
			{
				22,
				25,
				19,
				26,
				20,
				27,
				23,
				28,
				21,
				29
			}, new int[]
			{
				-27,
				33
			}));
			base.AddState(33, new State(-57));
			base.AddState(34, new State(new int[]
			{
				22,
				25,
				19,
				26,
				20,
				27,
				23,
				28,
				21,
				29
			}, new int[]
			{
				-27,
				35
			}));
			base.AddState(35, new State(-58));
			base.AddState(36, new State(new int[]
			{
				22,
				25,
				19,
				26,
				20,
				27,
				23,
				28,
				21,
				29
			}, new int[]
			{
				-27,
				37
			}));
			base.AddState(37, new State(-59));
			base.AddState(38, new State(new int[]
			{
				22,
				25,
				19,
				26,
				20,
				27,
				23,
				28,
				21,
				29
			}, new int[]
			{
				-27,
				39
			}));
			base.AddState(39, new State(-60));
			base.AddState(40, new State(new int[]
			{
				22,
				42
			}, new int[]
			{
				-28,
				41
			}));
			base.AddState(41, new State(-61));
			base.AddState(42, new State(-72));
			base.AddState(43, new State(new int[]
			{
				35,
				44
			}));
			base.AddState(44, new State(new int[]
			{
				22,
				42
			}, new int[]
			{
				-28,
				45
			}));
			base.AddState(45, new State(-62));
			base.AddState(46, new State(new int[]
			{
				17,
				13,
				34,
				46,
				37,
				48
			}, new int[]
			{
				-4,
				47,
				-25,
				19,
				-26,
				20
			}));
			base.AddState(47, new State(-53));
			base.AddState(48, new State(new int[]
			{
				17,
				13,
				34,
				46,
				37,
				48
			}, new int[]
			{
				-4,
				49,
				-25,
				19,
				-26,
				20
			}));
			base.AddState(49, new State(new int[]
			{
				38,
				50,
				27,
				15,
				26,
				17
			}));
			base.AddState(50, new State(-54));
			base.AddState(51, new State(new int[]
			{
				17,
				13
			}, new int[]
			{
				-5,
				52,
				-29,
				14,
				-26,
				10
			}));
			base.AddState(52, new State(new int[]
			{
				25,
				8,
				2,
				-4
			}));
			base.AddState(53, new State(new int[]
			{
				7,
				73,
				8,
				96,
				15,
				92,
				17,
				93,
				18,
				94
			}, new int[]
			{
				-9,
				54,
				-13,
				74,
				-14,
				99,
				-16,
				95
			}));
			base.AddState(54, new State(new int[]
			{
				9,
				55
			}));
			base.AddState(55, new State(new int[]
			{
				17,
				58
			}, new int[]
			{
				-10,
				56,
				-21,
				57,
				-19,
				65
			}));
			base.AddState(56, new State(-11));
			base.AddState(57, new State(-38));
			base.AddState(58, new State(new int[]
			{
				37,
				59,
				11,
				-44,
				12,
				-44,
				2,
				-44,
				16,
				-41
			}));
			base.AddState(59, new State(new int[]
			{
				24,
				23,
				22,
				25,
				19,
				26,
				20,
				27,
				23,
				28,
				21,
				29
			}, new int[]
			{
				-22,
				60,
				-23,
				64,
				-27,
				24
			}));
			base.AddState(60, new State(new int[]
			{
				38,
				61,
				25,
				62
			}));
			base.AddState(61, new State(-45));
			base.AddState(62, new State(new int[]
			{
				24,
				23,
				22,
				25,
				19,
				26,
				20,
				27,
				23,
				28,
				21,
				29
			}, new int[]
			{
				-23,
				63,
				-27,
				24
			}));
			base.AddState(63, new State(-47));
			base.AddState(64, new State(-46));
			base.AddState(65, new State(new int[]
			{
				16,
				66
			}));
			base.AddState(66, new State(new int[]
			{
				17,
				72
			}, new int[]
			{
				-21,
				67,
				-20,
				68
			}));
			base.AddState(67, new State(-39));
			base.AddState(68, new State(new int[]
			{
				16,
				69
			}));
			base.AddState(69, new State(new int[]
			{
				17,
				71
			}, new int[]
			{
				-21,
				70
			}));
			base.AddState(70, new State(-40));
			base.AddState(71, new State(new int[]
			{
				37,
				59,
				11,
				-44,
				12,
				-44,
				2,
				-44
			}));
			base.AddState(72, new State(new int[]
			{
				37,
				59,
				11,
				-44,
				12,
				-44,
				2,
				-44,
				16,
				-42
			}));
			base.AddState(73, new State(-15));
			base.AddState(74, new State(new int[]
			{
				15,
				92,
				17,
				93,
				18,
				94
			}, new int[]
			{
				-14,
				75,
				-16,
				95
			}));
			base.AddState(75, new State(new int[]
			{
				25,
				76,
				9,
				-16
			}));
			base.AddState(76, new State(new int[]
			{
				15,
				77,
				17,
				78,
				18,
				90
			}, new int[]
			{
				-16,
				91
			}));
			base.AddState(77, new State(-23));
			base.AddState(78, new State(new int[]
			{
				37,
				79,
				25,
				-24,
				9,
				-24
			}));
			base.AddState(79, new State(new int[]
			{
				38,
				80,
				15,
				87,
				17,
				88,
				18,
				89
			}, new int[]
			{
				-17,
				81
			}));
			base.AddState(80, new State(-27));
			base.AddState(81, new State(new int[]
			{
				38,
				82,
				25,
				83
			}));
			base.AddState(82, new State(-28));
			base.AddState(83, new State(new int[]
			{
				15,
				84,
				17,
				85,
				18,
				86
			}));
			base.AddState(84, new State(-32));
			base.AddState(85, new State(-33));
			base.AddState(86, new State(-34));
			base.AddState(87, new State(-29));
			base.AddState(88, new State(-30));
			base.AddState(89, new State(-31));
			base.AddState(90, new State(-25));
			base.AddState(91, new State(-26));
			base.AddState(92, new State(-19));
			base.AddState(93, new State(new int[]
			{
				37,
				79,
				25,
				-20,
				9,
				-20
			}));
			base.AddState(94, new State(-21));
			base.AddState(95, new State(-22));
			base.AddState(96, new State(new int[]
			{
				19,
				98
			}, new int[]
			{
				-15,
				97
			}));
			base.AddState(97, new State(-18));
			base.AddState(98, new State(-73));
			base.AddState(99, new State(new int[]
			{
				25,
				76,
				9,
				-17
			}));
			base.AddState(100, new State(new int[]
			{
				11,
				101,
				2,
				-6
			}));
			base.AddState(101, new State(new int[]
			{
				17,
				13,
				34,
				46,
				37,
				48
			}, new int[]
			{
				-4,
				102,
				-25,
				19,
				-26,
				20
			}));
			base.AddState(102, new State(new int[]
			{
				27,
				15,
				26,
				17,
				2,
				-7
			}));
			base.AddState(103, new State(new int[]
			{
				17,
				114
			}, new int[]
			{
				-11,
				104,
				-18,
				113,
				-19,
				115
			}));
			base.AddState(104, new State(new int[]
			{
				10,
				105
			}));
			base.AddState(105, new State(new int[]
			{
				17,
				13
			}, new int[]
			{
				-12,
				106,
				-24,
				112,
				-26,
				109
			}));
			base.AddState(106, new State(new int[]
			{
				25,
				107,
				11,
				-12,
				2,
				-12
			}));
			base.AddState(107, new State(new int[]
			{
				17,
				13
			}, new int[]
			{
				-24,
				108,
				-26,
				109
			}));
			base.AddState(108, new State(-49));
			base.AddState(109, new State(new int[]
			{
				28,
				110
			}));
			base.AddState(110, new State(new int[]
			{
				24,
				23,
				22,
				25,
				19,
				26,
				20,
				27,
				23,
				28,
				21,
				29
			}, new int[]
			{
				-23,
				111,
				-27,
				24
			}));
			base.AddState(111, new State(-63));
			base.AddState(112, new State(-48));
			base.AddState(113, new State(-35));
			base.AddState(114, new State(new int[]
			{
				10,
				-43,
				11,
				-43,
				2,
				-43,
				16,
				-41
			}));
			base.AddState(115, new State(new int[]
			{
				16,
				116
			}));
			base.AddState(116, new State(new int[]
			{
				17,
				122
			}, new int[]
			{
				-18,
				117,
				-20,
				118
			}));
			base.AddState(117, new State(-36));
			base.AddState(118, new State(new int[]
			{
				16,
				119
			}));
			base.AddState(119, new State(new int[]
			{
				17,
				121
			}, new int[]
			{
				-18,
				120
			}));
			base.AddState(120, new State(-37));
			base.AddState(121, new State(-43));
			base.AddState(122, new State(new int[]
			{
				10,
				-43,
				11,
				-43,
				2,
				-43,
				16,
				-42
			}));
			base.AddState(123, new State(-8));
			base.AddState(124, new State(new int[]
			{
				17,
				114
			}, new int[]
			{
				-11,
				125,
				-18,
				113,
				-19,
				115
			}));
			base.AddState(125, new State(new int[]
			{
				10,
				126
			}));
			base.AddState(126, new State(new int[]
			{
				17,
				13
			}, new int[]
			{
				-12,
				127,
				-24,
				112,
				-26,
				109
			}));
			base.AddState(127, new State(new int[]
			{
				25,
				107,
				2,
				-13
			}));
			base.AddState(128, new State(new int[]
			{
				11,
				129,
				2,
				-9
			}));
			base.AddState(129, new State(new int[]
			{
				17,
				13,
				34,
				46,
				37,
				48
			}, new int[]
			{
				-4,
				130,
				-25,
				19,
				-26,
				20
			}));
			base.AddState(130, new State(new int[]
			{
				27,
				15,
				26,
				17,
				2,
				-10
			}));
			base.AddState(131, new State(new int[]
			{
				17,
				114
			}, new int[]
			{
				-11,
				132,
				-18,
				113,
				-19,
				115
			}));
			base.AddState(132, new State(-14));
			this.rules = new Rule[79];
			this.rules[1] = new Rule(-2, new int[]
			{
				-1,
				2
			});
			this.rules[2] = new Rule(-1, new int[]
			{
				-3
			});
			this.rules[3] = new Rule(-1, new int[]
			{
				-3,
				11,
				-4
			});
			this.rules[4] = new Rule(-1, new int[]
			{
				-3,
				12,
				-5
			});
			this.rules[5] = new Rule(-1, new int[]
			{
				-3,
				11,
				-4,
				12,
				-5
			});
			this.rules[6] = new Rule(-1, new int[]
			{
				-6
			});
			this.rules[7] = new Rule(-1, new int[]
			{
				-6,
				11,
				-4
			});
			this.rules[8] = new Rule(-1, new int[]
			{
				-7
			});
			this.rules[9] = new Rule(-1, new int[]
			{
				-8
			});
			this.rules[10] = new Rule(-1, new int[]
			{
				-8,
				11,
				-4
			});
			this.rules[11] = new Rule(-3, new int[]
			{
				3,
				-9,
				9,
				-10
			});
			this.rules[12] = new Rule(-6, new int[]
			{
				4,
				-11,
				10,
				-12
			});
			this.rules[13] = new Rule(-7, new int[]
			{
				5,
				-11,
				10,
				-12
			});
			this.rules[14] = new Rule(-8, new int[]
			{
				6,
				-11
			});
			this.rules[15] = new Rule(-9, new int[]
			{
				7
			});
			this.rules[16] = new Rule(-9, new int[]
			{
				-13,
				-14
			});
			this.rules[17] = new Rule(-9, new int[]
			{
				-14
			});
			this.rules[18] = new Rule(-13, new int[]
			{
				8,
				-15
			});
			this.rules[19] = new Rule(-14, new int[]
			{
				15
			});
			this.rules[20] = new Rule(-14, new int[]
			{
				17
			});
			this.rules[21] = new Rule(-14, new int[]
			{
				18
			});
			this.rules[22] = new Rule(-14, new int[]
			{
				-16
			});
			this.rules[23] = new Rule(-14, new int[]
			{
				-14,
				25,
				15
			});
			this.rules[24] = new Rule(-14, new int[]
			{
				-14,
				25,
				17
			});
			this.rules[25] = new Rule(-14, new int[]
			{
				-14,
				25,
				18
			});
			this.rules[26] = new Rule(-14, new int[]
			{
				-14,
				25,
				-16
			});
			this.rules[27] = new Rule(-16, new int[]
			{
				17,
				37,
				38
			});
			this.rules[28] = new Rule(-16, new int[]
			{
				17,
				37,
				-17,
				38
			});
			this.rules[29] = new Rule(-17, new int[]
			{
				15
			});
			this.rules[30] = new Rule(-17, new int[]
			{
				17
			});
			this.rules[31] = new Rule(-17, new int[]
			{
				18
			});
			this.rules[32] = new Rule(-17, new int[]
			{
				-17,
				25,
				15
			});
			this.rules[33] = new Rule(-17, new int[]
			{
				-17,
				25,
				17
			});
			this.rules[34] = new Rule(-17, new int[]
			{
				-17,
				25,
				18
			});
			this.rules[35] = new Rule(-11, new int[]
			{
				-18
			});
			this.rules[36] = new Rule(-11, new int[]
			{
				-19,
				16,
				-18
			});
			this.rules[37] = new Rule(-11, new int[]
			{
				-19,
				16,
				-20,
				16,
				-18
			});
			this.rules[38] = new Rule(-10, new int[]
			{
				-21
			});
			this.rules[39] = new Rule(-10, new int[]
			{
				-19,
				16,
				-21
			});
			this.rules[40] = new Rule(-10, new int[]
			{
				-19,
				16,
				-20,
				16,
				-21
			});
			this.rules[41] = new Rule(-19, new int[]
			{
				17
			});
			this.rules[42] = new Rule(-20, new int[]
			{
				17
			});
			this.rules[43] = new Rule(-18, new int[]
			{
				17
			});
			this.rules[44] = new Rule(-21, new int[]
			{
				17
			});
			this.rules[45] = new Rule(-21, new int[]
			{
				17,
				37,
				-22,
				38
			});
			this.rules[46] = new Rule(-22, new int[]
			{
				-23
			});
			this.rules[47] = new Rule(-22, new int[]
			{
				-22,
				25,
				-23
			});
			this.rules[48] = new Rule(-12, new int[]
			{
				-24
			});
			this.rules[49] = new Rule(-12, new int[]
			{
				-12,
				25,
				-24
			});
			this.rules[50] = new Rule(-4, new int[]
			{
				-25
			});
			this.rules[51] = new Rule(-4, new int[]
			{
				-4,
				27,
				-4
			});
			this.rules[52] = new Rule(-4, new int[]
			{
				-4,
				26,
				-4
			});
			this.rules[53] = new Rule(-4, new int[]
			{
				34,
				-4
			});
			this.rules[54] = new Rule(-4, new int[]
			{
				37,
				-4,
				38
			});
			this.rules[55] = new Rule(-25, new int[]
			{
				-26,
				28,
				-23
			});
			this.rules[56] = new Rule(-25, new int[]
			{
				-26,
				29,
				-23
			});
			this.rules[57] = new Rule(-25, new int[]
			{
				-26,
				30,
				-27
			});
			this.rules[58] = new Rule(-25, new int[]
			{
				-26,
				31,
				-27
			});
			this.rules[59] = new Rule(-25, new int[]
			{
				-26,
				32,
				-27
			});
			this.rules[60] = new Rule(-25, new int[]
			{
				-26,
				33,
				-27
			});
			this.rules[61] = new Rule(-25, new int[]
			{
				-26,
				35,
				-28
			});
			this.rules[62] = new Rule(-25, new int[]
			{
				-26,
				34,
				35,
				-28
			});
			this.rules[63] = new Rule(-24, new int[]
			{
				-26,
				28,
				-23
			});
			this.rules[64] = new Rule(-26, new int[]
			{
				17
			});
			this.rules[65] = new Rule(-23, new int[]
			{
				24
			});
			this.rules[66] = new Rule(-23, new int[]
			{
				-27
			});
			this.rules[67] = new Rule(-27, new int[]
			{
				22
			});
			this.rules[68] = new Rule(-27, new int[]
			{
				19
			});
			this.rules[69] = new Rule(-27, new int[]
			{
				20
			});
			this.rules[70] = new Rule(-27, new int[]
			{
				23
			});
			this.rules[71] = new Rule(-27, new int[]
			{
				21
			});
			this.rules[72] = new Rule(-28, new int[]
			{
				22
			});
			this.rules[73] = new Rule(-15, new int[]
			{
				19
			});
			this.rules[74] = new Rule(-5, new int[]
			{
				-29
			});
			this.rules[75] = new Rule(-5, new int[]
			{
				-5,
				25,
				-29
			});
			this.rules[76] = new Rule(-29, new int[]
			{
				-26
			});
			this.rules[77] = new Rule(-29, new int[]
			{
				-26,
				13
			});
			this.rules[78] = new Rule(-29, new int[]
			{
				-26,
				14
			});
			this.nonTerminals = new string[]
			{
				"",
				"query",
				"$accept",
				"select_from",
				"expr",
				"orderby",
				"update_set",
				"insert_set",
				"delete_from",
				"select_list",
				"from_context",
				"modification_context",
				"set_list",
				"top_n",
				"columns",
				"integer_value",
				"processor",
				"arguments",
				"table",
				"database",
				"schema",
				"table_or_table_function",
				"param_list",
				"value_or_null",
				"set_expr",
				"condition",
				"name",
				"value",
				"string_value",
				"sortcolumn"
			};
		}

		protected override void DoAction(int action)
		{
			switch (action)
			{
			case 2:
				this.type = DiagnosticQueryParser.QueryType.Select;
				this.whereExpression = null;
				this.orderByList = null;
				return;
			case 3:
				this.type = DiagnosticQueryParser.QueryType.Select;
				this.whereExpression = (DiagnosticQueryCriteria)this.value_stack.array[this.value_stack.top - 1].Value;
				this.orderByList = null;
				return;
			case 4:
				this.type = DiagnosticQueryParser.QueryType.Select;
				this.whereExpression = null;
				this.orderByList = (IList<DiagnosticQueryParser.SortColumn>)this.value_stack.array[this.value_stack.top - 1].Value;
				return;
			case 5:
				this.type = DiagnosticQueryParser.QueryType.Select;
				this.whereExpression = (DiagnosticQueryCriteria)this.value_stack.array[this.value_stack.top - 3].Value;
				this.orderByList = (IList<DiagnosticQueryParser.SortColumn>)this.value_stack.array[this.value_stack.top - 1].Value;
				return;
			case 6:
				this.type = DiagnosticQueryParser.QueryType.Update;
				this.whereExpression = null;
				this.orderByList = null;
				return;
			case 7:
				this.type = DiagnosticQueryParser.QueryType.Update;
				this.whereExpression = (DiagnosticQueryCriteria)this.value_stack.array[this.value_stack.top - 1].Value;
				this.orderByList = null;
				return;
			case 8:
				this.type = DiagnosticQueryParser.QueryType.Insert;
				this.whereExpression = null;
				this.orderByList = null;
				return;
			case 9:
				this.type = DiagnosticQueryParser.QueryType.Delete;
				this.whereExpression = null;
				this.orderByList = null;
				return;
			case 10:
				this.type = DiagnosticQueryParser.QueryType.Delete;
				this.whereExpression = (DiagnosticQueryCriteria)this.value_stack.array[this.value_stack.top - 1].Value;
				this.orderByList = null;
				return;
			case 11:
				this.selectList = (IList<DiagnosticQueryParser.Column>)this.value_stack.array[this.value_stack.top - 3].Value;
				this.queryContext = (DiagnosticQueryParser.Context)this.value_stack.array[this.value_stack.top - 1].Value;
				this.setList = null;
				return;
			case 12:
				this.selectList = null;
				this.queryContext = (DiagnosticQueryParser.Context)this.value_stack.array[this.value_stack.top - 3].Value;
				this.setList = (IDictionary<string, string>)this.value_stack.array[this.value_stack.top - 1].Value;
				return;
			case 13:
				this.selectList = null;
				this.queryContext = (DiagnosticQueryParser.Context)this.value_stack.array[this.value_stack.top - 3].Value;
				this.setList = (IDictionary<string, string>)this.value_stack.array[this.value_stack.top - 1].Value;
				return;
			case 14:
				this.selectList = null;
				this.queryContext = (DiagnosticQueryParser.Context)this.value_stack.array[this.value_stack.top - 1].Value;
				this.setList = null;
				return;
			case 15:
				this.isCountQuery = true;
				this.yyval.Value = new List<DiagnosticQueryParser.Column>
				{
					DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier, false)
				};
				return;
			case 16:
			{
				int num;
				if (int.TryParse(this.value_stack.array[this.value_stack.top - 2].ValueAsString, out num) && num > 0)
				{
					this.maxRows = num;
					this.yyval = this.value_stack.array[this.value_stack.top - 1];
					return;
				}
				throw new DiagnosticQueryParserException(DiagnosticQueryStrings.InvalidTop());
			}
			case 17:
				this.yyval = this.value_stack.array[this.value_stack.top - 1];
				return;
			case 18:
				this.yyval = this.value_stack.array[this.value_stack.top - 1];
				return;
			case 19:
				this.yyval.Value = new List<DiagnosticQueryParser.Column>
				{
					DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier, false)
				};
				return;
			case 20:
				this.yyval.Value = new List<DiagnosticQueryParser.Column>
				{
					DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier, false)
				};
				return;
			case 21:
				this.yyval.Value = new List<DiagnosticQueryParser.Column>
				{
					DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsSubtractor, true)
				};
				return;
			case 22:
				this.yyval.Value = new List<DiagnosticQueryParser.Column>
				{
					(DiagnosticQueryParser.Column)this.value_stack.array[this.value_stack.top - 1].Value
				};
				return;
			case 23:
			{
				IList<DiagnosticQueryParser.Column> list = (IList<DiagnosticQueryParser.Column>)this.value_stack.array[this.value_stack.top - 3].Value;
				list.Add(DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier, false));
				this.yyval.Value = list;
				return;
			}
			case 24:
			{
				IList<DiagnosticQueryParser.Column> list2 = (IList<DiagnosticQueryParser.Column>)this.value_stack.array[this.value_stack.top - 3].Value;
				list2.Add(DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier, false));
				this.yyval.Value = list2;
				return;
			}
			case 25:
			{
				IList<DiagnosticQueryParser.Column> list3 = (IList<DiagnosticQueryParser.Column>)this.value_stack.array[this.value_stack.top - 3].Value;
				list3.Add(DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsSubtractor, true));
				this.yyval.Value = list3;
				return;
			}
			case 26:
			{
				IList<DiagnosticQueryParser.Column> list4 = (IList<DiagnosticQueryParser.Column>)this.value_stack.array[this.value_stack.top - 3].Value;
				list4.Add((DiagnosticQueryParser.Column)this.value_stack.array[this.value_stack.top - 1].Value);
				this.yyval.Value = list4;
				return;
			}
			case 27:
				this.yyval.Value = DiagnosticQueryParser.Processor.Create(this.value_stack.array[this.value_stack.top - 3].ValueAsIdentifier);
				return;
			case 28:
				this.yyval.Value = DiagnosticQueryParser.Processor.Create(this.value_stack.array[this.value_stack.top - 4].ValueAsIdentifier, (IList<DiagnosticQueryParser.Column>)this.value_stack.array[this.value_stack.top - 2].Value);
				return;
			case 29:
				this.yyval.Value = new List<DiagnosticQueryParser.Column>
				{
					DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier, false)
				};
				return;
			case 30:
				this.yyval.Value = new List<DiagnosticQueryParser.Column>
				{
					DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier, false)
				};
				return;
			case 31:
				this.yyval.Value = new List<DiagnosticQueryParser.Column>
				{
					DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsSubtractor, true)
				};
				return;
			case 32:
			{
				IList<DiagnosticQueryParser.Column> list5 = (IList<DiagnosticQueryParser.Column>)this.value_stack.array[this.value_stack.top - 3].Value;
				list5.Add(DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier, false));
				this.yyval.Value = list5;
				return;
			}
			case 33:
			{
				IList<DiagnosticQueryParser.Column> list6 = (IList<DiagnosticQueryParser.Column>)this.value_stack.array[this.value_stack.top - 3].Value;
				list6.Add(DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier, false));
				this.yyval.Value = list6;
				return;
			}
			case 34:
			{
				IList<DiagnosticQueryParser.Column> list7 = (IList<DiagnosticQueryParser.Column>)this.value_stack.array[this.value_stack.top - 3].Value;
				list7.Add(DiagnosticQueryParser.Column.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsSubtractor, true));
				this.yyval.Value = list7;
				return;
			}
			case 35:
				this.yyval.Value = DiagnosticQueryParser.Context.Create((DiagnosticQueryParser.TableInfo)this.value_stack.array[this.value_stack.top - 1].Value);
				return;
			case 36:
				this.yyval.Value = DiagnosticQueryParser.Context.Create(this.value_stack.array[this.value_stack.top - 3].ValueAsString, (DiagnosticQueryParser.TableInfo)this.value_stack.array[this.value_stack.top - 1].Value);
				return;
			case 37:
				this.yyval.Value = DiagnosticQueryParser.Context.Create(this.value_stack.array[this.value_stack.top - 5].ValueAsString, this.value_stack.array[this.value_stack.top - 3].ValueAsString, (DiagnosticQueryParser.TableInfo)this.value_stack.array[this.value_stack.top - 1].Value);
				return;
			case 38:
				this.yyval.Value = DiagnosticQueryParser.Context.Create((DiagnosticQueryParser.TableInfo)this.value_stack.array[this.value_stack.top - 1].Value);
				return;
			case 39:
				this.yyval.Value = DiagnosticQueryParser.Context.Create(this.value_stack.array[this.value_stack.top - 3].ValueAsString, (DiagnosticQueryParser.TableInfo)this.value_stack.array[this.value_stack.top - 1].Value);
				return;
			case 40:
				this.yyval.Value = DiagnosticQueryParser.Context.Create(this.value_stack.array[this.value_stack.top - 5].ValueAsString, this.value_stack.array[this.value_stack.top - 3].ValueAsString, (DiagnosticQueryParser.TableInfo)this.value_stack.array[this.value_stack.top - 1].Value);
				return;
			case 41:
				this.yyval.Value = this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier;
				return;
			case 42:
				this.yyval.Value = this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier;
				return;
			case 43:
				this.yyval.Value = DiagnosticQueryParser.TableInfo.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier);
				return;
			case 44:
				this.yyval.Value = DiagnosticQueryParser.TableInfo.Create(this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier);
				return;
			case 45:
				this.yyval.Value = DiagnosticQueryParser.TableInfo.Create(this.value_stack.array[this.value_stack.top - 4].ValueAsIdentifier, (List<string>)this.value_stack.array[this.value_stack.top - 2].Value);
				return;
			case 46:
				this.yyval.Value = new List<string>
				{
					this.value_stack.array[this.value_stack.top - 1].ValueAsString
				};
				return;
			case 47:
			{
				List<string> list8 = (List<string>)this.value_stack.array[this.value_stack.top - 3].Value;
				list8.Add(this.value_stack.array[this.value_stack.top - 1].ValueAsString);
				this.yyval.Value = list8;
				return;
			}
			case 48:
			{
				KeyValuePair<string, string> keyValuePair = (KeyValuePair<string, string>)this.value_stack.array[this.value_stack.top - 1].Value;
				this.yyval.Value = new Dictionary<string, string>
				{
					{
						keyValuePair.Key,
						keyValuePair.Value
					}
				};
				return;
			}
			case 49:
			{
				IDictionary<string, string> dictionary = (IDictionary<string, string>)this.value_stack.array[this.value_stack.top - 3].Value;
				KeyValuePair<string, string> keyValuePair2 = (KeyValuePair<string, string>)this.value_stack.array[this.value_stack.top - 1].Value;
				if (dictionary.ContainsKey(keyValuePair2.Key))
				{
					throw new DiagnosticQueryParserException(DiagnosticQueryStrings.DuplicateSet(keyValuePair2.Key));
				}
				dictionary.Add(keyValuePair2.Key, keyValuePair2.Value);
				this.yyval.Value = dictionary;
				return;
			}
			case 50:
				this.yyval = this.value_stack.array[this.value_stack.top - 1];
				return;
			case 51:
				this.yyval.Value = DiagnosticQueryCriteriaAnd.Create(new DiagnosticQueryCriteria[]
				{
					(DiagnosticQueryCriteria)this.value_stack.array[this.value_stack.top - 3].Value,
					(DiagnosticQueryCriteria)this.value_stack.array[this.value_stack.top - 1].Value
				});
				return;
			case 52:
				this.yyval.Value = DiagnosticQueryCriteriaOr.Create(new DiagnosticQueryCriteria[]
				{
					(DiagnosticQueryCriteria)this.value_stack.array[this.value_stack.top - 3].Value,
					(DiagnosticQueryCriteria)this.value_stack.array[this.value_stack.top - 1].Value
				});
				return;
			case 53:
				this.yyval.Value = DiagnosticQueryCriteriaNot.Create((DiagnosticQueryCriteria)this.value_stack.array[this.value_stack.top - 1].Value);
				return;
			case 54:
				this.yyval = this.value_stack.array[this.value_stack.top - 2];
				return;
			case 55:
				this.yyval.Value = DiagnosticQueryCriteriaCompare.Create(this.value_stack.array[this.value_stack.top - 3].ValueAsString, DiagnosticQueryOperator.Equal, this.value_stack.array[this.value_stack.top - 1].ValueAsString);
				return;
			case 56:
				this.yyval.Value = DiagnosticQueryCriteriaCompare.Create(this.value_stack.array[this.value_stack.top - 3].ValueAsString, DiagnosticQueryOperator.NotEqual, this.value_stack.array[this.value_stack.top - 1].ValueAsString);
				return;
			case 57:
				this.yyval.Value = DiagnosticQueryCriteriaCompare.Create(this.value_stack.array[this.value_stack.top - 3].ValueAsString, DiagnosticQueryOperator.GreaterThan, this.value_stack.array[this.value_stack.top - 1].ValueAsString);
				return;
			case 58:
				this.yyval.Value = DiagnosticQueryCriteriaCompare.Create(this.value_stack.array[this.value_stack.top - 3].ValueAsString, DiagnosticQueryOperator.LessThan, this.value_stack.array[this.value_stack.top - 1].ValueAsString);
				return;
			case 59:
				this.yyval.Value = DiagnosticQueryCriteriaCompare.Create(this.value_stack.array[this.value_stack.top - 3].ValueAsString, DiagnosticQueryOperator.GreaterThanOrEqual, this.value_stack.array[this.value_stack.top - 1].ValueAsString);
				return;
			case 60:
				this.yyval.Value = DiagnosticQueryCriteriaCompare.Create(this.value_stack.array[this.value_stack.top - 3].ValueAsString, DiagnosticQueryOperator.LessThanOrEqual, this.value_stack.array[this.value_stack.top - 1].ValueAsString);
				return;
			case 61:
				this.yyval.Value = DiagnosticQueryCriteriaCompare.Create(this.value_stack.array[this.value_stack.top - 3].ValueAsString, DiagnosticQueryOperator.Like, this.value_stack.array[this.value_stack.top - 1].ValueAsString);
				return;
			case 62:
				this.yyval.Value = DiagnosticQueryCriteriaNot.Create(DiagnosticQueryCriteriaCompare.Create(this.value_stack.array[this.value_stack.top - 4].ValueAsString, DiagnosticQueryOperator.Like, this.value_stack.array[this.value_stack.top - 1].ValueAsString));
				return;
			case 63:
				this.yyval.Value = new KeyValuePair<string, string>(this.value_stack.array[this.value_stack.top - 3].ValueAsString, this.value_stack.array[this.value_stack.top - 1].ValueAsString);
				return;
			case 64:
				this.yyval.Value = this.value_stack.array[this.value_stack.top - 1].ValueAsIdentifier;
				return;
			case 65:
				this.yyval.Value = null;
				return;
			case 66:
				this.yyval = this.value_stack.array[this.value_stack.top - 1];
				return;
			case 67:
				this.yyval.Value = this.value_stack.array[this.value_stack.top - 1].TrimOuter().Replace("''", "'");
				return;
			case 68:
				this.yyval = this.value_stack.array[this.value_stack.top - 1];
				return;
			case 69:
				this.yyval = this.value_stack.array[this.value_stack.top - 1];
				return;
			case 70:
				this.yyval = this.value_stack.array[this.value_stack.top - 1];
				return;
			case 71:
				this.yyval = this.value_stack.array[this.value_stack.top - 1];
				return;
			case 72:
				this.yyval.Value = this.value_stack.array[this.value_stack.top - 1].TrimOuter().Replace("''", "'");
				return;
			case 73:
				this.yyval = this.value_stack.array[this.value_stack.top - 1];
				return;
			case 74:
				this.yyval.Value = new List<DiagnosticQueryParser.SortColumn>
				{
					(DiagnosticQueryParser.SortColumn)this.value_stack.array[this.value_stack.top - 1].Value
				};
				return;
			case 75:
			{
				IList<DiagnosticQueryParser.SortColumn> list9 = (List<DiagnosticQueryParser.SortColumn>)this.value_stack.array[this.value_stack.top - 3].Value;
				list9.Add((DiagnosticQueryParser.SortColumn)this.value_stack.array[this.value_stack.top - 1].Value);
				this.yyval.Value = list9;
				return;
			}
			case 76:
				this.yyval.Value = new DiagnosticQueryParser.SortColumn(this.value_stack.array[this.value_stack.top - 1].ValueAsString);
				return;
			case 77:
				this.yyval.Value = new DiagnosticQueryParser.SortColumn(this.value_stack.array[this.value_stack.top - 2].ValueAsString, true);
				return;
			case 78:
				this.yyval.Value = new DiagnosticQueryParser.SortColumn(this.value_stack.array[this.value_stack.top - 2].ValueAsString, false);
				return;
			default:
				return;
			}
		}

		protected override string TerminalToString(int terminal)
		{
			if (((Tokens)terminal).ToString() != terminal.ToString())
			{
				return ((Tokens)terminal).ToString();
			}
			return base.CharToString((char)terminal);
		}

		internal Parser(string query)
		{
			this.scanner = new Scanner(query);
			base.Parse();
			if (this.whereExpression != null)
			{
				this.whereExpression = this.whereExpression.Reduce();
			}
		}

		public DiagnosticQueryParser.QueryType Type
		{
			get
			{
				return this.type;
			}
		}

		public IList<DiagnosticQueryParser.Column> Select
		{
			get
			{
				return this.selectList;
			}
		}

		public DiagnosticQueryParser.Context From
		{
			get
			{
				return this.queryContext;
			}
		}

		public IDictionary<string, string> Set
		{
			get
			{
				return this.setList;
			}
		}

		public DiagnosticQueryCriteria Where
		{
			get
			{
				return this.whereExpression;
			}
		}

		public IList<DiagnosticQueryParser.SortColumn> OrderBy
		{
			get
			{
				return this.orderByList;
			}
		}

		public bool IsCountQuery
		{
			get
			{
				return this.isCountQuery;
			}
		}

		public int MaxRows
		{
			get
			{
				return this.maxRows;
			}
		}

		private DiagnosticQueryParser.QueryType type;

		private IList<DiagnosticQueryParser.Column> selectList;

		private DiagnosticQueryParser.Context queryContext;

		private DiagnosticQueryCriteria whereExpression;

		private IList<DiagnosticQueryParser.SortColumn> orderByList;

		private IDictionary<string, string> setList;

		private bool isCountQuery;

		private int maxRows;
	}
}
