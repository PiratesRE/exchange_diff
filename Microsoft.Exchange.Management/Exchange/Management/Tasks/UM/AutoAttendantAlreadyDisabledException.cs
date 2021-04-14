using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AutoAttendantAlreadyDisabledException : LocalizedException
	{
		public AutoAttendantAlreadyDisabledException() : base(Strings.AAAlreadyDisabled)
		{
		}

		public AutoAttendantAlreadyDisabledException(Exception innerException) : base(Strings.AAAlreadyDisabled, innerException)
		{
		}

		protected AutoAttendantAlreadyDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
