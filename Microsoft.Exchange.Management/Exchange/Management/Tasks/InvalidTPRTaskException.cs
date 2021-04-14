using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidTPRTaskException : LocalizedException
	{
		public InvalidTPRTaskException(string taskName) : base(Strings.InvalidTPRTask(taskName))
		{
			this.taskName = taskName;
		}

		public InvalidTPRTaskException(string taskName, Exception innerException) : base(Strings.InvalidTPRTask(taskName), innerException)
		{
			this.taskName = taskName;
		}

		protected InvalidTPRTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.taskName = (string)info.GetValue("taskName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("taskName", this.taskName);
		}

		public string TaskName
		{
			get
			{
				return this.taskName;
			}
		}

		private readonly string taskName;
	}
}
