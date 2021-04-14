using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Core.LocStrings;

namespace Microsoft.Exchange.Configuration.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToRecieveWinRMDataException : WinRMDataExchangeException
	{
		public FailedToRecieveWinRMDataException(string identity) : base(Strings.FailedToReceiveWinRMData(identity))
		{
			this.identity = identity;
		}

		public FailedToRecieveWinRMDataException(string identity, Exception innerException) : base(Strings.FailedToReceiveWinRMData(identity), innerException)
		{
			this.identity = identity;
		}

		protected FailedToRecieveWinRMDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly string identity;
	}
}
