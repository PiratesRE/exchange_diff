using System;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal struct HtmlTagParts
	{
		internal HtmlTagParts(HtmlToken.TagPartMajor major, HtmlToken.TagPartMinor minor)
		{
			this.minor = minor;
			this.major = major;
		}

		public bool Begin
		{
			get
			{
				return 3 == (byte)(this.major & HtmlToken.TagPartMajor.Begin);
			}
		}

		public bool End
		{
			get
			{
				return 6 == (byte)(this.major & HtmlToken.TagPartMajor.End);
			}
		}

		public bool Complete
		{
			get
			{
				return HtmlToken.TagPartMajor.Complete == this.major;
			}
		}

		public bool NameBegin
		{
			get
			{
				return 3 == (byte)(this.minor & HtmlToken.TagPartMinor.BeginName);
			}
		}

		public bool Name
		{
			get
			{
				return 2 == (byte)(this.minor & HtmlToken.TagPartMinor.ContinueName);
			}
		}

		public bool NameEnd
		{
			get
			{
				return 6 == (byte)(this.minor & HtmlToken.TagPartMinor.EndName);
			}
		}

		public bool NameComplete
		{
			get
			{
				return 7 == (byte)(this.minor & HtmlToken.TagPartMinor.CompleteName);
			}
		}

		public bool Attributes
		{
			get
			{
				return 0 != (byte)(this.minor & (HtmlToken.TagPartMinor)144);
			}
		}

		public override string ToString()
		{
			return this.major.ToString() + " /" + this.minor.ToString() + "/";
		}

		internal void Reset()
		{
			this.minor = HtmlToken.TagPartMinor.Empty;
			this.major = HtmlToken.TagPartMajor.None;
		}

		private HtmlToken.TagPartMajor major;

		private HtmlToken.TagPartMinor minor;
	}
}
