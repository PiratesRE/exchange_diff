using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ServiceHost;
using Microsoft.Exchange.Servicelets.ExchangeCertificate.Messages;

namespace Microsoft.Exchange.Servicelets.ExchangeCertificate
{
	public class Servicelet : Servicelet
	{
		public override void Work()
		{
			bool flag = false;
			try
			{
				Exception ex;
				if (!ExchangeCertificateServer.Start(out ex))
				{
					this.eventLog.LogEvent(MSExchangeExchangeCertificateEventLogConstants.Tuple_PermanentException, null, new object[]
					{
						ex.Message
					});
				}
				else if (!ExchangeCertificateServer2.Start(out ex))
				{
					this.eventLog.LogEvent(MSExchangeExchangeCertificateEventLogConstants.Tuple_PermanentException, null, new object[]
					{
						ex.Message
					});
					ExchangeCertificateServer.Stop();
				}
				else
				{
					flag = true;
					base.StopEvent.WaitOne();
				}
			}
			finally
			{
				if (flag)
				{
					ExchangeCertificateServer2.Stop();
					ExchangeCertificateServer.Stop();
				}
			}
		}

		private static readonly Guid ComponentGuid = new Guid("7ACE8E2A-A2F7-4229-8641-CACE7C48EC2B");

		private readonly ExEventLog eventLog = new ExEventLog(Servicelet.ComponentGuid, "MSExchange Certificate");
	}
}
