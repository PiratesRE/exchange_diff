using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class OmexConstants
	{
		internal static readonly XNamespace OfficeNamespace = XNamespace.Get("urn:schemas-microsoft-com:office:office");

		public enum AppState
		{
			Undefined = -1,
			Killed,
			OK,
			Withdrawn,
			Flagged,
			WithdrawingSoon
		}

		public class StringKeys
		{
			public const string AppStateProductId = "prodid";

			public const string Asset = "asset";

			public const string AssetId = "assetid";

			public const string Name = "name";

			public const string Service = "service";

			public const string State = "state";

			public const string Token = "token";

			public const string Url = "url";

			public const string Version = "ver";

			public const string AppInstallInfoQuery15 = "AppInstallInfoQuery15";

			public const string AppQuery15 = "AppQuery15";

			public const string AppStateQuery15 = "AppStateQuery15";

			public const string AppInfoQuery15 = "AppInfoQuery15";
		}
	}
}
