using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "LogConfig")]
	[Serializable]
	public class LogConfigXML : XMLSerializableBase
	{
		public LogConfigXML()
		{
			this.MaxAge = LogConfigXML.DefaultMaxAge;
			this.MaxDirectorySize = LogConfigXML.DefaultMaxDirectorySize;
			this.MaxFileSize = LogConfigXML.DefaultMaxFileSize;
			this.Enabled = true;
		}

		[XmlIgnore]
		internal EnhancedTimeSpan MaxAge { get; set; }

		[XmlAttribute(AttributeName = "MA")]
		public long MaxAgeTicks
		{
			get
			{
				return this.MaxAge.Ticks;
			}
			set
			{
				this.MaxAge = EnhancedTimeSpan.FromTicks(value);
			}
		}

		[XmlIgnore]
		internal Unlimited<ByteQuantifiedSize> MaxDirectorySize { get; set; }

		[XmlAttribute(AttributeName = "MDS")]
		public ulong MaxDirectorySizeLong
		{
			get
			{
				return XMLSerializableBase.UnlimitedSizeToUlong(this.MaxDirectorySize);
			}
			set
			{
				this.MaxDirectorySize = XMLSerializableBase.UlongToUnlimitedSize(value);
			}
		}

		[XmlIgnore]
		internal Unlimited<ByteQuantifiedSize> MaxFileSize { get; set; }

		[XmlAttribute(AttributeName = "MFS")]
		public ulong MaxFileSizeLong
		{
			get
			{
				return XMLSerializableBase.UnlimitedSizeToUlong(this.MaxFileSize);
			}
			set
			{
				this.MaxFileSize = XMLSerializableBase.UlongToUnlimitedSize(value);
			}
		}

		[XmlIgnore]
		internal LocalLongFullPath Path { get; set; }

		[XmlAttribute(AttributeName = "PATH")]
		public string PathString
		{
			get
			{
				if (!(this.Path != null))
				{
					return null;
				}
				return this.Path.PathName;
			}
			set
			{
				this.Path = ((value != null) ? LocalLongFullPath.Parse(value) : null);
			}
		}

		[XmlAttribute(AttributeName = "ENA")]
		public bool Enabled { get; set; }

		internal static readonly EnhancedTimeSpan DefaultMaxAge = EnhancedTimeSpan.FromDays(7.0);

		internal static readonly Unlimited<ByteQuantifiedSize> DefaultMaxDirectorySize = ByteQuantifiedSize.FromMB(200UL);

		internal static readonly Unlimited<ByteQuantifiedSize> DefaultMaxFileSize = ByteQuantifiedSize.FromMB(10UL);
	}
}
