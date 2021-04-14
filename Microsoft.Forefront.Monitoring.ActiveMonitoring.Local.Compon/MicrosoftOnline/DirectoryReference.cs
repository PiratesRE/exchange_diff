using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlInclude(typeof(DirectoryReferenceContact))]
	[DebuggerStepThrough]
	[XmlInclude(typeof(DirectoryReferenceAny))]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[XmlInclude(typeof(DirectoryReferenceAddressList))]
	[DesignerCategory("code")]
	[XmlInclude(typeof(DirectoryReferenceServicePlan))]
	[Serializable]
	public abstract class DirectoryReference
	{
		public DirectoryReference()
		{
			this.targetDeletedField = false;
		}

		[DefaultValue(false)]
		[XmlAttribute]
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
