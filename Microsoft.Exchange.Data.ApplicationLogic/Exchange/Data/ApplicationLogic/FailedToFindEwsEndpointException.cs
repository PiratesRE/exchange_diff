using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToFindEwsEndpointException : AuditLogException
	{
		public FailedToFindEwsEndpointException(string mailbox) : base(Strings.FailedToFindEwsEndpoint(mailbox))
		{
			this.mailbox = mailbox;
		}

		public FailedToFindEwsEndpointException(string mailbox, Exception innerException) : base(Strings.FailedToFindEwsEndpoint(mailbox), innerException)
		{
			this.mailbox = mailbox;
		}

		protected FailedToFindEwsEndpointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailbox = (string)info.GetValue("mailbox", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailbox", this.mailbox);
		}

		public string Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		private readonly string mailbox;
	}
}
