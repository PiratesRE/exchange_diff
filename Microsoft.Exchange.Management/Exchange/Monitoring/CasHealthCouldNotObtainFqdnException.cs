using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthCouldNotObtainFqdnException : LocalizedException
	{
		public CasHealthCouldNotObtainFqdnException() : base(Strings.CasHealthCouldNotObtainFqdn)
		{
		}

		public CasHealthCouldNotObtainFqdnException(Exception innerException) : base(Strings.CasHealthCouldNotObtainFqdn, innerException)
		{
		}

		protected CasHealthCouldNotObtainFqdnException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
