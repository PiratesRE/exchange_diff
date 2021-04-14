using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.PushNotifications.Server.Commands;
using Microsoft.Exchange.PushNotifications.Server.LocStrings;

namespace Microsoft.Exchange.PushNotifications.Server
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToAcquireBudgetException : ServiceCommandPermanentException
	{
		public FailedToAcquireBudgetException(string windowsIdentity, string principal) : base(Strings.FailedToAcquireBudget(windowsIdentity, principal))
		{
			this.windowsIdentity = windowsIdentity;
			this.principal = principal;
		}

		public FailedToAcquireBudgetException(string windowsIdentity, string principal, Exception innerException) : base(Strings.FailedToAcquireBudget(windowsIdentity, principal), innerException)
		{
			this.windowsIdentity = windowsIdentity;
			this.principal = principal;
		}

		protected FailedToAcquireBudgetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.windowsIdentity = (string)info.GetValue("windowsIdentity", typeof(string));
			this.principal = (string)info.GetValue("principal", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("windowsIdentity", this.windowsIdentity);
			info.AddValue("principal", this.principal);
		}

		public string WindowsIdentity
		{
			get
			{
				return this.windowsIdentity;
			}
		}

		public string Principal
		{
			get
			{
				return this.principal;
			}
		}

		private readonly string windowsIdentity;

		private readonly string principal;
	}
}
