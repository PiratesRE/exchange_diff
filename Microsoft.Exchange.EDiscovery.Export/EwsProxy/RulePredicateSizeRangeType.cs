using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class RulePredicateSizeRangeType
	{
		public int MinimumSize
		{
			get
			{
				return this.minimumSizeField;
			}
			set
			{
				this.minimumSizeField = value;
			}
		}

		[XmlIgnore]
		public bool MinimumSizeSpecified
		{
			get
			{
				return this.minimumSizeFieldSpecified;
			}
			set
			{
				this.minimumSizeFieldSpecified = value;
			}
		}

		public int MaximumSize
		{
			get
			{
				return this.maximumSizeField;
			}
			set
			{
				this.maximumSizeField = value;
			}
		}

		[XmlIgnore]
		public bool MaximumSizeSpecified
		{
			get
			{
				return this.maximumSizeFieldSpecified;
			}
			set
			{
				this.maximumSizeFieldSpecified = value;
			}
		}

		private int minimumSizeField;

		private bool minimumSizeFieldSpecified;

		private int maximumSizeField;

		private bool maximumSizeFieldSpecified;
	}
}
