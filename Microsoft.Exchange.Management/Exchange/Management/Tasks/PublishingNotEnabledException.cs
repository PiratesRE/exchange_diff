using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PublishingNotEnabledException : LocalizedException
	{
		public PublishingNotEnabledException() : base(Strings.PublishingNotEnabled)
		{
		}

		public PublishingNotEnabledException(Exception innerException) : base(Strings.PublishingNotEnabled, innerException)
		{
		}

		protected PublishingNotEnabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
