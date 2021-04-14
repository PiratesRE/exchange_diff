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
	public class PermissionType : BasePermissionType
	{
		public PermissionReadAccessType ReadItems
		{
			get
			{
				return this.readItemsField;
			}
			set
			{
				this.readItemsField = value;
			}
		}

		[XmlIgnore]
		public bool ReadItemsSpecified
		{
			get
			{
				return this.readItemsFieldSpecified;
			}
			set
			{
				this.readItemsFieldSpecified = value;
			}
		}

		public PermissionLevelType PermissionLevel
		{
			get
			{
				return this.permissionLevelField;
			}
			set
			{
				this.permissionLevelField = value;
			}
		}

		private PermissionReadAccessType readItemsField;

		private bool readItemsFieldSpecified;

		private PermissionLevelType permissionLevelField;
	}
}
