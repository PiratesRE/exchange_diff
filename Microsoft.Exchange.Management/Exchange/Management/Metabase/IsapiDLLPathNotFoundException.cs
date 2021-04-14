using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IsapiDLLPathNotFoundException : LocalizedException
	{
		public IsapiDLLPathNotFoundException() : base(Strings.IsapiDLLPathNotFoundException)
		{
		}

		public IsapiDLLPathNotFoundException(Exception innerException) : base(Strings.IsapiDLLPathNotFoundException, innerException)
		{
		}

		protected IsapiDLLPathNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
