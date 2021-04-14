using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TaskOperationFailedWithEcException : TaskServerException
	{
		public TaskOperationFailedWithEcException(int ec) : base(ServerStrings.TaskOperationFailedWithEcException(ec))
		{
			this.ec = ec;
		}

		public TaskOperationFailedWithEcException(int ec, Exception innerException) : base(ServerStrings.TaskOperationFailedWithEcException(ec), innerException)
		{
			this.ec = ec;
		}

		protected TaskOperationFailedWithEcException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ec = (int)info.GetValue("ec", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ec", this.ec);
		}

		public int Ec
		{
			get
			{
				return this.ec;
			}
		}

		private readonly int ec;
	}
}
