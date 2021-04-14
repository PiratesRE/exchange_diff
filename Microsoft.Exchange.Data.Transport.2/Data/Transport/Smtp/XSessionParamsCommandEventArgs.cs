using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	internal class XSessionParamsCommandEventArgs : ReceiveCommandEventArgs
	{
		public XSessionParamsCommandEventArgs(SmtpSession smtpSession, Guid destMdbGuid, XSessionType type) : base(smtpSession)
		{
			this.DestinationMdbGuid = destMdbGuid;
			this.SessionType = type;
		}

		public Guid DestinationMdbGuid { get; private set; }

		public XSessionType SessionType { get; private set; }
	}
}
