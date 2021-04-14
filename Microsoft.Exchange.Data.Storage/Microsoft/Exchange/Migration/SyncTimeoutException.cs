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
	internal class SyncTimeoutException : MigrationPermanentException
	{
		public SyncTimeoutException(string timespan) : base(Strings.SyncTimeOutFailure(timespan))
		{
			this.timespan = timespan;
		}

		public SyncTimeoutException(string timespan, Exception innerException) : base(Strings.SyncTimeOutFailure(timespan), innerException)
		{
			this.timespan = timespan;
		}

		protected SyncTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.timespan = (string)info.GetValue("timespan", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("timespan", this.timespan);
		}

		public string Timespan
		{
			get
			{
				return this.timespan;
			}
		}

		private readonly string timespan;
	}
}
