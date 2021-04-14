using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class CalendarPermissionType : BasePermissionType
	{
		public CalendarPermissionReadAccessType ReadItems
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

		public CalendarPermissionLevelType CalendarPermissionLevel
		{
			get
			{
				return this.calendarPermissionLevelField;
			}
			set
			{
				this.calendarPermissionLevelField = value;
			}
		}

		private CalendarPermissionReadAccessType readItemsField;

		private bool readItemsFieldSpecified;

		private CalendarPermissionLevelType calendarPermissionLevelField;
	}
}
