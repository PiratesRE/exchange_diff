using System;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal interface IAirSyncRequest
	{
		string LogonUserName { get; }

		string Url { get; }

		string PathAndQuery { get; }

		Stream InputStream { get; }

		bool IsEmpty { get; }

		int ContentLength { get; }

		string ContentType { get; }

		Encoding ContentEncoding { get; }

		bool IsSecureConnection { get; }

		string UserHostName { get; }

		string HostHeaderInfo { get; }

		WindowsIdentity LogonUserIdentity { get; }

		uint? PolicyKey { get; }

		string DescriptionForEventLog { get; }

		CommandType CommandType { get; }

		int Version { get; }

		string VersionString { get; }

		bool HasOutlookExtensions { get; }

		DeviceIdentity DeviceIdentity { get; }

		string User { get; }

		string UserAgent { get; }

		XmlDocument XmlDocument { get; set; }

		XmlElement CommandXml { get; }

		CultureInfo Culture { get; }

		bool WasDCProxied { get; }

		string DCProxyHeader { get; }

		bool WasProxied { get; }

		bool WasFromCafe { get; }

		bool WasBasicAuthProxied { get; }

		string ProxyHeader { get; }

		string VDirSettingsHeader { get; }

		bool AcceptMultiPartResponse { get; }

		string XMSWLHeader { get; }

		HttpRequest GetRawHttpRequest();

		XmlDocument LoadRequestDocument();

		string GetLegacyUrlParameter(string name);

		string GetHeadersAsString();

		void ParseAndValidateHeaders();

		void TraceHeaders();

		void PrepareToHang();

		bool SupportsExtension(OutlookExtension extension);
	}
}
