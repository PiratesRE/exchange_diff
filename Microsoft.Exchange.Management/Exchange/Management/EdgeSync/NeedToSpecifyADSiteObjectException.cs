using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NeedToSpecifyADSiteObjectException : LocalizedException
	{
		public NeedToSpecifyADSiteObjectException() : base(Strings.NeedToSpecifyADSiteObject)
		{
		}

		public NeedToSpecifyADSiteObjectException(Exception innerException) : base(Strings.NeedToSpecifyADSiteObject, innerException)
		{
		}

		protected NeedToSpecifyADSiteObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
