using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Autodiscover.Providers.Outlook
{
	internal class MapiHttpProvider
	{
		public MapiHttpProvider()
		{
			this.loggingStrategy = null;
		}

		public MapiHttpProvider(Action<string, object> loggingStrategy)
		{
			this.loggingStrategy = loggingStrategy;
		}

		public bool ShouldWriteMapiHttpProtocolNode(UserConfigurationSettings settings, int clientMapiHttpResponseVersion, bool? mapiHttpOverrideRegistryValue, Version minimumMapiHttpAutodiscoverVersion, bool useMapiHttpADSettingFlightEnabled, bool mapiHttpFlightEnabled, bool mapiHttpForOutlook14FlightEnabled, Version callingOutlookVersion)
		{
			bool flag = false;
			bool flag2 = false;
			if (clientMapiHttpResponseVersion > 0)
			{
				if (mapiHttpOverrideRegistryValue != null)
				{
					flag = mapiHttpOverrideRegistryValue.Value;
					if (flag)
					{
						flag = this.VerifyMapiHttpServerVersion(settings, minimumMapiHttpAutodiscoverVersion, "MapiHttpRegKeySetting");
					}
					else
					{
						this.Log("ForceDisabled(Registry)");
					}
				}
				else if (useMapiHttpADSettingFlightEnabled)
				{
					bool? flag3 = settings.Get<bool?>(UserConfigurationSettingName.MapiHttpEnabledForUser);
					flag = ((flag3 != null) ? flag3.Value : settings.Get<bool>(UserConfigurationSettingName.MapiHttpEnabled));
					if (flag)
					{
						flag = this.VerifyMapiHttpServerVersion(settings, MapiHttpProvider.MinimumMapiHttpServerVersion, "MapiHttpADSetting");
					}
					else
					{
						this.Log("ForceDisabled(AD)");
					}
				}
				else if (callingOutlookVersion != null)
				{
					if (callingOutlookVersion.Major == 14)
					{
						flag = mapiHttpForOutlook14FlightEnabled;
						this.Log(flag ? "Flighted(Outlook 14)" : "FlightedDisabled(Outlook 14)");
					}
					else if (callingOutlookVersion.Major >= 15)
					{
						flag = mapiHttpFlightEnabled;
						this.Log(flag ? "Flighted" : "FlightedDisabled");
					}
					else
					{
						this.Log("VersionTooLow");
					}
				}
				else
				{
					this.Log("ParseFailed");
				}
				if (flag)
				{
					MapiHttpProtocolUrls mapiHttpProtocolUrls = settings.Get<MapiHttpProtocolUrls>(UserConfigurationSettingName.MapiHttpUrls);
					flag2 = (mapiHttpProtocolUrls != null && mapiHttpProtocolUrls.HasUrls);
				}
			}
			else
			{
				this.Log("NotRequested");
			}
			return flag && flag2;
		}

		private bool VerifyMapiHttpServerVersion(UserConfigurationSettings settings, Version minimumVersion, string logPrefix)
		{
			Version version = new Version(settings.Get<string>(UserConfigurationSettingName.MailboxVersion));
			bool flag = version >= minimumVersion;
			string value = string.Format("{0}({1}): Compared (minimum: {2}; target: {3})", new object[]
			{
				flag ? "Enabled" : "IncompatibleVersion",
				logPrefix,
				minimumVersion.ToString(),
				version.ToString()
			});
			this.Log(value);
			return flag;
		}

		private void Log(object value)
		{
			if (this.loggingStrategy != null)
			{
				this.loggingStrategy("MapiHttpEnabledSource", value);
			}
		}

		public bool TryWriteMapiHttpNodes(UserConfigurationSettings settings, XmlWriter xmlFragment, int clientMapiHttpResponseVersion, ClientAccessModes clientAccessMode)
		{
			MapiHttpProtocolUrls mapiHttpProtocolUrls = settings.Get<MapiHttpProtocolUrls>(UserConfigurationSettingName.MapiHttpUrls);
			bool flag = mapiHttpProtocolUrls != null && mapiHttpProtocolUrls.HasUrls;
			if (flag)
			{
				xmlFragment.WriteStartElement("Protocol", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
				xmlFragment.WriteAttributeString("Type", null, "mapiHttp");
				xmlFragment.WriteAttributeString("Version", null, Math.Min(clientMapiHttpResponseVersion, 1).ToString());
				this.WriteMapiHttpProtocolUrls(clientAccessMode, MapiHttpProtocolUrls.Protocol.Emsmdb, mapiHttpProtocolUrls, xmlFragment);
				this.WriteMapiHttpProtocolUrls(clientAccessMode, MapiHttpProtocolUrls.Protocol.Nspi, mapiHttpProtocolUrls, xmlFragment);
				xmlFragment.WriteEndElement();
				return true;
			}
			return false;
		}

		private void WriteMapiHttpProtocolUrls(ClientAccessModes clientAccessMode, MapiHttpProtocolUrls.Protocol protocol, MapiHttpProtocolUrls urls, XmlWriter xmlFragment)
		{
			xmlFragment.WriteStartElement(MapiHttpProvider.ProtocolNameToElementName[protocol], "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
			Uri uri;
			if ((clientAccessMode & ClientAccessModes.InternalAccess) != ClientAccessModes.None && urls.TryGetProtocolUrl(ClientAccessType.Internal, protocol, out uri))
			{
				xmlFragment.WriteElementString("InternalUrl", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", uri.ToString());
			}
			if ((clientAccessMode & ClientAccessModes.ExternalAccess) != ClientAccessModes.None && urls.TryGetProtocolUrl(ClientAccessType.External, protocol, out uri))
			{
				xmlFragment.WriteElementString("ExternalUrl", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", uri.ToString());
			}
			xmlFragment.WriteEndElement();
		}

		public const int ServerMapiHttpResponseVersion = 1;

		private const string MapiHttpAttributeName = "mapiHttp";

		private const string VersionAttributeName = "Version";

		private const string EmsmdbElementName = "MailStore";

		private const string NspiElementName = "AddressBook";

		private const string InternalUrlElementName = "InternalUrl";

		private const string ExternalUrlElementName = "ExternalUrl";

		private const string LoggingKey = "MapiHttpEnabledSource";

		public static readonly Version MinimumMapiHttpServerVersion = new Version(15, 0, 847, 0);

		private static readonly Dictionary<MapiHttpProtocolUrls.Protocol, string> ProtocolNameToElementName = new Dictionary<MapiHttpProtocolUrls.Protocol, string>
		{
			{
				MapiHttpProtocolUrls.Protocol.Emsmdb,
				"MailStore"
			},
			{
				MapiHttpProtocolUrls.Protocol.Nspi,
				"AddressBook"
			}
		};

		private readonly Action<string, object> loggingStrategy;
	}
}
