using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidReceiveMeetingMessageCopiesException : StoragePermanentException
	{
		public InvalidReceiveMeetingMessageCopiesException(string delegateUser) : base(ServerStrings.InvalidReceiveMeetingMessageCopiesException(delegateUser))
		{
			this.delegateUser = delegateUser;
		}

		public InvalidReceiveMeetingMessageCopiesException(string delegateUser, Exception innerException) : base(ServerStrings.InvalidReceiveMeetingMessageCopiesException(delegateUser), innerException)
		{
			this.delegateUser = delegateUser;
		}

		protected InvalidReceiveMeetingMessageCopiesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.delegateUser = (string)info.GetValue("delegateUser", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("delegateUser", this.delegateUser);
		}

		public string DelegateUser
		{
			get
			{
				return this.delegateUser;
			}
		}

		private readonly string delegateUser;
	}
}
