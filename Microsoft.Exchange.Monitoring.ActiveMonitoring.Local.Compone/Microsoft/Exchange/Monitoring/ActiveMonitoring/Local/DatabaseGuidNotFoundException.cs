using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DatabaseGuidNotFoundException : LocalizedException
	{
		public DatabaseGuidNotFoundException() : base(Strings.DatabaseGuidNotFound)
		{
		}

		public DatabaseGuidNotFoundException(Exception innerException) : base(Strings.DatabaseGuidNotFound, innerException)
		{
		}

		protected DatabaseGuidNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
