using System;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal struct HtmlAttributeParts
	{
		internal HtmlAttributeParts(HtmlToken.AttrPartMajor major, HtmlToken.AttrPartMinor minor)
		{
			this.minor = minor;
			this.major = major;
		}

		public bool Begin
		{
			get
			{
				return 24 == (byte)(this.major & HtmlToken.AttrPartMajor.Begin);
			}
		}

		public bool End
		{
			get
			{
				return 48 == (byte)(this.major & HtmlToken.AttrPartMajor.End);
			}
		}

		public bool Complete
		{
			get
			{
				return HtmlToken.AttrPartMajor.Complete == this.major;
			}
		}

		public bool NameBegin
		{
			get
			{
				return 3 == (byte)(this.minor & HtmlToken.AttrPartMinor.BeginName);
			}
		}

		public bool Name
		{
			get
			{
				return 2 == (byte)(this.minor & HtmlToken.AttrPartMinor.ContinueName);
			}
		}

		public bool NameEnd
		{
			get
			{
				return 6 == (byte)(this.minor & HtmlToken.AttrPartMinor.EndName);
			}
		}

		public bool NameComplete
		{
			get
			{
				return 7 == (byte)(this.minor & HtmlToken.AttrPartMinor.CompleteName);
			}
		}

		public bool ValueBegin
		{
			get
			{
				return 24 == (byte)(this.minor & HtmlToken.AttrPartMinor.BeginValue);
			}
		}

		public bool Value
		{
			get
			{
				return 16 == (byte)(this.minor & HtmlToken.AttrPartMinor.ContinueValue);
			}
		}

		public bool ValueEnd
		{
			get
			{
				return 48 == (byte)(this.minor & HtmlToken.AttrPartMinor.EndValue);
			}
		}

		public bool ValueComplete
		{
			get
			{
				return 56 == (byte)(this.minor & HtmlToken.AttrPartMinor.CompleteValue);
			}
		}

		public override string ToString()
		{
			return this.major.ToString() + " /" + this.minor.ToString() + "/";
		}

		private HtmlToken.AttrPartMajor major;

		private HtmlToken.AttrPartMinor minor;
	}
}
