using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotFindFirstOrgPerimeterConfigObjectException : LocalizedException
	{
		public CouldNotFindFirstOrgPerimeterConfigObjectException() : base(Strings.CouldNotFindFirstOrgPerimeterConfigObjectId)
		{
		}

		public CouldNotFindFirstOrgPerimeterConfigObjectException(Exception innerException) : base(Strings.CouldNotFindFirstOrgPerimeterConfigObjectId, innerException)
		{
		}

		protected CouldNotFindFirstOrgPerimeterConfigObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
