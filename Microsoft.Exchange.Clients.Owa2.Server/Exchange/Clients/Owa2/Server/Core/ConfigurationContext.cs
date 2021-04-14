using System;
using System.Collections.Specialized;
using System.Web;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Configuration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ConfigurationContext : ConfigurationContextBase, IConfigurationContext
	{
		public ConfigurationContext(UserContext userContext) : this(userContext, null)
		{
		}

		public ConfigurationContext(UserContext userContext, UserConfigurationManager.IAggregationContext aggregationContext)
		{
			this.principal = userContext.ExchangePrincipal;
			this.allowedCapabilitiesFlags = userContext.AllowedCapabilitiesFlags;
			this.MySiteUrl = ((userContext.MailboxIdentity == null) ? null : (userContext.MailboxIdentity.GetOWAMiniRecipient()[ADRecipientSchema.WebPage] as string));
			this.aggregationContext = aggregationContext;
		}

		public override WebBeaconFilterLevels FilterWebBeaconsAndHtmlForms
		{
			get
			{
				return VdirConfiguration.Instance.FilterWebBeaconsAndHtmlForms;
			}
		}

		public override AttachmentPolicy AttachmentPolicy
		{
			get
			{
				return this.GetConfiguration().AttachmentPolicy;
			}
		}

		public ulong SegmentationFlags
		{
			get
			{
				return this.GetConfiguration().SegmentationFlags;
			}
		}

		public string DefaultTheme
		{
			get
			{
				return this.GetConfiguration().DefaultTheme;
			}
		}

		public bool UseGB18030
		{
			get
			{
				return this.GetConfiguration().UseGB18030;
			}
		}

		public bool UseISO885915
		{
			get
			{
				return this.GetConfiguration().UseISO885915;
			}
		}

		public OutboundCharsetOptions OutboundCharset
		{
			get
			{
				return this.GetConfiguration().OutboundCharset;
			}
		}

		public InstantMessagingTypeOptions InstantMessagingType
		{
			get
			{
				return this.GetConfiguration().InstantMessagingType;
			}
		}

		public bool PlacesEnabled
		{
			get
			{
				return this.GetConfiguration().PlacesEnabled;
			}
		}

		public bool WeatherEnabled
		{
			get
			{
				return this.GetConfiguration().WeatherEnabled;
			}
		}

		public bool AllowCopyContactsToDeviceAddressBook
		{
			get
			{
				return this.GetConfiguration().AllowCopyContactsToDeviceAddressBook;
			}
		}

		public AllowOfflineOnEnum AllowOfflineOn
		{
			get
			{
				return this.GetConfiguration().AllowOfflineOn;
			}
		}

		public bool RecoverDeletedItemsEnabled
		{
			get
			{
				return this.GetConfiguration().RecoverDeletedItemsEnabled;
			}
		}

		public string MySiteUrl { get; private set; }

		public bool IsFeatureNotRestricted(ulong feature)
		{
			return this.allowedCapabilitiesFlags == 0UL || (this.allowedCapabilitiesFlags & feature) == feature;
		}

		public override bool IsFeatureEnabled(Feature feature)
		{
			return (feature & (Feature)this.GetConfiguration().SegmentationFlags) == feature && this.IsFeatureNotRestricted((ulong)feature);
		}

		public Feature GetEnabledFeatures()
		{
			Feature feature = (Feature)0UL;
			foreach (object obj in Enum.GetValues(typeof(Feature)))
			{
				if (this.IsFeatureEnabled((Feature)obj))
				{
					feature ^= (Feature)obj;
				}
			}
			return feature;
		}

		public override ulong GetFeaturesEnabled(Feature feature)
		{
			ulong num = (ulong)feature;
			ulong num2 = (ulong)(feature & (Feature)this.GetConfiguration().SegmentationFlags);
			while ((num & 1UL) == 0UL && num != 0UL)
			{
				num >>= 1;
				num2 >>= 1;
			}
			return num2;
		}

		private ConfigurationBase GetConfiguration()
		{
			ConfigurationBase result = this.configurationBase;
			if (result == null)
			{
				if (this.aggregationContext == null)
				{
					result = this.GetPolicyOrVdirConfiguration(null, null, null, null, null, new bool?(false));
				}
				else
				{
					NameValueCollection requestHeaders = null;
					string requestUserAgent = null;
					string rawUrl = null;
					Uri uri = null;
					string userHostAddress = null;
					bool? isLocal = null;
					if (HttpContext.Current != null && HttpContext.Current.Request != null)
					{
						requestHeaders = HttpContext.Current.Request.Headers;
						requestUserAgent = HttpContext.Current.Request.UserAgent;
						rawUrl = HttpContext.Current.Request.RawUrl;
						uri = HttpContext.Current.Request.Url;
						userHostAddress = HttpContext.Current.Request.UserHostAddress;
						isLocal = new bool?(HttpContext.Current.Request.IsLocal);
					}
					OwaConfigurationBaseData data = this.aggregationContext.ReadType<OwaConfigurationBaseData>("OWA.ConfigurationBase", delegate
					{
						result = this.GetPolicyOrVdirConfiguration(requestHeaders, requestUserAgent, rawUrl, uri, userHostAddress, isLocal);
						return AggregatedBaseConfiguration.DataFromConfiguration(result);
					});
					if (result == null)
					{
						result = AggregatedBaseConfiguration.ConfigurationFromData(data);
					}
				}
			}
			this.configurationBase = result;
			return result;
		}

		private ConfigurationBase GetPolicyOrVdirConfiguration(NameValueCollection requestHeaders = null, string requestUserAgent = null, string rawUrl = null, Uri uri = null, string userHostAddress = null, bool? isLocal = false)
		{
			ConfigurationBase configurationBase = null;
			if (this.principal != null)
			{
				configurationBase = OwaMailboxPolicyCache.GetPolicyConfiguration(this.principal.MailboxInfo.Configuration.OwaMailboxPolicy, this.principal.MailboxInfo.OrganizationId);
			}
			return configurationBase ?? VdirConfiguration.GetInstance(requestHeaders, requestUserAgent, rawUrl, uri, userHostAddress, isLocal);
		}

		private readonly ulong allowedCapabilitiesFlags;

		private ExchangePrincipal principal;

		private UserConfigurationManager.IAggregationContext aggregationContext;

		private ConfigurationBase configurationBase;
	}
}
