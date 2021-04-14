using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[Serializable]
	public class ContextMoveWatermarksValue
	{
		[XmlElement(DataType = "base64Binary")]
		public byte[] Source
		{
			get
			{
				return this.sourceField;
			}
			set
			{
				this.sourceField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] Target
		{
			get
			{
				return this.targetField;
			}
			set
			{
				this.targetField = value;
			}
		}

		[XmlArrayItem("SourceSubscriberFilterVersion", IsNullable = false)]
		public ContextMoveWatermarksValueSourceSubscriberFilterVersion[] SourceSubscriberFilterVersions
		{
			get
			{
				return this.sourceSubscriberFilterVersionsField;
			}
			set
			{
				this.sourceSubscriberFilterVersionsField = value;
			}
		}

		private byte[] sourceField;

		private byte[] targetField;

		private ContextMoveWatermarksValueSourceSubscriberFilterVersion[] sourceSubscriberFilterVersionsField;
	}
}
