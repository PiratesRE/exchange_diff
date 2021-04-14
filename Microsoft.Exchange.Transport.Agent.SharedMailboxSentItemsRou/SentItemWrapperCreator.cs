using System;
using System.IO;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Agent.SharedMailboxSentItemsRoutingAgent
{
	internal sealed class SentItemWrapperCreator : ISentItemWrapperCreator
	{
		internal SentItemWrapperCreator(ITracer tracer)
		{
			if (tracer == null)
			{
				throw new ArgumentNullException("tracer");
			}
			this.tracer = tracer;
		}

		public Exception CreateAndSubmit(MailItem mailItem, int traceId)
		{
			this.tracer.TraceDebug((long)traceId, "Create the wrapper message with the original message as an attachment.");
			ITransportMailItemFacade sentItemWrapperMessage = this.CreateWrapperMessageForTheSentItem(mailItem);
			this.tracer.TraceDebug((long)traceId, "Submit the wrapper message to transport.");
			return this.SubmitWrapperMessage(sentItemWrapperMessage, mailItem);
		}

		private static void WriteBody(EmailMessage sentItemWrapperMessage)
		{
			Stream contentWriteStream = sentItemWrapperMessage.RootPart.GetContentWriteStream(ContentTransferEncoding.Base64);
			using (TextWriter textWriter = new StreamWriter(contentWriteStream))
			{
				textWriter.Write(AgentStrings.WrapperMessageBody);
			}
		}

		private ITransportMailItemFacade CreateWrapperMessageForTheSentItem(MailItem transportMailItem)
		{
			EmailMessage message = transportMailItem.Message;
			ITransportMailItemFacade transportMailItemFacade = TransportFacades.NewMailItem(((ITransportMailItemWrapperFacade)transportMailItem).TransportMailItem);
			transportMailItemFacade.Recipients.AddWithoutDsnRequested(message.From.SmtpAddress);
			transportMailItemFacade.From = new RoutingAddress(message.From.SmtpAddress);
			transportMailItemFacade.Message.RootPart.Headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-SharedMailbox-SentItem-Message", "True"));
			transportMailItemFacade.Message.Subject = AgentStrings.WrapperMessageSubjectFormat(message.Subject);
			SentItemWrapperCreator.WriteBody(transportMailItemFacade.Message);
			Attachment attachment = transportMailItemFacade.Message.Attachments.Add(null, "message/rfc822");
			attachment.EmbeddedMessage = message;
			TransportFacades.EnsureSecurityAttributes(transportMailItemFacade);
			return transportMailItemFacade;
		}

		private Exception SubmitWrapperMessage(ITransportMailItemFacade sentItemWrapperMessage, MailItem originalMailItem)
		{
			IAsyncResult asyncResult = sentItemWrapperMessage.BeginCommitForReceive(new AsyncCallback(this.OnMailSubmitted), this);
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			Exception ex;
			sentItemWrapperMessage.EndCommitForReceive(asyncResult, out ex);
			if (ex == null)
			{
				TransportFacades.CategorizerComponent.EnqueueSideEffectMessage(((ITransportMailItemWrapperFacade)originalMailItem).TransportMailItem, sentItemWrapperMessage, "SharedMailboxSentItemsRoutingAgent");
			}
			return ex;
		}

		private void OnMailSubmitted(IAsyncResult ar)
		{
		}

		private readonly ITracer tracer;
	}
}
