using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Supervision
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SupervisionPolicyTaskException : LocalizedException
	{
		public SupervisionPolicyTaskException(LocalizedString message) : base(message)
		{
		}

		public SupervisionPolicyTaskException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SupervisionPolicyTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
