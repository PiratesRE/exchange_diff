using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.GUI
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PartiallyConfiguredException : LocalizedException
	{
		public PartiallyConfiguredException() : base(Strings.PartiallyConfiguredCannotRunUnInstall)
		{
		}

		public PartiallyConfiguredException(Exception innerException) : base(Strings.PartiallyConfiguredCannotRunUnInstall, innerException)
		{
		}

		protected PartiallyConfiguredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
