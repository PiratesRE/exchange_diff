using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[XmlRoot(IsNullable = true, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ServerVersionInfo
	{
		internal static ServerVersionInfo BuildFromExecutingAssembly()
		{
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
			return new ServerVersionInfo
			{
				MajorVersion = versionInfo.FileMajorPart,
				MinorVersion = versionInfo.FileMinorPart,
				MajorBuildNumber = versionInfo.FileBuildPart,
				MinorBuildNumber = versionInfo.FilePrivatePart,
				Version = ExchangeVersion.Latest.Version
			};
		}

		[DataMember(Order = 1)]
		[XmlAttribute]
		public int MajorVersion
		{
			get
			{
				return this.majorVersionField;
			}
			set
			{
				this.majorVersionField = value;
				this.majorVersionFieldSpecified = true;
			}
		}

		[XmlIgnore]
		public bool MajorVersionSpecified
		{
			get
			{
				return this.majorVersionFieldSpecified;
			}
			set
			{
				this.majorVersionFieldSpecified = value;
			}
		}

		[DataMember(Order = 2)]
		[XmlAttribute]
		public int MinorVersion
		{
			get
			{
				return this.minorVersionField;
			}
			set
			{
				this.minorVersionField = value;
				this.minorVersionFieldSpecified = true;
			}
		}

		[XmlIgnore]
		public bool MinorVersionSpecified
		{
			get
			{
				return this.minorVersionFieldSpecified;
			}
			set
			{
				this.minorVersionFieldSpecified = value;
			}
		}

		[DataMember(Order = 3)]
		[XmlAttribute]
		public int MajorBuildNumber
		{
			get
			{
				return this.majorBuildNumberField;
			}
			set
			{
				this.majorBuildNumberField = value;
				this.majorBuildNumberFieldSpecified = true;
			}
		}

		[XmlIgnore]
		public bool MajorBuildNumberSpecified
		{
			get
			{
				return this.majorBuildNumberFieldSpecified;
			}
			set
			{
				this.majorBuildNumberFieldSpecified = value;
			}
		}

		[DataMember(Order = 4)]
		[XmlAttribute]
		public int MinorBuildNumber
		{
			get
			{
				return this.minorBuildNumberField;
			}
			set
			{
				this.minorBuildNumberField = value;
				this.minorBuildNumberFieldSpecified = true;
			}
		}

		[XmlIgnore]
		public bool MinorBuildNumberSpecified
		{
			get
			{
				return this.minorBuildNumberFieldSpecified;
			}
			set
			{
				this.minorBuildNumberFieldSpecified = value;
			}
		}

		[IgnoreDataMember]
		[XmlAttribute]
		public ExchangeVersionType Version
		{
			get
			{
				return this.versionField;
			}
			set
			{
				this.versionField = value;
			}
		}

		[XmlIgnore]
		public bool VersionSpecified
		{
			get
			{
				return ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "Version", Order = 5)]
		public string VersionString
		{
			get
			{
				return EnumUtilities.ToString<ExchangeVersionType>(this.Version);
			}
			set
			{
				this.versionField = EnumUtilities.Parse<ExchangeVersionType>(value);
			}
		}

		[XmlIgnore]
		public static ServerVersionInfo CurrentAssemblyVersion
		{
			get
			{
				return ServerVersionInfo.serverVersionSingleton.Member;
			}
		}

		private static LazyMember<ServerVersionInfo> serverVersionSingleton = new LazyMember<ServerVersionInfo>(() => ServerVersionInfo.BuildFromExecutingAssembly());

		private int majorVersionField;

		private bool majorVersionFieldSpecified;

		private int minorVersionField;

		private bool minorVersionFieldSpecified;

		private int majorBuildNumberField;

		private bool majorBuildNumberFieldSpecified;

		private int minorBuildNumberField;

		private bool minorBuildNumberFieldSpecified;

		private ExchangeVersionType versionField;
	}
}
