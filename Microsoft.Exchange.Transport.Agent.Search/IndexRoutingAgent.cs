using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Fast;

namespace Microsoft.Exchange.Transport.Agent.Search
{
	internal class IndexRoutingAgent : RoutingAgent
	{
		internal IndexRoutingAgent(IndexRoutingAgentFactory factory)
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("IndexRoutingAgent", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.IndexRoutingAgentTracer, (long)this.GetHashCode());
			this.factory = factory;
			base.OnResolvedMessage += this.OnResolvedMessageHandler;
		}

		private void OnResolvedMessageHandler(ResolvedMessageEventSource source, QueuedMessageEventArgs args)
		{
			this.mailItem = args.MailItem;
			this.mimeHeaders = args.MailItem.MimeDocument.RootPart.Headers;
			this.diagnosticsSession.TraceDebug<Guid>("Processing message {0}", args.MailItem.NetworkMessageId);
			if (!this.ShouldIndexMessage())
			{
				TransportFlowFeeder.ReportSkippedDocument();
				return;
			}
			AgentAsyncContext agentAsyncContext = base.GetAgentAsyncContext();
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.AsyncResolvedMessageHandler), agentAsyncContext);
		}

		private void AsyncResolvedMessageHandler(object state)
		{
			AgentAsyncContext agentAsyncContext = (AgentAsyncContext)state;
			try
			{
				agentAsyncContext.Resume();
				if (!this.factory.IsReadyToProcessMessages())
				{
					this.diagnosticsSession.TraceError("Skipping processing because connection to Fast is not ready.", new object[0]);
					TransportFlowFeeder.ReportSkippedDocument();
				}
				else
				{
					Stopwatch stopwatch = Stopwatch.StartNew();
					bool flag = false;
					TransportFlowOperatorTimings transportFlowOperatorTimings = null;
					bool languageDetectionFailed = false;
					try
					{
						using (Stream mimeReadStream = this.mailItem.GetMimeReadStream())
						{
							XHeaderStream xheaderStream = new XHeaderStream(new Action<string, string>(this.SetProperty));
							using (BufferedStream bufferedStream = new BufferedStream(xheaderStream, 20000))
							{
								try
								{
									this.factory.TransportFlowFeeder.ProcessMessage(mimeReadStream, bufferedStream, this.GetMessageFlags());
									this.factory.ReportConnectionStatus(true);
									flag = true;
								}
								finally
								{
									stopwatch.Stop();
									bufferedStream.Flush();
									if (!flag)
									{
										xheaderStream.RemoveHeaders();
									}
								}
							}
						}
						EmailMessageHash emailMessageHash = new EmailMessageHash(this.mailItem.Message);
						this.diagnosticsSession.TraceDebug<EmailMessageHash>("Computed hash value for processed message: {0}.", emailMessageHash);
						emailMessageHash.SetToHeader(this.mimeHeaders);
						using (XHeaderStream xheaderStream2 = new XHeaderStream(new Func<string, string>(this.GetProperty)))
						{
							foreach (ISerializableProperty serializableProperty in SerializableProperties.DeserializeFrom(xheaderStream2))
							{
								switch (serializableProperty.Id)
								{
								case SerializablePropertyId.Language:
									languageDetectionFailed = true;
									this.SetProperty("X-MS-Exchange-Forest-Language", (string)serializableProperty.Value);
									this.diagnosticsSession.TraceDebug<string, object>("{0}: {1}", "X-MS-Exchange-Forest-Language", serializableProperty.Value);
									break;
								case SerializablePropertyId.OperatorTiming:
								{
									string text = (string)serializableProperty.Value;
									if (!string.IsNullOrWhiteSpace(text))
									{
										transportFlowOperatorTimings = new TransportFlowOperatorTimings(text);
									}
									this.diagnosticsSession.TraceDebug<string>("Processing times: {0}", text);
									break;
								}
								default:
									if (serializableProperty.Type == SerializablePropertyType.Stream)
									{
										SerializableStreamProperty serializableStreamProperty = (SerializableStreamProperty)serializableProperty;
										serializableStreamProperty.CopyTo(Stream.Null);
									}
									break;
								}
							}
						}
					}
					catch (FastTransientDocumentException ex)
					{
						this.diagnosticsSession.TraceError<FastTransientDocumentException>("FastTransientDocumentException during ProcessMessage: {0}", ex);
						if (ex.InnerException is TimeoutException || ex.InnerException is OperationCanceledException)
						{
							this.factory.ReportConnectionStatus(false);
						}
						else
						{
							this.factory.ReportConnectionStatus(true);
						}
					}
					catch (FastPermanentDocumentException arg)
					{
						this.diagnosticsSession.TraceError<FastPermanentDocumentException>("FastPermanentDocumentException during ProcessMessage: {0}", arg);
						this.factory.ReportConnectionStatus(true);
					}
					catch (Exception arg2)
					{
						this.diagnosticsSession.TraceError<Exception>("Exception during ProcessMessage: {0}", arg2);
						this.factory.ReportConnectionStatus(false);
					}
					long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
					this.diagnosticsSession.TraceDebug<bool, long>("Message processed: {0}, elapsed {1} ms", flag, elapsedMilliseconds);
					if (flag)
					{
						TransportFlowFeeder.ReportTimings(transportFlowOperatorTimings, elapsedMilliseconds, languageDetectionFailed);
					}
				}
			}
			catch (Exception exception)
			{
				this.diagnosticsSession.SendInformationalWatsonReport(exception, "Received unexpected exception in OnResolvedMessage Handler.");
			}
			finally
			{
				agentAsyncContext.Complete();
			}
		}

		private TransportFlowMessageFlags GetMessageFlags()
		{
			TransportFlowMessageFlags transportFlowMessageFlags = TransportFlowMessageFlags.None;
			if (base.MailItem.Message.MessageSecurityType == MessageSecurityType.Encrypted)
			{
				transportFlowMessageFlags = TransportFlowMessageFlags.ShouldDiscardToken;
			}
			else
			{
				if (this.factory.Config.SkipMdmGeneration)
				{
					transportFlowMessageFlags |= TransportFlowMessageFlags.SkipMdmGeneration;
				}
				if (this.factory.Config.SkipTokenInfoGeneration)
				{
					transportFlowMessageFlags |= TransportFlowMessageFlags.SkipTokenInfoGeneration;
				}
			}
			return transportFlowMessageFlags;
		}

		private string GetProperty(string name)
		{
			return XHeaderUtils.GetProperty(this.mimeHeaders, name);
		}

		private void SetProperty(string name, string value)
		{
			XHeaderUtils.SetProperty(this.mimeHeaders, name, value);
		}

		private bool ShouldIndexMessage()
		{
			Header header = this.mimeHeaders.FindFirst("X-MS-Exchange-Forest-IndexAgent");
			if (header != null && XHeaderStream.IsVersionSupported(header.Value))
			{
				this.diagnosticsSession.TraceDebug("Property stamped. Skipping", new object[0]);
				return false;
			}
			return true;
		}

		private const string LanguageHeader = "X-MS-Exchange-Forest-Language";

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly IndexRoutingAgentFactory factory;

		private MailItem mailItem;

		private HeaderList mimeHeaders;
	}
}
