using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.Online.Administration
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ValidationError", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class ValidationError : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public XmlElement ErrorDetail
		{
			get
			{
				return this.ErrorDetailField;
			}
			set
			{
				this.ErrorDetailField = value;
			}
		}

		[DataMember]
		public bool Resolved
		{
			get
			{
				return this.ResolvedField;
			}
			set
			{
				this.ResolvedField = value;
			}
		}

		[DataMember]
		public string ServiceInstance
		{
			get
			{
				return this.ServiceInstanceField;
			}
			set
			{
				this.ServiceInstanceField = value;
			}
		}

		[DataMember]
		public DateTime Timestamp
		{
			get
			{
				return this.TimestampField;
			}
			set
			{
				this.TimestampField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private XmlElement ErrorDetailField;

		private bool ResolvedField;

		private string ServiceInstanceField;

		private DateTime TimestampField;
	}
}
