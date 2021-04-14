using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class PathToExtendedFieldType : BasePathToElementType
	{
		[XmlAttribute]
		public DistinguishedPropertySetType DistinguishedPropertySetId
		{
			get
			{
				return this.distinguishedPropertySetIdField;
			}
			set
			{
				this.distinguishedPropertySetIdField = value;
			}
		}

		[XmlIgnore]
		public bool DistinguishedPropertySetIdSpecified
		{
			get
			{
				return this.distinguishedPropertySetIdFieldSpecified;
			}
			set
			{
				this.distinguishedPropertySetIdFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string PropertySetId
		{
			get
			{
				return this.propertySetIdField;
			}
			set
			{
				this.propertySetIdField = value;
			}
		}

		[XmlAttribute]
		public string PropertyTag
		{
			get
			{
				return this.propertyTagField;
			}
			set
			{
				this.propertyTagField = value;
			}
		}

		[XmlAttribute]
		public string PropertyName
		{
			get
			{
				return this.propertyNameField;
			}
			set
			{
				this.propertyNameField = value;
			}
		}

		[XmlAttribute]
		public int PropertyId
		{
			get
			{
				return this.propertyIdField;
			}
			set
			{
				this.propertyIdField = value;
			}
		}

		[XmlIgnore]
		public bool PropertyIdSpecified
		{
			get
			{
				return this.propertyIdFieldSpecified;
			}
			set
			{
				this.propertyIdFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public MapiPropertyTypeType PropertyType
		{
			get
			{
				return this.propertyTypeField;
			}
			set
			{
				this.propertyTypeField = value;
			}
		}

		private DistinguishedPropertySetType distinguishedPropertySetIdField;

		private bool distinguishedPropertySetIdFieldSpecified;

		private string propertySetIdField;

		private string propertyTagField;

		private string propertyNameField;

		private int propertyIdField;

		private bool propertyIdFieldSpecified;

		private MapiPropertyTypeType propertyTypeField;
	}
}
