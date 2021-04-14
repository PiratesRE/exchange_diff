using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnexpectedSubscriptionStatusException : MigrationPermanentException
	{
		public UnexpectedSubscriptionStatusException(string status) : base(Strings.UnexpectedSubscriptionStatus(status))
		{
			this.status = status;
		}

		public UnexpectedSubscriptionStatusException(string status, Exception innerException) : base(Strings.UnexpectedSubscriptionStatus(status), innerException)
		{
			this.status = status;
		}

		protected UnexpectedSubscriptionStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.status = (string)info.GetValue("status", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("status", this.status);
		}

		public string Status
		{
			get
			{
				return this.status;
			}
		}

		private readonly string status;
	}
}
