using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SystemMailboxGuidNotFoundException : LocalizedException
	{
		public SystemMailboxGuidNotFoundException() : base(Strings.SystemMailboxGuidNotFound)
		{
		}

		public SystemMailboxGuidNotFoundException(Exception innerException) : base(Strings.SystemMailboxGuidNotFound, innerException)
		{
		}

		protected SystemMailboxGuidNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
