using System;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class AppStateResponseAsset
	{
		public string MarketplaceAssetID { get; set; }

		public string ExtensionID { get; set; }

		public Version Version { get; set; }

		public OmexConstants.AppState? State { get; set; }

		internal AppStateResponseAsset()
		{
		}

		internal AppStateResponseAsset(XElement element, BaseAsyncCommand.LogResponseParseFailureEventCallback logParseFailureCallback)
		{
			this.MarketplaceAssetID = this.ParseMarketplaceAssetID(element, OmexConstants.OfficeNamespace + "assetid", logParseFailureCallback);
			if (!string.IsNullOrWhiteSpace(this.MarketplaceAssetID))
			{
				this.ExtensionID = this.ParseExtensionID(element, OmexConstants.OfficeNamespace + "prodid", logParseFailureCallback);
				this.Version = this.ParseVersion(element, OmexConstants.OfficeNamespace + "ver", logParseFailureCallback);
				this.State = this.ParseState(element, OmexConstants.OfficeNamespace + "state", logParseFailureCallback);
			}
		}

		private string ParseMarketplaceAssetID(XElement element, XName assetIDKey, BaseAsyncCommand.LogResponseParseFailureEventCallback logParseFailureCallback)
		{
			string text = (string)element.Attribute(assetIDKey);
			if (string.IsNullOrWhiteSpace(text))
			{
				AppStateResponseAsset.Tracer.TraceError<XElement>(0L, "AppStateResponseAsset.ParseMarketplaceAssetID: Marketplace asset id was not returned: {0}", element);
				logParseFailureCallback(ApplicationLogicEventLogConstants.Tuple_AppStateResponseInvalidMarketplaceAssetID, text, element);
			}
			return text;
		}

		private string ParseExtensionID(XElement element, XName extensionIDKey, BaseAsyncCommand.LogResponseParseFailureEventCallback logParseFailureCallback)
		{
			string text = null;
			string text2 = (string)element.Attribute(extensionIDKey);
			if (!string.IsNullOrWhiteSpace(text2))
			{
				text = ExtensionDataHelper.FormatExtensionId(text2);
				Guid guid;
				if (!GuidHelper.TryParseGuid(text, out guid))
				{
					text = null;
				}
			}
			if (text == null)
			{
				AppStateResponseAsset.Tracer.TraceError<XElement>(0L, "AppStateResponseAsset.ParseExtensionID: Extension id is invalid: {0}", element);
				logParseFailureCallback(ApplicationLogicEventLogConstants.Tuple_AppStateResponseInvalidExtensionID, this.MarketplaceAssetID, element);
			}
			return text;
		}

		private Version ParseVersion(XElement element, XName versionKey, BaseAsyncCommand.LogResponseParseFailureEventCallback logParseFailureCallback)
		{
			Version result = null;
			string versionAsString = (string)element.Attribute(versionKey);
			if (!ExtensionData.TryParseVersion(versionAsString, out result))
			{
				AppStateResponseAsset.Tracer.TraceError<XElement>(0L, "AppStateResponseAsset.ParseVersion: Unable to parse version for: {0}", element);
				logParseFailureCallback(ApplicationLogicEventLogConstants.Tuple_AppStateResponseInvalidVersion, this.MarketplaceAssetID, element);
			}
			return result;
		}

		private OmexConstants.AppState? ParseState(XElement element, XName stateKey, BaseAsyncCommand.LogResponseParseFailureEventCallback logParseFailureCallback)
		{
			OmexConstants.AppState? result = null;
			string text = (string)element.Attribute(stateKey);
			int value;
			if (!string.IsNullOrWhiteSpace(text) && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
			{
				result = new OmexConstants.AppState?((OmexConstants.AppState)value);
			}
			if (result == null)
			{
				AppStateResponseAsset.Tracer.TraceError<XElement>(0L, "AppStateResponseAsset.ParseState: Unable to parse state for: {0}", element);
				logParseFailureCallback(ApplicationLogicEventLogConstants.Tuple_AppStateResponseInvalidState, this.MarketplaceAssetID, element);
			}
			return result;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;
	}
}
