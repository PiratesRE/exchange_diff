using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsResponse;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SendResponse;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse;
using Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessResponse;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SyncResponse;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncResponseHandler
	{
		internal DeltaSyncResponseHandler(SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			this.syncLogSession = syncLogSession;
		}

		private XmlSerializer SyncDeserializer
		{
			get
			{
				if (this.syncDeserializer == null)
				{
					this.syncDeserializer = new XmlSerializer(typeof(Sync));
				}
				return this.syncDeserializer;
			}
		}

		private XmlSerializer ItemOperationsDeserializer
		{
			get
			{
				if (this.itemOperationsDeserializer == null)
				{
					this.itemOperationsDeserializer = new XmlSerializer(typeof(ItemOperations));
				}
				return this.itemOperationsDeserializer;
			}
		}

		private XmlSerializer SettingsDeserializer
		{
			get
			{
				if (this.settingsDeserializer == null)
				{
					this.settingsDeserializer = new XmlSerializer(typeof(Settings));
				}
				return this.settingsDeserializer;
			}
		}

		private XmlSerializer SendDeserializer
		{
			get
			{
				if (this.sendDeserializer == null)
				{
					this.sendDeserializer = new XmlSerializer(typeof(Send));
				}
				return this.sendDeserializer;
			}
		}

		private XmlSerializer StatelessDeserializer
		{
			get
			{
				if (this.statelessDeserializer == null)
				{
					this.statelessDeserializer = new XmlSerializer(typeof(Stateless));
				}
				return this.statelessDeserializer;
			}
		}

		internal DeltaSyncResultData ParseDeltaSyncResponse(DownloadResult deltaSyncResponse, DeltaSyncCommandType commandType)
		{
			this.syncLogSession.LogDebugging((TSLID)670UL, DeltaSyncResponseHandler.Tracer, "Try Parse DS Response for Command Type: {0}", new object[]
			{
				commandType
			});
			if (deltaSyncResponse.ResponseStream == null)
			{
				this.syncLogSession.LogError((TSLID)671UL, DeltaSyncResponseHandler.Tracer, "Http Response Stream cannot be null", new object[0]);
				throw new InvalidServerResponseException(new HttpResponseStreamNullException());
			}
			switch (commandType)
			{
			case DeltaSyncCommandType.Sync:
			{
				Sync syncResponse = (Sync)this.ParseResponse(delegate
				{
					Microsoft.Exchange.Diagnostics.Components.ContentAggregation.ExTraceGlobals.FaultInjectionTracer.TraceTest(4100336957U);
					return (Sync)this.SyncDeserializer.Deserialize(deltaSyncResponse.ResponseStream);
				});
				return new DeltaSyncResultData(syncResponse);
			}
			case DeltaSyncCommandType.Fetch:
			{
				ItemOperations itemOperationsResponse = this.ParseFetchResponse(deltaSyncResponse);
				return new DeltaSyncResultData(itemOperationsResponse);
			}
			case DeltaSyncCommandType.Settings:
			{
				Settings settingsResponse = (Settings)this.ParseResponse(() => (Settings)this.SettingsDeserializer.Deserialize(deltaSyncResponse.ResponseStream));
				return new DeltaSyncResultData(settingsResponse);
			}
			case DeltaSyncCommandType.Send:
			{
				Send sendResponse = (Send)this.ParseResponse(() => (Send)this.SendDeserializer.Deserialize(deltaSyncResponse.ResponseStream));
				return new DeltaSyncResultData(sendResponse);
			}
			case DeltaSyncCommandType.Stateless:
			{
				Stateless statelessResponse = (Stateless)this.ParseResponse(() => (Stateless)this.StatelessDeserializer.Deserialize(deltaSyncResponse.ResponseStream));
				return new DeltaSyncResultData(statelessResponse);
			}
			default:
				this.syncLogSession.LogError((TSLID)672UL, DeltaSyncResponseHandler.Tracer, "Unknown DeltaSync Command Type {0}", new object[]
				{
					commandType
				});
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unknown Command: {0}", new object[]
				{
					commandType
				}));
			}
		}

		private static bool TryGetIncludeContentId(ItemOperations fetchResponse, out string contentId)
		{
			contentId = null;
			if (fetchResponse.Responses != null && fetchResponse.Responses.Fetch != null && fetchResponse.Responses.Fetch.Message != null && fetchResponse.Responses.Fetch.Message.Include != null && fetchResponse.Responses.Fetch.Message.Include.href != null && fetchResponse.Responses.Fetch.Message.Include.href.StartsWith("cid:", StringComparison.OrdinalIgnoreCase))
			{
				contentId = fetchResponse.Responses.Fetch.Message.Include.href.Remove(0, "cid:".Length);
				return true;
			}
			return false;
		}

		private object ParseResponse(DeltaSyncResponseHandler.ResponseParser parser)
		{
			Exception ex = null;
			try
			{
				return parser();
			}
			catch (InvalidOperationException ex2)
			{
				ex = ex2;
			}
			catch (InvalidCastException ex3)
			{
				ex = ex3;
			}
			this.syncLogSession.LogError((TSLID)673UL, DeltaSyncResponseHandler.Tracer, "Response Parsing error: {0}", new object[]
			{
				ex
			});
			throw new InvalidServerResponseException(ex);
		}

		private ItemOperations ParseFetchResponse(DownloadResult deltaSyncResponse)
		{
			Microsoft.Exchange.Diagnostics.Components.ContentAggregation.ExTraceGlobals.FaultInjectionTracer.TraceTest(3798347069U);
			string text = deltaSyncResponse.ResponseHeaders[HttpResponseHeader.ContentType];
			if (text != null && text.Equals(DeltaSyncCommon.TextXmlContentType, StringComparison.OrdinalIgnoreCase))
			{
				return this.ParseItemOperationsXmlResponse(deltaSyncResponse.ResponseStream);
			}
			if (text != null && text.Equals(DeltaSyncCommon.ApplicationXopXmlContentType, StringComparison.OrdinalIgnoreCase))
			{
				return this.ParseFetchMtomResponse(deltaSyncResponse);
			}
			this.syncLogSession.LogError((TSLID)674UL, DeltaSyncResponseHandler.Tracer, "Unexpected Response Content Type: {0}", new object[]
			{
				text
			});
			throw new InvalidServerResponseException(new UnexpectedContentTypeException(text));
		}

		private ItemOperations ParseFetchMtomResponse(DownloadResult deltaSyncResponse)
		{
			Exception ex = null;
			MimeReader mimeReader = new MimeReader(deltaSyncResponse.ResponseStream);
			if (mimeReader.ReadFirstChildPart() && mimeReader.ReadFirstChildPart())
			{
				ItemOperations itemOperations = this.ParseItemOperationsXmlResponse(mimeReader.GetContentReadStream());
				string text = null;
				if (DeltaSyncResponseHandler.TryGetIncludeContentId(itemOperations, out text))
				{
					while (mimeReader.ReadNextPart())
					{
						MimeHeaderReader headerReader = mimeReader.HeaderReader;
						while (headerReader.ReadNextHeader())
						{
							if (headerReader.HeaderId == HeaderId.ContentId)
							{
								if (headerReader.Value == null || headerReader.Value.Length < 2)
								{
									break;
								}
								string value = headerReader.Value.Substring(1, headerReader.Value.Length - 2);
								if (!text.Equals(value, StringComparison.OrdinalIgnoreCase))
								{
									break;
								}
								Stream contentReadStream = mimeReader.GetContentReadStream();
								itemOperations.Responses.Fetch.Message.EmailMessage = TemporaryStorage.Create();
								if (DeltaSyncDecompressor.TryDeCompress(contentReadStream, itemOperations.Responses.Fetch.Message.EmailMessage))
								{
									itemOperations.Responses.Fetch.Message.EmailMessage.Position = 0L;
									return itemOperations;
								}
								itemOperations.Responses.Fetch.Message.EmailMessage.Close();
								itemOperations.Responses.Fetch.Message.EmailMessage = null;
								ex = new MessageDecompressionFailedException(itemOperations.Responses.Fetch.ServerId);
								break;
							}
						}
					}
				}
			}
			if (ex == null)
			{
				ex = new MTOMParsingFailedException();
			}
			this.syncLogSession.LogError((TSLID)675UL, DeltaSyncResponseHandler.Tracer, "Fetch Response Parsing error: {0}", new object[]
			{
				ex
			});
			throw new InvalidServerResponseException(ex);
		}

		private ItemOperations ParseItemOperationsXmlResponse(Stream responseStream)
		{
			return (ItemOperations)this.ParseResponse(() => (ItemOperations)this.ItemOperationsDeserializer.Deserialize(responseStream));
		}

		private const string ContentIdPrefix = "cid:";

		private static readonly Trace Tracer = Microsoft.Exchange.Diagnostics.Components.Net.ExTraceGlobals.DeltaSyncResponseHandlerTracer;

		private readonly SyncLogSession syncLogSession;

		private XmlSerializer syncDeserializer;

		private XmlSerializer itemOperationsDeserializer;

		private XmlSerializer settingsDeserializer;

		private XmlSerializer sendDeserializer;

		private XmlSerializer statelessDeserializer;

		private delegate object ResponseParser();
	}
}
