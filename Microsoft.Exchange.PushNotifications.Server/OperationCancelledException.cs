using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.PushNotifications.Server.LocStrings;

namespace Microsoft.Exchange.PushNotifications.Server
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OperationCancelledException : PushNotificationTransientException
	{
		public OperationCancelledException(string command) : base(Strings.OperationCancelled(command))
		{
			this.command = command;
		}

		public OperationCancelledException(string command, Exception innerException) : base(Strings.OperationCancelled(command), innerException)
		{
			this.command = command;
		}

		protected OperationCancelledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.command = (string)info.GetValue("command", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("command", this.command);
		}

		public string Command
		{
			get
			{
				return this.command;
			}
		}

		private readonly string command;
	}
}
