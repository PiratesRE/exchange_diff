using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class QueryStringType
	{
		[XmlAttribute]
		public bool ResetCache
		{
			get
			{
				return this.resetCacheField;
			}
			set
			{
				this.resetCacheField = value;
			}
		}

		[XmlIgnore]
		public bool ResetCacheSpecified
		{
			get
			{
				return this.resetCacheFieldSpecified;
			}
			set
			{
				this.resetCacheFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public bool ReturnHighlightTerms
		{
			get
			{
				return this.returnHighlightTermsField;
			}
			set
			{
				this.returnHighlightTermsField = value;
			}
		}

		[XmlIgnore]
		public bool ReturnHighlightTermsSpecified
		{
			get
			{
				return this.returnHighlightTermsFieldSpecified;
			}
			set
			{
				this.returnHighlightTermsFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public bool ReturnDeletedItems
		{
			get
			{
				return this.returnDeletedItemsField;
			}
			set
			{
				this.returnDeletedItemsField = value;
			}
		}

		[XmlIgnore]
		public bool ReturnDeletedItemsSpecified
		{
			get
			{
				return this.returnDeletedItemsFieldSpecified;
			}
			set
			{
				this.returnDeletedItemsFieldSpecified = value;
			}
		}

		[XmlText]
		public string Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		private bool resetCacheField;

		private bool resetCacheFieldSpecified;

		private bool returnHighlightTermsField;

		private bool returnHighlightTermsFieldSpecified;

		private bool returnDeletedItemsField;

		private bool returnDeletedItemsFieldSpecified;

		private string valueField;
	}
}
