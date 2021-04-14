using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NullInstanceIdentityParameterException : LocalizedException
	{
		public NullInstanceIdentityParameterException() : base(Strings.ErrorInstanceObjectConatinsNullIdentity)
		{
		}

		public NullInstanceIdentityParameterException(Exception innerException) : base(Strings.ErrorInstanceObjectConatinsNullIdentity, innerException)
		{
		}

		protected NullInstanceIdentityParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
