using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class ServiceInstanceMoveTask
	{
		[XmlAttribute]
		public string ContextId
		{
			get
			{
				return this.contextIdField;
			}
			set
			{
				this.contextIdField = value;
			}
		}

		[XmlAttribute]
		public string TaskId
		{
			get
			{
				return this.taskIdField;
			}
			set
			{
				this.taskIdField = value;
			}
		}

		[XmlAttribute]
		public string OldServiceInstance
		{
			get
			{
				return this.oldServiceInstanceField;
			}
			set
			{
				this.oldServiceInstanceField = value;
			}
		}

		[XmlAttribute]
		public string NewServiceInstance
		{
			get
			{
				return this.newServiceInstanceField;
			}
			set
			{
				this.newServiceInstanceField = value;
			}
		}

		[XmlAttribute]
		public ServiceInstanceMoveTaskStatusCode StatusCode
		{
			get
			{
				return this.statusCodeField;
			}
			set
			{
				this.statusCodeField = value;
			}
		}

		[XmlAttribute]
		public string TaskFailureReason
		{
			get
			{
				return this.taskFailureReasonField;
			}
			set
			{
				this.taskFailureReasonField = value;
			}
		}

		private string contextIdField;

		private string taskIdField;

		private string oldServiceInstanceField;

		private string newServiceInstanceField;

		private ServiceInstanceMoveTaskStatusCode statusCodeField;

		private string taskFailureReasonField;
	}
}
