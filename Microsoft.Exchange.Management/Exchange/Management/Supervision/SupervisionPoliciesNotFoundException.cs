using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Supervision
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SupervisionPoliciesNotFoundException : LocalizedException
	{
		public SupervisionPoliciesNotFoundException(LocalizedString message) : base(message)
		{
		}

		public SupervisionPoliciesNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SupervisionPoliciesNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
