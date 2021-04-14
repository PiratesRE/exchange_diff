using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class MessageEncoderWithXmlDeclaration : MessageEncoder
	{
		public MessageEncoderWithXmlDeclaration(MessageEncoderWithXmlDeclarationFactory factory)
		{
			this.version = factory.MessageVersion;
			this.Initialize();
		}

		public MessageEncoderWithXmlDeclaration(MessageVersion messageVersion)
		{
			this.version = messageVersion;
			this.Initialize();
		}

		private void Initialize()
		{
			this.mediaType = "text/xml";
			this.contentType = string.Format(CultureInfo.InvariantCulture, "{0}; charset={1}", new object[]
			{
				this.mediaType,
				Encoding.UTF8.WebName
			});
			MessageEncoderFactory messageEncoderFactory = new TextMessageEncodingBindingElement(this.MessageVersion, Encoding.UTF8)
			{
				ReaderQuotas = MessageEncoderWithXmlDeclaration.readerQuotasForEws.Member
			}.CreateMessageEncoderFactory();
			this.textEncoder = messageEncoderFactory.Encoder;
		}

		public override string ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public override string MediaType
		{
			get
			{
				return this.mediaType;
			}
		}

		public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
		{
			MemoryStream stream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count);
			Message result = this.ReadMessage(stream, 163840, contentType);
			bufferManager.ReturnBuffer(buffer.Array);
			return result;
		}

		private void SniffRequestForVersionAndMethodNameAndWSSecurityTokenHash(Stream stream, int maxSizeOfHeaders, out string methodName, out string methodNamespace, out ExchangeVersion requestVersion, out string wsSecurityTokenHash)
		{
			wsSecurityTokenHash = null;
			long position = stream.Position;
			XmlReaderSettings settings = new XmlReaderSettings
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
			XmlReader xmlReader = XmlReader.Create(stream, settings);
			long num = stream.Position - position;
			if (num > (long)maxSizeOfHeaders)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<long, int>((long)this.GetHashCode(), "[MessageEncoderWithXmlDeclaration::SniffRequestForVersionAndMethodNameAndWSSecurityTokenHash] XmlReader buffer size ({0}) exceeded maxSizeOfHeaders ({1}).  Use buffer size instead.", num, maxSizeOfHeaders);
				maxSizeOfHeaders = (int)num;
			}
			else
			{
				maxSizeOfHeaders += (int)num;
			}
			xmlReader.MoveToContent();
			methodName = string.Empty;
			methodNamespace = string.Empty;
			requestVersion = ExchangeVersion.Exchange2007;
			string arg = string.Empty;
			bool flag = false;
			bool flag2 = false;
			try
			{
				while (!flag2)
				{
					if (!xmlReader.Read())
					{
						requestVersion = ExchangeVersion.Exchange2007;
						flag2 = true;
						break;
					}
					if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.LocalName == "Header")
					{
						if (!(xmlReader.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/"))
						{
							if (!(xmlReader.NamespaceURI == "http://www.w3.org/2003/05/soap-envelope"))
							{
								goto IL_403;
							}
						}
						while (xmlReader.Read())
						{
							if (stream.Position > (long)maxSizeOfHeaders)
							{
								throw new QuotaExceededException();
							}
							if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.LocalName == "Header" && (xmlReader.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || xmlReader.NamespaceURI == "http://www.w3.org/2003/05/soap-envelope"))
							{
								if (!flag2)
								{
									arg = "NotSet";
									requestVersion = ExchangeVersion.Exchange2007;
									flag2 = true;
									break;
								}
								break;
							}
							else
							{
								if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.LocalName == "RequestServerVersion" && xmlReader.NamespaceURI == "http://schemas.microsoft.com/exchange/services/2006/types")
								{
									if (flag)
									{
										arg = string.Format("{0}+Multiple", arg);
										requestVersion = ExchangeVersion.Exchange2007;
										throw new DuplicateSOAPHeaderException();
									}
									try
									{
										SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(RequestServerVersion));
										RequestServerVersion requestServerVersion = (RequestServerVersion)safeXmlSerializer.Deserialize(xmlReader);
										if (!requestServerVersion.VersionSpecified)
										{
											arg = "NotSpecified";
											requestVersion = ExchangeVersion.Exchange2007;
											throw new InvalidServerVersionException();
										}
										arg = requestServerVersion.VersionString;
										if (requestServerVersion.Version == ExchangeVersionType.Exchange2009)
										{
											requestServerVersion.Version = ExchangeVersionType.Exchange2010;
										}
										if (requestServerVersion.Version == ExchangeVersionType.Exchange2012)
										{
											requestServerVersion.Version = ExchangeVersionType.Exchange2013;
										}
										requestVersion = new ExchangeVersion(requestServerVersion.Version);
										flag2 = true;
										flag = true;
										if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.LocalName == "Header" && (xmlReader.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || xmlReader.NamespaceURI == "http://www.w3.org/2003/05/soap-envelope"))
										{
											break;
										}
									}
									catch (InvalidOperationException)
									{
										requestVersion = ExchangeVersion.Exchange2007;
										throw new InvalidServerVersionException();
									}
								}
								if (wsSecurityTokenHash == null && xmlReader.NodeType == XmlNodeType.Element && xmlReader.LocalName == "Security" && xmlReader.NamespaceURI == "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")
								{
									bool flag3 = false;
									bool flag4 = false;
									while (xmlReader.Read())
									{
										if (stream.Position > (long)maxSizeOfHeaders)
										{
											throw new QuotaExceededException();
										}
										if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.LocalName == "Security" && xmlReader.NamespaceURI == "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")
										{
											ExTraceGlobals.AuthenticationTracer.TraceDebug<bool, bool>(0L, "Hit the end of the WS-Security header unexpectedly - state inEncryptedDataElement = {0}; foundOuterKeyInfoElement = {1}", flag3, flag4);
											break;
										}
										if (!flag3 && xmlReader.NodeType == XmlNodeType.Element && xmlReader.LocalName == "EncryptedData" && xmlReader.NamespaceURI == "http://www.w3.org/2001/04/xmlenc#")
										{
											flag3 = true;
										}
										else if (flag3 && !flag4 && xmlReader.NodeType == XmlNodeType.Element && xmlReader.LocalName == "KeyInfo" && xmlReader.NamespaceURI == "http://www.w3.org/2000/09/xmldsig#")
										{
											flag4 = true;
											xmlReader.Skip();
										}
										if (flag3 && flag4 && xmlReader.NodeType == XmlNodeType.Element && xmlReader.LocalName == "CipherValue" && xmlReader.NamespaceURI == "http://www.w3.org/2001/04/xmlenc#")
										{
											wsSecurityTokenHash = BadTokenHashCache.BuildKey(xmlReader.ReadInnerXml());
											break;
										}
									}
								}
							}
						}
						continue;
					}
					IL_403:
					if (xmlReader.LocalName == "Body" && (xmlReader.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || xmlReader.NamespaceURI == "http://www.w3.org/2003/05/soap-envelope"))
					{
						requestVersion = ExchangeVersion.Exchange2007;
						flag2 = true;
					}
				}
			}
			finally
			{
				string value = string.Format("{0}/{1}", arg, (requestVersion != null) ? requestVersion.Version.ToString() : "Unkown");
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendColumn(RequestDetailsLogger.Current, EwsMetadata.VersionInfo, "Req", value);
			}
			if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.Headers != null)
			{
				string headerValue = HttpContext.Current.Request.Headers.Get("X-EWS-TargetVersion");
				ExchangeVersionHeader exchangeVersionHeader = new ExchangeVersionHeader(headerValue);
				if (!exchangeVersionHeader.IsMissing)
				{
					ExchangeVersionType exchangeVersionType = exchangeVersionHeader.CheckAndGetRequestedVersion();
					if (exchangeVersionType == ExchangeVersionType.Exchange2012)
					{
						exchangeVersionType = ExchangeVersionType.Exchange2013;
					}
					requestVersion = new ExchangeVersion(exchangeVersionType);
					flag2 = true;
				}
			}
			position = stream.Position;
			for (;;)
			{
				if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.LocalName == "Body")
				{
					if (!(xmlReader.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/"))
					{
						if (!(xmlReader.NamespaceURI == "http://www.w3.org/2003/05/soap-envelope"))
						{
							goto IL_584;
						}
					}
					while (xmlReader.Read())
					{
						if (xmlReader.NodeType == XmlNodeType.Element)
						{
							goto Block_12;
						}
						if (stream.Position - position > (long)maxSizeOfHeaders)
						{
							goto Block_13;
						}
					}
				}
				IL_584:
				if (stream.Position - position > (long)maxSizeOfHeaders)
				{
					goto Block_14;
				}
				if (!xmlReader.Read())
				{
					return;
				}
			}
			Block_12:
			methodName = xmlReader.LocalName;
			methodNamespace = xmlReader.NamespaceURI;
			return;
			Block_13:
			throw new QuotaExceededException();
			Block_14:
			throw new QuotaExceededException();
		}

		public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string streamContentType)
		{
			Message message = null;
			if (HttpContext.Current != null && HttpContext.Current.Items.Contains("EwsHttpContextMessage"))
			{
				return (Message)HttpContext.Current.Items["EwsHttpContextMessage"];
			}
			ServiceDiagnostics.SendWatsonReportOnUnhandledException(delegate
			{
				string empty = string.Empty;
				string empty2 = string.Empty;
				ExchangeVersion exchangeVersion = null;
				string text = null;
				bool flag = false;
				Exception delayedException = null;
				BufferedRegionStream bufferedRegionStream = null;
				Guid guid = Guid.NewGuid();
				try
				{
					if (!stream.CanSeek)
					{
						bufferedRegionStream = BufferedRegionStream.CreateWithBufferPoolCollection(stream, maxSizeOfHeaders + 8192, false);
						stream = bufferedRegionStream;
					}
					IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
					if (currentActivityScope != null && currentActivityScope.Status == ActivityContextStatus.ActivityStarted)
					{
						currentActivityScope.SetProperty(EwsMetadata.CorrelationGuid, guid.ToString("D"));
					}
					this.TraceRequest(stream, maxSizeOfHeaders, guid);
					try
					{
						this.SniffRequestForVersionAndMethodNameAndWSSecurityTokenHash(stream, maxSizeOfHeaders, out empty, out empty2, out exchangeVersion, out text);
						flag = true;
					}
					catch (InvalidServerVersionException exception)
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceError((long)this.GetHashCode(), "[MessageEncoderWithXmlDeclaration::ReadMessage] Incoming request had invalid RequestServerVersion header.");
						InvalidServerVersionException exception4;
						delayedException = FaultExceptionUtilities.CreateFault(exception4, FaultParty.Sender, exchangeVersion);
					}
					catch (DuplicateSOAPHeaderException exception2)
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceError((long)this.GetHashCode(), "[MessageEncoderWithXmlDeclaration::ReadMessage] Incoming request had duplicate RequestServerVersion headers.");
						delayedException = FaultExceptionUtilities.CreateFault(exception2, FaultParty.Sender, exchangeVersion);
					}
					catch (XmlException ex)
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceError((long)this.GetHashCode(), "[MessageEncoderWithXmlDeclaration::ReadMessage] Incoming request had invalid xml.  Error message: " + ex.Message);
						SchemaValidationException exception3 = new SchemaValidationException(ex, ex.LineNumber, ex.LinePosition, ex.Message);
						delayedException = FaultExceptionUtilities.CreateFault(exception3, FaultParty.Sender);
					}
					stream.Position = 0L;
					if (flag && this.ShouldValidateRequest(empty, empty2, exchangeVersion, (int)stream.Length))
					{
						SchemaValidator schemaValidator = new SchemaValidator(delegate(XmlSchemaException exception, SoapSavvyReader.SoapSection section)
						{
							SchemaValidationException ex3 = new SchemaValidationException(exception, exception.LineNumber, exception.LinePosition, exception.Message);
							ExTraceGlobals.CommonAlgorithmTracer.TraceError<string>(0L, "[SchemaValidator] Encountered an XmlException when creating a Message from the request. Error: {0}", exception.Message);
							if (section == SoapSavvyReader.SoapSection.Body && !EWSSettings.InWCFChannelLayer)
							{
								throw ex3;
							}
							if (delayedException == null)
							{
								delayedException = ex3;
							}
						});
						EWSSettings.InWCFChannelLayer = true;
						message = Message.CreateMessage(schemaValidator.GetValidatingReader(stream, exchangeVersion, MessageInspectorManager.IsEWSRequest(empty), true), maxSizeOfHeaders, this.MessageVersion);
						EWSSettings.InWCFChannelLayer = false;
					}
					else
					{
						try
						{
							message = this.textEncoder.ReadMessage(stream, maxSizeOfHeaders, streamContentType);
						}
						catch (Exception ex2)
						{
							ExTraceGlobals.CommonAlgorithmTracer.TraceError<string>(0L, "[BypassSchemaValidation] Exception in ReadMessage.\r\n{0}", ex2.ToString());
							if (delayedException == null)
							{
								delayedException = ex2;
							}
						}
					}
					bool flag2;
					if (text != null && BadTokenHashCache.Singleton.TryGetValue(text, out flag2))
					{
						message.Headers.RemoveAll("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
					}
					try
					{
						EndpointAddress replyTo = message.Headers.ReplyTo;
					}
					catch (XmlException arg)
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceError<XmlException>((long)this.GetHashCode(), "Bogus ReplyTo header found - XmlException received was {0}.  Removing bogus ReplyToHeader.", arg);
						message.Headers.RemoveAll("ReplyTo", "http://www.w3.org/2005/08/addressing");
					}
					message.Properties["MethodName"] = empty;
					message.Properties["MethodNamespace"] = empty2;
					message.Properties["WS_ServerVersionKey"] = exchangeVersion;
					message.Properties["MessageStream"] = stream;
					message.Properties["MessageStreamLength"] = stream.Length;
				}
				finally
				{
					if (bufferedRegionStream != null && (message == null || !message.Properties.ContainsKey("MessageStream") || message.Properties["MessageStream"] == null))
					{
						bufferedRegionStream.Dispose();
						bufferedRegionStream = null;
					}
				}
				message.Properties["WS_RequestCorrelationKey"] = guid;
				if (text != null)
				{
					message.Properties["WSSecurityTokenHash"] = text;
				}
				message.Properties["WS_RequestThreadIdKey"] = Environment.CurrentManagedThreadId;
				message.Properties["DelayedException"] = delayedException;
			});
			if (message != null && message.Properties != null)
			{
				message.Properties["ReadMsgEnd"] = DateTime.UtcNow;
			}
			return message;
		}

		public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceError((long)this.GetHashCode(), "[MessageEncoderWithXmlDeclaration::WriteMessage(buffered)] We should ONLY be using a streamed transfer mode in EWS.");
			throw new InvalidOperationException("[MessageEncoderWithXmlDeclaration::WriteMessage(buffered)] We should ONLY be using a streamed transfer mode in EWS.");
		}

		public override void WriteMessage(Message message, Stream stream)
		{
			BaseResponseRenderer responseRenderer = EWSSettings.ResponseRenderer;
			if (responseRenderer is SoapWcfResponseRenderer)
			{
				this.TraceResponse(ref message);
			}
			responseRenderer.Render(message, stream);
		}

		private void TraceResponse(ref Message message)
		{
			if (ExTraceGlobals.AllResponsesTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				bool flag = false;
				if (CallContext.Current != null && CallContext.Current.AccessingPrincipal != null && ExUserTracingAdaptor.Instance.IsTracingEnabledUser(CallContext.Current.AccessingPrincipal.LegacyDn))
				{
					flag = true;
					BaseTrace.CurrentThreadSettings.EnableTracing();
				}
				try
				{
					using (MessageBuffer messageBuffer = message.CreateBufferedCopy(int.MaxValue))
					{
						using (Message message2 = messageBuffer.CreateMessage())
						{
							message = messageBuffer.CreateMessage();
							using (MemoryStream memoryStream = new MemoryStream())
							{
								XmlDictionaryWriter xmlDictionaryWriter = XmlDictionaryWriter.CreateTextWriter(memoryStream);
								message2.WriteMessage(xmlDictionaryWriter);
								xmlDictionaryWriter.Flush();
								memoryStream.Position = 0L;
								StreamReader streamReader = new StreamReader(memoryStream);
								this.TraceMessage(ExTraceGlobals.AllResponsesTracer, streamReader.ReadToEnd(), EWSSettings.RequestCorrelation);
							}
						}
					}
				}
				finally
				{
					if (flag)
					{
						BaseTrace.CurrentThreadSettings.DisableTracing();
					}
				}
			}
		}

		private void TraceRequest(Stream stream, int bufferSize, Guid correlationGuid)
		{
			if (ExTraceGlobals.AllRequestsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (stream.Length > this.MaxTraceRequestSize)
				{
					this.TraceMessage(ExTraceGlobals.AllRequestsTracer, string.Format("Request size, {0}, is larger than the maximum size for tracing, {1}.", stream.Length, this.MaxTraceRequestSize), correlationGuid);
					return;
				}
				if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.Headers != null && string.Compare(HttpContext.Current.Request.Headers["Transfer-Encoding"], "chunked", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.TraceMessage(ExTraceGlobals.AllRequestsTracer, string.Format("Cannot trace this request because it was sent using chunked transfer encoding.", new object[0]), correlationGuid);
					return;
				}
				long position = 0L;
				try
				{
					string message;
					if (stream is BufferedRegionStream)
					{
						if (bufferSize > 0)
						{
							stream.Position = 0L;
							StreamReader streamReader = new StreamReader(stream);
							char[] array = new char[bufferSize];
							streamReader.ReadBlock(array, 0, bufferSize);
							message = new string(array);
						}
						else
						{
							message = string.Format("BufferedRegionStream not traced due to unexpected buffer size {0}", bufferSize);
						}
					}
					else
					{
						position = stream.Position;
						stream.Position = 0L;
						StreamReader streamReader2 = new StreamReader(stream);
						message = streamReader2.ReadToEnd();
					}
					this.TraceMessage(ExTraceGlobals.AllRequestsTracer, message, correlationGuid);
				}
				finally
				{
					try
					{
						stream.Position = position;
					}
					catch (InvalidOperationException)
					{
					}
				}
			}
		}

		private void TraceMessage(Trace tracer, string message, Guid correlationGuid)
		{
			string traceCorrelationHeader = MessageEncoderWithXmlDeclaration.GetTraceCorrelationHeader(correlationGuid);
			tracer.TraceDebug((long)this.GetHashCode(), traceCorrelationHeader + message);
		}

		internal static string GetTraceCorrelationHeader(Guid correlationGuid)
		{
			string arg = string.Empty;
			string arg2 = string.Empty;
			if (HttpContext.Current != null && HttpContext.Current.User != null)
			{
				WindowsIdentity windowsIdentity = HttpContext.Current.User.Identity as WindowsIdentity;
				if (windowsIdentity == null)
				{
					arg = HttpContext.Current.User.Identity.GetSafeName(true);
				}
				else if (windowsIdentity.User == null)
				{
					arg = "<Anonymous>";
				}
				else
				{
					arg = SidToAccountMap.Singleton.Get(windowsIdentity.User);
				}
				arg2 = (HttpContext.Current.Request.UserAgent ?? "<NULL>");
			}
			return string.Format("[User:'{0}';Agent:'{1}';Key:'{2}']", arg, arg2, (correlationGuid == Guid.Empty) ? "<Unrelated>" : correlationGuid.ToString());
		}

		public override MessageVersion MessageVersion
		{
			get
			{
				return this.version;
			}
		}

		public override bool IsContentTypeSupported(string contentType)
		{
			if (base.IsContentTypeSupported(contentType))
			{
				return true;
			}
			if (contentType.Trim().Length == this.MediaType.Length)
			{
				return contentType.Trim().Equals(this.MediaType, StringComparison.OrdinalIgnoreCase);
			}
			return contentType.StartsWith(this.MediaType, StringComparison.OrdinalIgnoreCase) && contentType[this.MediaType.Length] == ';';
		}

		protected virtual bool ShouldValidateRequest(string methodName, string methodNamespace, ExchangeVersion requestVersion, int requestSize)
		{
			if (methodNamespace.Equals("http://schemas.microsoft.com/exchange/services/2006/messages") && methodName.Equals("ExecuteDiagnosticMethod"))
			{
				return false;
			}
			bool? enableSchemaValidationOverride = Global.EnableSchemaValidationOverride;
			if (enableSchemaValidationOverride != null)
			{
				return enableSchemaValidationOverride.Value;
			}
			if ((methodName.Equals("CreateItem") || methodName.Equals("CreateAttachment")) && requestSize > Global.CreateItemRequestSizeThreshold)
			{
				return false;
			}
			bool flag = requestVersion > ExchangeVersion.Exchange2013;
			return !flag;
		}

		protected virtual long MaxTraceRequestSize
		{
			get
			{
				return MessageEncoderWithXmlDeclaration.maxTraceRequestSizeForEWS.Member;
			}
		}

		private const string SoapHeaderElementName = "Header";

		private const string SoapBodyElementName = "Body";

		internal const string RequestServerVersionElementName = "RequestServerVersion";

		private const string SecurityHeaderElementName = "Security";

		private const string EncryptedDataElementName = "EncryptedData";

		private const string KeyInfoElementName = "KeyInfo";

		private const string CipherValueElementName = "CipherValue";

		private const string ReplyToHeaderElementName = "ReplyTo";

		public const string ConnectionCostTypeKey = "ConnectionCostType";

		public const string MethodNamePropertyKey = "MethodName";

		public const string MethodNamespacePropertyKey = "MethodNamespace";

		public const string MessageStreamPropertyKey = "MessageStream";

		public const string MessageHeaderProcessorKey = "MessageHeaderProcessor";

		public const string MessageStreamLengthPropertyKey = "MessageStreamLength";

		public const string WSSecurityTokenHashPropertyKey = "WSSecurityTokenHash";

		public const string WebMethodEntryPropertyKey = "WebMethodEntry";

		public const string WcfLatencyKey = "WcfLatency";

		public const string ReadMessageEndTimeKey = "ReadMsgEnd";

		public const string WebConfigFileName = "~/web.config";

		public const string BindingsSectionName = "system.serviceModel/bindings";

		public const int ReadMessageBufferedRegionStreamPaddingSize = 8192;

		public const string EwsHttpContextMessagePropertyKey = "EwsHttpContextMessage";

		public const string DelayedExceptionKey = "DelayedException";

		public const int MaxHeaderSizeToSniff = 163840;

		private const string EwsMaxNameTableCharCountKeyName = "EwsMaxNameTableCharCount";

		public static readonly string[] EwsBindingNames = new string[]
		{
			"EWSAnonymousHttpsBinding",
			"EWSAnonymousHttpBinding",
			"EWSBasicHttpsBinding",
			"EWSBasicHttpBinding",
			"EWSNegotiateHttpsBinding",
			"EWSNegotiateHttpBinding",
			"EWSWSSecurityHttpsBinding",
			"EWSWSSecurityHttpBinding"
		};

		private static readonly int EwsMaxNameTableCharCount = Global.GetAppSettingAsInt("EwsMaxNameTableCharCount", 32768);

		private static LazyMember<long> maxTraceRequestSizeForEWS = new LazyMember<long>(delegate()
		{
			long num = 0L;
			List<CustomBindingElement> list = new List<CustomBindingElement>(MessageEncoderWithXmlDeclaration.EwsBindingNames.Length);
			Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~/web.config");
			BindingsSection bindingsSection = (BindingsSection)configuration.GetSection("system.serviceModel/bindings");
			foreach (string text in MessageEncoderWithXmlDeclaration.EwsBindingNames)
			{
				if (bindingsSection.CustomBinding.Bindings.ContainsKey(text))
				{
					list.Add(bindingsSection.CustomBinding.Bindings[text]);
				}
				else
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "Binding {0} was not found in web.config file.", text);
				}
			}
			foreach (CustomBindingElement customBindingElement in list)
			{
				TransportElement transportElement = (TransportElement)customBindingElement[typeof(HttpsTransportElement)];
				if (transportElement == null)
				{
					transportElement = (TransportElement)customBindingElement[typeof(HttpTransportElement)];
				}
				if (transportElement != null)
				{
					if (num == 0L || transportElement.MaxReceivedMessageSize < num)
					{
						num = transportElement.MaxReceivedMessageSize;
					}
				}
				else
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "No transport element found for binding {0} in web.config file.", customBindingElement.Name);
				}
			}
			return (long)((double)num * 0.45);
		});

		private static LazyMember<XmlDictionaryReaderQuotas> readerQuotasForEws = new LazyMember<XmlDictionaryReaderQuotas>(() => new XmlDictionaryReaderQuotas
		{
			MaxNameTableCharCount = Math.Max(65536, MessageEncoderWithXmlDeclaration.EwsMaxNameTableCharCount),
			MaxStringContentLength = Math.Max(8388608, Global.GetAttachmentSizeLimit),
			MaxArrayLength = 8388608,
			MaxBytesPerRead = 32767,
			MaxDepth = 32
		});

		private string mediaType;

		private string contentType;

		private MessageVersion version;

		private MessageEncoder textEncoder;
	}
}
