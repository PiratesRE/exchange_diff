using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class CalendarPermissionSetType
	{
		[XmlArrayItem("CalendarPermission", IsNullable = false)]
		public CalendarPermissionType[] CalendarPermissions
		{
			get
			{
				return this.calendarPermissionsField;
			}
			set
			{
				this.calendarPermissionsField = value;
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

		private CalendarPermissionType[] calendarPermissionsField;

		private string[] unknownEntriesField;
	}
}
