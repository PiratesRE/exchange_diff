using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NoSharingHandlerFoundException : StoragePermanentException
	{
		public NoSharingHandlerFoundException(string recipient) : base(ServerStrings.NoSharingHandlerFoundException(recipient))
		{
			this.recipient = recipient;
		}

		public NoSharingHandlerFoundException(string recipient, Exception innerException) : base(ServerStrings.NoSharingHandlerFoundException(recipient), innerException)
		{
			this.recipient = recipient;
		}

		protected NoSharingHandlerFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.recipient = (string)info.GetValue("recipient", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("recipient", this.recipient);
		}

		public string Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		private readonly string recipient;
	}
}
