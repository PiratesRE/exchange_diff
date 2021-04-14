using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IncrementalMergesRequireSuspendWhenReadyToCompleteException : MailboxReplicationPermanentException
	{
		public IncrementalMergesRequireSuspendWhenReadyToCompleteException(string name) : base(Strings.ErrorIncrementalMergesRequireSuspendWhenReadyToComplete(name))
		{
			this.name = name;
		}

		public IncrementalMergesRequireSuspendWhenReadyToCompleteException(string name, Exception innerException) : base(Strings.ErrorIncrementalMergesRequireSuspendWhenReadyToComplete(name), innerException)
		{
			this.name = name;
		}

		protected IncrementalMergesRequireSuspendWhenReadyToCompleteException(SerializationInfo info, StreamingContext context) : base(info, context)
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
