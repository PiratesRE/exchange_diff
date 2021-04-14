using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DatabaseNotAttachedReadOnlyException : LocalizedException
	{
		public DatabaseNotAttachedReadOnlyException() : base(Strings.DatabaseNotAttachedReadOnly)
		{
		}

		public DatabaseNotAttachedReadOnlyException(Exception innerException) : base(Strings.DatabaseNotAttachedReadOnly, innerException)
		{
		}

		protected DatabaseNotAttachedReadOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
