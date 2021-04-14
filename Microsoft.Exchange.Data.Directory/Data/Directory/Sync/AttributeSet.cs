using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/metadata/2010/01")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class AttributeSet
	{
		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		[XmlAttribute]
		public bool ExchangeMastered
		{
			get
			{
				return this.exchangeMasteredField;
			}
			set
			{
				this.exchangeMasteredField = value;
			}
		}

		[XmlAttribute(DataType = "positiveInteger")]
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

		[XmlAttribute(DataType = "positiveInteger")]
		public string LastVersionSeized
		{
			get
			{
				return this.lastVersionSeizedField;
			}
			set
			{
				this.lastVersionSeizedField = value;
			}
		}

		private string nameField;

		private bool exchangeMasteredField;

		private string versionField;

		private string lastVersionSeizedField;
	}
}
