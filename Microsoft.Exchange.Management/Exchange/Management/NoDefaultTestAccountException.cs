using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoDefaultTestAccountException : LocalizedException
	{
		public NoDefaultTestAccountException() : base(Strings.NoDefaultTestAccount)
		{
		}

		public NoDefaultTestAccountException(Exception innerException) : base(Strings.NoDefaultTestAccount, innerException)
		{
		}

		protected NoDefaultTestAccountException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
