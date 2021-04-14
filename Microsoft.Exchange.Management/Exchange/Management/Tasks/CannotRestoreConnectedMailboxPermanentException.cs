﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotRestoreConnectedMailboxPermanentException : MailboxReplicationPermanentException
	{
		public CannotRestoreConnectedMailboxPermanentException(string identity) : base(Strings.ErrorCannotRestoreFromConnectedMailbox(identity))
		{
			this.identity = identity;
		}

		public CannotRestoreConnectedMailboxPermanentException(string identity, Exception innerException) : base(Strings.ErrorCannotRestoreFromConnectedMailbox(identity), innerException)
		{
			this.identity = identity;
		}

		protected CannotRestoreConnectedMailboxPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
