using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotGenerateExtensionException : LocalizedException
	{
		public CouldNotGenerateExtensionException() : base(Strings.ExceptionCouldNotGenerateExtension)
		{
		}

		public CouldNotGenerateExtensionException(Exception innerException) : base(Strings.ExceptionCouldNotGenerateExtension, innerException)
		{
		}

		protected CouldNotGenerateExtensionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
