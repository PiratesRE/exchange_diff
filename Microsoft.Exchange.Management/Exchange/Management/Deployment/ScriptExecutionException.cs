using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Deployment
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ScriptExecutionException : LocalizedException
	{
		public ScriptExecutionException(LocalizedString message) : base(message)
		{
		}

		public ScriptExecutionException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ScriptExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
