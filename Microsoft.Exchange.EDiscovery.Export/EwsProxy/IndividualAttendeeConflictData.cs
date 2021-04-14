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
	public class IndividualAttendeeConflictData : AttendeeConflictData
	{
		public LegacyFreeBusyType BusyType
		{
			get
			{
				return this.busyTypeField;
			}
			set
			{
				this.busyTypeField = value;
			}
		}

		private LegacyFreeBusyType busyTypeField;
	}
}
