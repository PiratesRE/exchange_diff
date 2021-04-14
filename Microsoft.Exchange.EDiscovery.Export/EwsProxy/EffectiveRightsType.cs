using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class EffectiveRightsType
	{
		public bool CreateAssociated
		{
			get
			{
				return this.createAssociatedField;
			}
			set
			{
				this.createAssociatedField = value;
			}
		}

		public bool CreateContents
		{
			get
			{
				return this.createContentsField;
			}
			set
			{
				this.createContentsField = value;
			}
		}

		public bool CreateHierarchy
		{
			get
			{
				return this.createHierarchyField;
			}
			set
			{
				this.createHierarchyField = value;
			}
		}

		public bool Delete
		{
			get
			{
				return this.deleteField;
			}
			set
			{
				this.deleteField = value;
			}
		}

		public bool Modify
		{
			get
			{
				return this.modifyField;
			}
			set
			{
				this.modifyField = value;
			}
		}

		public bool Read
		{
			get
			{
				return this.readField;
			}
			set
			{
				this.readField = value;
			}
		}

		public bool ViewPrivateItems
		{
			get
			{
				return this.viewPrivateItemsField;
			}
			set
			{
				this.viewPrivateItemsField = value;
			}
		}

		[XmlIgnore]
		public bool ViewPrivateItemsSpecified
		{
			get
			{
				return this.viewPrivateItemsFieldSpecified;
			}
			set
			{
				this.viewPrivateItemsFieldSpecified = value;
			}
		}

		private bool createAssociatedField;

		private bool createContentsField;

		private bool createHierarchyField;

		private bool deleteField;

		private bool modifyField;

		private bool readField;

		private bool viewPrivateItemsField;

		private bool viewPrivateItemsFieldSpecified;
	}
}
