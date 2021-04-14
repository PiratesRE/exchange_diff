using System;
using System.Globalization;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class SearchCommand : Command
	{
		internal SearchCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfSearches;
		}

		protected override string RootNodeName
		{
			get
			{
				return "Search";
			}
		}

		protected override bool IsInteractiveCommand
		{
			get
			{
				return true;
			}
		}

		internal override bool RightsManagementSupportFlag
		{
			get
			{
				return this.provider != null && this.provider.RightsManagementSupport;
			}
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			try
			{
				this.ParseXmlRequest();
				this.provider = this.CreateProvider(this.reqStoreName);
				if (this.provider == null)
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "Unknown search provider specified!");
					base.XmlResponse = this.GetValidationErrorXml();
					return Command.ExecutionState.Complete;
				}
				XmlDocument xmlResponse = this.BuildXmlResponse();
				this.provider.ParseOptions(this.reqOptionsNode);
				this.provider.ParseQueryNode(this.reqQueryNode);
				MailboxSearchProvider mailboxSearchProvider = this.provider as MailboxSearchProvider;
				if (mailboxSearchProvider != null)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.TotalFolders, mailboxSearchProvider.Folders);
					if (mailboxSearchProvider.DeepTraversal)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.SearchDeep, 1);
					}
				}
				this.provider.Execute();
				this.provider.BuildResponse(this.respStoreNode);
				base.ProtocolLogger.IncrementValueBy("S", PerFolderProtocolLoggerData.ServerAdds, this.provider.NumberResponses);
				base.XmlResponse = xmlResponse;
			}
			catch (AirSyncPermanentException ex)
			{
				if (ex.HttpStatusCode != HttpStatusCode.OK)
				{
					throw;
				}
				AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(ex);
				AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.ProtocolTracer, this, "AirSyncPermanentException was thrown. Location Execute.\r\n{0}", arg);
				base.ProtocolLogger.IncrementValue(ProtocolLoggerData.NumErrors);
				base.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, ex.ErrorStringForProtocolLogger);
				if (base.MailboxLogger != null)
				{
					base.MailboxLogger.SetData(MailboxLogDataName.SearchCommand_Execute_Exception, ex);
				}
				base.XmlResponse = this.GetProviderErrorXml(ex.AirSyncStatusCodeInInt);
				base.PartialFailure = true;
			}
			finally
			{
				if (this.provider is IDisposable)
				{
					((IDisposable)this.provider).Dispose();
				}
				this.provider = null;
			}
			return Command.ExecutionState.Complete;
		}

		protected override bool HandleQuarantinedState()
		{
			base.XmlResponse = this.GetProviderErrorXml(3);
			return false;
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			if (SearchCommand.validationErrorXml == null)
			{
				SearchCommand.validationErrorXml = this.GetProviderErrorXml(2);
			}
			return SearchCommand.validationErrorXml;
		}

		private ISearchProvider CreateProvider(string provider)
		{
			if (provider != null)
			{
				if (provider == "mailbox")
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.SearchProvider, "Mbx");
					return new MailboxSearchProvider(base.MailboxSession, base.SyncStateStorage, base.Context);
				}
				if (provider == "gal")
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.SearchProvider, "Gal");
					return new GalSearchProvider(base.User, base.Request.Culture.LCID, base.Version);
				}
				if (provider == "documentlibrary")
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.SearchProvider, "Doc");
					return new DocumentLibrarySearchProvider(base.User);
				}
			}
			return null;
		}

		private void ParseXmlRequest()
		{
			XmlNode xmlRequest = base.XmlRequest;
			XmlNode firstChild = xmlRequest.FirstChild;
			XmlNode xmlNode = firstChild["Name", "Search:"];
			this.reqStoreName = xmlNode.InnerText.ToLower(CultureInfo.InvariantCulture);
			this.reqQueryNode = firstChild["Query", "Search:"];
			this.reqOptionsNode = firstChild["Options", "Search:"];
		}

		private XmlDocument BuildXmlResponse()
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = xmlDocument.CreateElement("Search", "Search:");
			xmlDocument.AppendChild(xmlNode);
			XmlNode xmlNode2 = xmlDocument.CreateElement("Status", "Search:");
			xmlNode2.InnerText = 1.ToString(CultureInfo.InvariantCulture);
			xmlNode.AppendChild(xmlNode2);
			XmlNode xmlNode3 = xmlDocument.CreateElement("Response", "Search:");
			xmlNode.AppendChild(xmlNode3);
			this.respStoreNode = xmlDocument.CreateElement("Store", "Search:");
			xmlNode3.AppendChild(this.respStoreNode);
			return xmlDocument;
		}

		private XmlDocument GetProviderErrorXml(int statusCode)
		{
			XmlDocument xmlDocument = this.BuildXmlResponse();
			XmlNode xmlNode = xmlDocument["Search", "Search:"];
			XmlNode xmlNode2 = xmlNode["Response", "Search:"];
			XmlNode xmlNode3 = xmlNode2["Store", "Search:"];
			XmlNode xmlNode4 = xmlNode3.OwnerDocument.CreateElement("Status", "Search:");
			xmlNode3.AppendChild(xmlNode4);
			xmlNode4.InnerText = statusCode.ToString(CultureInfo.InvariantCulture);
			return xmlDocument;
		}

		private static XmlDocument validationErrorXml;

		private string reqStoreName;

		private XmlElement respStoreNode;

		private XmlElement reqQueryNode;

		private XmlElement reqOptionsNode;

		private ISearchProvider provider;

		private struct SearchProviders
		{
			public const string Mailbox = "mailbox";

			public const string Gal = "gal";

			public const string DocumentLibrary = "documentlibrary";
		}
	}
}
