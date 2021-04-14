using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SIPResouceIdNotUniqueException : LocalizedException
	{
		public SIPResouceIdNotUniqueException() : base(Strings.ExceptionSipResourceIdNotUnique)
		{
		}

		public SIPResouceIdNotUniqueException(Exception innerException) : base(Strings.ExceptionSipResourceIdNotUnique, innerException)
		{
		}

		protected SIPResouceIdNotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
