﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EasMoveFailedPermanentException : MailboxReplicationPermanentException
	{
		public EasMoveFailedPermanentException(string errorMessage) : base(MrsStrings.EasMoveFailed(errorMessage))
		{
			this.errorMessage = errorMessage;
		}

		public EasMoveFailedPermanentException(string errorMessage, Exception innerException) : base(MrsStrings.EasMoveFailed(errorMessage), innerException)
		{
			this.errorMessage = errorMessage;
		}

		protected EasMoveFailedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string errorMessage;
	}
}
