using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.PushNotifications.Server.LocStrings;

namespace Microsoft.Exchange.PushNotifications.Server
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceBusyException : PushNotificationTransientException
	{
		public ServiceBusyException(string command) : base(Strings.ServiceBusy(command))
		{
			this.command = command;
		}

		public ServiceBusyException(string command, Exception innerException) : base(Strings.ServiceBusy(command), innerException)
		{
			this.command = command;
		}

		protected ServiceBusyException(SerializationInfo info, StreamingContext context) : base(info, context)
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
