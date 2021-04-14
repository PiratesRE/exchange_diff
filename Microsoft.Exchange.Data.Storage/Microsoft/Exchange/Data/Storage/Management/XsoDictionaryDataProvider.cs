using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class XsoDictionaryDataProvider : XsoMailboxDataProviderBase
	{
		public XsoDictionaryDataProvider(ExchangePrincipal mailboxOwner, string action) : base(mailboxOwner, action)
		{
		}

		public XsoDictionaryDataProvider(ExchangePrincipal mailboxOwner, ISecurityAccessToken userToken, string action) : base(mailboxOwner, userToken, action)
		{
		}

		public XsoDictionaryDataProvider(MailboxSession session) : base(session)
		{
		}

		internal XsoDictionaryDataProvider()
		{
		}

		protected XsoDictionaryDataProvider(ExchangePrincipal mailboxOwner, string action, Func<MailboxSession, string, UserConfigurationTypes, bool, IUserConfiguration> getUserConfiguration, Func<MailboxSession, string, UserConfigurationTypes, bool, IReadableUserConfiguration> getReadOnlyUserConfiguration = null) : base(mailboxOwner, action)
		{
			this.getUserConfiguration = getUserConfiguration;
			this.getReadOnlyUserConfiguration = (getReadOnlyUserConfiguration ?? ((Func<MailboxSession, string, UserConfigurationTypes, bool, IReadableUserConfiguration>)getUserConfiguration));
		}

		protected XsoDictionaryDataProvider(MailboxSession session, Func<MailboxSession, string, UserConfigurationTypes, bool, IUserConfiguration> getUserConfiguration, Func<MailboxSession, string, UserConfigurationTypes, bool, IReadableUserConfiguration> getReadOnlyUserConfiguration = null) : base(session)
		{
			this.getUserConfiguration = getUserConfiguration;
			this.getReadOnlyUserConfiguration = (getReadOnlyUserConfiguration ?? ((Func<MailboxSession, string, UserConfigurationTypes, bool, IReadableUserConfiguration>)getUserConfiguration));
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			if (filter != null && !(filter is FalseFilter))
			{
				throw new NotSupportedException("filter");
			}
			ADObjectId adUserId = (!base.MailboxSession.MailboxOwner.ObjectId.IsNullOrEmpty()) ? base.MailboxSession.MailboxOwner.ObjectId : null;
			if (rootId != null && rootId is ADObjectId && !ADObjectId.Equals((ADObjectId)rootId, adUserId))
			{
				throw new NotSupportedException("rootId");
			}
			if (!typeof(XsoMailboxConfigurationObject).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			{
				throw new NotSupportedException("FindPaged: " + typeof(T).FullName);
			}
			XsoMailboxConfigurationObject configObject = (XsoMailboxConfigurationObject)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
			if (adUserId != null)
			{
				configObject.MailboxOwnerId = adUserId;
			}
			HashSet<string> uniqueConfigurationNames = new HashSet<string>(from x in configObject.Schema.AllProperties
			where x is XsoDictionaryPropertyDefinition
			select ((XsoDictionaryPropertyDefinition)x).UserConfigurationName);
			foreach (string userConfigurationName in uniqueConfigurationNames)
			{
				try
				{
					this.LoadUserConfigurationToConfigObject(userConfigurationName, configObject);
				}
				catch (CorruptDataException)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "The calendar configuration for {0} is corrupt", base.MailboxSession.MailboxOwner);
					yield break;
				}
				catch (VirusDetectedException)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "The calendar configuration for {0} is virus infected.", base.MailboxSession.MailboxOwner);
					yield break;
				}
			}
			yield return (T)((object)configObject);
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			XsoMailboxConfigurationObject configObject = (XsoMailboxConfigurationObject)instance;
			HashSet<string> hashSet = new HashSet<string>(from x in configObject.Schema.AllProperties
			where x is XsoDictionaryPropertyDefinition && configObject.IsModified((XsoDictionaryPropertyDefinition)x)
			select ((XsoDictionaryPropertyDefinition)x).UserConfigurationName);
			foreach (string userConfigurationName in hashSet)
			{
				this.SaveConfigObjectToUserConfiguration(userConfigurationName, configObject);
			}
		}

		private void LoadUserConfigurationToConfigObject(string userConfigurationName, XsoMailboxConfigurationObject configObject)
		{
			using (IReadableUserConfiguration readableUserConfiguration = this.getReadOnlyUserConfiguration(base.MailboxSession, userConfigurationName, UserConfigurationTypes.Dictionary, false))
			{
				if (readableUserConfiguration != null)
				{
					IDictionary dictionary = readableUserConfiguration.GetDictionary();
					foreach (PropertyDefinition propertyDefinition in configObject.Schema.AllProperties)
					{
						XsoDictionaryPropertyDefinition xsoDictionaryPropertyDefinition = propertyDefinition as XsoDictionaryPropertyDefinition;
						if (xsoDictionaryPropertyDefinition != null && !(xsoDictionaryPropertyDefinition.UserConfigurationName != userConfigurationName) && dictionary.Contains(xsoDictionaryPropertyDefinition.Name))
						{
							configObject.propertyBag.SetField(xsoDictionaryPropertyDefinition, StoreValueConverter.ConvertValueFromStore(xsoDictionaryPropertyDefinition, dictionary[xsoDictionaryPropertyDefinition.Name]));
						}
					}
				}
			}
		}

		private void SaveConfigObjectToUserConfiguration(string userConfigurationName, XsoMailboxConfigurationObject configObject)
		{
			bool flag = false;
			int num = 0;
			do
			{
				using (IUserConfiguration userConfiguration = this.getUserConfiguration(base.MailboxSession, userConfigurationName, UserConfigurationTypes.Dictionary, !flag))
				{
					if (userConfiguration != null)
					{
						IDictionary dictionary;
						try
						{
							dictionary = userConfiguration.GetDictionary();
						}
						catch (CorruptDataException)
						{
							ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "The calendar configuration for {0} is corrupt", base.MailboxSession.MailboxOwner);
							dictionary = new ConfigurationDictionary();
						}
						catch (VirusDetectedException)
						{
							ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "The calendar configuration for {0} is virus infected.", base.MailboxSession.MailboxOwner);
							dictionary = new ConfigurationDictionary();
						}
						foreach (PropertyDefinition propertyDefinition in configObject.Schema.AllProperties)
						{
							XsoDictionaryPropertyDefinition xsoDictionaryPropertyDefinition = propertyDefinition as XsoDictionaryPropertyDefinition;
							if (xsoDictionaryPropertyDefinition != null && !xsoDictionaryPropertyDefinition.IsReadOnly && !(xsoDictionaryPropertyDefinition.UserConfigurationName != userConfigurationName) && configObject.IsModified(xsoDictionaryPropertyDefinition))
							{
								object obj = configObject[xsoDictionaryPropertyDefinition];
								if (obj == null || (obj is ICollection && ((ICollection)obj).Count == 0))
								{
									dictionary.Remove(xsoDictionaryPropertyDefinition.Name);
								}
								else
								{
									dictionary[xsoDictionaryPropertyDefinition.Name] = StoreValueConverter.ConvertValueToStore(obj);
								}
							}
						}
						try
						{
							ExTraceGlobals.FaultInjectionTracer.TraceTest(4289080637U);
							userConfiguration.Save();
							break;
						}
						catch (ObjectExistedException)
						{
							if (flag)
							{
								throw;
							}
							flag = true;
						}
						catch (SaveConflictException)
						{
							if (num >= 5)
							{
								throw;
							}
							num++;
							flag = true;
						}
					}
				}
			}
			while (flag);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<XsoDictionaryDataProvider>(this);
		}

		internal const string OwaUserOptionConfigurationName = "OWA.UserOptions";

		private Func<MailboxSession, string, UserConfigurationTypes, bool, IUserConfiguration> getUserConfiguration = new Func<MailboxSession, string, UserConfigurationTypes, bool, IUserConfiguration>(UserConfigurationHelper.GetMailboxConfiguration);

		private Func<MailboxSession, string, UserConfigurationTypes, bool, IReadableUserConfiguration> getReadOnlyUserConfiguration = new Func<MailboxSession, string, UserConfigurationTypes, bool, IReadableUserConfiguration>(UserConfigurationHelper.GetMailboxConfiguration);
	}
}
