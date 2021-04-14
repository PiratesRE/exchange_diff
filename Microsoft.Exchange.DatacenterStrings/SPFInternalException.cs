using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.DatacenterStrings
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SPFInternalException : RecipientTaskException
	{
		public SPFInternalException(LocalizedString message) : base(message)
		{
		}

		public SPFInternalException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SPFInternalException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
