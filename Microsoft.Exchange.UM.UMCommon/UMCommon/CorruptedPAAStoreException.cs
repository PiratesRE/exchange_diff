using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CorruptedPAAStoreException : LocalizedException
	{
		public CorruptedPAAStoreException() : base(Strings.CorruptedPAAStore)
		{
		}

		public CorruptedPAAStoreException(Exception innerException) : base(Strings.CorruptedPAAStore, innerException)
		{
		}

		protected CorruptedPAAStoreException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
