using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class FormatBarState
	{
		public FormatBarState(string fontName, int fontSize, FontFlags fontFlags, string fontColor)
		{
			this.sFontName = fontName;
			this.iFontSize = fontSize;
			this.fBold = ((fontFlags & FontFlags.Bold) == FontFlags.Bold);
			this.fItalics = ((fontFlags & FontFlags.Italic) == FontFlags.Italic);
			this.fUnderline = ((fontFlags & FontFlags.Underline) == FontFlags.Underline);
			this.sTextColor = fontColor;
		}

		[DataMember]
		public string sFontName { get; private set; }

		[DataMember]
		public int iFontSize { get; private set; }

		[DataMember]
		public bool fBold { get; private set; }

		[DataMember]
		public bool fItalics { get; private set; }

		[DataMember]
		public bool fUnderline { get; private set; }

		[DataMember]
		public string sTextColor { get; private set; }

		[DataMember]
		public string sHighlightColor { get; private set; }
	}
}
