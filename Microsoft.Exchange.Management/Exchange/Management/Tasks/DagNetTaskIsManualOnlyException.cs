using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagNetTaskIsManualOnlyException : LocalizedException
	{
		public DagNetTaskIsManualOnlyException(string taskName, string dagName) : base(Strings.DagNetTaskIsManualOnly(taskName, dagName))
		{
			this.taskName = taskName;
			this.dagName = dagName;
		}

		public DagNetTaskIsManualOnlyException(string taskName, string dagName, Exception innerException) : base(Strings.DagNetTaskIsManualOnly(taskName, dagName), innerException)
		{
			this.taskName = taskName;
			this.dagName = dagName;
		}

		protected DagNetTaskIsManualOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.taskName = (string)info.GetValue("taskName", typeof(string));
			this.dagName = (string)info.GetValue("dagName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("taskName", this.taskName);
			info.AddValue("dagName", this.dagName);
		}

		public string TaskName
		{
			get
			{
				return this.taskName;
			}
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		private readonly string taskName;

		private readonly string dagName;
	}
}
