using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidExternalSharingInitiatorException : StoragePermanentException
	{
		public InvalidExternalSharingInitiatorException(string initiator, string sender) : base(ServerStrings.InvalidExternalSharingInitiatorException(initiator, sender))
		{
			this.initiator = initiator;
			this.sender = sender;
		}

		public InvalidExternalSharingInitiatorException(string initiator, string sender, Exception innerException) : base(ServerStrings.InvalidExternalSharingInitiatorException(initiator, sender), innerException)
		{
			this.initiator = initiator;
			this.sender = sender;
		}

		protected InvalidExternalSharingInitiatorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.initiator = (string)info.GetValue("initiator", typeof(string));
			this.sender = (string)info.GetValue("sender", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("initiator", this.initiator);
			info.AddValue("sender", this.sender);
		}

		public string Initiator
		{
			get
			{
				return this.initiator;
			}
		}

		public string Sender
		{
			get
			{
				return this.sender;
			}
		}

		private readonly string initiator;

		private readonly string sender;
	}
}
