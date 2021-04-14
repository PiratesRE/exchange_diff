using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class XmlValueAssignedRoleSlice
	{
		[XmlElement(Order = 0)]
		public AssignedRoleSliceValue AssignedRoleSlice
		{
			get
			{
				return this.assignedRoleSliceField;
			}
			set
			{
				this.assignedRoleSliceField = value;
			}
		}

		private AssignedRoleSliceValue assignedRoleSliceField;
	}
}
