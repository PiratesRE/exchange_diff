using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ErrorInvalidPolicyCenterSiteOwnerException : LocalizedException
	{
		public ErrorInvalidPolicyCenterSiteOwnerException() : base(Strings.ErrorInvalidPolicyCenterSiteOwner)
		{
		}

		public ErrorInvalidPolicyCenterSiteOwnerException(Exception innerException) : base(Strings.ErrorInvalidPolicyCenterSiteOwner, innerException)
		{
		}

		protected ErrorInvalidPolicyCenterSiteOwnerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
