using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthMustSpecifyCasException : LocalizedException
	{
		public CasHealthMustSpecifyCasException() : base(Strings.CasHealthMustSpecifyCas)
		{
		}

		public CasHealthMustSpecifyCasException(Exception innerException) : base(Strings.CasHealthMustSpecifyCas, innerException)
		{
		}

		protected CasHealthMustSpecifyCasException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
