using System;

namespace Microsoft.Exchange.Data.ContentTypes.Tnef
{
	internal static class TnefCommon
	{
		public static bool BytesEqualToPattern(byte[] buffer, int offset, string pattern)
		{
			int length = pattern.Length;
			for (int i = 0; i < pattern.Length; i++)
			{
				if ((char)buffer[offset + i] != pattern[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool BytesEqualToPattern(byte[] buffer, int offset, byte[] pattern)
		{
			for (int i = 0; i < pattern.Length; i++)
			{
				if (buffer[offset + i] != pattern[i])
				{
					return false;
				}
			}
			return true;
		}

		public static int StrZLength(byte[] buffer, int offset, int maxEndOffset)
		{
			int num = offset;
			while (num < maxEndOffset && buffer[num] != 0)
			{
				num++;
			}
			return num - offset;
		}

		public static bool IsUnicodeCodepage(int messageCodePage)
		{
			return messageCodePage == 1200 || messageCodePage == 1201 || messageCodePage == 12000 || messageCodePage == 12001 || messageCodePage == 65005 || messageCodePage == 65006;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static TnefCommon()
		{
			byte[] padding = new byte[4];
			TnefCommon.Padding = padding;
			TnefCommon.OidOle1Storage = new byte[]
			{
				42,
				134,
				72,
				134,
				247,
				20,
				3,
				10,
				3,
				1,
				1
			};
			TnefCommon.OidMacBinary = new byte[]
			{
				42,
				134,
				72,
				134,
				247,
				20,
				3,
				11,
				1
			};
			TnefCommon.MuidOOP = new byte[]
			{
				129,
				43,
				31,
				164,
				190,
				163,
				16,
				25,
				157,
				110,
				0,
				221,
				1,
				15,
				84,
				2
			};
			TnefCommon.oleGuid = new byte[]
			{
				192,
				0,
				0,
				0,
				0,
				0,
				0,
				70
			};
			TnefCommon.MessageIID = new Guid(131847, 0, 0, TnefCommon.oleGuid);
			TnefCommon.MessageClassLegacyPrefix = "Microsoft Mail v3.0 ";
			TnefCommon.MessageClassMappingTable = new TnefCommon.MessageClassMapping[]
			{
				new TnefCommon.MessageClassMapping("IPM.Microsoft Mail.Note", "IPM.Note", false, false),
				new TnefCommon.MessageClassMapping("IPM.Microsoft Mail.Note", "IPM", false, false),
				new TnefCommon.MessageClassMapping("IPM.Microsoft Mail.Read Receipt", "Report.IPM.Note.IPNRN", false, false),
				new TnefCommon.MessageClassMapping("IPM.Microsoft Mail.Non-Delivery", "Report.IPM.Note.NDR", false, false),
				new TnefCommon.MessageClassMapping("IPM.Microsoft Schedule.MtgRespP", "IPM.Schedule.Meeting.Resp.Pos", true, true),
				new TnefCommon.MessageClassMapping("IPM.Microsoft Schedule.MtgRespN", "IPM.Schedule.Meeting.Resp.Neg", true, true),
				new TnefCommon.MessageClassMapping("IPM.Microsoft Schedule.MtgRespA", "IPM.Schedule.Meeting.Resp.Tent", true, true),
				new TnefCommon.MessageClassMapping("IPM.Microsoft Schedule.MtgReq", "IPM.Schedule.Meeting.Request", true, false),
				new TnefCommon.MessageClassMapping("IPM.Microsoft Schedule.MtgCncl", "IPM.Schedule.Meeting.Canceled", true, false)
			};
		}

		public const int TnefSignature = 574529400;

		public const int MaxTnefVersion = 65536;

		public const int AttributeHeaderLength = 9;

		public const int CheckSumLength = 2;

		public const int StringNameKind = 1;

		public const int MaxNestingDepth = 100;

		public static readonly byte[] HexDigit = new byte[]
		{
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			65,
			66,
			67,
			68,
			69,
			70
		};

		public static readonly byte[] Padding;

		public static readonly byte[] OidOle1Storage;

		public static readonly byte[] OidMacBinary;

		public static readonly byte[] MuidOOP;

		private static readonly byte[] oleGuid;

		public static readonly Guid MessageIID;

		public static readonly string MessageClassLegacyPrefix;

		public static readonly TnefCommon.MessageClassMapping[] MessageClassMappingTable;

		public struct MessageClassMapping
		{
			public MessageClassMapping(string legacyName, string mapiName, bool splus, bool splusResponse)
			{
				this.LegacyName = legacyName;
				this.MapiName = mapiName;
				this.Splus = splus;
				this.SplusResponse = splusResponse;
			}

			public string LegacyName;

			public string MapiName;

			public bool Splus;

			public bool SplusResponse;
		}
	}
}
