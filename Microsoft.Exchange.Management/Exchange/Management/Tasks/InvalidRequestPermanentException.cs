using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidRequestPermanentException : MailboxReplicationPermanentException
	{
		public InvalidRequestPermanentException(string identity, string validationMessage) : base(Strings.ErrorInvalidRequest(identity, validationMessage))
		{
			this.identity = identity;
			this.validationMessage = validationMessage;
		}

		public InvalidRequestPermanentException(string identity, string validationMessage, Exception innerException) : base(Strings.ErrorInvalidRequest(identity, validationMessage), innerException)
		{
			this.identity = identity;
			this.validationMessage = validationMessage;
		}

		protected InvalidRequestPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
			this.validationMessage = (string)info.GetValue("validationMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
			info.AddValue("validationMessage", this.validationMessage);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		public string ValidationMessage
		{
			get
			{
				return this.validationMessage;
			}
		}

		private readonly string identity;

		private readonly string validationMessage;
	}
}
