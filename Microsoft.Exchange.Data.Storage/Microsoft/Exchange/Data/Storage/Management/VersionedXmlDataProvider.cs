using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MobileTransport;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class VersionedXmlDataProvider : XsoMailboxDataProviderBase
	{
		public VersionedXmlDataProvider(ExchangePrincipal mailboxOwner, ISecurityAccessToken userToken, string action) : base(mailboxOwner, userToken, action)
		{
		}

		public VersionedXmlDataProvider(ExchangePrincipal mailboxOwner, string action) : base(mailboxOwner, action)
		{
		}

		public VersionedXmlDataProvider(MailboxSession session) : base(session)
		{
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			if (filter != null && !(filter is FalseFilter))
			{
				throw new NotSupportedException("filter");
			}
			if (rootId != null && rootId is ADObjectId && !ADObjectId.Equals((ADObjectId)rootId, base.MailboxSession.MailboxOwner.ObjectId))
			{
				throw new NotSupportedException("rootId");
			}
			if (!typeof(VersionedXmlConfigurationObject).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			{
				throw new NotSupportedException("FindPaged: " + typeof(T).FullName);
			}
			VersionedXmlConfigurationObject configObject = (VersionedXmlConfigurationObject)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
			configObject[VersionedXmlConfigurationObjectSchema.Identity] = base.MailboxSession.MailboxOwner.ObjectId;
			VersionedXmlBase configXml = null;
			using (UserConfiguration mailboxConfiguration = UserConfigurationHelper.GetMailboxConfiguration(base.MailboxSession, configObject.UserConfigurationName, UserConfigurationTypes.XML, false))
			{
				if (mailboxConfiguration != null)
				{
					using (Stream xmlStream = mailboxConfiguration.GetXmlStream())
					{
						try
						{
							configXml = VersionedXmlBase.Deserialize(xmlStream);
						}
						catch (InvalidOperationException ex)
						{
							ExTraceGlobals.XsoTracer.TraceDebug<string>((long)this.GetHashCode(), "Deserialize TextMessagingSettings failed: {0}", ex.ToString());
						}
					}
				}
			}
			if (configXml != null)
			{
				if (configObject.RawVersionedXmlPropertyDefinition.Type != configXml.GetType())
				{
					throw new NotSupportedException("FindPaged: " + typeof(T).FullName);
				}
				configObject[configObject.RawVersionedXmlPropertyDefinition] = configXml;
				TextMessagingAccount textMessagingAccount = configObject as TextMessagingAccount;
				if (textMessagingAccount != null)
				{
					textMessagingAccount.NotificationPreferredCulture = base.MailboxSession.PreferedCulture;
					if (textMessagingAccount.CountryRegionId != null)
					{
						if (string.Equals(textMessagingAccount.CountryRegionId.TwoLetterISORegionName, "US", StringComparison.OrdinalIgnoreCase))
						{
							textMessagingAccount.NotificationPreferredCulture = new CultureInfo("en-US");
						}
						else if (string.Equals(textMessagingAccount.CountryRegionId.TwoLetterISORegionName, "CA", StringComparison.OrdinalIgnoreCase))
						{
							if (textMessagingAccount.NotificationPreferredCulture != null && string.Equals(textMessagingAccount.NotificationPreferredCulture.TwoLetterISOLanguageName, "fr", StringComparison.OrdinalIgnoreCase))
							{
								textMessagingAccount.NotificationPreferredCulture = new CultureInfo("fr-CA");
							}
							else
							{
								textMessagingAccount.NotificationPreferredCulture = new CultureInfo("en-CA");
							}
						}
					}
				}
				ValidationError[] array = configObject.Validate();
				if (array != null && 0 < array.Length)
				{
					ExTraceGlobals.XsoTracer.TraceDebug<string>((long)this.GetHashCode(), "TextMessagingSettings validation failed: {0}", array[0].ToString());
					configObject[configObject.RawVersionedXmlPropertyDefinition] = null;
				}
				configObject.ResetChangeTracking();
			}
			yield return (T)((object)configObject);
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			VersionedXmlConfigurationObject versionedXmlConfigurationObject = (VersionedXmlConfigurationObject)instance;
			using (UserConfiguration mailboxConfiguration = UserConfigurationHelper.GetMailboxConfiguration(base.MailboxSession, versionedXmlConfigurationObject.UserConfigurationName, UserConfigurationTypes.XML, true))
			{
				using (Stream xmlStream = mailboxConfiguration.GetXmlStream())
				{
					VersionedXmlBase.Serialize(xmlStream, (VersionedXmlBase)versionedXmlConfigurationObject[versionedXmlConfigurationObject.RawVersionedXmlPropertyDefinition]);
				}
				mailboxConfiguration.Save();
			}
			instance.ResetChangeTracking();
		}

		protected override void InternalDelete(ConfigurableObject instance)
		{
			VersionedXmlConfigurationObject versionedXmlConfigurationObject = (VersionedXmlConfigurationObject)instance;
			using (UserConfiguration mailboxConfiguration = UserConfigurationHelper.GetMailboxConfiguration(base.MailboxSession, versionedXmlConfigurationObject.UserConfigurationName, UserConfigurationTypes.XML, false))
			{
				if (mailboxConfiguration == null)
				{
					return;
				}
			}
			UserConfigurationHelper.DeleteMailboxConfiguration(base.MailboxSession, versionedXmlConfigurationObject.UserConfigurationName);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<VersionedXmlDataProvider>(this);
		}
	}
}
