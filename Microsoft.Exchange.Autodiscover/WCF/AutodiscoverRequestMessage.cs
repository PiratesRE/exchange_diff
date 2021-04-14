using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[MessageContract]
	public abstract class AutodiscoverRequestMessage
	{
		public AutodiscoverRequestMessage()
		{
		}

		internal static bool ValidateRequest<T>(AutodiscoverRequest request, Collection<T> identities, RequestedSettingCollection requestedSettingCollection, ExchangeVersion? requestedVersion, LazyMember<int> maxIdentities, string maxIdentitiesString, out string errorMessage, out ExchangeVersion requestVersion)
		{
			bool result = false;
			if (!AutodiscoverRequestMessage.TryGetRequestVersion(out requestVersion))
			{
				errorMessage = Strings.MissingOrInvalidRequestedServerVersion;
			}
			else if (request == null)
			{
				errorMessage = Strings.InvalidRequest;
			}
			else if (identities == null || identities.Count == 0)
			{
				errorMessage = Strings.NoUsersRequested;
			}
			else if (requestedSettingCollection == null || requestedSettingCollection.Count == 0)
			{
				errorMessage = Strings.NoSettingsRequested;
			}
			else if (identities.Count > maxIdentities.Member)
			{
				errorMessage = string.Format(maxIdentitiesString, maxIdentities.Member);
			}
			else if (requestedVersion != null && !AutodiscoverRequestMessage.IsValidExchangeRequestedVersion(requestedVersion.Value))
			{
				errorMessage = Strings.InvalidRequestedVersion;
			}
			else
			{
				errorMessage = Strings.NoError;
				result = true;
			}
			return result;
		}

		[MessageHeader(MustUnderstand = true)]
		public string RequestedServerVersion { get; set; }

		internal static bool HasBinarySecretHeader(out string binarySecretHeader)
		{
			binarySecretHeader = null;
			int num = 0;
			MessageHeaders incomingMessageHeaders = OperationContext.Current.IncomingMessageHeaders;
			try
			{
				num = incomingMessageHeaders.FindHeader("BinarySecret", "http://schemas.microsoft.com/exchange/2010/Autodiscover");
			}
			catch (MessageHeaderException)
			{
				return false;
			}
			if (num < 0)
			{
				return false;
			}
			binarySecretHeader = incomingMessageHeaders.GetHeader<string>(num);
			return true;
		}

		internal abstract AutodiscoverResponseMessage Execute();

		private static bool IsValidExchangeRequestedVersion(ExchangeVersion version)
		{
			switch (version)
			{
			case ExchangeVersion.Exchange2010:
			case ExchangeVersion.Exchange2010_SP1:
			case ExchangeVersion.Exchange2010_SP2:
			case ExchangeVersion.Exchange2012:
			case ExchangeVersion.Exchange2013:
			case ExchangeVersion.Exchange2013_SP1:
				return true;
			default:
				return false;
			}
		}

		private static bool TryGetRequestVersion(out ExchangeVersion requestVersion)
		{
			requestVersion = ExchangeVersion.Exchange2010;
			bool result;
			try
			{
				MessageHeaders incomingMessageHeaders = OperationContext.Current.IncomingMessageHeaders;
				string header = incomingMessageHeaders.GetHeader<string>("RequestedServerVersion", "http://schemas.microsoft.com/exchange/2010/Autodiscover");
				AutodiscoverRequestMessage.RemapEquivalentRequestedExchangeVersion(ref header);
				result = EnumValidator<ExchangeVersion>.TryParse(header, EnumParseOptions.Default, out requestVersion);
			}
			catch (MessageHeaderException)
			{
				result = false;
			}
			catch (SerializationException)
			{
				result = false;
			}
			return result;
		}

		private static void RemapEquivalentRequestedExchangeVersion(ref string version)
		{
			string text;
			if (!string.IsNullOrEmpty(version) && AutodiscoverRequestMessage.equivalentRequestedExchangeVersionStrings.TryGetValue(version, out text))
			{
				version = text;
			}
		}

		private static Dictionary<string, string> equivalentRequestedExchangeVersionStrings = new Dictionary<string, string>(1)
		{
			{
				"Exchange2012",
				"Exchange2013"
			}
		};

		internal static LazyMember<List<UserConfigurationSettingName>> RestrictedSettings = new LazyMember<List<UserConfigurationSettingName>>(delegate()
		{
			List<UserConfigurationSettingName> list = new List<UserConfigurationSettingName>();
			if (Common.IsPartnerHostedOnly || VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.RestrictedSettings.Enabled)
			{
				list.Add(UserConfigurationSettingName.ActiveDirectoryServer);
				list.Add(UserConfigurationSettingName.InternalOABUrl);
				list.Add(UserConfigurationSettingName.InternalUMUrl);
				list.Add(UserConfigurationSettingName.InternalPop3Connections);
				list.Add(UserConfigurationSettingName.InternalImap4Connections);
				list.Add(UserConfigurationSettingName.InternalSmtpConnections);
				list.Add(UserConfigurationSettingName.InternalWebClientUrls);
				list.Add(UserConfigurationSettingName.InternalEwsUrl);
				list.Add(UserConfigurationSettingName.InternalEmwsUrl);
				list.Add(UserConfigurationSettingName.InternalEcpUrl);
				list.Add(UserConfigurationSettingName.InternalEcpConnectUrl);
				list.Add(UserConfigurationSettingName.InternalEcpPhotoUrl);
				list.Add(UserConfigurationSettingName.InternalEcpDeliveryReportUrl);
				list.Add(UserConfigurationSettingName.InternalEcpEmailSubscriptionsUrl);
				list.Add(UserConfigurationSettingName.InternalEcpPublishingUrl);
				list.Add(UserConfigurationSettingName.InternalEcpRetentionPolicyTagsUrl);
				list.Add(UserConfigurationSettingName.InternalEcpTextMessagingUrl);
				list.Add(UserConfigurationSettingName.InternalEcpVoicemailUrl);
				list.Add(UserConfigurationSettingName.InternalEcpExtensionInstallationUrl);
				list.Add(UserConfigurationSettingName.InternalPhotosUrl);
				list.Add(UserConfigurationSettingName.PublicFolderServer);
			}
			return list;
		});
	}
}
