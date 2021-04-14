using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SUC_CouldnotRetreivePasswd : LocalizedException
	{
		public SUC_CouldnotRetreivePasswd() : base(Strings.CouldnotRetreivePasswd)
		{
		}

		public SUC_CouldnotRetreivePasswd(Exception innerException) : base(Strings.CouldnotRetreivePasswd, innerException)
		{
		}

		protected SUC_CouldnotRetreivePasswd(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
