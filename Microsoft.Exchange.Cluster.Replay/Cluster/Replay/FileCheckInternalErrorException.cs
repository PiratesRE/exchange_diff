using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckInternalErrorException : FileCheckException
	{
		public FileCheckInternalErrorException(string condition) : base(ReplayStrings.FileCheckInternalError(condition))
		{
			this.condition = condition;
		}

		public FileCheckInternalErrorException(string condition, Exception innerException) : base(ReplayStrings.FileCheckInternalError(condition), innerException)
		{
			this.condition = condition;
		}

		protected FileCheckInternalErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.condition = (string)info.GetValue("condition", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("condition", this.condition);
		}

		public string Condition
		{
			get
			{
				return this.condition;
			}
		}

		private readonly string condition;
	}
}
