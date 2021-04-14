using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.Tools
{
	[DesignerCategory("code")]
	[XmlInclude(typeof(CustomSupportedVersion))]
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class SupportedVersion
	{
		[XmlAttribute]
		public string minSupportedVersion
		{
			get
			{
				return this.minSupportedVersionField;
			}
			set
			{
				this.minSupportedVersionField = value;
			}
		}

		[XmlAttribute]
		public string latestVersion
		{
			get
			{
				return this.latestVersionField;
			}
			set
			{
				this.latestVersionField = value;
			}
		}

		[XmlAttribute(DataType = "anyURI")]
		public string updateUrl
		{
			get
			{
				return this.updateUrlField;
			}
			set
			{
				this.updateUrlField = value;
			}
		}

		private string minSupportedVersionField;

		private string latestVersionField;

		private string updateUrlField;
	}
}
