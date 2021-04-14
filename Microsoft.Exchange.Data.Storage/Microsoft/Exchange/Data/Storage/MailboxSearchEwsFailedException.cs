using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MailboxSearchEwsFailedException : StoragePermanentException
	{
		public MailboxSearchEwsFailedException(string error) : base(ServerStrings.MailboxSearchEwsFailedExceptionWithError(error))
		{
			this.error = error;
		}

		public MailboxSearchEwsFailedException(string error, Exception innerException) : base(ServerStrings.MailboxSearchEwsFailedExceptionWithError(error), innerException)
		{
			this.error = error;
		}

		protected MailboxSearchEwsFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
