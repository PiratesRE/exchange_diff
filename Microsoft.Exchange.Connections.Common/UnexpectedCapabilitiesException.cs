using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnexpectedCapabilitiesException : OperationLevelTransientException
	{
		public UnexpectedCapabilitiesException(string unexpectedCapabilitiesMsg) : base(CXStrings.UnexpectedCapabilitiesError(unexpectedCapabilitiesMsg))
		{
			this.unexpectedCapabilitiesMsg = unexpectedCapabilitiesMsg;
		}

		public UnexpectedCapabilitiesException(string unexpectedCapabilitiesMsg, Exception innerException) : base(CXStrings.UnexpectedCapabilitiesError(unexpectedCapabilitiesMsg), innerException)
		{
			this.unexpectedCapabilitiesMsg = unexpectedCapabilitiesMsg;
		}

		protected UnexpectedCapabilitiesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.unexpectedCapabilitiesMsg = (string)info.GetValue("unexpectedCapabilitiesMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("unexpectedCapabilitiesMsg", this.unexpectedCapabilitiesMsg);
		}

		public string UnexpectedCapabilitiesMsg
		{
			get
			{
				return this.unexpectedCapabilitiesMsg;
			}
		}

		private readonly string unexpectedCapabilitiesMsg;
	}
}
