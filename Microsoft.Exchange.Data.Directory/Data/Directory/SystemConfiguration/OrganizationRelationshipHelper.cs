using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public static class OrganizationRelationshipHelper
	{
		internal static GetterDelegate GetOrganizationRelationshipState(string sharedResource, ProviderPropertyDefinition federationEnabledActions)
		{
			return delegate(IPropertyBag properties)
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)properties[federationEnabledActions];
				return multiValuedProperty.Contains(sharedResource);
			};
		}

		internal static SetterDelegate SetOrganizationRelationshipState(string sharedResource, ProviderPropertyDefinition federationEnabledActions)
		{
			return delegate(object isEnabledObject, IPropertyBag properties)
			{
				bool flag = (bool)isEnabledObject;
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)properties[federationEnabledActions];
				bool flag2 = multiValuedProperty.Contains(sharedResource);
				if (flag && !flag2)
				{
					multiValuedProperty.Add(sharedResource);
					return;
				}
				if (!flag && flag2)
				{
					multiValuedProperty.Remove(sharedResource);
				}
			};
		}

		internal static object GetFreeBusyAccessLevel(IPropertyBag properties)
		{
			return OrganizationRelationshipHelper.GetAccessLevel<FreeBusyAccessLevel>(properties, "MSExchange.SharingCalendarFreeBusyLevel:", FreeBusyAccessLevel.None);
		}

		internal static void SetFreeBusyAccessLevel(object value, IPropertyBag properties)
		{
			OrganizationRelationshipHelper.SetAccessLevel<FreeBusyAccessLevel>((FreeBusyAccessLevel)value, properties, "MSExchange.SharingCalendarFreeBusyLevel:");
		}

		internal static object GetFreeBusyAccessScope(IPropertyBag properties)
		{
			return OrganizationRelationshipHelper.GetAccessScope(properties, "MSExchange.SharingCalendarFreeBusyLevel:", OrganizationRelationshipNonAdProperties.FreeBusyAccessScopeCache);
		}

		internal static void SetFreeBusyAccessScope(object value, IPropertyBag properties)
		{
			OrganizationRelationshipHelper.SetAccessScope<FreeBusyAccessLevel>(value as ADObjectId, properties, "MSExchange.SharingCalendarFreeBusyLevel:", FreeBusyAccessLevel.None);
		}

		internal static object GetMailTipsAccessLevel(IPropertyBag properties)
		{
			return OrganizationRelationshipHelper.GetAccessLevel<MailTipsAccessLevel>(properties, "MSExchange.MailTipsAccessLevel:", MailTipsAccessLevel.None);
		}

		internal static void SetMailTipsAccessLevel(object value, IPropertyBag properties)
		{
			OrganizationRelationshipHelper.SetAccessLevel<MailTipsAccessLevel>((MailTipsAccessLevel)value, properties, "MSExchange.MailTipsAccessLevel:");
		}

		internal static object GetMailTipsAccessScope(IPropertyBag properties)
		{
			return OrganizationRelationshipHelper.GetAccessScope(properties, "MSExchange.MailTipsAccessLevel:", OrganizationRelationshipNonAdProperties.MailTipsAccessScopeScopeCache);
		}

		internal static void SetMailTipsAccessScope(object value, IPropertyBag properties)
		{
			OrganizationRelationshipHelper.SetAccessScope<MailTipsAccessLevel>(value as ADObjectId, properties, "MSExchange.MailTipsAccessLevel:", MailTipsAccessLevel.None);
		}

		private static object GetAccessLevel<T>(IPropertyBag properties, string actionPrefix, T defaultLevel)
		{
			MultiValuedProperty<string> actions = (MultiValuedProperty<string>)properties[OrganizationRelationshipSchema.FederationEnabledActions];
			string action = OrganizationRelationshipHelper.GetAction(actions, actionPrefix);
			string levelElement = OrganizationRelationshipHelper.GetLevelElement(action);
			if (levelElement == null)
			{
				return defaultLevel;
			}
			object result;
			try
			{
				result = (T)((object)Enum.Parse(typeof(T), levelElement, true));
			}
			catch (ArgumentNullException)
			{
				result = defaultLevel;
			}
			catch (ArgumentException)
			{
				result = defaultLevel;
			}
			return result;
		}

		private static void SetAccessLevel<T>(T accessLevel, IPropertyBag properties, string prefix)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)properties[OrganizationRelationshipSchema.FederationEnabledActions];
			string andRemoveAction = OrganizationRelationshipHelper.GetAndRemoveAction(multiValuedProperty, prefix);
			string targetElement = OrganizationRelationshipHelper.GetTargetElement(andRemoveAction);
			multiValuedProperty.Add(OrganizationRelationshipHelper.GenerateAction(prefix, accessLevel.ToString(), targetElement));
		}

		private static object GetAccessScope(IPropertyBag properties, string prefix, ADPropertyDefinition cacheDefinition)
		{
			MultiValuedProperty<string> actions = (MultiValuedProperty<string>)properties[OrganizationRelationshipSchema.FederationEnabledActions];
			string action = OrganizationRelationshipHelper.GetAction(actions, prefix);
			string targetElement = OrganizationRelationshipHelper.GetTargetElement(action);
			if (targetElement == null)
			{
				return null;
			}
			Guid guid;
			try
			{
				guid = new Guid(targetElement);
			}
			catch (FormatException)
			{
				return null;
			}
			catch (OverflowException)
			{
				return null;
			}
			ADObjectId adobjectId = (ADObjectId)properties[cacheDefinition];
			if (adobjectId != null && adobjectId.ObjectGuid == guid)
			{
				return adobjectId;
			}
			return new ADObjectId(guid);
		}

		private static void SetAccessScope<T>(ADObjectId objectId, IPropertyBag properties, string prefix, T defaultLevel)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)properties[OrganizationRelationshipSchema.FederationEnabledActions];
			string target = (objectId != null) ? objectId.ObjectGuid.ToString() : null;
			string andRemoveAction = OrganizationRelationshipHelper.GetAndRemoveAction(multiValuedProperty, prefix);
			string text = OrganizationRelationshipHelper.GetLevelElement(andRemoveAction);
			if (text == null)
			{
				text = defaultLevel.ToString();
			}
			multiValuedProperty.Add(OrganizationRelationshipHelper.GenerateAction(prefix, text, target));
		}

		private static string GetLevelElement(string action)
		{
			if (string.IsNullOrEmpty(action))
			{
				return null;
			}
			int num = action.IndexOf(':');
			if (num == -1)
			{
				return null;
			}
			int num2 = action.IndexOf(':', num + 1);
			if (num2 == -1)
			{
				num2 = action.Length;
			}
			return action.Substring(num + 1, num2 - num - 1);
		}

		private static string GetTargetElement(string action)
		{
			if (string.IsNullOrEmpty(action))
			{
				return null;
			}
			int num = action.IndexOf(':');
			if (num == -1)
			{
				return null;
			}
			int num2 = action.IndexOf(':', num + 1);
			if (num2 == -1)
			{
				return null;
			}
			return action.Substring(num2 + 1, action.Length - num2 - 1);
		}

		private static string GenerateAction(string prefix, string level, string target)
		{
			if (target == null)
			{
				return prefix + level;
			}
			return prefix + level + ":" + target;
		}

		private static string GetAction(MultiValuedProperty<string> actions, string prefix)
		{
			foreach (string text in actions)
			{
				if (text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				{
					return text;
				}
			}
			return null;
		}

		private static string GetAndRemoveAction(MultiValuedProperty<string> actions, string prefix)
		{
			string action = OrganizationRelationshipHelper.GetAction(actions, prefix);
			if (action != null)
			{
				actions.Remove(action);
			}
			return action;
		}

		private const string AvailabilityLevelWithSeparator = "MSExchange.SharingCalendarFreeBusyLevel:";

		private const string MailTipsAccessLevelWithSeparator = "MSExchange.MailTipsAccessLevel:";
	}
}
