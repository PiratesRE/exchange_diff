using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlInclude(typeof(DirectoryReferenceUserAndServicePrincipal))]
	[XmlInclude(typeof(DirectoryReferenceAddressList))]
	[XmlInclude(typeof(DirectoryReferenceAny))]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public abstract class DirectoryReference
	{
		public abstract DirectoryObjectClass GetTargetClass();

		public DirectoryReference()
		{
			this.targetDeletedField = false;
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool TargetDeleted
		{
			get
			{
				return this.targetDeletedField;
			}
			set
			{
				this.targetDeletedField = value;
			}
		}

		[XmlText]
		public string Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		private bool targetDeletedField;

		private string valueField;
	}
}
