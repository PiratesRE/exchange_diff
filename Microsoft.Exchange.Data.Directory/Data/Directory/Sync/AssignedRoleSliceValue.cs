using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class AssignedRoleSliceValue
	{
		[XmlAttribute]
		public string RoleName
		{
			get
			{
				return this.roleNameField;
			}
			set
			{
				this.roleNameField = value;
			}
		}

		[XmlAttribute]
		public int SliceId
		{
			get
			{
				return this.sliceIdField;
			}
			set
			{
				this.sliceIdField = value;
			}
		}

		private string roleNameField;

		private int sliceIdField;
	}
}
