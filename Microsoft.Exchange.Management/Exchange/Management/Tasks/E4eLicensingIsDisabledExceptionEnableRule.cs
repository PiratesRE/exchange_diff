using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class E4eLicensingIsDisabledExceptionEnableRule : LocalizedException
	{
		public E4eLicensingIsDisabledExceptionEnableRule() : base(Strings.E4eLicensingIsDisabledEnableRule)
		{
		}

		public E4eLicensingIsDisabledExceptionEnableRule(Exception innerException) : base(Strings.E4eLicensingIsDisabledEnableRule, innerException)
		{
		}

		protected E4eLicensingIsDisabledExceptionEnableRule(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
