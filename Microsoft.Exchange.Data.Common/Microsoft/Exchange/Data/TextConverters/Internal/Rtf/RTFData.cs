using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal sealed class RTFData
	{
		public static short Hash(byte[] chars, int off, int len)
		{
			short num = 0;
			while (len != 0)
			{
				byte b = chars[off];
				num = (short)((((int)num << 3) + (num >> 6) ^ (int)b) % 299);
				len--;
				off++;
			}
			return num;
		}

		public static short AddHash(short hash, byte ch)
		{
			return (short)((((int)hash << 3) + (hash >> 6) ^ (int)ch) % 299);
		}

		private RTFData()
		{
		}

		public const int ID__unknownKeyword = 0;

		public const int ID__ignorableDest = 1;

		public const int ID__formulaChar = 2;

		public const int ID__indexSubentry = 3;

		public const int ID_aul = 4;

		public const int ID_ulw = 5;

		public const int ID_pichgoal = 6;

		public const int ID_trbrdrb = 7;

		public const int ID_leveltext = 8;

		public const int ID_listlevel = 9;

		public const int ID_trbrdrh = 10;

		public const int ID_brdrengrave = 11;

		public const int ID_trbrdrl = 12;

		public const int ID_irow = 13;

		public const int ID_brdrtriple = 14;

		public const int ID_footer = 15;

		public const int ID_trbrdrr = 16;

		public const int ID_caps = 17;

		public const int ID_fscript = 18;

		public const int ID_uldash = 19;

		public const int ID_expndtw = 20;

		public const int ID_pnucrm = 21;

		public const int ID_trbrdrt = 22;

		public const int ID_brdrwavydb = 23;

		public const int ID_header = 24;

		public const int ID_trbrdrv = 25;

		public const int ID_embo = 26;

		public const int ID_pnindent = 27;

		public const int ID_zwj = 28;

		public const int ID_field = 29;

		public const int ID_fnil = 30;

		public const int ID_link = 31;

		public const int ID_disabled = 32;

		public const int ID_footnote = 33;

		public const int ID_fcharset = 34;

		public const int ID_mac = 35;

		public const int ID_pnucltr = 36;

		public const int ID_fbidis = 37;

		public const int ID_lquote = 38;

		public const int ID_macpict = 39;

		public const int ID_row = 40;

		public const int ID_rtlrow = 41;

		public const int ID_fprq = 42;

		public const int ID_picprop = 43;

		public const int ID_levelstartat = 44;

		public const int ID_pich = 45;

		public const int ID_brdrwavy = 46;

		public const int ID_bin = 47;

		public const int ID_line = 48;

		public const int ID_fmodern = 49;

		public const int ID_pict = 50;

		public const int ID_pnlvlblt = 51;

		public const int ID_brdrdashsm = 52;

		public const int ID_clcfpat = 53;

		public const int ID_list = 54;

		public const int ID_nestrow = 55;

		public const int ID_brdrbtw = 56;

		public const int ID_picw = 57;

		public const int ID_cbpat = 58;

		public const int ID_rtlmark = 59;

		public const int ID_deflang = 60;

		public const int ID_ulhwave = 61;

		public const int ID_pnaiud = 62;

		public const int ID_ulldash = 63;

		public const int ID_brdrdashdotstr = 64;

		public const int ID_nesttableprops = 65;

		public const int ID_trleft = 66;

		public const int ID_bghoriz = 67;

		public const int ID_par = 68;

		public const int ID_keepn = 69;

		public const int ID_pnordt = 70;

		public const int ID_lang = 71;

		public const int ID_fbidi = 72;

		public const int ID_lastrow = 73;

		public const int ID_bullet = 74;

		public const int ID_sectd = 75;

		public const int ID_ul = 76;

		public const int ID_pnlcltr = 77;

		public const int ID_clvmrg = 78;

		public const int ID_shad = 79;

		public const int ID_brdrdash = 80;

		public const int ID_uc = 81;

		public const int ID_highlight = 82;

		public const int ID_htmlbase = 83;

		public const int ID_pncnum = 84;

		public const int ID_ud = 85;

		public const int ID_pnstart = 86;

		public const int ID_adeff = 87;

		public const int ID_blue = 88;

		public const int ID_brdrdb = 89;

		public const int ID_brdrframe = 90;

		public const int ID_taprtl = 91;

		public const int ID_comment = 92;

		public const int ID_froman = 93;

		public const int ID_fdecor = 94;

		public const int ID_dbch = 95;

		public const int ID_up = 96;

		public const int ID_brdroutset = 97;

		public const int ID_clvertalb = 98;

		public const int ID_striked = 99;

		public const int ID_clvertalc = 100;

		public const int ID_itap = 101;

		public const int ID_pnlvl = 102;

		public const int ID_clftsWidth = 103;

		public const int ID_upr = 104;

		public const int ID_nestcell = 105;

		public const int ID_pnord = 106;

		public const int ID_protect = 107;

		public const int ID_pc = 108;

		public const int ID_b = 109;

		public const int ID_deftab = 110;

		public const int ID_qj = 111;

		public const int ID_ql = 112;

		public const int ID_f = 113;

		public const int ID_sp = 114;

		public const int ID_i = 115;

		public const int ID_qc = 116;

		public const int ID_revised = 117;

		public const int ID_trpaddr = 118;

		public const int ID_cell = 119;

		public const int ID_qd = 120;

		public const int ID_trpaddt = 121;

		public const int ID_brdrthtnlg = 122;

		public const int ID_pn = 123;

		public const int ID_sv = 124;

		public const int ID_brsp = 125;

		public const int ID_tab = 126;

		public const int ID_trgaph = 127;

		public const int ID_shp = 128;

		public const int ID_s = 129;

		public const int ID_sl = 130;

		public const int ID_trpaddl = 131;

		public const int ID_brdrthtnmg = 132;

		public const int ID_u = 133;

		public const int ID_ltrch = 134;

		public const int ID_sn = 135;

		public const int ID_v = 136;

		public const int ID_hich = 137;

		public const int ID_ri = 138;

		public const int ID_sa = 139;

		public const int ID_qs = 140;

		public const int ID_qr = 141;

		public const int ID_sb = 142;

		public const int ID_trcbpat = 143;

		public const int ID_trpaddb = 144;

		public const int ID_cfpat = 145;

		public const int ID_keep = 146;

		public const int ID_bgvert = 147;

		public const int ID_red = 148;

		public const int ID_deflangfe = 149;

		public const int ID_ululdbwave = 150;

		public const int ID_trrh = 151;

		public const int ID__hyphen = 152;

		public const int ID_htmlrtf = 153;

		public const int ID_picwgoal = 154;

		public const int ID_uldashdd = 155;

		public const int ID_brdrtnthsg = 156;

		public const int ID_objattph = 157;

		public const int ID_bgdkbdiag = 158;

		public const int ID_uldb = 159;

		public const int ID_clmrg = 160;

		public const int ID_clpadr = 161;

		public const int ID_outl = 162;

		public const int ID_clpadt = 163;

		public const int ID_fcs = 164;

		public const int ID_ansicpg = 165;

		public const int ID_shpinst = 166;

		public const int ID_brdrcf = 167;

		public const int ID_sect = 168;

		public const int ID_afs = 169;

		public const int ID_plain = 170;

		public const int ID_brdrinset = 171;

		public const int ID_clpadl = 172;

		public const int ID_listid = 173;

		public const int ID_acf = 174;

		public const int ID_fonttbl = 175;

		public const int ID_rtlpar = 176;

		public const int ID_htmltag = 177;

		public const int ID_ulthdashd = 178;

		public const int ID_clpadb = 179;

		public const int ID_scaps = 180;

		public const int ID_clbrdrr = 181;

		public const int ID_shading = 182;

		public const int ID_trqr = 183;

		public const int ID_pard = 184;

		public const int ID_pnfs = 185;

		public const int ID_clbrdrt = 186;

		public const int ID_bgdkvert = 187;

		public const int ID_brdrl = 188;

		public const int ID_ltrmark = 189;

		public const int ID_ulthldash = 190;

		public const int ID_intbl = 191;

		public const int ID_bgbdiag = 192;

		public const int ID_bkmkstart = 193;

		public const int ID_brdrb = 194;

		public const int ID_clbrdrl = 195;

		public const int ID_clbrdrb = 196;

		public const int ID_pagebb = 197;

		public const int ID_strike = 198;

		public const int ID_clvmgf = 199;

		public const int ID_trowd = 200;

		public const int ID_info = 201;

		public const int ID_ldblquote = 202;

		public const int ID_listoverride = 203;

		public const int ID_trqc = 204;

		public const int ID_ilvl = 205;

		public const int ID_li = 206;

		public const int ID_zwnj = 207;

		public const int ID_listsimple = 208;

		public const int ID_brdrdashdd = 209;

		public const int ID_fname = 210;

		public const int ID_ulnone = 211;

		public const int ID_bkmkend = 212;

		public const int ID_clwWidth = 213;

		public const int ID_adeflang = 214;

		public const int ID_brdrr = 215;

		public const int ID_brdrs = 216;

		public const int ID_pniroha = 217;

		public const int ID_brdrt = 218;

		public const int ID_ls = 219;

		public const int ID_brdrw = 220;

		public const int ID_irowband = 221;

		public const int ID_loch = 222;

		public const int ID_pnf = 223;

		public const int ID_slmult = 224;

		public const int ID_ulthdash = 225;

		public const int ID_bgdkdcross = 226;

		public const int ID_pntxta = 227;

		public const int ID_cellx = 228;

		public const int ID_pnlvlcont = 229;

		public const int ID_headerl = 230;

		public const int ID_jclisttab = 231;

		public const int ID_endash = 232;

		public const int ID_pntxtb = 233;

		public const int ID_bgfdiag = 234;

		public const int ID_brdrbar = 235;

		public const int ID_bgdkfdiag = 236;

		public const int ID_trwWidth = 237;

		public const int ID_alang = 238;

		public const int ID_brdrtnthtnlg = 239;

		public const int ID_cpg = 240;

		public const int ID_headerf = 241;

		public const int ID_ltrrow = 242;

		public const int ID_brdrtnthtnsg = 243;

		public const int ID_bgdkhoriz = 244;

		public const int ID_dropcapt = 245;

		public const int ID_listtable = 246;

		public const int ID_brdrtnthtnmg = 247;

		public const int ID_pnlcrm = 248;

		public const int ID_pndecd = 249;

		public const int ID_leveljc = 250;

		public const int ID_brdrtnthmg = 251;

		public const int ID_colortbl = 252;

		public const int ID_headerr = 253;

		public const int ID_brdrth = 254;

		public const int ID_levelpicture = 255;

		public const int ID_brdrtnthlg = 256;

		public const int ID_listtext = 257;

		public const int ID_footerr = 258;

		public const int ID_clmgf = 259;

		public const int ID_bgdcross = 260;

		public const int ID_brdrthtnsg = 261;

		public const int ID_sub = 262;

		public const int ID_rdblquote = 263;

		public const int ID_nosupersub = 264;

		public const int ID_fs = 265;

		public const int ID_ftech = 266;

		public const int ID_rtf = 267;

		public const int ID_falt = 268;

		public const int ID_fldrslt = 269;

		public const int ID_ltrpar = 270;

		public const int ID_urtf = 271;

		public const int ID_pncard = 272;

		public const int ID_footerf = 273;

		public const int ID_pndec = 274;

		public const int ID_dn = 275;

		public const int ID_brdrdashd = 276;

		public const int ID_footerl = 277;

		public const int ID_uldashd = 278;

		public const int ID_ulwave = 279;

		public const int ID_deff = 280;

		public const int ID_langfe = 281;

		public const int ID_bgdkcross = 282;

		public const int ID_nonesttables = 283;

		public const int ID_fi = 284;

		public const int ID_chcbpat = 285;

		public const int ID_rquote = 286;

		public const int ID_rtlch = 287;

		public const int ID_pndbnum = 288;

		public const int ID_brdrsh = 289;

		public const int ID_pnaiu = 290;

		public const int ID_ai = 291;

		public const int ID_fromhtml = 292;

		public const int ID_trhdr = 293;

		public const int ID_ulhair = 294;

		public const int ID_emdash = 295;

		public const int ID_levelnfc = 296;

		public const int ID_deleted = 297;

		public const int ID_dropcapli = 298;

		public const int ID_ulth = 299;

		public const int ID_mhtmltag = 300;

		public const int ID_pmmetafile = 301;

		public const int ID_impr = 302;

		public const int ID_pca = 303;

		public const int ID_pnirohad = 304;

		public const int ID_cs = 305;

		public const int ID_fldinst = 306;

		public const int ID_ab = 307;

		public const int ID_fswiss = 308;

		public const int ID_green = 309;

		public const int ID_ulthd = 310;

		public const int ID_ulc = 311;

		public const int ID_af = 312;

		public const int ID_uld = 313;

		public const int ID_bgcross = 314;

		public const int ID_stylesheet = 315;

		public const int ID_listoverridetable = 316;

		public const int ID_background = 317;

		public const int ID_clcbpat = 318;

		public const int ID_pntext = 319;

		public const int ID_trftsWidth = 320;

		public const int ID_brdrhair = 321;

		public const int ID_pnlvlbody = 322;

		public const int ID_super = 323;

		public const int ID_objdata = 324;

		public const int ID_ansi = 325;

		public const int ID_ulthdashdd = 326;

		public const int ID_ulp = 327;

		public const int ID_brdrdot = 328;

		public const int ID_fromtext = 329;

		public const int ID_brdremboss = 330;

		public const int ID_cf = 331;

		public static short[] keywordHashTable = new short[]
		{
			4,
			0,
			6,
			0,
			0,
			8,
			0,
			0,
			10,
			11,
			0,
			0,
			12,
			0,
			0,
			0,
			13,
			0,
			14,
			17,
			20,
			23,
			25,
			26,
			29,
			32,
			34,
			0,
			37,
			38,
			42,
			44,
			0,
			46,
			47,
			49,
			52,
			56,
			57,
			58,
			59,
			60,
			61,
			62,
			63,
			0,
			0,
			0,
			0,
			64,
			67,
			0,
			0,
			68,
			69,
			70,
			0,
			71,
			0,
			0,
			0,
			0,
			0,
			0,
			72,
			0,
			74,
			0,
			76,
			77,
			78,
			79,
			80,
			81,
			82,
			83,
			85,
			0,
			0,
			0,
			86,
			87,
			0,
			0,
			88,
			92,
			93,
			94,
			95,
			0,
			97,
			100,
			102,
			103,
			0,
			105,
			107,
			108,
			109,
			0,
			112,
			0,
			113,
			0,
			114,
			115,
			117,
			0,
			119,
			122,
			123,
			125,
			128,
			0,
			0,
			129,
			130,
			132,
			134,
			137,
			0,
			140,
			141,
			145,
			147,
			149,
			0,
			0,
			0,
			151,
			0,
			152,
			0,
			155,
			0,
			156,
			157,
			158,
			160,
			162,
			163,
			0,
			164,
			165,
			167,
			168,
			169,
			170,
			171,
			174,
			0,
			177,
			178,
			0,
			179,
			180,
			181,
			0,
			182,
			183,
			0,
			184,
			186,
			0,
			187,
			191,
			0,
			0,
			192,
			0,
			194,
			0,
			196,
			199,
			201,
			0,
			204,
			205,
			206,
			0,
			208,
			209,
			210,
			212,
			0,
			213,
			214,
			216,
			218,
			0,
			0,
			220,
			0,
			222,
			226,
			228,
			230,
			232,
			234,
			235,
			0,
			236,
			0,
			0,
			238,
			0,
			240,
			243,
			0,
			0,
			244,
			0,
			247,
			249,
			250,
			251,
			252,
			0,
			253,
			0,
			0,
			0,
			254,
			256,
			0,
			0,
			258,
			259,
			0,
			260,
			263,
			0,
			0,
			0,
			264,
			0,
			265,
			0,
			0,
			0,
			0,
			266,
			268,
			0,
			271,
			272,
			273,
			0,
			0,
			275,
			276,
			0,
			277,
			0,
			280,
			0,
			282,
			0,
			284,
			285,
			286,
			288,
			289,
			290,
			0,
			0,
			291,
			295,
			0,
			296,
			297,
			298,
			300,
			301,
			302,
			0,
			305,
			307,
			309,
			311,
			0,
			312,
			313,
			0,
			314,
			316,
			317,
			319,
			320,
			321,
			324,
			325,
			0,
			326,
			327,
			328,
			329,
			330,
			0
		};

		public static RTFData.KeyDef[] keywords = new RTFData.KeyDef[]
		{
			new RTFData.KeyDef(-1, 0, '\0', 0, false, "null"),
			new RTFData.KeyDef(-1, 0, '\0', 0, true, "null"),
			new RTFData.KeyDef(-1, 0, '\0', 0, false, "null"),
			new RTFData.KeyDef(-1, 0, '\0', 0, false, "null"),
			new RTFData.KeyDef(0, 0, '\0', 0, false, "aul"),
			new RTFData.KeyDef(0, 2, '\0', -1, false, "ulw"),
			new RTFData.KeyDef(2, 0, '\0', 0, false, "pichgoal"),
			new RTFData.KeyDef(2, 7, '\0', 0, false, "trbrdrb"),
			new RTFData.KeyDef(5, 0, '\0', 0, false, "leveltext"),
			new RTFData.KeyDef(5, 0, '\0', 0, false, "listlevel"),
			new RTFData.KeyDef(8, 8, '\0', 0, false, "trbrdrh"),
			new RTFData.KeyDef(9, 5, '\0', 0, false, "brdrengrave"),
			new RTFData.KeyDef(12, 4, '\0', 0, false, "trbrdrl"),
			new RTFData.KeyDef(16, 0, '\0', 0, false, "irow"),
			new RTFData.KeyDef(18, 4, '\0', 0, false, "brdrtriple"),
			new RTFData.KeyDef(18, 0, '\0', 0, false, "footer"),
			new RTFData.KeyDef(18, 6, '\0', 0, false, "trbrdrr"),
			new RTFData.KeyDef(19, 0, '\0', -1, false, "caps"),
			new RTFData.KeyDef(19, 4, '\0', 0, true, "fscript"),
			new RTFData.KeyDef(19, 5, '\0', -1, false, "uldash"),
			new RTFData.KeyDef(20, 0, '\0', 0, false, "expndtw"),
			new RTFData.KeyDef(20, 6, '\0', 0, false, "pnucrm"),
			new RTFData.KeyDef(20, 5, '\0', 0, false, "trbrdrt"),
			new RTFData.KeyDef(21, 4, '\0', 0, false, "brdrwavydb"),
			new RTFData.KeyDef(21, 0, '\0', 0, false, "header"),
			new RTFData.KeyDef(22, 9, '\0', 0, false, "trbrdrv"),
			new RTFData.KeyDef(23, 0, '\0', -1, false, "embo"),
			new RTFData.KeyDef(23, 0, '\0', 0, false, "pnindent"),
			new RTFData.KeyDef(23, 0, '‍', 0, false, "zwj"),
			new RTFData.KeyDef(24, 0, '\0', 0, false, "field"),
			new RTFData.KeyDef(24, 0, '\0', 0, true, "fnil"),
			new RTFData.KeyDef(24, 0, '\0', -1, false, "link"),
			new RTFData.KeyDef(25, 0, '\0', -1, false, "disabled"),
			new RTFData.KeyDef(25, 0, '\0', 0, false, "footnote"),
			new RTFData.KeyDef(26, 0, '\0', 0, true, "fcharset"),
			new RTFData.KeyDef(26, 0, '\0', 0, true, "mac"),
			new RTFData.KeyDef(26, 4, '\0', 0, false, "pnucltr"),
			new RTFData.KeyDef(28, 0, '\0', 0, true, "fbidis"),
			new RTFData.KeyDef(29, 0, '‘', 0, false, "lquote"),
			new RTFData.KeyDef(29, 0, '\0', 0, false, "macpict"),
			new RTFData.KeyDef(29, 0, '\0', 0, false, "row"),
			new RTFData.KeyDef(29, 1, '\0', 0, false, "rtlrow"),
			new RTFData.KeyDef(30, 0, '\0', 0, true, "fprq"),
			new RTFData.KeyDef(30, 0, '\0', 0, false, "picprop"),
			new RTFData.KeyDef(31, 0, '\0', 0, false, "levelstartat"),
			new RTFData.KeyDef(31, 0, '\0', 0, false, "pich"),
			new RTFData.KeyDef(33, 3, '\0', 0, false, "brdrwavy"),
			new RTFData.KeyDef(34, 0, '\0', 0, true, "bin"),
			new RTFData.KeyDef(34, 0, '\0', 0, false, "line"),
			new RTFData.KeyDef(35, 3, '\0', 0, true, "fmodern"),
			new RTFData.KeyDef(35, 0, '\0', 0, false, "pict"),
			new RTFData.KeyDef(35, 1, '\0', 0, false, "pnlvlblt"),
			new RTFData.KeyDef(36, 2, '\0', 0, false, "brdrdashsm"),
			new RTFData.KeyDef(36, 0, '\0', 0, false, "clcfpat"),
			new RTFData.KeyDef(36, 0, '\0', 0, false, "list"),
			new RTFData.KeyDef(36, 0, '\0', 0, false, "nestrow"),
			new RTFData.KeyDef(37, 0, '\0', 0, false, "brdrbtw"),
			new RTFData.KeyDef(38, 0, '\0', 0, false, "picw"),
			new RTFData.KeyDef(39, 0, '\0', 0, false, "cbpat"),
			new RTFData.KeyDef(40, 0, '‎', 0, false, "rtlmark"),
			new RTFData.KeyDef(41, 0, '\0', 0, true, "deflang"),
			new RTFData.KeyDef(42, 12, '\0', -1, false, "ulhwave"),
			new RTFData.KeyDef(43, 0, '\0', 0, false, "pnaiud"),
			new RTFData.KeyDef(44, 13, '\0', -1, false, "ulldash"),
			new RTFData.KeyDef(49, 1, '\0', 0, false, "brdrdashdotstr"),
			new RTFData.KeyDef(49, 0, '\0', 0, false, "nesttableprops"),
			new RTFData.KeyDef(49, 0, '\0', 0, false, "trleft"),
			new RTFData.KeyDef(50, 2, '\0', 0, false, "bghoriz"),
			new RTFData.KeyDef(53, 0, '\0', 0, false, "par"),
			new RTFData.KeyDef(54, 0, '\0', 0, false, "keepn"),
			new RTFData.KeyDef(55, 0, '\0', 0, false, "pnordt"),
			new RTFData.KeyDef(57, 0, '\0', 0, true, "lang"),
			new RTFData.KeyDef(64, 6, '\0', 0, true, "fbidi"),
			new RTFData.KeyDef(64, 0, '\0', 0, false, "lastrow"),
			new RTFData.KeyDef(66, 0, '•', 0, false, "bullet"),
			new RTFData.KeyDef(66, 0, '\0', 0, false, "sectd"),
			new RTFData.KeyDef(68, 1, '\0', -1, false, "ul"),
			new RTFData.KeyDef(69, 3, '\0', 0, false, "pnlcltr"),
			new RTFData.KeyDef(70, 0, '\0', 0, false, "clvmrg"),
			new RTFData.KeyDef(71, 0, '\0', -1, false, "shad"),
			new RTFData.KeyDef(72, 2, '\0', 0, false, "brdrdash"),
			new RTFData.KeyDef(73, 0, '\0', 1, true, "uc"),
			new RTFData.KeyDef(74, 0, '\0', 0, false, "highlight"),
			new RTFData.KeyDef(75, 0, '\0', 0, false, "htmlbase"),
			new RTFData.KeyDef(75, 0, '\0', 0, false, "pncnum"),
			new RTFData.KeyDef(76, 0, '\0', 0, false, "ud"),
			new RTFData.KeyDef(80, 0, '\0', 0, false, "pnstart"),
			new RTFData.KeyDef(81, 0, '\0', 0, true, "adeff"),
			new RTFData.KeyDef(84, 0, '\0', 0, false, "blue"),
			new RTFData.KeyDef(84, 4, '\0', 0, false, "brdrdb"),
			new RTFData.KeyDef(84, 4, '\0', 0, false, "brdrframe"),
			new RTFData.KeyDef(84, 0, '\0', 0, false, "taprtl"),
			new RTFData.KeyDef(85, 0, '\0', 0, false, "comment"),
			new RTFData.KeyDef(86, 1, '\0', 0, true, "froman"),
			new RTFData.KeyDef(87, 5, '\0', 0, true, "fdecor"),
			new RTFData.KeyDef(88, 4, '\0', 0, true, "dbch"),
			new RTFData.KeyDef(88, 1, '\0', 0, false, "up"),
			new RTFData.KeyDef(90, 8, '\0', 0, false, "brdroutset"),
			new RTFData.KeyDef(90, 2, '\0', 0, false, "clvertalb"),
			new RTFData.KeyDef(90, 0, '\0', -1, false, "striked"),
			new RTFData.KeyDef(91, 1, '\0', 0, false, "clvertalc"),
			new RTFData.KeyDef(91, 0, '\0', 1, false, "itap"),
			new RTFData.KeyDef(92, 0, '\0', 0, false, "pnlvl"),
			new RTFData.KeyDef(93, 0, '\0', 0, false, "clftsWidth"),
			new RTFData.KeyDef(93, 0, '\0', 0, false, "upr"),
			new RTFData.KeyDef(95, 0, '\0', 0, false, "nestcell"),
			new RTFData.KeyDef(95, 0, '\0', 0, false, "pnord"),
			new RTFData.KeyDef(96, 0, '\0', -1, false, "protect"),
			new RTFData.KeyDef(97, 0, '\0', 0, true, "pc"),
			new RTFData.KeyDef(98, 0, '\0', -1, true, "b"),
			new RTFData.KeyDef(98, 0, '\0', 0, false, "deftab"),
			new RTFData.KeyDef(98, 3, '\0', 0, false, "qj"),
			new RTFData.KeyDef(100, 0, '\0', 0, false, "ql"),
			new RTFData.KeyDef(102, 0, '\0', -1, true, "f"),
			new RTFData.KeyDef(104, 0, '\0', 0, false, "sp"),
			new RTFData.KeyDef(105, 0, '\0', -1, true, "i"),
			new RTFData.KeyDef(105, 1, '\0', 0, false, "qc"),
			new RTFData.KeyDef(106, 0, '\0', -1, false, "revised"),
			new RTFData.KeyDef(106, 0, '\0', 0, false, "trpaddr"),
			new RTFData.KeyDef(108, 0, '\0', 0, false, "cell"),
			new RTFData.KeyDef(108, 4, '\0', 0, false, "qd"),
			new RTFData.KeyDef(108, 0, '\0', 0, false, "trpaddt"),
			new RTFData.KeyDef(109, 4, '\0', 0, false, "brdrthtnlg"),
			new RTFData.KeyDef(110, 0, '\0', 0, false, "pn"),
			new RTFData.KeyDef(110, 0, '\0', 0, false, "sv"),
			new RTFData.KeyDef(111, 0, '\0', 0, false, "brsp"),
			new RTFData.KeyDef(111, 0, '\0', 0, false, "tab"),
			new RTFData.KeyDef(111, 0, '\0', 0, false, "trgaph"),
			new RTFData.KeyDef(112, 0, '\0', 0, false, "shp"),
			new RTFData.KeyDef(115, 0, '\0', 0, false, "s"),
			new RTFData.KeyDef(116, 0, '\0', 0, false, "sl"),
			new RTFData.KeyDef(116, 0, '\0', 0, false, "trpaddl"),
			new RTFData.KeyDef(117, 4, '\0', 0, false, "brdrthtnmg"),
			new RTFData.KeyDef(117, 0, '\0', 0, true, "u"),
			new RTFData.KeyDef(118, 0, '\0', 0, true, "ltrch"),
			new RTFData.KeyDef(118, 0, '\0', 0, false, "sn"),
			new RTFData.KeyDef(118, 0, '\0', -1, false, "v"),
			new RTFData.KeyDef(119, 3, '\0', 0, true, "hich"),
			new RTFData.KeyDef(119, 0, '\0', 0, false, "ri"),
			new RTFData.KeyDef(119, 0, '\0', 0, false, "sa"),
			new RTFData.KeyDef(121, 5, '\0', 0, false, "qs"),
			new RTFData.KeyDef(122, 2, '\0', 0, false, "qr"),
			new RTFData.KeyDef(122, 0, '\0', 0, false, "sb"),
			new RTFData.KeyDef(122, 0, '\0', 0, false, "trcbpat"),
			new RTFData.KeyDef(122, 0, '\0', 0, false, "trpaddb"),
			new RTFData.KeyDef(123, 0, '\0', 0, false, "cfpat"),
			new RTFData.KeyDef(123, 0, '\0', 0, false, "keep"),
			new RTFData.KeyDef(124, 1, '\0', 0, false, "bgvert"),
			new RTFData.KeyDef(124, 2, '\0', 0, false, "red"),
			new RTFData.KeyDef(125, 0, '\0', 0, true, "deflangfe"),
			new RTFData.KeyDef(125, 11, '\0', -1, false, "ululdbwave"),
			new RTFData.KeyDef(129, 0, '\0', 0, false, "trrh"),
			new RTFData.KeyDef(131, 0, '\0', 0, false, "_hyphen"),
			new RTFData.KeyDef(131, 0, '\0', 1, false, "htmlrtf"),
			new RTFData.KeyDef(131, 0, '\0', 0, false, "picwgoal"),
			new RTFData.KeyDef(133, 7, '\0', -1, false, "uldashdd"),
			new RTFData.KeyDef(135, 4, '\0', 0, false, "brdrtnthsg"),
			new RTFData.KeyDef(136, 0, '\0', 0, false, "objattph"),
			new RTFData.KeyDef(137, 9, '\0', 0, false, "bgdkbdiag"),
			new RTFData.KeyDef(137, 3, '\0', -1, false, "uldb"),
			new RTFData.KeyDef(138, 0, '\0', 0, false, "clmrg"),
			new RTFData.KeyDef(138, 0, '\0', 0, false, "clpadr"),
			new RTFData.KeyDef(139, 0, '\0', -1, false, "outl"),
			new RTFData.KeyDef(140, 0, '\0', 0, false, "clpadt"),
			new RTFData.KeyDef(142, 0, '\0', 0, true, "fcs"),
			new RTFData.KeyDef(143, 0, '\0', 0, true, "ansicpg"),
			new RTFData.KeyDef(143, 0, '\0', 0, false, "shpinst"),
			new RTFData.KeyDef(144, 0, '\0', 0, false, "brdrcf"),
			new RTFData.KeyDef(145, 0, '\0', 0, false, "sect"),
			new RTFData.KeyDef(146, 0, '\0', 0, true, "afs"),
			new RTFData.KeyDef(147, 0, '\0', 0, true, "plain"),
			new RTFData.KeyDef(148, 7, '\0', 0, false, "brdrinset"),
			new RTFData.KeyDef(148, 0, '\0', 0, false, "clpadl"),
			new RTFData.KeyDef(148, 0, '\0', 0, false, "listid"),
			new RTFData.KeyDef(149, 0, '\0', 0, false, "acf"),
			new RTFData.KeyDef(149, 0, '\0', 0, true, "fonttbl"),
			new RTFData.KeyDef(149, 1, '\0', 0, false, "rtlpar"),
			new RTFData.KeyDef(151, 0, '\0', 0, false, "htmltag"),
			new RTFData.KeyDef(152, 15, '\0', -1, false, "ulthdashd"),
			new RTFData.KeyDef(154, 0, '\0', 0, false, "clpadb"),
			new RTFData.KeyDef(155, 0, '\0', -1, false, "scaps"),
			new RTFData.KeyDef(156, 12, '\0', 0, false, "clbrdrr"),
			new RTFData.KeyDef(158, 0, '\0', 0, false, "shading"),
			new RTFData.KeyDef(159, 2, '\0', 0, false, "trqr"),
			new RTFData.KeyDef(161, 0, '\0', 0, false, "pard"),
			new RTFData.KeyDef(161, 0, '\0', 0, true, "pnfs"),
			new RTFData.KeyDef(162, 11, '\0', 0, false, "clbrdrt"),
			new RTFData.KeyDef(164, 4, '\0', 0, false, "bgdkvert"),
			new RTFData.KeyDef(164, 0, '\0', 0, false, "brdrl"),
			new RTFData.KeyDef(164, 0, '‏', 0, false, "ltrmark"),
			new RTFData.KeyDef(164, 18, '\0', -1, false, "ulthldash"),
			new RTFData.KeyDef(165, 0, '\0', 0, false, "intbl"),
			new RTFData.KeyDef(168, 12, '\0', 0, false, "bgbdiag"),
			new RTFData.KeyDef(168, 0, '\0', 0, false, "bkmkstart"),
			new RTFData.KeyDef(170, 3, '\0', 0, false, "brdrb"),
			new RTFData.KeyDef(170, 10, '\0', 0, false, "clbrdrl"),
			new RTFData.KeyDef(172, 13, '\0', 0, false, "clbrdrb"),
			new RTFData.KeyDef(172, 0, '\0', 0, false, "pagebb"),
			new RTFData.KeyDef(172, 0, '\0', -1, false, "strike"),
			new RTFData.KeyDef(173, 0, '\0', 0, false, "clvmgf"),
			new RTFData.KeyDef(173, 0, '\0', 0, false, "trowd"),
			new RTFData.KeyDef(174, 0, '\0', 0, false, "info"),
			new RTFData.KeyDef(174, 0, '“', 0, false, "ldblquote"),
			new RTFData.KeyDef(174, 0, '\0', 0, false, "listoverride"),
			new RTFData.KeyDef(176, 1, '\0', 0, false, "trqc"),
			new RTFData.KeyDef(177, 0, '\0', 0, false, "ilvl"),
			new RTFData.KeyDef(178, 0, '\0', 0, false, "li"),
			new RTFData.KeyDef(178, 0, '‌', 0, false, "zwnj"),
			new RTFData.KeyDef(180, 0, '\0', 0, false, "listsimple"),
			new RTFData.KeyDef(181, 2, '\0', 0, false, "brdrdashdd"),
			new RTFData.KeyDef(182, 0, '\0', 0, true, "fname"),
			new RTFData.KeyDef(182, 0, '\0', 0, false, "ulnone"),
			new RTFData.KeyDef(183, 0, '\0', 0, false, "bkmkend"),
			new RTFData.KeyDef(185, 0, '\0', 0, false, "clwWidth"),
			new RTFData.KeyDef(186, 0, '\0', 0, true, "adeflang"),
			new RTFData.KeyDef(186, 2, '\0', 0, false, "brdrr"),
			new RTFData.KeyDef(187, 3, '\0', 0, false, "brdrs"),
			new RTFData.KeyDef(187, 0, '\0', 0, false, "pniroha"),
			new RTFData.KeyDef(188, 1, '\0', 0, false, "brdrt"),
			new RTFData.KeyDef(188, 0, '\0', 0, false, "ls"),
			new RTFData.KeyDef(191, 0, '\0', 0, false, "brdrw"),
			new RTFData.KeyDef(191, 0, '\0', 0, false, "irowband"),
			new RTFData.KeyDef(193, 2, '\0', 0, true, "loch"),
			new RTFData.KeyDef(193, 0, '\0', -1, true, "pnf"),
			new RTFData.KeyDef(193, 0, '\0', 0, false, "slmult"),
			new RTFData.KeyDef(193, 14, '\0', -1, false, "ulthdash"),
			new RTFData.KeyDef(194, 7, '\0', 0, false, "bgdkdcross"),
			new RTFData.KeyDef(194, 0, '\0', 0, false, "pntxta"),
			new RTFData.KeyDef(195, 0, '\0', 0, false, "cellx"),
			new RTFData.KeyDef(195, 0, '\0', 0, false, "pnlvlcont"),
			new RTFData.KeyDef(196, 0, '\0', 0, false, "headerl"),
			new RTFData.KeyDef(196, 0, '\0', 0, false, "jclisttab"),
			new RTFData.KeyDef(197, 0, '–', 0, false, "endash"),
			new RTFData.KeyDef(197, 0, '\0', 0, false, "pntxtb"),
			new RTFData.KeyDef(198, 3, '\0', 0, false, "bgfdiag"),
			new RTFData.KeyDef(199, 0, '\0', 0, false, "brdrbar"),
			new RTFData.KeyDef(201, 6, '\0', 0, false, "bgdkfdiag"),
			new RTFData.KeyDef(201, 0, '\0', 0, false, "trwWidth"),
			new RTFData.KeyDef(204, 0, '\0', 0, true, "alang"),
			new RTFData.KeyDef(204, 4, '\0', 0, false, "brdrtnthtnlg"),
			new RTFData.KeyDef(206, 0, '\0', 0, true, "cpg"),
			new RTFData.KeyDef(206, 0, '\0', 0, false, "headerf"),
			new RTFData.KeyDef(206, 0, '\0', 0, false, "ltrrow"),
			new RTFData.KeyDef(207, 4, '\0', 0, false, "brdrtnthtnsg"),
			new RTFData.KeyDef(210, 5, '\0', 0, false, "bgdkhoriz"),
			new RTFData.KeyDef(210, 0, '\0', 0, false, "dropcapt"),
			new RTFData.KeyDef(210, 0, '\0', 0, false, "listtable"),
			new RTFData.KeyDef(212, 4, '\0', 0, false, "brdrtnthtnmg"),
			new RTFData.KeyDef(212, 5, '\0', 0, false, "pnlcrm"),
			new RTFData.KeyDef(213, 0, '\0', 0, false, "pndecd"),
			new RTFData.KeyDef(214, 0, '\0', 0, false, "leveljc"),
			new RTFData.KeyDef(215, 4, '\0', 0, false, "brdrtnthmg"),
			new RTFData.KeyDef(216, 0, '\0', 0, false, "colortbl"),
			new RTFData.KeyDef(218, 0, '\0', 0, false, "headerr"),
			new RTFData.KeyDef(222, 3, '\0', 0, false, "brdrth"),
			new RTFData.KeyDef(222, 0, '\0', 0, false, "levelpicture"),
			new RTFData.KeyDef(223, 4, '\0', 0, false, "brdrtnthlg"),
			new RTFData.KeyDef(223, 0, '\0', 0, false, "listtext"),
			new RTFData.KeyDef(226, 0, '\0', 0, false, "footerr"),
			new RTFData.KeyDef(227, 0, '\0', 0, false, "clmgf"),
			new RTFData.KeyDef(229, 10, '\0', 0, false, "bgdcross"),
			new RTFData.KeyDef(229, 4, '\0', 0, false, "brdrthtnsg"),
			new RTFData.KeyDef(229, 0, '\0', -1, false, "sub"),
			new RTFData.KeyDef(230, 0, '”', 0, false, "rdblquote"),
			new RTFData.KeyDef(234, 0, '\0', 0, false, "nosupersub"),
			new RTFData.KeyDef(236, 0, '\0', 0, true, "fs"),
			new RTFData.KeyDef(241, 7, '\0', 0, true, "ftech"),
			new RTFData.KeyDef(241, 0, '\0', 0, true, "rtf"),
			new RTFData.KeyDef(242, 0, '\0', 0, true, "falt"),
			new RTFData.KeyDef(242, 0, '\0', 0, false, "fldrslt"),
			new RTFData.KeyDef(242, 0, '\0', 0, false, "ltrpar"),
			new RTFData.KeyDef(244, 0, '\0', 0, true, "urtf"),
			new RTFData.KeyDef(245, 0, '\0', 0, false, "pncard"),
			new RTFData.KeyDef(246, 0, '\0', 0, false, "footerf"),
			new RTFData.KeyDef(246, 2, '\0', 0, false, "pndec"),
			new RTFData.KeyDef(249, -1, '\0', 0, false, "dn"),
			new RTFData.KeyDef(250, 2, '\0', 0, false, "brdrdashd"),
			new RTFData.KeyDef(252, 0, '\0', 0, false, "footerl"),
			new RTFData.KeyDef(252, 6, '\0', -1, false, "uldashd"),
			new RTFData.KeyDef(252, 8, '\0', -1, false, "ulwave"),
			new RTFData.KeyDef(254, 0, '\0', 0, true, "deff"),
			new RTFData.KeyDef(254, 0, '\0', 0, true, "langfe"),
			new RTFData.KeyDef(256, 8, '\0', 0, false, "bgdkcross"),
			new RTFData.KeyDef(256, 0, '\0', 0, false, "nonesttables"),
			new RTFData.KeyDef(258, 0, '\0', 0, false, "fi"),
			new RTFData.KeyDef(259, 0, '\0', 0, false, "chcbpat"),
			new RTFData.KeyDef(260, 0, '’', 0, false, "rquote"),
			new RTFData.KeyDef(260, 1, '\0', 0, true, "rtlch"),
			new RTFData.KeyDef(261, 0, '\0', 0, false, "pndbnum"),
			new RTFData.KeyDef(262, 3, '\0', 0, false, "brdrsh"),
			new RTFData.KeyDef(263, 0, '\0', 0, false, "pnaiu"),
			new RTFData.KeyDef(266, 0, '\0', -1, true, "ai"),
			new RTFData.KeyDef(266, 0, '\0', 0, true, "fromhtml"),
			new RTFData.KeyDef(266, 0, '\0', 0, false, "trhdr"),
			new RTFData.KeyDef(266, 10, '\0', -1, false, "ulhair"),
			new RTFData.KeyDef(267, 0, '—', 0, false, "emdash"),
			new RTFData.KeyDef(269, 0, '\0', 0, false, "levelnfc"),
			new RTFData.KeyDef(270, 0, '\0', -1, false, "deleted"),
			new RTFData.KeyDef(271, 0, '\0', 0, false, "dropcapli"),
			new RTFData.KeyDef(271, 9, '\0', -1, false, "ulth"),
			new RTFData.KeyDef(272, 0, '\0', 0, false, "mhtmltag"),
			new RTFData.KeyDef(273, 0, '\0', 0, false, "pmmetafile"),
			new RTFData.KeyDef(274, 0, '\0', -1, false, "impr"),
			new RTFData.KeyDef(274, 0, '\0', 0, true, "pca"),
			new RTFData.KeyDef(274, 0, '\0', 0, false, "pnirohad"),
			new RTFData.KeyDef(276, 0, '\0', 0, false, "cs"),
			new RTFData.KeyDef(276, 0, '\0', 0, false, "fldinst"),
			new RTFData.KeyDef(277, 0, '\0', -1, true, "ab"),
			new RTFData.KeyDef(277, 2, '\0', 0, true, "fswiss"),
			new RTFData.KeyDef(278, 1, '\0', 0, false, "green"),
			new RTFData.KeyDef(278, 17, '\0', -1, false, "ulthd"),
			new RTFData.KeyDef(279, 0, '\0', 0, false, "ulc"),
			new RTFData.KeyDef(281, 0, '\0', -1, true, "af"),
			new RTFData.KeyDef(282, 4, '\0', -1, false, "uld"),
			new RTFData.KeyDef(284, 11, '\0', 0, false, "bgcross"),
			new RTFData.KeyDef(284, 0, '\0', 0, false, "stylesheet"),
			new RTFData.KeyDef(285, 0, '\0', 0, false, "listoverridetable"),
			new RTFData.KeyDef(286, 0, '\0', 0, false, "background"),
			new RTFData.KeyDef(286, 0, '\0', 0, false, "clcbpat"),
			new RTFData.KeyDef(287, 0, '\0', 0, false, "pntext"),
			new RTFData.KeyDef(288, 0, '\0', 0, false, "trftsWidth"),
			new RTFData.KeyDef(289, 3, '\0', 0, false, "brdrhair"),
			new RTFData.KeyDef(289, 0, '\0', 0, false, "pnlvlbody"),
			new RTFData.KeyDef(289, 0, '\0', -1, false, "super"),
			new RTFData.KeyDef(290, 0, '\0', 0, false, "objdata"),
			new RTFData.KeyDef(291, 0, '\0', 0, true, "ansi"),
			new RTFData.KeyDef(293, 16, '\0', -1, false, "ulthdashdd"),
			new RTFData.KeyDef(294, 0, '\0', 0, false, "ulp"),
			new RTFData.KeyDef(295, 1, '\0', 0, false, "brdrdot"),
			new RTFData.KeyDef(296, 0, '\0', 0, true, "fromtext"),
			new RTFData.KeyDef(297, 6, '\0', 0, false, "brdremboss"),
			new RTFData.KeyDef(297, 0, '\0', 0, false, "cf")
		};

		public static int[] allowedCodePages = new int[]
		{
			28591,
			437,
			708,
			720,
			850,
			852,
			860,
			862,
			863,
			864,
			865,
			866,
			874,
			932,
			936,
			949,
			950,
			1250,
			1251,
			1252,
			1253,
			1254,
			1255,
			1256,
			1257,
			1258,
			1361,
			10001,
			10002,
			10003,
			10008,
			65001
		};

		public struct KeyDef
		{
			public KeyDef(short hash, short idx, char character, short defaultValue, bool affectsParsing, string name)
			{
				this.hash = hash;
				this.idx = idx;
				this.character = character;
				this.defaultValue = defaultValue;
				this.affectsParsing = affectsParsing;
				this.name = name;
			}

			public short hash;

			public short idx;

			public char character;

			public short defaultValue;

			public bool affectsParsing;

			public string name;
		}
	}
}
