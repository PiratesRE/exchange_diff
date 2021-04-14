using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SuspendWhenReadyToCompleteNotSupportedException : MailboxReplicationPermanentException
	{
		public SuspendWhenReadyToCompleteNotSupportedException(string name) : base(Strings.ErrorSuspendWhenReadyToCompleteNotSupported(name))
		{
			this.name = name;
		}

		public SuspendWhenReadyToCompleteNotSupportedException(string name, Exception innerException) : base(Strings.ErrorSuspendWhenReadyToCompleteNotSupported(name), innerException)
		{
			this.name = name;
		}

		protected SuspendWhenReadyToCompleteNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
