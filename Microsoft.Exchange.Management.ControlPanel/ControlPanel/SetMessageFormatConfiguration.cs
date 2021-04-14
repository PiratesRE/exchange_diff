using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetMessageFormatConfiguration : SetMessagingConfigurationBase
	{
		[DataMember]
		public bool AlwaysShowBcc
		{
			get
			{
				return (bool)(base["AlwaysShowBcc"] ?? false);
			}
			set
			{
				base["AlwaysShowBcc"] = value;
			}
		}

		[DataMember]
		public bool AlwaysShowFrom
		{
			get
			{
				return (bool)(base["AlwaysShowFrom"] ?? false);
			}
			set
			{
				base["AlwaysShowFrom"] = value;
			}
		}

		[DataMember]
		public string DefaultFormat
		{
			get
			{
				return (string)base["DefaultFormat"];
			}
			set
			{
				base["DefaultFormat"] = value;
			}
		}

		[DataMember]
		public FormatBarState MessageFont
		{
			get
			{
				return new FormatBarState((string)base["DefaultFontName"], (int)(base["DefaultFontSize"] ?? 2), (FontFlags)(base["DefaultFontFlags"] ?? FontFlags.Normal), (string)base["DefaultFontColor"]);
			}
			set
			{
				base["DefaultFontName"] = value.sFontName;
				base["DefaultFontSize"] = value.iFontSize;
				base["DefaultFontFlags"] = ((value.fBold ? FontFlags.Bold : FontFlags.Normal) | (value.fItalics ? FontFlags.Italic : FontFlags.Normal) | (value.fUnderline ? FontFlags.Underline : FontFlags.Normal));
				base["DefaultFontColor"] = "#" + value.sTextColor;
			}
		}
	}
}
