using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GroupAttendeeConflictData : AttendeeConflictData
	{
		public int NumberOfMembers;

		public int NumberOfMembersAvailable;

		public int NumberOfMembersWithConflict;

		public int NumberOfMembersWithNoData;
	}
}
