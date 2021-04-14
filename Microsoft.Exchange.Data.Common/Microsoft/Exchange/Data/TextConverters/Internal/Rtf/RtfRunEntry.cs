using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal struct RtfRunEntry
	{
		public RtfRunKind Kind
		{
			get
			{
				return (RtfRunKind)(this.bitFields & 61440);
			}
		}

		public short KeywordId
		{
			get
			{
				return (short)(this.bitFields & 511);
			}
		}

		public bool Skip
		{
			get
			{
				return 0 != (this.bitFields & 1024);
			}
		}

		public bool Lead
		{
			get
			{
				return 0 != (this.bitFields & 512);
			}
		}

		public ushort Length
		{
			get
			{
				return this.length;
			}
		}

		public int Value
		{
			get
			{
				return this.value;
			}
		}

		public bool IsSkiped
		{
			get
			{
				return this.Kind == RtfRunKind.Ignore || this.Skip;
			}
		}

		public bool IsSmall
		{
			get
			{
				RtfRunKind kind = this.Kind;
				return kind == RtfRunKind.Escape || kind == RtfRunKind.Zero || (kind == RtfRunKind.Text && this.length == 1);
			}
		}

		public bool IsUnicode
		{
			get
			{
				return this.Kind == RtfRunKind.Unicode;
			}
		}

		internal void Reset()
		{
			this.bitFields = 0;
			this.length = 0;
			this.value = 0;
		}

		internal void Initialize(RtfRunKind kind, int length, int value)
		{
			this.bitFields = (ushort)kind;
			this.length = (ushort)length;
			this.value = value;
		}

		internal void Initialize(RtfRunKind kind, int length, int unescaped, bool skip, bool lead)
		{
			ushort num = (ushort)kind;
			if (skip)
			{
				num |= 1024;
			}
			if (lead)
			{
				num |= 512;
			}
			this.bitFields = num;
			this.length = (ushort)length;
			this.value = unescaped;
		}

		internal void InitializeKeyword(short keywordId, int value, int length, bool skip, bool firstKeyword)
		{
			ushort num = 20480 | (ushort)keywordId;
			if (skip)
			{
				num |= 1024;
			}
			if (firstKeyword)
			{
				num |= 512;
			}
			this.bitFields = num;
			this.length = (ushort)length;
			this.value = value;
		}

		private const ushort RunKindMask = 61440;

		private const ushort SkipBit = 1024;

		private const ushort LeadBit = 512;

		private const ushort KeywordIdMask = 511;

		private ushort bitFields;

		private ushort length;

		private int value;
	}
}
