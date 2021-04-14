using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Deployment
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OrganizationUpgradeException : LocalizedException
	{
		public OrganizationUpgradeException(LocalizedString message) : base(message)
		{
		}

		public OrganizationUpgradeException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected OrganizationUpgradeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
