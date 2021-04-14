using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal struct RtfKeyword
	{
		internal RtfKeyword(RtfToken token)
		{
			this.token = token;
		}

		public byte[] Buffer
		{
			get
			{
				return this.token.Buffer;
			}
		}

		public int Offset
		{
			get
			{
				return this.token.CurrentRunOffset;
			}
		}

		public int Length
		{
			get
			{
				return (int)this.token.RunQueue[this.token.CurrentRun].Length;
			}
		}

		public short Id
		{
			get
			{
				return this.token.RunQueue[this.token.CurrentRun].KeywordId;
			}
		}

		public int Value
		{
			get
			{
				return this.token.RunQueue[this.token.CurrentRun].Value;
			}
		}

		public bool Skip
		{
			get
			{
				return this.token.RunQueue[this.token.CurrentRun].Skip;
			}
		}

		public bool First
		{
			get
			{
				return this.token.RunQueue[this.token.CurrentRun].Lead;
			}
		}

		[Conditional("DEBUG")]
		private void AssertCurrent()
		{
		}

		private RtfToken token;
	}
}
