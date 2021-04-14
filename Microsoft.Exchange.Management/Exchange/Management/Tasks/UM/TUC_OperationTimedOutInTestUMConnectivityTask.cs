using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TUC_OperationTimedOutInTestUMConnectivityTask : LocalizedException
	{
		public TUC_OperationTimedOutInTestUMConnectivityTask(string operation, string timeout) : base(Strings.OperationTimedOutInTestUMConnectivityTask(operation, timeout))
		{
			this.operation = operation;
			this.timeout = timeout;
		}

		public TUC_OperationTimedOutInTestUMConnectivityTask(string operation, string timeout, Exception innerException) : base(Strings.OperationTimedOutInTestUMConnectivityTask(operation, timeout), innerException)
		{
			this.operation = operation;
			this.timeout = timeout;
		}

		protected TUC_OperationTimedOutInTestUMConnectivityTask(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operation = (string)info.GetValue("operation", typeof(string));
			this.timeout = (string)info.GetValue("timeout", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operation", this.operation);
			info.AddValue("timeout", this.timeout);
		}

		public string Operation
		{
			get
			{
				return this.operation;
			}
		}

		public string Timeout
		{
			get
			{
				return this.timeout;
			}
		}

		private readonly string operation;

		private readonly string timeout;
	}
}
