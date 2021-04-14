using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class JsonMetadataDocument
	{
		public string id;

		public string version;

		public string name;

		public string realm;

		public string serviceName;

		public string issuer;

		public string[] allowedAudiences;

		public JsonKey[] keys;

		public JsonEndpoint[] endpoints;
	}
}
