using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NullInstanceParameterException : LocalizedException
	{
		public NullInstanceParameterException() : base(Strings.ExceptionNullInstanceParameter)
		{
		}

		public NullInstanceParameterException(Exception innerException) : base(Strings.ExceptionNullInstanceParameter, innerException)
		{
		}

		protected NullInstanceParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
