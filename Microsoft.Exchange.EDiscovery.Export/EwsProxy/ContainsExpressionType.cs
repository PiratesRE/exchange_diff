using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ContainsExpressionType : SearchExpressionType
	{
		[XmlElement("FieldURI", typeof(PathToUnindexedFieldType))]
		[XmlElement("ExtendedFieldURI", typeof(PathToExtendedFieldType))]
		[XmlElement("IndexedFieldURI", typeof(PathToIndexedFieldType))]
		public BasePathToElementType Item
		{
			get
			{
				return this.itemField;
			}
			set
			{
				this.itemField = value;
			}
		}

		public ConstantValueType Constant
		{
			get
			{
				return this.constantField;
			}
			set
			{
				this.constantField = value;
			}
		}

		[XmlAttribute]
		public ContainmentModeType ContainmentMode
		{
			get
			{
				return this.containmentModeField;
			}
			set
			{
				this.containmentModeField = value;
			}
		}

		[XmlIgnore]
		public bool ContainmentModeSpecified
		{
			get
			{
				return this.containmentModeFieldSpecified;
			}
			set
			{
				this.containmentModeFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public ContainmentComparisonType ContainmentComparison
		{
			get
			{
				return this.containmentComparisonField;
			}
			set
			{
				this.containmentComparisonField = value;
			}
		}

		[XmlIgnore]
		public bool ContainmentComparisonSpecified
		{
			get
			{
				return this.containmentComparisonFieldSpecified;
			}
			set
			{
				this.containmentComparisonFieldSpecified = value;
			}
		}

		private BasePathToElementType itemField;

		private ConstantValueType constantField;

		private ContainmentModeType containmentModeField;

		private bool containmentModeFieldSpecified;

		private ContainmentComparisonType containmentComparisonField;

		private bool containmentComparisonFieldSpecified;
	}
}
