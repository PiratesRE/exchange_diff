using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SuspendWhenReadyToCompleteCannotBeUsedOnAutoResumeJobsException : MailboxReplicationPermanentException
	{
		public SuspendWhenReadyToCompleteCannotBeUsedOnAutoResumeJobsException(string name) : base(Strings.ErrorSuspendWhenReadyToCompleteCannotBeUsedOnAutoResumeJobs(name))
		{
			this.name = name;
		}

		public SuspendWhenReadyToCompleteCannotBeUsedOnAutoResumeJobsException(string name, Exception innerException) : base(Strings.ErrorSuspendWhenReadyToCompleteCannotBeUsedOnAutoResumeJobs(name), innerException)
		{
			this.name = name;
		}

		protected SuspendWhenReadyToCompleteCannotBeUsedOnAutoResumeJobsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
