using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotSetCancelledRequestPermanentException : MailboxReplicationPermanentException
	{
		public CannotSetCancelledRequestPermanentException(string identity) : base(Strings.ErrorRequestAlreadyCanceled(identity))
		{
			this.identity = identity;
		}

		public CannotSetCancelledRequestPermanentException(string identity, Exception innerException) : base(Strings.ErrorRequestAlreadyCanceled(identity), innerException)
		{
			this.identity = identity;
		}

		protected CannotSetCancelledRequestPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly string identity;
	}
}
