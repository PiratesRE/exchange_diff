using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SourceDirNotSpecifiedException : LocalizedException
	{
		public SourceDirNotSpecifiedException() : base(Strings.SourceDirNotSpecifiedError)
		{
		}

		public SourceDirNotSpecifiedException(Exception innerException) : base(Strings.SourceDirNotSpecifiedError, innerException)
		{
		}

		protected SourceDirNotSpecifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
