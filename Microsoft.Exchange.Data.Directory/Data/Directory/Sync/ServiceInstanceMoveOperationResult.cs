using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class ServiceInstanceMoveOperationResult
	{
		[XmlElement(IsNullable = true, Order = 0)]
		public ServiceInstanceMoveTask ServiceInstanceMoveTask
		{
			get
			{
				return this.serviceInstanceMoveTaskField;
			}
			set
			{
				this.serviceInstanceMoveTaskField = value;
			}
		}

		[XmlAttribute]
		public int OperationStatusCode
		{
			get
			{
				return this.operationStatusCodeField;
			}
			set
			{
				this.operationStatusCodeField = value;
			}
		}

		[XmlAttribute]
		public string ErrorMessage
		{
			get
			{
				return this.errorMessageField;
			}
			set
			{
				this.errorMessageField = value;
			}
		}

		private ServiceInstanceMoveTask serviceInstanceMoveTaskField;

		private int operationStatusCodeField;

		private string errorMessageField;
	}
}
