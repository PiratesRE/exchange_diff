using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxSearchTaskException : LocalizedException
	{
		public MailboxSearchTaskException(string failure) : base(Strings.MailboxSearchTaskException(failure))
		{
			this.failure = failure;
		}

		public MailboxSearchTaskException(string failure, Exception innerException) : base(Strings.MailboxSearchTaskException(failure), innerException)
		{
			this.failure = failure;
		}

		protected MailboxSearchTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.failure = (string)info.GetValue("failure", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("failure", this.failure);
		}

		public string Failure
		{
			get
			{
				return this.failure;
			}
		}

		private readonly string failure;
	}
}
