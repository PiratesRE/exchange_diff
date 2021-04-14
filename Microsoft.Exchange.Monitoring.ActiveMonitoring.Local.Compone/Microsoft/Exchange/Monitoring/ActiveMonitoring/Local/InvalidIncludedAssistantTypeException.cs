using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidIncludedAssistantTypeException : LocalizedException
	{
		public InvalidIncludedAssistantTypeException() : base(Strings.InvalidIncludedAssistantType)
		{
		}

		public InvalidIncludedAssistantTypeException(Exception innerException) : base(Strings.InvalidIncludedAssistantType, innerException)
		{
		}

		protected InvalidIncludedAssistantTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
