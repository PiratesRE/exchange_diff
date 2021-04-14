using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class E4eLicensingIsDisabledExceptionCreateRule : LocalizedException
	{
		public E4eLicensingIsDisabledExceptionCreateRule() : base(Strings.E4eLicensingIsDisabledCreateRule)
		{
		}

		public E4eLicensingIsDisabledExceptionCreateRule(Exception innerException) : base(Strings.E4eLicensingIsDisabledCreateRule, innerException)
		{
		}

		protected E4eLicensingIsDisabledExceptionCreateRule(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
