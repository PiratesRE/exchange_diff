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
	internal class MigrationItemLastUpdatedInTheFutureTransientException : MigrationTransientException
	{
		public MigrationItemLastUpdatedInTheFutureTransientException(string time) : base(Strings.MigrationItemLastUpdatedInTheFuture(time))
		{
			this.time = time;
		}

		public MigrationItemLastUpdatedInTheFutureTransientException(string time, Exception innerException) : base(Strings.MigrationItemLastUpdatedInTheFuture(time), innerException)
		{
			this.time = time;
		}

		protected MigrationItemLastUpdatedInTheFutureTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.time = (string)info.GetValue("time", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("time", this.time);
		}

		public string Time
		{
			get
			{
				return this.time;
			}
		}

		private readonly string time;
	}
}
