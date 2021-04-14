using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class E4eLicensingIsDisabledExceptionSetRule : LocalizedException
	{
		public E4eLicensingIsDisabledExceptionSetRule() : base(Strings.E4eLicensingIsDisabledSetRule)
		{
		}

		public E4eLicensingIsDisabledExceptionSetRule(Exception innerException) : base(Strings.E4eLicensingIsDisabledSetRule, innerException)
		{
		}

		protected E4eLicensingIsDisabledExceptionSetRule(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
