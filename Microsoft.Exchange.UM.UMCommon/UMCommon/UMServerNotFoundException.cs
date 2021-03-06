using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UMServerNotFoundException : LocalizedException
	{
		public UMServerNotFoundException(LocalizedString message) : base(message)
		{
		}

		public UMServerNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected UMServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
