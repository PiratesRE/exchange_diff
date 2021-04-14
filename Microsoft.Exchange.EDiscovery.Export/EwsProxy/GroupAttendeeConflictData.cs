using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class GroupAttendeeConflictData : AttendeeConflictData
	{
		public int NumberOfMembers
		{
			get
			{
				return this.numberOfMembersField;
			}
			set
			{
				this.numberOfMembersField = value;
			}
		}

		public int NumberOfMembersAvailable
		{
			get
			{
				return this.numberOfMembersAvailableField;
			}
			set
			{
				this.numberOfMembersAvailableField = value;
			}
		}

		public int NumberOfMembersWithConflict
		{
			get
			{
				return this.numberOfMembersWithConflictField;
			}
			set
			{
				this.numberOfMembersWithConflictField = value;
			}
		}

		public int NumberOfMembersWithNoData
		{
			get
			{
				return this.numberOfMembersWithNoDataField;
			}
			set
			{
				this.numberOfMembersWithNoDataField = value;
			}
		}

		private int numberOfMembersField;

		private int numberOfMembersAvailableField;

		private int numberOfMembersWithConflictField;

		private int numberOfMembersWithNoDataField;
	}
}
