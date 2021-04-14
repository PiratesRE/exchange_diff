using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DatabaseGuidNotSuppliedException : LocalizedException
	{
		public DatabaseGuidNotSuppliedException() : base(Strings.DatabaseGuidNotSupplied)
		{
		}

		public DatabaseGuidNotSuppliedException(Exception innerException) : base(Strings.DatabaseGuidNotSupplied, innerException)
		{
		}

		protected DatabaseGuidNotSuppliedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
