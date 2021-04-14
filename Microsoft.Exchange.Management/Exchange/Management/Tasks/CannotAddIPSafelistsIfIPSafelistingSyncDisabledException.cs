using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotAddIPSafelistsIfIPSafelistingSyncDisabledException : LocalizedException
	{
		public CannotAddIPSafelistsIfIPSafelistingSyncDisabledException() : base(Strings.CannotAddIPSafelistsIfIPSafelistingSyncDisabledId)
		{
		}

		public CannotAddIPSafelistsIfIPSafelistingSyncDisabledException(Exception innerException) : base(Strings.CannotAddIPSafelistsIfIPSafelistingSyncDisabledId, innerException)
		{
		}

		protected CannotAddIPSafelistsIfIPSafelistingSyncDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
