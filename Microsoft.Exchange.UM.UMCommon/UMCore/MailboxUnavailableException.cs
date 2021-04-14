using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MailboxUnavailableException : TransientException
	{
		public MailboxUnavailableException(string messageType, string database, string exceptionMessage) : base(Strings.MailboxUnavailableException(messageType, database, exceptionMessage))
		{
			this.messageType = messageType;
			this.database = database;
			this.exceptionMessage = exceptionMessage;
		}

		public MailboxUnavailableException(string messageType, string database, string exceptionMessage, Exception innerException) : base(Strings.MailboxUnavailableException(messageType, database, exceptionMessage), innerException)
		{
			this.messageType = messageType;
			this.database = database;
			this.exceptionMessage = exceptionMessage;
		}

		protected MailboxUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.messageType = (string)info.GetValue("messageType", typeof(string));
			this.database = (string)info.GetValue("database", typeof(string));
			this.exceptionMessage = (string)info.GetValue("exceptionMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("messageType", this.messageType);
			info.AddValue("database", this.database);
			info.AddValue("exceptionMessage", this.exceptionMessage);
		}

		public string MessageType
		{
			get
			{
				return this.messageType;
			}
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		public string ExceptionMessage
		{
			get
			{
				return this.exceptionMessage;
			}
		}

		private readonly string messageType;

		private readonly string database;

		private readonly string exceptionMessage;
	}
}
