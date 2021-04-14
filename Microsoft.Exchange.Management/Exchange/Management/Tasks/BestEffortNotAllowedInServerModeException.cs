using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BestEffortNotAllowedInServerModeException : LocalizedException
	{
		public BestEffortNotAllowedInServerModeException() : base(Strings.BestEffortNotAllowedInServerModeException)
		{
		}

		public BestEffortNotAllowedInServerModeException(Exception innerException) : base(Strings.BestEffortNotAllowedInServerModeException, innerException)
		{
		}

		protected BestEffortNotAllowedInServerModeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
