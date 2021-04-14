using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[XmlType(TypeName = "RequestServerVersion", AnonymousType = true, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlRoot(IsNullable = false, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class RequestServerVersion
	{
		[DefaultValue(ExchangeVersionType.Exchange2010)]
		[XmlAttribute]
		[IgnoreDataMember]
		public ExchangeVersionType Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
				this.versionSpecified = true;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool VersionSpecified
		{
			get
			{
				return this.versionSpecified;
			}
			set
			{
				this.versionSpecified = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "Version", IsRequired = true)]
		public string VersionString
		{
			get
			{
				return EnumUtilities.ToString<ExchangeVersionType>(this.Version);
			}
			set
			{
				this.Version = EnumUtilities.Parse<ExchangeVersionType>(value);
			}
		}

		private ExchangeVersionType version;

		private bool versionSpecified;
	}
}
