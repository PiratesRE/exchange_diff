using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission.Agents
{
	internal class MfnSubmitterAgent : SubmissionAgent
	{
		public MfnSubmitterAgent()
		{
			base.OnDemotedMessage += this.OnDemotedMessageHandler;
		}

		public void OnDemotedMessageHandler(StoreDriverEventSource source, StoreDriverSubmissionEventArgs args)
		{
			StoreDriverSubmissionEventArgsImpl storeDriverSubmissionEventArgsImpl = (StoreDriverSubmissionEventArgsImpl)args;
			if (MfnSubmitterAgent.ShouldGenerateMfn(storeDriverSubmissionEventArgsImpl.SubmissionItem.MessageClass))
			{
				using (MfnSubmitter mfnSubmitter = new MfnSubmitter(storeDriverSubmissionEventArgsImpl.SubmissionItem, storeDriverSubmissionEventArgsImpl.MailItemSubmitter))
				{
					TransportMailItem originalMailItem = null;
					TransportMailItemWrapper transportMailItemWrapper = args.MailItem as TransportMailItemWrapper;
					if (transportMailItemWrapper != null)
					{
						originalMailItem = transportMailItemWrapper.TransportMailItem;
					}
					mfnSubmitter.CheckAndSubmitMfn(originalMailItem);
				}
			}
		}

		private static bool ShouldGenerateMfn(string messageClass)
		{
			return ObjectClass.IsMeetingRequest(messageClass) || (ObjectClass.IsMeetingRequestSeries(messageClass) && SubmissionConfiguration.Instance.App.EnableSeriesMessageProcessing);
		}
	}
}
