using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure
{
	internal static class SearchHelper
	{
		public static string ConvertToString(object item, Type type)
		{
			if (item != null)
			{
				type = (type ?? item.GetType());
				if (type == typeof(ExchangeObjectVersion))
				{
					return ((ExchangeObjectVersion)item).ToInt64().ToString();
				}
				if (typeof(Enum).IsAssignableFrom(type))
				{
					return item.ToString();
				}
				if (type == typeof(ProxyAddressCollection))
				{
					ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)item;
					if (proxyAddressCollection.Count <= 0)
					{
						goto IL_21C;
					}
					StringBuilder stringBuilder = new StringBuilder();
					using (MultiValuedProperty<ProxyAddress>.Enumerator enumerator = proxyAddressCollection.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ProxyAddress item2 = enumerator.Current;
							stringBuilder.Append(SearchHelper.ConvertToString(item2, typeof(ProxyAddress)));
							stringBuilder.Append(',');
						}
						goto IL_21C;
					}
				}
				if (type == typeof(ADObjectId))
				{
					return SearchHelper.ConvertToString(((ADObjectId)item).GetBytes(), typeof(byte[]));
				}
				if (type == typeof(OrganizationId))
				{
					return SearchHelper.ConvertToString(((OrganizationId)item).GetBytes(Encoding.Unicode), typeof(byte[]));
				}
				if (type == typeof(string))
				{
					return (string)item;
				}
				if (type == typeof(ProxyAddress))
				{
					return ((ProxyAddress)item).ProxyAddressString;
				}
				if (type == typeof(SmtpAddress))
				{
					return SearchHelper.ConvertToString(((SmtpAddress)item).GetBytes(), typeof(byte[]));
				}
				if (type == typeof(Guid))
				{
					return ((Guid)item).ToString();
				}
				if (type == typeof(SmtpDomain))
				{
					return ((SmtpDomain)item).ToString();
				}
				if (type == typeof(SecurityIdentifier))
				{
					return ((SecurityIdentifier)item).ToString();
				}
				if (type == typeof(byte[]))
				{
					return Convert.ToBase64String((byte[])item);
				}
				return ValueConvertor.ConvertValueToString(item, null);
			}
			IL_21C:
			return string.Empty;
		}

		public static T ConvertFromString<T>(string item)
		{
			return (T)((object)SearchHelper.ConvertFromString(item, typeof(T)));
		}

		public static object ConvertFromString(string item, Type type)
		{
			if (!string.IsNullOrEmpty(item))
			{
				type = (type ?? item.GetType());
				if (type == typeof(ExchangeObjectVersion))
				{
					return new ExchangeObjectVersion(long.Parse(item));
				}
				if (typeof(Enum).IsAssignableFrom(type))
				{
					return Enum.Parse(type, item);
				}
				if (type == typeof(ADObjectId))
				{
					return new ADObjectId(SearchHelper.ConvertFromString<byte[]>(item));
				}
				if (type == typeof(ProxyAddressCollection))
				{
					ProxyAddressCollection proxyAddressCollection = new ProxyAddressCollection();
					foreach (string item2 in item.Split(new char[]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries))
					{
						proxyAddressCollection.Add(SearchHelper.ConvertFromString<ProxyAddress>(item2));
					}
					return proxyAddressCollection;
				}
				if (type == typeof(OrganizationId))
				{
					OrganizationId result;
					if (OrganizationId.TryCreateFromBytes(SearchHelper.ConvertFromString<byte[]>(item), Encoding.Unicode, out result))
					{
						return result;
					}
				}
				else
				{
					if (type == typeof(string))
					{
						return item;
					}
					if (type == typeof(ProxyAddress))
					{
						return ProxyAddress.Parse(item);
					}
					if (type == typeof(SmtpAddress))
					{
						return new SmtpAddress(SearchHelper.ConvertFromString<byte[]>(item));
					}
					if (type == typeof(Guid))
					{
						return new Guid(item);
					}
					if (type == typeof(SmtpDomain))
					{
						return SmtpDomain.Parse(item);
					}
					if (type == typeof(SecurityIdentifier))
					{
						return new SecurityIdentifier(item);
					}
					if (type == typeof(byte[]))
					{
						return Convert.FromBase64String(item);
					}
					return ValueConvertor.ConvertValueFromString(item, type, null);
				}
			}
			return null;
		}

		public static string ConvertToString(object item, PropertyDefinition definition)
		{
			ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)definition;
			if (providerPropertyDefinition.IsMultivalued)
			{
				MultiValuedPropertyBase multiValuedPropertyBase = item as MultiValuedPropertyBase;
				if (multiValuedPropertyBase != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (object item2 in ((IEnumerable)multiValuedPropertyBase))
					{
						string text = SearchHelper.ConvertToString(item2, definition.Type);
						stringBuilder.Append(text.Length);
						stringBuilder.Append("|");
						stringBuilder.Append(text);
					}
					return stringBuilder.ToString();
				}
			}
			return SearchHelper.ConvertToString(item, definition.Type);
		}

		public static object ConvertFromString(string item, PropertyDefinition definition)
		{
			ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)definition;
			if (providerPropertyDefinition.IsMultivalued && !string.IsNullOrEmpty(item))
			{
				List<object> list = new List<object>();
				int num = 0;
				do
				{
					int num2 = item.IndexOf("|", num);
					int num3 = int.Parse(item.Substring(num, num2 - num));
					num2++;
					string item2 = item.Substring(num2, num3);
					num = num2 + num3;
					list.Add(SearchHelper.ConvertFromString(item2, definition.Type));
				}
				while (num < item.Length);
				return ValueConvertor.CreateGenericMultiValuedProperty(providerPropertyDefinition, false, list, null, null);
			}
			return SearchHelper.ConvertFromString(item, definition.Type);
		}
	}
}
