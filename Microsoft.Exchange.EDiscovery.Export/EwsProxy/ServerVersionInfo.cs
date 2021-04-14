using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlRoot(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ServerVersionInfo : SoapHeader
	{
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

		[XmlAttribute]
		public string Version
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

		private int majorVersionField;

		private bool majorVersionFieldSpecified;

		private int minorVersionField;

		private bool minorVersionFieldSpecified;

		private int majorBuildNumberField;

		private bool majorBuildNumberFieldSpecified;

		private int minorBuildNumberField;

		private bool minorBuildNumberFieldSpecified;

		private string versionField;
	}
}
