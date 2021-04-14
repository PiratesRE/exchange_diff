using System;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.MapiHttp;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.HttpProxy
{
	internal class MapiProxyRequestHandler : BEServerCookieProxyRequestHandler<WebServicesService>
	{
		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.InternalNLBBypass;
			}
		}

		protected override bool ShouldForceUnbufferedClientResponseOutput
		{
			get
			{
				return true;
			}
		}

		protected override bool ShouldSendFullActivityScope
		{
			get
			{
				return false;
			}
		}

		protected override BufferPool GetResponseBufferPool()
		{
			if (MapiProxyRequestHandler.UseCustomNotificationWaitBuffers.Value)
			{
				string text = base.ClientRequest.Headers["X-RequestType"];
				if (!string.IsNullOrEmpty(text) && string.Equals(text, "NotificationWait", StringComparison.OrdinalIgnoreCase))
				{
					return MapiProxyRequestHandler.NotificationWaitBufferPool;
				}
			}
			return base.GetResponseBufferPool();
		}

		protected override bool ShouldCopyHeaderToServerRequest(string headerName)
		{
			return !MapiProxyRequestHandler.ProtectedHeaderNames.Contains(headerName, StringComparer.OrdinalIgnoreCase) && base.ShouldCopyHeaderToServerRequest(headerName);
		}

		protected override void DoProtocolSpecificBeginRequestLogging()
		{
			this.LogClientRequestInfo();
		}

		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			DatabaseBasedAnchorMailbox databaseBasedAnchorMailbox = base.AnchoredRoutingTarget.AnchorMailbox as DatabaseBasedAnchorMailbox;
			if (databaseBasedAnchorMailbox != null)
			{
				ADObjectId database = databaseBasedAnchorMailbox.GetDatabase();
				if (database != null)
				{
					headers[WellKnownHeader.MailboxDatabaseGuid] = database.ObjectGuid.ToString();
				}
			}
			base.AddProtocolSpecificHeadersToServerRequest(headers);
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			string text = base.ClientRequest.QueryString["mailboxId"];
			if (!string.IsNullOrEmpty(text))
			{
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "MailboxGuidWithDomain");
				return this.GetAnchorMailboxFromMailboxId(text);
			}
			text = base.ClientRequest.QueryString["smtpAddress"];
			if (!string.IsNullOrEmpty(text))
			{
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "SMTP");
				return this.GetAnchorMailboxFromSmtpAddress(text);
			}
			text = base.ClientRequest.QueryString["useMailboxOfAuthenticatedUser"];
			bool flag = false;
			if (!string.IsNullOrEmpty(text) && bool.TryParse(text, out flag) && flag)
			{
				return base.ResolveAnchorMailbox();
			}
			if (string.Compare(base.ClientRequest.RequestType, "GET", true) == 0)
			{
				return base.ResolveAnchorMailbox();
			}
			throw new HttpProxyException(HttpStatusCode.BadRequest, HttpProxySubErrorCode.MailboxGuidWithDomainNotFound, "No target mailbox specified.");
		}

		private AnchorMailbox GetAnchorMailboxFromMailboxId(string mailboxId)
		{
			Guid guid = Guid.Empty;
			string domain = string.Empty;
			if (!SmtpAddress.IsValidSmtpAddress(mailboxId))
			{
				throw new HttpProxyException(HttpStatusCode.BadRequest, HttpProxySubErrorCode.MailboxGuidWithDomainNotFound, "Malformed mailbox id.");
			}
			try
			{
				SmtpAddress smtpAddress = new SmtpAddress(mailboxId);
				guid = new Guid(smtpAddress.Local);
				domain = smtpAddress.Domain;
			}
			catch (FormatException innerException)
			{
				throw new HttpProxyException(HttpStatusCode.BadRequest, HttpProxySubErrorCode.MailboxGuidWithDomainNotFound, string.Format("Invalid mailboxGuid {0}", guid), innerException);
			}
			return new MailboxGuidAnchorMailbox(guid, domain, this);
		}

		private AnchorMailbox GetAnchorMailboxFromSmtpAddress(string smtpAddress)
		{
			if (!SmtpAddress.IsValidSmtpAddress(smtpAddress))
			{
				throw new HttpProxyException(HttpStatusCode.BadRequest, HttpProxySubErrorCode.MailboxGuidWithDomainNotFound, "Malformed smtp address.");
			}
			return new SmtpAnchorMailbox(smtpAddress, this);
		}

		private void LogClientRequestInfo()
		{
			if (string.Compare(base.ClientRequest.RequestType, "POST", true) != 0)
			{
				return;
			}
			string clientRequestInfo = MapiHttpEndpoints.GetClientRequestInfo(base.HttpContext);
			base.ClientResponse.AppendToLog("&ClientRequestInfo=" + clientRequestInfo);
			base.Logger.Set(ActivityStandardMetadata.ClientRequestId, clientRequestInfo);
		}

		private const string MailboxIdParameter = "mailboxId";

		private const string SmtpAddressParameter = "smtpAddress";

		private const string UseMailboxOfAuthenticatedUserParameter = "useMailboxOfAuthenticatedUser";

		private const string HttpVerbGet = "GET";

		private const string HttpVerbPost = "POST";

		private const string XRequestType = "X-RequestType";

		private const string ClientRequestInfoLogParameter = "&ClientRequestInfo=";

		private const string RequestTypeEmsmdbNotificationWait = "NotificationWait";

		private static readonly string[] ProtectedHeaderNames = new string[]
		{
			WellKnownHeader.MailboxDatabaseGuid
		};

		private static readonly BoolAppSettingsEntry UseCustomNotificationWaitBuffers = new BoolAppSettingsEntry(HttpProxySettings.Prefix("UseCustomNotificationWaitBuffers"), true, ExTraceGlobals.VerboseTracer);

		private static readonly IntAppSettingsEntry NotificationWaitBufferSize = new IntAppSettingsEntry(HttpProxySettings.Prefix("NotificationWaitBufferSize"), 256, ExTraceGlobals.VerboseTracer);

		private static readonly IntAppSettingsEntry NotificationWaitBuffersPerProcessor = new IntAppSettingsEntry(HttpProxySettings.Prefix("NotificationWaitBuffersPerProcessor"), 512, ExTraceGlobals.VerboseTracer);

		private static readonly BufferPool NotificationWaitBufferPool = new BufferPool(MapiProxyRequestHandler.NotificationWaitBufferSize.Value, MapiProxyRequestHandler.NotificationWaitBuffersPerProcessor.Value, false);
	}
}
