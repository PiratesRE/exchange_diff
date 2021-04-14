using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TaskTotalFailureException : TaskException
	{
		public TaskTotalFailureException(Type taskType, Exception rollbackException) : base(Strings.ExceptionRollbackFailed(taskType, rollbackException))
		{
			this.taskType = taskType;
			this.rollbackException = rollbackException;
		}

		public TaskTotalFailureException(Type taskType, Exception rollbackException, Exception innerException) : base(Strings.ExceptionRollbackFailed(taskType, rollbackException), innerException)
		{
			this.taskType = taskType;
			this.rollbackException = rollbackException;
		}

		protected TaskTotalFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.taskType = (Type)info.GetValue("taskType", typeof(Type));
			this.rollbackException = (Exception)info.GetValue("rollbackException", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("taskType", this.taskType);
			info.AddValue("rollbackException", this.rollbackException);
		}

		public Type TaskType
		{
			get
			{
				return this.taskType;
			}
		}

		public Exception RollbackException
		{
			get
			{
				return this.rollbackException;
			}
		}

		private readonly Type taskType;

		private readonly Exception rollbackException;
	}
}
