using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	internal static class Countries
	{
		static Countries()
		{
			Countries.stringIDs.Add(3216539067U, "BT_64_34");
			Countries.stringIDs.Add(1699208365U, "DZ_12_4");
			Countries.stringIDs.Add(886545238U, "KI_296_133");
			Countries.stringIDs.Add(1759591854U, "LS_426_146");
			Countries.stringIDs.Add(2344949245U, "TC_796_349");
			Countries.stringIDs.Add(53559490U, "LI_438_145");
			Countries.stringIDs.Add(170381786U, "HR_191_108");
			Countries.stringIDs.Add(1977258562U, "VC_670_248");
			Countries.stringIDs.Add(74285194U, "AN_530_333");
			Countries.stringIDs.Add(1999176122U, "JO_400_126");
			Countries.stringIDs.Add(2368586127U, "PM_666_206");
			Countries.stringIDs.Add(1498891191U, "RU_643_203");
			Countries.stringIDs.Add(3794883283U, "DE_276_94");
			Countries.stringIDs.Add(2103499475U, "PY_600_185");
			Countries.stringIDs.Add(2019047508U, "KE_404_129");
			Countries.stringIDs.Add(1082660990U, "CN_156_45");
			Countries.stringIDs.Add(2512148763U, "CL_152_46");
			Countries.stringIDs.Add(1330749269U, "TG_768_232");
			Countries.stringIDs.Add(2005956254U, "BR_76_32");
			Countries.stringIDs.Add(1592266550U, "IS_352_110");
			Countries.stringIDs.Add(1174698536U, "ST_678_233");
			Countries.stringIDs.Add(377801761U, "SA_682_205");
			Countries.stringIDs.Add(3159776897U, "ZW_716_264");
			Countries.stringIDs.Add(12936108U, "UZ_860_247");
			Countries.stringIDs.Add(3194236654U, "MA_504_159");
			Countries.stringIDs.Add(1422904081U, "GB_826_242");
			Countries.stringIDs.Add(2764932561U, "SS_728_0");
			Countries.stringIDs.Add(1200282230U, "VE_862_249");
			Countries.stringIDs.Add(1037502137U, "AO_24_9");
			Countries.stringIDs.Add(2125592240U, "CC_166_311");
			Countries.stringIDs.Add(621889182U, "CM_120_49");
			Countries.stringIDs.Add(999565292U, "CY_196_59");
			Countries.stringIDs.Add(373651598U, "NU_570_335");
			Countries.stringIDs.Add(55331012U, "TR_792_235");
			Countries.stringIDs.Add(645986668U, "KG_417_130");
			Countries.stringIDs.Add(702227408U, "SL_694_213");
			Countries.stringIDs.Add(3602902706U, "BG_100_35");
			Countries.stringIDs.Add(3029734706U, "LT_440_141");
			Countries.stringIDs.Add(2562573117U, "TK_772_347");
			Countries.stringIDs.Add(1004246475U, "GF_254_317");
			Countries.stringIDs.Add(4002790908U, "IL_376_117");
			Countries.stringIDs.Add(3087470301U, "MH_584_199");
			Countries.stringIDs.Add(56143679U, "NP_524_178");
			Countries.stringIDs.Add(4104315101U, "BI_108_38");
			Countries.stringIDs.Add(636004392U, "HT_332_103");
			Countries.stringIDs.Add(2620723795U, "JE_832_328");
			Countries.stringIDs.Add(1441347144U, "VU_548_174");
			Countries.stringIDs.Add(2320250384U, "BY_112_29");
			Countries.stringIDs.Add(3646896638U, "QA_634_197");
			Countries.stringIDs.Add(2144138889U, "UY_858_246");
			Countries.stringIDs.Add(3789356486U, "GE_268_88");
			Countries.stringIDs.Add(2818474250U, "BW_72_19");
			Countries.stringIDs.Add(1718335698U, "NZ_554_183");
			Countries.stringIDs.Add(151849765U, "GU_316_322");
			Countries.stringIDs.Add(1843367414U, "BJ_204_28");
			Countries.stringIDs.Add(4222597191U, "AM_51_7");
			Countries.stringIDs.Add(1017460496U, "LA_418_138");
			Countries.stringIDs.Add(2132819958U, "LC_662_218");
			Countries.stringIDs.Add(3469746324U, "GL_304_93");
			Countries.stringIDs.Add(46288124U, "GW_624_196");
			Countries.stringIDs.Add(359940844U, "WF_876_352");
			Countries.stringIDs.Add(673912308U, "HN_340_106");
			Countries.stringIDs.Add(3393077234U, "BD_50_23");
			Countries.stringIDs.Add(3346454225U, "MO_446_151");
			Countries.stringIDs.Add(471386291U, "AX_248_0");
			Countries.stringIDs.Add(1333137622U, "XX_581_329");
			Countries.stringIDs.Add(4169766046U, "KZ_398_137");
			Countries.stringIDs.Add(3768655288U, "SJ_744_125");
			Countries.stringIDs.Add(1626681835U, "UA_804_241");
			Countries.stringIDs.Add(1734702041U, "SY_760_222");
			Countries.stringIDs.Add(1952734284U, "IO_86_114");
			Countries.stringIDs.Add(2248145120U, "GT_320_99");
			Countries.stringIDs.Add(1528190210U, "ER_232_71");
			Countries.stringIDs.Add(1633154011U, "BO_68_26");
			Countries.stringIDs.Add(19448000U, "BV_74_306");
			Countries.stringIDs.Add(1476028295U, "FJ_242_78");
			Countries.stringIDs.Add(803870055U, "MP_580_337");
			Countries.stringIDs.Add(2821667208U, "HU_348_109");
			Countries.stringIDs.Add(1635698251U, "SD_736_219");
			Countries.stringIDs.Add(987373943U, "LY_434_148");
			Countries.stringIDs.Add(940316003U, "JM_388_124");
			Countries.stringIDs.Add(1798672883U, "TV_798_236");
			Countries.stringIDs.Add(237449392U, "CZ_203_75");
			Countries.stringIDs.Add(3044481890U, "DJ_262_62");
			Countries.stringIDs.Add(2547794864U, "TT_780_225");
			Countries.stringIDs.Add(2237927463U, "DO_214_65");
			Countries.stringIDs.Add(2346964842U, "MM_104_27");
			Countries.stringIDs.Add(1696279285U, "MZ_508_168");
			Countries.stringIDs.Add(1175635471U, "BM_60_20");
			Countries.stringIDs.Add(1364962133U, "PH_608_201");
			Countries.stringIDs.Add(340243436U, "SN_686_210");
			Countries.stringIDs.Add(2135437507U, "NG_566_175");
			Countries.stringIDs.Add(980293189U, "HK_344_104");
			Countries.stringIDs.Add(3086115215U, "SE_752_221");
			Countries.stringIDs.Add(925955849U, "ZM_894_263");
			Countries.stringIDs.Add(363631353U, "LU_442_147");
			Countries.stringIDs.Add(1242399272U, "MX_484_166");
			Countries.stringIDs.Add(3729559185U, "PE_604_187");
			Countries.stringIDs.Add(588242161U, "MW_454_156");
			Countries.stringIDs.Add(2083627458U, "AU_36_12");
			Countries.stringIDs.Add(2041103264U, "MN_496_154");
			Countries.stringIDs.Add(1824151771U, "GA_266_87");
			Countries.stringIDs.Add(563124314U, "CF_140_55");
			Countries.stringIDs.Add(3453824596U, "NA_516_254");
			Countries.stringIDs.Add(2482828915U, "ES_724_217");
			Countries.stringIDs.Add(1308671137U, "GN_324_100");
			Countries.stringIDs.Add(1369040140U, "SM_674_214");
			Countries.stringIDs.Add(2073326121U, "EC_218_66");
			Countries.stringIDs.Add(4095102297U, "MY_458_167");
			Countries.stringIDs.Add(2075754043U, "PF_258_318");
			Countries.stringIDs.Add(2648439129U, "HM_334_325");
			Countries.stringIDs.Add(3696500301U, "VN_704_251");
			Countries.stringIDs.Add(3293351338U, "LK_144_42");
			Countries.stringIDs.Add(95964139U, "FK_238_315");
			Countries.stringIDs.Add(294596775U, "VI_850_252");
			Countries.stringIDs.Add(1733922887U, "PG_598_194");
			Countries.stringIDs.Add(4107113277U, "GM_270_86");
			Countries.stringIDs.Add(2753544182U, "IE_372_68");
			Countries.stringIDs.Add(592549877U, "CW_531_273");
			Countries.stringIDs.Add(2583116736U, "RE_638_198");
			Countries.stringIDs.Add(747893188U, "BB_52_18");
			Countries.stringIDs.Add(3330283586U, "ME_499_0");
			Countries.stringIDs.Add(3004793540U, "TN_788_234");
			Countries.stringIDs.Add(2161146289U, "MC_492_158");
			Countries.stringIDs.Add(2758059055U, "XX_581_258");
			Countries.stringIDs.Add(3896219658U, "AR_32_11");
			Countries.stringIDs.Add(2554496014U, "SZ_748_260");
			Countries.stringIDs.Add(782746222U, "CD_180_44");
			Countries.stringIDs.Add(2165366304U, "AT_40_14");
			Countries.stringIDs.Add(1341011849U, "TZ_834_239");
			Countries.stringIDs.Add(4274738721U, "CR_188_54");
			Countries.stringIDs.Add(170580533U, "VA_336_253");
			Countries.stringIDs.Add(1584863906U, "SO_706_216");
			Countries.stringIDs.Add(2354249789U, "SI_705_212");
			Countries.stringIDs.Add(1201614501U, "MS_500_332");
			Countries.stringIDs.Add(2946275728U, "XX_581_305");
			Countries.stringIDs.Add(656316569U, "ET_231_73");
			Countries.stringIDs.Add(2040666425U, "FM_583_80");
			Countries.stringIDs.Add(545947107U, "BL_652_0");
			Countries.stringIDs.Add(1565053238U, "CO_170_51");
			Countries.stringIDs.Add(1428988741U, "FO_234_81");
			Countries.stringIDs.Add(2985947664U, "PK_586_190");
			Countries.stringIDs.Add(928593759U, "GG_831_324");
			Countries.stringIDs.Add(2976346167U, "RW_646_204");
			Countries.stringIDs.Add(437931514U, "SK_703_143");
			Countries.stringIDs.Add(4052652744U, "KY_136_307");
			Countries.stringIDs.Add(3719348580U, "CX_162_309");
			Countries.stringIDs.Add(4062020976U, "XX_581_338");
			Countries.stringIDs.Add(1754296421U, "X1_581_0");
			Countries.stringIDs.Add(3225390919U, "CA_124_39");
			Countries.stringIDs.Add(1365152447U, "SB_90_30");
			Countries.stringIDs.Add(3035186627U, "NE_562_173");
			Countries.stringIDs.Add(1688203950U, "BF_854_245");
			Countries.stringIDs.Add(1655549906U, "NF_574_336");
			Countries.stringIDs.Add(486955070U, "LR_430_142");
			Countries.stringIDs.Add(1018128401U, "VG_92_351");
			Countries.stringIDs.Add(599535093U, "KW_414_136");
			Countries.stringIDs.Add(396161002U, "SR_740_181");
			Countries.stringIDs.Add(3388400993U, "MR_478_162");
			Countries.stringIDs.Add(1705178486U, "SJ_744_220");
			Countries.stringIDs.Add(2587334274U, "GD_308_91");
			Countries.stringIDs.Add(3436089648U, "NO_578_177");
			Countries.stringIDs.Add(238207211U, "YT_175_331");
			Countries.stringIDs.Add(3688589511U, "CU_192_56");
			Countries.stringIDs.Add(1919917050U, "BZ_84_24");
			Countries.stringIDs.Add(1544599245U, "TW_158_237");
			Countries.stringIDs.Add(1783361531U, "CV_132_57");
			Countries.stringIDs.Add(537487922U, "WS_882_259");
			Countries.stringIDs.Add(1814940642U, "SH_654_343");
			Countries.stringIDs.Add(477577130U, "SV_222_72");
			Countries.stringIDs.Add(2287217386U, "MD_498_152");
			Countries.stringIDs.Add(3284921140U, "UM_581_0");
			Countries.stringIDs.Add(67238401U, "CK_184_312");
			Countries.stringIDs.Add(506016315U, "TL_626_7299303");
			Countries.stringIDs.Add(3734185636U, "AS_16_10");
			Countries.stringIDs.Add(1701642011U, "FI_246_77");
			Countries.stringIDs.Add(1849186379U, "EH_732_0");
			Countries.stringIDs.Add(2420224391U, "PT_620_193");
			Countries.stringIDs.Add(3729747244U, "IT_380_118");
			Countries.stringIDs.Add(3852071526U, "ZA_710_209");
			Countries.stringIDs.Add(2003796335U, "MU_480_160");
			Countries.stringIDs.Add(2583256387U, "BE_56_21");
			Countries.stringIDs.Add(2128751088U, "PN_612_339");
			Countries.stringIDs.Add(593733822U, "BQ_535_161832258");
			Countries.stringIDs.Add(367215151U, "MG_450_149");
			Countries.stringIDs.Add(194553287U, "GQ_226_69");
			Countries.stringIDs.Add(4278334414U, "YE_887_261");
			Countries.stringIDs.Add(4045474244U, "PA_591_192");
			Countries.stringIDs.Add(3843922323U, "GY_328_101");
			Countries.stringIDs.Add(4027338091U, "GR_300_98");
			Countries.stringIDs.Add(1024316098U, "PW_585_195");
			Countries.stringIDs.Add(4109075144U, "XX_581_327");
			Countries.stringIDs.Add(4154687243U, "PR_630_202");
			Countries.stringIDs.Add(610628279U, "BA_70_25");
			Countries.stringIDs.Add(764528929U, "IM_833_15126");
			Countries.stringIDs.Add(3143800767U, "OM_512_164");
			Countries.stringIDs.Add(1276695365U, "LV_428_140");
			Countries.stringIDs.Add(1638715076U, "IN_356_113");
			Countries.stringIDs.Add(764391797U, "TJ_762_228");
			Countries.stringIDs.Add(3187096181U, "LB_422_139");
			Countries.stringIDs.Add(1924338336U, "TD_148_41");
			Countries.stringIDs.Add(1501151314U, "XX_581_127");
			Countries.stringIDs.Add(4280564641U, "KM_174_50");
			Countries.stringIDs.Add(1604790229U, "ID_360_111");
			Countries.stringIDs.Add(1267503226U, "DK_208_61");
			Countries.stringIDs.Add(1299098906U, "KR_410_134");
			Countries.stringIDs.Add(1861222396U, "MT_470_163");
			Countries.stringIDs.Add(2962213296U, "KN_659_207");
			Countries.stringIDs.Add(159636800U, "NR_520_180");
			Countries.stringIDs.Add(1551997871U, "AD_20_8");
			Countries.stringIDs.Add(2053873167U, "RO_642_200");
			Countries.stringIDs.Add(3427917068U, "MF_663_0");
			Countries.stringIDs.Add(306446999U, "JP_392_122");
			Countries.stringIDs.Add(2769383184U, "AW_533_302");
			Countries.stringIDs.Add(399548618U, "EG_818_67");
			Countries.stringIDs.Add(1028635198U, "GH_288_89");
			Countries.stringIDs.Add(2656889581U, "CS_891_269");
			Countries.stringIDs.Add(2166136329U, "BS_44_22");
			Countries.stringIDs.Add(479226287U, "AI_660_300");
			Countries.stringIDs.Add(1882146854U, "BH_48_17");
			Countries.stringIDs.Add(3433977374U, "PS_275_184");
			Countries.stringIDs.Add(2602129354U, "KP_408_131");
			Countries.stringIDs.Add(752455398U, "MK_807_19618");
			Countries.stringIDs.Add(4274021177U, "DM_212_63");
			Countries.stringIDs.Add(2679938597U, "KH_116_40");
			Countries.stringIDs.Add(1528703389U, "MV_462_165");
			Countries.stringIDs.Add(1267550736U, "NI_558_182");
			Countries.stringIDs.Add(3553295807U, "BN_96_37");
			Countries.stringIDs.Add(2813375133U, "CG_178_43");
			Countries.stringIDs.Add(4203270843U, "AQ_10_301");
			Countries.stringIDs.Add(3386522572U, "TH_764_227");
			Countries.stringIDs.Add(448115262U, "CI_384_119");
			Countries.stringIDs.Add(1819434036U, "GI_292_90");
			Countries.stringIDs.Add(3422027552U, "NC_540_334");
			Countries.stringIDs.Add(3231516104U, "SX_534_30967");
			Countries.stringIDs.Add(3501602525U, "TO_776_231");
			Countries.stringIDs.Add(1985192657U, "SC_690_208");
			Countries.stringIDs.Add(175597935U, "GP_312_321");
			Countries.stringIDs.Add(399345023U, "MQ_474_330");
			Countries.stringIDs.Add(2965338442U, "PL_616_191");
			Countries.stringIDs.Add(2643193431U, "XX_581_21242");
			Countries.stringIDs.Add(17747661U, "TF_260_319");
			Countries.stringIDs.Add(1096093064U, "US_840_244");
			Countries.stringIDs.Add(2542991203U, "XX_581_326");
			Countries.stringIDs.Add(4043652412U, "UG_800_240");
			Countries.stringIDs.Add(1010461863U, "AE_784_224");
			Countries.stringIDs.Add(5918779U, "TM_795_238");
			Countries.stringIDs.Add(148087037U, "NL_528_176");
			Countries.stringIDs.Add(1712846428U, "AG_28_2");
			Countries.stringIDs.Add(2482983095U, "SG_702_215");
			Countries.stringIDs.Add(2173922407U, "AL_8_6");
			Countries.stringIDs.Add(477647416U, "AF_4_3");
			Countries.stringIDs.Add(2022988190U, "IR_364_116");
			Countries.stringIDs.Add(3265625955U, "FR_250_84");
			Countries.stringIDs.Add(3212103298U, "CH_756_223");
			Countries.stringIDs.Add(566440899U, "IQ_368_121");
			Countries.stringIDs.Add(2841443953U, "EE_233_70");
			Countries.stringIDs.Add(2954305905U, "GS_239_342");
			Countries.stringIDs.Add(727743244U, "ML_466_157");
			Countries.stringIDs.Add(2845896855U, "RS_688_0");
			Countries.stringIDs.Add(495964926U, "AZ_31_5");
		}

		public static LocalizedString BT_64_34
		{
			get
			{
				return new LocalizedString("BT_64_34", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DZ_12_4
		{
			get
			{
				return new LocalizedString("DZ_12_4", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KI_296_133
		{
			get
			{
				return new LocalizedString("KI_296_133", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LS_426_146
		{
			get
			{
				return new LocalizedString("LS_426_146", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TC_796_349
		{
			get
			{
				return new LocalizedString("TC_796_349", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LI_438_145
		{
			get
			{
				return new LocalizedString("LI_438_145", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HR_191_108
		{
			get
			{
				return new LocalizedString("HR_191_108", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VC_670_248
		{
			get
			{
				return new LocalizedString("VC_670_248", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AN_530_333
		{
			get
			{
				return new LocalizedString("AN_530_333", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JO_400_126
		{
			get
			{
				return new LocalizedString("JO_400_126", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PM_666_206
		{
			get
			{
				return new LocalizedString("PM_666_206", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RU_643_203
		{
			get
			{
				return new LocalizedString("RU_643_203", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DE_276_94
		{
			get
			{
				return new LocalizedString("DE_276_94", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PY_600_185
		{
			get
			{
				return new LocalizedString("PY_600_185", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KE_404_129
		{
			get
			{
				return new LocalizedString("KE_404_129", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CN_156_45
		{
			get
			{
				return new LocalizedString("CN_156_45", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CL_152_46
		{
			get
			{
				return new LocalizedString("CL_152_46", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TG_768_232
		{
			get
			{
				return new LocalizedString("TG_768_232", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BR_76_32
		{
			get
			{
				return new LocalizedString("BR_76_32", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IS_352_110
		{
			get
			{
				return new LocalizedString("IS_352_110", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ST_678_233
		{
			get
			{
				return new LocalizedString("ST_678_233", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SA_682_205
		{
			get
			{
				return new LocalizedString("SA_682_205", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ZW_716_264
		{
			get
			{
				return new LocalizedString("ZW_716_264", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UZ_860_247
		{
			get
			{
				return new LocalizedString("UZ_860_247", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MA_504_159
		{
			get
			{
				return new LocalizedString("MA_504_159", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GB_826_242
		{
			get
			{
				return new LocalizedString("GB_826_242", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SS_728_0
		{
			get
			{
				return new LocalizedString("SS_728_0", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VE_862_249
		{
			get
			{
				return new LocalizedString("VE_862_249", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AO_24_9
		{
			get
			{
				return new LocalizedString("AO_24_9", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CC_166_311
		{
			get
			{
				return new LocalizedString("CC_166_311", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CM_120_49
		{
			get
			{
				return new LocalizedString("CM_120_49", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CY_196_59
		{
			get
			{
				return new LocalizedString("CY_196_59", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NU_570_335
		{
			get
			{
				return new LocalizedString("NU_570_335", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TR_792_235
		{
			get
			{
				return new LocalizedString("TR_792_235", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KG_417_130
		{
			get
			{
				return new LocalizedString("KG_417_130", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SL_694_213
		{
			get
			{
				return new LocalizedString("SL_694_213", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BG_100_35
		{
			get
			{
				return new LocalizedString("BG_100_35", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LT_440_141
		{
			get
			{
				return new LocalizedString("LT_440_141", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TK_772_347
		{
			get
			{
				return new LocalizedString("TK_772_347", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GF_254_317
		{
			get
			{
				return new LocalizedString("GF_254_317", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IL_376_117
		{
			get
			{
				return new LocalizedString("IL_376_117", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MH_584_199
		{
			get
			{
				return new LocalizedString("MH_584_199", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NP_524_178
		{
			get
			{
				return new LocalizedString("NP_524_178", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BI_108_38
		{
			get
			{
				return new LocalizedString("BI_108_38", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HT_332_103
		{
			get
			{
				return new LocalizedString("HT_332_103", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JE_832_328
		{
			get
			{
				return new LocalizedString("JE_832_328", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VU_548_174
		{
			get
			{
				return new LocalizedString("VU_548_174", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BY_112_29
		{
			get
			{
				return new LocalizedString("BY_112_29", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QA_634_197
		{
			get
			{
				return new LocalizedString("QA_634_197", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UY_858_246
		{
			get
			{
				return new LocalizedString("UY_858_246", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GE_268_88
		{
			get
			{
				return new LocalizedString("GE_268_88", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BW_72_19
		{
			get
			{
				return new LocalizedString("BW_72_19", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NZ_554_183
		{
			get
			{
				return new LocalizedString("NZ_554_183", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GU_316_322
		{
			get
			{
				return new LocalizedString("GU_316_322", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BJ_204_28
		{
			get
			{
				return new LocalizedString("BJ_204_28", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AM_51_7
		{
			get
			{
				return new LocalizedString("AM_51_7", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LA_418_138
		{
			get
			{
				return new LocalizedString("LA_418_138", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LC_662_218
		{
			get
			{
				return new LocalizedString("LC_662_218", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GL_304_93
		{
			get
			{
				return new LocalizedString("GL_304_93", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GW_624_196
		{
			get
			{
				return new LocalizedString("GW_624_196", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WF_876_352
		{
			get
			{
				return new LocalizedString("WF_876_352", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HN_340_106
		{
			get
			{
				return new LocalizedString("HN_340_106", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BD_50_23
		{
			get
			{
				return new LocalizedString("BD_50_23", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MO_446_151
		{
			get
			{
				return new LocalizedString("MO_446_151", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AX_248_0
		{
			get
			{
				return new LocalizedString("AX_248_0", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString XX_581_329
		{
			get
			{
				return new LocalizedString("XX_581_329", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KZ_398_137
		{
			get
			{
				return new LocalizedString("KZ_398_137", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SJ_744_125
		{
			get
			{
				return new LocalizedString("SJ_744_125", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UA_804_241
		{
			get
			{
				return new LocalizedString("UA_804_241", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SY_760_222
		{
			get
			{
				return new LocalizedString("SY_760_222", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IO_86_114
		{
			get
			{
				return new LocalizedString("IO_86_114", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GT_320_99
		{
			get
			{
				return new LocalizedString("GT_320_99", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ER_232_71
		{
			get
			{
				return new LocalizedString("ER_232_71", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BO_68_26
		{
			get
			{
				return new LocalizedString("BO_68_26", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BV_74_306
		{
			get
			{
				return new LocalizedString("BV_74_306", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FJ_242_78
		{
			get
			{
				return new LocalizedString("FJ_242_78", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MP_580_337
		{
			get
			{
				return new LocalizedString("MP_580_337", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HU_348_109
		{
			get
			{
				return new LocalizedString("HU_348_109", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SD_736_219
		{
			get
			{
				return new LocalizedString("SD_736_219", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LY_434_148
		{
			get
			{
				return new LocalizedString("LY_434_148", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JM_388_124
		{
			get
			{
				return new LocalizedString("JM_388_124", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TV_798_236
		{
			get
			{
				return new LocalizedString("TV_798_236", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CZ_203_75
		{
			get
			{
				return new LocalizedString("CZ_203_75", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DJ_262_62
		{
			get
			{
				return new LocalizedString("DJ_262_62", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TT_780_225
		{
			get
			{
				return new LocalizedString("TT_780_225", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DO_214_65
		{
			get
			{
				return new LocalizedString("DO_214_65", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MM_104_27
		{
			get
			{
				return new LocalizedString("MM_104_27", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MZ_508_168
		{
			get
			{
				return new LocalizedString("MZ_508_168", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BM_60_20
		{
			get
			{
				return new LocalizedString("BM_60_20", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PH_608_201
		{
			get
			{
				return new LocalizedString("PH_608_201", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SN_686_210
		{
			get
			{
				return new LocalizedString("SN_686_210", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NG_566_175
		{
			get
			{
				return new LocalizedString("NG_566_175", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HK_344_104
		{
			get
			{
				return new LocalizedString("HK_344_104", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SE_752_221
		{
			get
			{
				return new LocalizedString("SE_752_221", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ZM_894_263
		{
			get
			{
				return new LocalizedString("ZM_894_263", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LU_442_147
		{
			get
			{
				return new LocalizedString("LU_442_147", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MX_484_166
		{
			get
			{
				return new LocalizedString("MX_484_166", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PE_604_187
		{
			get
			{
				return new LocalizedString("PE_604_187", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MW_454_156
		{
			get
			{
				return new LocalizedString("MW_454_156", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AU_36_12
		{
			get
			{
				return new LocalizedString("AU_36_12", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MN_496_154
		{
			get
			{
				return new LocalizedString("MN_496_154", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GA_266_87
		{
			get
			{
				return new LocalizedString("GA_266_87", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CF_140_55
		{
			get
			{
				return new LocalizedString("CF_140_55", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NA_516_254
		{
			get
			{
				return new LocalizedString("NA_516_254", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ES_724_217
		{
			get
			{
				return new LocalizedString("ES_724_217", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GN_324_100
		{
			get
			{
				return new LocalizedString("GN_324_100", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SM_674_214
		{
			get
			{
				return new LocalizedString("SM_674_214", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EC_218_66
		{
			get
			{
				return new LocalizedString("EC_218_66", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MY_458_167
		{
			get
			{
				return new LocalizedString("MY_458_167", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PF_258_318
		{
			get
			{
				return new LocalizedString("PF_258_318", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HM_334_325
		{
			get
			{
				return new LocalizedString("HM_334_325", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VN_704_251
		{
			get
			{
				return new LocalizedString("VN_704_251", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LK_144_42
		{
			get
			{
				return new LocalizedString("LK_144_42", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FK_238_315
		{
			get
			{
				return new LocalizedString("FK_238_315", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VI_850_252
		{
			get
			{
				return new LocalizedString("VI_850_252", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PG_598_194
		{
			get
			{
				return new LocalizedString("PG_598_194", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GM_270_86
		{
			get
			{
				return new LocalizedString("GM_270_86", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IE_372_68
		{
			get
			{
				return new LocalizedString("IE_372_68", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CW_531_273
		{
			get
			{
				return new LocalizedString("CW_531_273", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RE_638_198
		{
			get
			{
				return new LocalizedString("RE_638_198", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BB_52_18
		{
			get
			{
				return new LocalizedString("BB_52_18", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ME_499_0
		{
			get
			{
				return new LocalizedString("ME_499_0", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TN_788_234
		{
			get
			{
				return new LocalizedString("TN_788_234", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MC_492_158
		{
			get
			{
				return new LocalizedString("MC_492_158", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString XX_581_258
		{
			get
			{
				return new LocalizedString("XX_581_258", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AR_32_11
		{
			get
			{
				return new LocalizedString("AR_32_11", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SZ_748_260
		{
			get
			{
				return new LocalizedString("SZ_748_260", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CD_180_44
		{
			get
			{
				return new LocalizedString("CD_180_44", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AT_40_14
		{
			get
			{
				return new LocalizedString("AT_40_14", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TZ_834_239
		{
			get
			{
				return new LocalizedString("TZ_834_239", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CR_188_54
		{
			get
			{
				return new LocalizedString("CR_188_54", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VA_336_253
		{
			get
			{
				return new LocalizedString("VA_336_253", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SO_706_216
		{
			get
			{
				return new LocalizedString("SO_706_216", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SI_705_212
		{
			get
			{
				return new LocalizedString("SI_705_212", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MS_500_332
		{
			get
			{
				return new LocalizedString("MS_500_332", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString XX_581_305
		{
			get
			{
				return new LocalizedString("XX_581_305", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ET_231_73
		{
			get
			{
				return new LocalizedString("ET_231_73", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FM_583_80
		{
			get
			{
				return new LocalizedString("FM_583_80", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BL_652_0
		{
			get
			{
				return new LocalizedString("BL_652_0", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CO_170_51
		{
			get
			{
				return new LocalizedString("CO_170_51", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FO_234_81
		{
			get
			{
				return new LocalizedString("FO_234_81", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PK_586_190
		{
			get
			{
				return new LocalizedString("PK_586_190", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GG_831_324
		{
			get
			{
				return new LocalizedString("GG_831_324", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RW_646_204
		{
			get
			{
				return new LocalizedString("RW_646_204", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SK_703_143
		{
			get
			{
				return new LocalizedString("SK_703_143", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KY_136_307
		{
			get
			{
				return new LocalizedString("KY_136_307", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CX_162_309
		{
			get
			{
				return new LocalizedString("CX_162_309", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString XX_581_338
		{
			get
			{
				return new LocalizedString("XX_581_338", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString X1_581_0
		{
			get
			{
				return new LocalizedString("X1_581_0", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CA_124_39
		{
			get
			{
				return new LocalizedString("CA_124_39", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SB_90_30
		{
			get
			{
				return new LocalizedString("SB_90_30", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NE_562_173
		{
			get
			{
				return new LocalizedString("NE_562_173", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BF_854_245
		{
			get
			{
				return new LocalizedString("BF_854_245", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NF_574_336
		{
			get
			{
				return new LocalizedString("NF_574_336", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LR_430_142
		{
			get
			{
				return new LocalizedString("LR_430_142", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VG_92_351
		{
			get
			{
				return new LocalizedString("VG_92_351", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KW_414_136
		{
			get
			{
				return new LocalizedString("KW_414_136", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SR_740_181
		{
			get
			{
				return new LocalizedString("SR_740_181", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MR_478_162
		{
			get
			{
				return new LocalizedString("MR_478_162", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SJ_744_220
		{
			get
			{
				return new LocalizedString("SJ_744_220", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GD_308_91
		{
			get
			{
				return new LocalizedString("GD_308_91", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NO_578_177
		{
			get
			{
				return new LocalizedString("NO_578_177", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString YT_175_331
		{
			get
			{
				return new LocalizedString("YT_175_331", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CU_192_56
		{
			get
			{
				return new LocalizedString("CU_192_56", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BZ_84_24
		{
			get
			{
				return new LocalizedString("BZ_84_24", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TW_158_237
		{
			get
			{
				return new LocalizedString("TW_158_237", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CV_132_57
		{
			get
			{
				return new LocalizedString("CV_132_57", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WS_882_259
		{
			get
			{
				return new LocalizedString("WS_882_259", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SH_654_343
		{
			get
			{
				return new LocalizedString("SH_654_343", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SV_222_72
		{
			get
			{
				return new LocalizedString("SV_222_72", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MD_498_152
		{
			get
			{
				return new LocalizedString("MD_498_152", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UM_581_0
		{
			get
			{
				return new LocalizedString("UM_581_0", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CK_184_312
		{
			get
			{
				return new LocalizedString("CK_184_312", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TL_626_7299303
		{
			get
			{
				return new LocalizedString("TL_626_7299303", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AS_16_10
		{
			get
			{
				return new LocalizedString("AS_16_10", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FI_246_77
		{
			get
			{
				return new LocalizedString("FI_246_77", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EH_732_0
		{
			get
			{
				return new LocalizedString("EH_732_0", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PT_620_193
		{
			get
			{
				return new LocalizedString("PT_620_193", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IT_380_118
		{
			get
			{
				return new LocalizedString("IT_380_118", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ZA_710_209
		{
			get
			{
				return new LocalizedString("ZA_710_209", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MU_480_160
		{
			get
			{
				return new LocalizedString("MU_480_160", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BE_56_21
		{
			get
			{
				return new LocalizedString("BE_56_21", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PN_612_339
		{
			get
			{
				return new LocalizedString("PN_612_339", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BQ_535_161832258
		{
			get
			{
				return new LocalizedString("BQ_535_161832258", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MG_450_149
		{
			get
			{
				return new LocalizedString("MG_450_149", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GQ_226_69
		{
			get
			{
				return new LocalizedString("GQ_226_69", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString YE_887_261
		{
			get
			{
				return new LocalizedString("YE_887_261", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PA_591_192
		{
			get
			{
				return new LocalizedString("PA_591_192", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GY_328_101
		{
			get
			{
				return new LocalizedString("GY_328_101", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GR_300_98
		{
			get
			{
				return new LocalizedString("GR_300_98", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PW_585_195
		{
			get
			{
				return new LocalizedString("PW_585_195", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString XX_581_327
		{
			get
			{
				return new LocalizedString("XX_581_327", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PR_630_202
		{
			get
			{
				return new LocalizedString("PR_630_202", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BA_70_25
		{
			get
			{
				return new LocalizedString("BA_70_25", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IM_833_15126
		{
			get
			{
				return new LocalizedString("IM_833_15126", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OM_512_164
		{
			get
			{
				return new LocalizedString("OM_512_164", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LV_428_140
		{
			get
			{
				return new LocalizedString("LV_428_140", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IN_356_113
		{
			get
			{
				return new LocalizedString("IN_356_113", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TJ_762_228
		{
			get
			{
				return new LocalizedString("TJ_762_228", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LB_422_139
		{
			get
			{
				return new LocalizedString("LB_422_139", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TD_148_41
		{
			get
			{
				return new LocalizedString("TD_148_41", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString XX_581_127
		{
			get
			{
				return new LocalizedString("XX_581_127", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KM_174_50
		{
			get
			{
				return new LocalizedString("KM_174_50", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ID_360_111
		{
			get
			{
				return new LocalizedString("ID_360_111", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DK_208_61
		{
			get
			{
				return new LocalizedString("DK_208_61", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KR_410_134
		{
			get
			{
				return new LocalizedString("KR_410_134", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MT_470_163
		{
			get
			{
				return new LocalizedString("MT_470_163", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KN_659_207
		{
			get
			{
				return new LocalizedString("KN_659_207", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NR_520_180
		{
			get
			{
				return new LocalizedString("NR_520_180", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AD_20_8
		{
			get
			{
				return new LocalizedString("AD_20_8", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RO_642_200
		{
			get
			{
				return new LocalizedString("RO_642_200", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MF_663_0
		{
			get
			{
				return new LocalizedString("MF_663_0", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JP_392_122
		{
			get
			{
				return new LocalizedString("JP_392_122", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AW_533_302
		{
			get
			{
				return new LocalizedString("AW_533_302", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EG_818_67
		{
			get
			{
				return new LocalizedString("EG_818_67", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GH_288_89
		{
			get
			{
				return new LocalizedString("GH_288_89", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CS_891_269
		{
			get
			{
				return new LocalizedString("CS_891_269", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BS_44_22
		{
			get
			{
				return new LocalizedString("BS_44_22", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AI_660_300
		{
			get
			{
				return new LocalizedString("AI_660_300", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BH_48_17
		{
			get
			{
				return new LocalizedString("BH_48_17", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PS_275_184
		{
			get
			{
				return new LocalizedString("PS_275_184", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KP_408_131
		{
			get
			{
				return new LocalizedString("KP_408_131", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MK_807_19618
		{
			get
			{
				return new LocalizedString("MK_807_19618", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DM_212_63
		{
			get
			{
				return new LocalizedString("DM_212_63", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KH_116_40
		{
			get
			{
				return new LocalizedString("KH_116_40", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MV_462_165
		{
			get
			{
				return new LocalizedString("MV_462_165", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NI_558_182
		{
			get
			{
				return new LocalizedString("NI_558_182", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BN_96_37
		{
			get
			{
				return new LocalizedString("BN_96_37", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CG_178_43
		{
			get
			{
				return new LocalizedString("CG_178_43", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AQ_10_301
		{
			get
			{
				return new LocalizedString("AQ_10_301", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TH_764_227
		{
			get
			{
				return new LocalizedString("TH_764_227", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CI_384_119
		{
			get
			{
				return new LocalizedString("CI_384_119", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GI_292_90
		{
			get
			{
				return new LocalizedString("GI_292_90", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NC_540_334
		{
			get
			{
				return new LocalizedString("NC_540_334", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SX_534_30967
		{
			get
			{
				return new LocalizedString("SX_534_30967", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TO_776_231
		{
			get
			{
				return new LocalizedString("TO_776_231", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SC_690_208
		{
			get
			{
				return new LocalizedString("SC_690_208", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GP_312_321
		{
			get
			{
				return new LocalizedString("GP_312_321", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MQ_474_330
		{
			get
			{
				return new LocalizedString("MQ_474_330", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PL_616_191
		{
			get
			{
				return new LocalizedString("PL_616_191", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString XX_581_21242
		{
			get
			{
				return new LocalizedString("XX_581_21242", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TF_260_319
		{
			get
			{
				return new LocalizedString("TF_260_319", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString US_840_244
		{
			get
			{
				return new LocalizedString("US_840_244", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString XX_581_326
		{
			get
			{
				return new LocalizedString("XX_581_326", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UG_800_240
		{
			get
			{
				return new LocalizedString("UG_800_240", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AE_784_224
		{
			get
			{
				return new LocalizedString("AE_784_224", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TM_795_238
		{
			get
			{
				return new LocalizedString("TM_795_238", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NL_528_176
		{
			get
			{
				return new LocalizedString("NL_528_176", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AG_28_2
		{
			get
			{
				return new LocalizedString("AG_28_2", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SG_702_215
		{
			get
			{
				return new LocalizedString("SG_702_215", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AL_8_6
		{
			get
			{
				return new LocalizedString("AL_8_6", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AF_4_3
		{
			get
			{
				return new LocalizedString("AF_4_3", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IR_364_116
		{
			get
			{
				return new LocalizedString("IR_364_116", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FR_250_84
		{
			get
			{
				return new LocalizedString("FR_250_84", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CH_756_223
		{
			get
			{
				return new LocalizedString("CH_756_223", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IQ_368_121
		{
			get
			{
				return new LocalizedString("IQ_368_121", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EE_233_70
		{
			get
			{
				return new LocalizedString("EE_233_70", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GS_239_342
		{
			get
			{
				return new LocalizedString("GS_239_342", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ML_466_157
		{
			get
			{
				return new LocalizedString("ML_466_157", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RS_688_0
		{
			get
			{
				return new LocalizedString("RS_688_0", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AZ_31_5
		{
			get
			{
				return new LocalizedString("AZ_31_5", Countries.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Countries.IDs key)
		{
			return new LocalizedString(Countries.stringIDs[(uint)key], Countries.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(261);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Core.Countries", typeof(Countries).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			BT_64_34 = 3216539067U,
			DZ_12_4 = 1699208365U,
			KI_296_133 = 886545238U,
			LS_426_146 = 1759591854U,
			TC_796_349 = 2344949245U,
			LI_438_145 = 53559490U,
			HR_191_108 = 170381786U,
			VC_670_248 = 1977258562U,
			AN_530_333 = 74285194U,
			JO_400_126 = 1999176122U,
			PM_666_206 = 2368586127U,
			RU_643_203 = 1498891191U,
			DE_276_94 = 3794883283U,
			PY_600_185 = 2103499475U,
			KE_404_129 = 2019047508U,
			CN_156_45 = 1082660990U,
			CL_152_46 = 2512148763U,
			TG_768_232 = 1330749269U,
			BR_76_32 = 2005956254U,
			IS_352_110 = 1592266550U,
			ST_678_233 = 1174698536U,
			SA_682_205 = 377801761U,
			ZW_716_264 = 3159776897U,
			UZ_860_247 = 12936108U,
			MA_504_159 = 3194236654U,
			GB_826_242 = 1422904081U,
			SS_728_0 = 2764932561U,
			VE_862_249 = 1200282230U,
			AO_24_9 = 1037502137U,
			CC_166_311 = 2125592240U,
			CM_120_49 = 621889182U,
			CY_196_59 = 999565292U,
			NU_570_335 = 373651598U,
			TR_792_235 = 55331012U,
			KG_417_130 = 645986668U,
			SL_694_213 = 702227408U,
			BG_100_35 = 3602902706U,
			LT_440_141 = 3029734706U,
			TK_772_347 = 2562573117U,
			GF_254_317 = 1004246475U,
			IL_376_117 = 4002790908U,
			MH_584_199 = 3087470301U,
			NP_524_178 = 56143679U,
			BI_108_38 = 4104315101U,
			HT_332_103 = 636004392U,
			JE_832_328 = 2620723795U,
			VU_548_174 = 1441347144U,
			BY_112_29 = 2320250384U,
			QA_634_197 = 3646896638U,
			UY_858_246 = 2144138889U,
			GE_268_88 = 3789356486U,
			BW_72_19 = 2818474250U,
			NZ_554_183 = 1718335698U,
			GU_316_322 = 151849765U,
			BJ_204_28 = 1843367414U,
			AM_51_7 = 4222597191U,
			LA_418_138 = 1017460496U,
			LC_662_218 = 2132819958U,
			GL_304_93 = 3469746324U,
			GW_624_196 = 46288124U,
			WF_876_352 = 359940844U,
			HN_340_106 = 673912308U,
			BD_50_23 = 3393077234U,
			MO_446_151 = 3346454225U,
			AX_248_0 = 471386291U,
			XX_581_329 = 1333137622U,
			KZ_398_137 = 4169766046U,
			SJ_744_125 = 3768655288U,
			UA_804_241 = 1626681835U,
			SY_760_222 = 1734702041U,
			IO_86_114 = 1952734284U,
			GT_320_99 = 2248145120U,
			ER_232_71 = 1528190210U,
			BO_68_26 = 1633154011U,
			BV_74_306 = 19448000U,
			FJ_242_78 = 1476028295U,
			MP_580_337 = 803870055U,
			HU_348_109 = 2821667208U,
			SD_736_219 = 1635698251U,
			LY_434_148 = 987373943U,
			JM_388_124 = 940316003U,
			TV_798_236 = 1798672883U,
			CZ_203_75 = 237449392U,
			DJ_262_62 = 3044481890U,
			TT_780_225 = 2547794864U,
			DO_214_65 = 2237927463U,
			MM_104_27 = 2346964842U,
			MZ_508_168 = 1696279285U,
			BM_60_20 = 1175635471U,
			PH_608_201 = 1364962133U,
			SN_686_210 = 340243436U,
			NG_566_175 = 2135437507U,
			HK_344_104 = 980293189U,
			SE_752_221 = 3086115215U,
			ZM_894_263 = 925955849U,
			LU_442_147 = 363631353U,
			MX_484_166 = 1242399272U,
			PE_604_187 = 3729559185U,
			MW_454_156 = 588242161U,
			AU_36_12 = 2083627458U,
			MN_496_154 = 2041103264U,
			GA_266_87 = 1824151771U,
			CF_140_55 = 563124314U,
			NA_516_254 = 3453824596U,
			ES_724_217 = 2482828915U,
			GN_324_100 = 1308671137U,
			SM_674_214 = 1369040140U,
			EC_218_66 = 2073326121U,
			MY_458_167 = 4095102297U,
			PF_258_318 = 2075754043U,
			HM_334_325 = 2648439129U,
			VN_704_251 = 3696500301U,
			LK_144_42 = 3293351338U,
			FK_238_315 = 95964139U,
			VI_850_252 = 294596775U,
			PG_598_194 = 1733922887U,
			GM_270_86 = 4107113277U,
			IE_372_68 = 2753544182U,
			CW_531_273 = 592549877U,
			RE_638_198 = 2583116736U,
			BB_52_18 = 747893188U,
			ME_499_0 = 3330283586U,
			TN_788_234 = 3004793540U,
			MC_492_158 = 2161146289U,
			XX_581_258 = 2758059055U,
			AR_32_11 = 3896219658U,
			SZ_748_260 = 2554496014U,
			CD_180_44 = 782746222U,
			AT_40_14 = 2165366304U,
			TZ_834_239 = 1341011849U,
			CR_188_54 = 4274738721U,
			VA_336_253 = 170580533U,
			SO_706_216 = 1584863906U,
			SI_705_212 = 2354249789U,
			MS_500_332 = 1201614501U,
			XX_581_305 = 2946275728U,
			ET_231_73 = 656316569U,
			FM_583_80 = 2040666425U,
			BL_652_0 = 545947107U,
			CO_170_51 = 1565053238U,
			FO_234_81 = 1428988741U,
			PK_586_190 = 2985947664U,
			GG_831_324 = 928593759U,
			RW_646_204 = 2976346167U,
			SK_703_143 = 437931514U,
			KY_136_307 = 4052652744U,
			CX_162_309 = 3719348580U,
			XX_581_338 = 4062020976U,
			X1_581_0 = 1754296421U,
			CA_124_39 = 3225390919U,
			SB_90_30 = 1365152447U,
			NE_562_173 = 3035186627U,
			BF_854_245 = 1688203950U,
			NF_574_336 = 1655549906U,
			LR_430_142 = 486955070U,
			VG_92_351 = 1018128401U,
			KW_414_136 = 599535093U,
			SR_740_181 = 396161002U,
			MR_478_162 = 3388400993U,
			SJ_744_220 = 1705178486U,
			GD_308_91 = 2587334274U,
			NO_578_177 = 3436089648U,
			YT_175_331 = 238207211U,
			CU_192_56 = 3688589511U,
			BZ_84_24 = 1919917050U,
			TW_158_237 = 1544599245U,
			CV_132_57 = 1783361531U,
			WS_882_259 = 537487922U,
			SH_654_343 = 1814940642U,
			SV_222_72 = 477577130U,
			MD_498_152 = 2287217386U,
			UM_581_0 = 3284921140U,
			CK_184_312 = 67238401U,
			TL_626_7299303 = 506016315U,
			AS_16_10 = 3734185636U,
			FI_246_77 = 1701642011U,
			EH_732_0 = 1849186379U,
			PT_620_193 = 2420224391U,
			IT_380_118 = 3729747244U,
			ZA_710_209 = 3852071526U,
			MU_480_160 = 2003796335U,
			BE_56_21 = 2583256387U,
			PN_612_339 = 2128751088U,
			BQ_535_161832258 = 593733822U,
			MG_450_149 = 367215151U,
			GQ_226_69 = 194553287U,
			YE_887_261 = 4278334414U,
			PA_591_192 = 4045474244U,
			GY_328_101 = 3843922323U,
			GR_300_98 = 4027338091U,
			PW_585_195 = 1024316098U,
			XX_581_327 = 4109075144U,
			PR_630_202 = 4154687243U,
			BA_70_25 = 610628279U,
			IM_833_15126 = 764528929U,
			OM_512_164 = 3143800767U,
			LV_428_140 = 1276695365U,
			IN_356_113 = 1638715076U,
			TJ_762_228 = 764391797U,
			LB_422_139 = 3187096181U,
			TD_148_41 = 1924338336U,
			XX_581_127 = 1501151314U,
			KM_174_50 = 4280564641U,
			ID_360_111 = 1604790229U,
			DK_208_61 = 1267503226U,
			KR_410_134 = 1299098906U,
			MT_470_163 = 1861222396U,
			KN_659_207 = 2962213296U,
			NR_520_180 = 159636800U,
			AD_20_8 = 1551997871U,
			RO_642_200 = 2053873167U,
			MF_663_0 = 3427917068U,
			JP_392_122 = 306446999U,
			AW_533_302 = 2769383184U,
			EG_818_67 = 399548618U,
			GH_288_89 = 1028635198U,
			CS_891_269 = 2656889581U,
			BS_44_22 = 2166136329U,
			AI_660_300 = 479226287U,
			BH_48_17 = 1882146854U,
			PS_275_184 = 3433977374U,
			KP_408_131 = 2602129354U,
			MK_807_19618 = 752455398U,
			DM_212_63 = 4274021177U,
			KH_116_40 = 2679938597U,
			MV_462_165 = 1528703389U,
			NI_558_182 = 1267550736U,
			BN_96_37 = 3553295807U,
			CG_178_43 = 2813375133U,
			AQ_10_301 = 4203270843U,
			TH_764_227 = 3386522572U,
			CI_384_119 = 448115262U,
			GI_292_90 = 1819434036U,
			NC_540_334 = 3422027552U,
			SX_534_30967 = 3231516104U,
			TO_776_231 = 3501602525U,
			SC_690_208 = 1985192657U,
			GP_312_321 = 175597935U,
			MQ_474_330 = 399345023U,
			PL_616_191 = 2965338442U,
			XX_581_21242 = 2643193431U,
			TF_260_319 = 17747661U,
			US_840_244 = 1096093064U,
			XX_581_326 = 2542991203U,
			UG_800_240 = 4043652412U,
			AE_784_224 = 1010461863U,
			TM_795_238 = 5918779U,
			NL_528_176 = 148087037U,
			AG_28_2 = 1712846428U,
			SG_702_215 = 2482983095U,
			AL_8_6 = 2173922407U,
			AF_4_3 = 477647416U,
			IR_364_116 = 2022988190U,
			FR_250_84 = 3265625955U,
			CH_756_223 = 3212103298U,
			IQ_368_121 = 566440899U,
			EE_233_70 = 2841443953U,
			GS_239_342 = 2954305905U,
			ML_466_157 = 727743244U,
			RS_688_0 = 2845896855U,
			AZ_31_5 = 495964926U
		}
	}
}
