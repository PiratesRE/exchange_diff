using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NoInternalEwsAvailableException : StoragePermanentException
	{
		public NoInternalEwsAvailableException(string mailbox) : base(ServerStrings.NoInternalEwsAvailableException(mailbox))
		{
			this.mailbox = mailbox;
		}

		public NoInternalEwsAvailableException(string mailbox, Exception innerException) : base(ServerStrings.NoInternalEwsAvailableException(mailbox), innerException)
		{
			this.mailbox = mailbox;
		}

		protected NoInternalEwsAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
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
