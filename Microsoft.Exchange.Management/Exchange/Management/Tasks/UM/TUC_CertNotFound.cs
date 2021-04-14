using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TUC_CertNotFound : LocalizedException
	{
		public TUC_CertNotFound() : base(Strings.CertNotFound)
		{
		}

		public TUC_CertNotFound(Exception innerException) : base(Strings.CertNotFound, innerException)
		{
		}

		protected TUC_CertNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
