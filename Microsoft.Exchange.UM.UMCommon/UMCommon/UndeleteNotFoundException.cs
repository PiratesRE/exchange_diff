using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UndeleteNotFoundException : LocalizedException
	{
		public UndeleteNotFoundException() : base(Strings.UndeleteNotFound)
		{
		}

		public UndeleteNotFoundException(Exception innerException) : base(Strings.UndeleteNotFound, innerException)
		{
		}

		protected UndeleteNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
