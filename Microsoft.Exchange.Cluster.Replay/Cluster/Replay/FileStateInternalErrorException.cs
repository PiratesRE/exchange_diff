using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileStateInternalErrorException : FileCheckException
	{
		public FileStateInternalErrorException(string condition) : base(ReplayStrings.FileStateInternalError(condition))
		{
			this.condition = condition;
		}

		public FileStateInternalErrorException(string condition, Exception innerException) : base(ReplayStrings.FileStateInternalError(condition), innerException)
		{
			this.condition = condition;
		}

		protected FileStateInternalErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
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
