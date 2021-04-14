using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ForestWideOrganizationException : LocalizedException
	{
		public ForestWideOrganizationException() : base(Strings.ErrorNeedOrganizationId)
		{
		}

		public ForestWideOrganizationException(Exception innerException) : base(Strings.ErrorNeedOrganizationId, innerException)
		{
		}

		protected ForestWideOrganizationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
