using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UMRPCIncompabibleVersionException : LocalizedException
	{
		public UMRPCIncompabibleVersionException() : base(Strings.UMRPCIncompatibleVersionException)
		{
		}

		public UMRPCIncompabibleVersionException(Exception innerException) : base(Strings.UMRPCIncompatibleVersionException, innerException)
		{
		}

		protected UMRPCIncompabibleVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
