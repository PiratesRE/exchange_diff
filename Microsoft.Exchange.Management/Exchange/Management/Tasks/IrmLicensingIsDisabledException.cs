using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IrmLicensingIsDisabledException : LocalizedException
	{
		public IrmLicensingIsDisabledException() : base(Strings.IrmLicensingIsDisabled)
		{
		}

		public IrmLicensingIsDisabledException(Exception innerException) : base(Strings.IrmLicensingIsDisabled, innerException)
		{
		}

		protected IrmLicensingIsDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
