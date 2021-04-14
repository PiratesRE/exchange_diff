using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CorruptFilterRuleException : LocalizedException
	{
		public CorruptFilterRuleException(LocalizedString message) : base(message)
		{
		}

		public CorruptFilterRuleException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected CorruptFilterRuleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
