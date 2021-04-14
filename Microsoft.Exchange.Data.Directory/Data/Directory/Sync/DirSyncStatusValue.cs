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
	public class DirSyncStatusValue
	{
		[XmlAttribute]
		public string AttributeSetName
		{
			get
			{
				return this.attributeSetNameField;
			}
			set
			{
				this.attributeSetNameField = value;
			}
		}

		[XmlAttribute]
		public DirSyncState State
		{
			get
			{
				return this.stateField;
			}
			set
			{
				this.stateField = value;
			}
		}

		[XmlAttribute(DataType = "positiveInteger")]
		public string Version
		{
			get
			{
				return this.versionField;
			}
			set
			{
				this.versionField = value;
			}
		}

		private string attributeSetNameField;

		private DirSyncState stateField;

		private string versionField;
	}
}
