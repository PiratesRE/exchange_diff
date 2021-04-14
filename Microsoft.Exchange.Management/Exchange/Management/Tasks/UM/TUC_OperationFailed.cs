using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TUC_OperationFailed : LocalizedException
	{
		public TUC_OperationFailed(string operation) : base(Strings.OperationFailed(operation))
		{
			this.operation = operation;
		}

		public TUC_OperationFailed(string operation, Exception innerException) : base(Strings.OperationFailed(operation), innerException)
		{
			this.operation = operation;
		}

		protected TUC_OperationFailed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operation = (string)info.GetValue("operation", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operation", this.operation);
		}

		public string Operation
		{
			get
			{
				return this.operation;
			}
		}

		private readonly string operation;
	}
}
