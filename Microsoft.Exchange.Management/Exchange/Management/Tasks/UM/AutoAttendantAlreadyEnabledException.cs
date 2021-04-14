using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AutoAttendantAlreadyEnabledException : LocalizedException
	{
		public AutoAttendantAlreadyEnabledException() : base(Strings.AAAlreadyEnabled)
		{
		}

		public AutoAttendantAlreadyEnabledException(Exception innerException) : base(Strings.AAAlreadyEnabled, innerException)
		{
		}

		protected AutoAttendantAlreadyEnabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
