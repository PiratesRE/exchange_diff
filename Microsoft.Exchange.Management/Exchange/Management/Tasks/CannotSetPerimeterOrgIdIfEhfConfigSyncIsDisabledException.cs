using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotSetPerimeterOrgIdIfEhfConfigSyncIsDisabledException : LocalizedException
	{
		public CannotSetPerimeterOrgIdIfEhfConfigSyncIsDisabledException() : base(Strings.CannotSetPerimeterOrgIdIfEhfConfigSyncIsDisabledId)
		{
		}

		public CannotSetPerimeterOrgIdIfEhfConfigSyncIsDisabledException(Exception innerException) : base(Strings.CannotSetPerimeterOrgIdIfEhfConfigSyncIsDisabledId, innerException)
		{
		}

		protected CannotSetPerimeterOrgIdIfEhfConfigSyncIsDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
