using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class PermissionSetType
	{
		[XmlArrayItem("Permission", IsNullable = false)]
		public PermissionType[] Permissions
		{
			get
			{
				return this.permissionsField;
			}
			set
			{
				this.permissionsField = value;
			}
		}

		[XmlArrayItem("UnknownEntry", IsNullable = false)]
		public string[] UnknownEntries
		{
			get
			{
				return this.unknownEntriesField;
			}
			set
			{
				this.unknownEntriesField = value;
			}
		}

		private PermissionType[] permissionsField;

		private string[] unknownEntriesField;
	}
}
