using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.DispatchPipe.Ews;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ProxyResponseMessage : Message
	{
		private ProxyResponseMessage(HttpWebResponse response, Stream responseStream, Message wrappedMessage)
		{
			this.webResponse = response;
			this.responseStream = responseStream;
			this.wrappedMessage = wrappedMessage;
		}

		public static ProxyResponseMessage Create()
		{
			Stream stream = null;
			bool flag = false;
			ProxyResponseMessage result;
			try
			{
				stream = EWSSettings.ProxyResponse.GetResponseStream();
				XmlReaderSettings settings = new XmlReaderSettings
				{
					CheckCharacters = false
				};
				XmlReader envelopeReader = SafeXmlFactory.CreateSafeXmlReader(stream, settings);
				Message message = Message.CreateMessage(envelopeReader, int.MaxValue, EwsOperationContextBase.Current.IncomingMessageVersion);
				ProxyResponseMessage proxyResponseMessage = new ProxyResponseMessage(EWSSettings.ProxyResponse, stream, message);
				flag = true;
				result = proxyResponseMessage;
			}
			finally
			{
				if (!flag)
				{
					if (stream != null)
					{
						stream.Close();
					}
					EWSSettings.ProxyResponse.Close();
					EWSSettings.ProxyResponse = null;
				}
			}
			return result;
		}

		public override MessageHeaders Headers
		{
			get
			{
				return this.wrappedMessage.Headers;
			}
		}

		public override bool IsFault
		{
			get
			{
				return this.wrappedMessage.IsFault;
			}
		}

		public override bool IsEmpty
		{
			get
			{
				return this.wrappedMessage.IsEmpty;
			}
		}

		protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
		{
			try
			{
				FaultInjection.GenerateFault((FaultInjection.LIDs)3594923325U);
				this.wrappedMessage.WriteBodyContents(writer);
			}
			catch (IOException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string>((long)this.GetHashCode(), "[ProxyResponseMessage::OnWriteBodyContents] Caught IOException trying to write body contents from proxy request stream: {0}", ex.ToString());
				FaultException ex2 = FaultExceptionUtilities.CreateFault(new TransientException(CoreResources.GetLocalizedString((CoreResources.IDs)3995283118U), ex), FaultParty.Receiver);
				throw ex2;
			}
		}

		public override MessageProperties Properties
		{
			get
			{
				return this.wrappedMessage.Properties;
			}
		}

		public override MessageVersion Version
		{
			get
			{
				return this.wrappedMessage.Version;
			}
		}

		protected override void OnClose()
		{
			base.OnClose();
			this.responseStream.Close();
			this.responseStream = null;
			this.webResponse.Close();
			this.webResponse = null;
		}

		private Message wrappedMessage;

		private HttpWebResponse webResponse;

		private Stream responseStream;
	}
}
