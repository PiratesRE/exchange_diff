using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConnectorValidationFailedException : LocalizedException
	{
		public ConnectorValidationFailedException() : base(Strings.ConnectorValidationFailedId)
		{
		}

		public ConnectorValidationFailedException(Exception innerException) : base(Strings.ConnectorValidationFailedId, innerException)
		{
		}

		protected ConnectorValidationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
