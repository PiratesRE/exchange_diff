using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TaskCanOnlyRunOnDacException : LocalizedException
	{
		public TaskCanOnlyRunOnDacException(string dag) : base(Strings.TaskCanOnlyRunOnDac(dag))
		{
			this.dag = dag;
		}

		public TaskCanOnlyRunOnDacException(string dag, Exception innerException) : base(Strings.TaskCanOnlyRunOnDac(dag), innerException)
		{
			this.dag = dag;
		}

		protected TaskCanOnlyRunOnDacException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dag = (string)info.GetValue("dag", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dag", this.dag);
		}

		public string Dag
		{
			get
			{
				return this.dag;
			}
		}

		private readonly string dag;
	}
}
