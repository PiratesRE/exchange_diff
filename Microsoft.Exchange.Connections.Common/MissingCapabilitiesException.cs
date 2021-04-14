using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MissingCapabilitiesException : OperationLevelTransientException
	{
		public MissingCapabilitiesException(string missingCapabilitiesMsg) : base(CXStrings.MissingCapabilitiesError(missingCapabilitiesMsg))
		{
			this.missingCapabilitiesMsg = missingCapabilitiesMsg;
		}

		public MissingCapabilitiesException(string missingCapabilitiesMsg, Exception innerException) : base(CXStrings.MissingCapabilitiesError(missingCapabilitiesMsg), innerException)
		{
			this.missingCapabilitiesMsg = missingCapabilitiesMsg;
		}

		protected MissingCapabilitiesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.missingCapabilitiesMsg = (string)info.GetValue("missingCapabilitiesMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("missingCapabilitiesMsg", this.missingCapabilitiesMsg);
		}

		public string MissingCapabilitiesMsg
		{
			get
			{
				return this.missingCapabilitiesMsg;
			}
		}

		private readonly string missingCapabilitiesMsg;
	}
}
