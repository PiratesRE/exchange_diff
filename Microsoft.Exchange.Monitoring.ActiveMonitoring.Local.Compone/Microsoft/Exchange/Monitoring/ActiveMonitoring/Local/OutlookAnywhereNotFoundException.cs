using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class OutlookAnywhereNotFoundException : LocalizedException
	{
		public OutlookAnywhereNotFoundException() : base(Strings.RcaDiscoveryOutlookAnywhereNotFound)
		{
		}

		public OutlookAnywhereNotFoundException(Exception innerException) : base(Strings.RcaDiscoveryOutlookAnywhereNotFound, innerException)
		{
		}

		protected OutlookAnywhereNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
