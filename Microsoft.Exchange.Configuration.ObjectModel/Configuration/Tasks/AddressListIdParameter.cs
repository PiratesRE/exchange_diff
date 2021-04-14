using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AddressListIdParameter : ADIdParameter
	{
		public AddressListIdParameter()
		{
		}

		public AddressListIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public AddressListIdParameter(AddressList adList) : base(adList.Id)
		{
		}

		public AddressListIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected AddressListIdParameter(string identity) : base(identity)
		{
		}

		public static AddressListIdParameter Parse(string identity)
		{
			return new AddressListIdParameter(identity);
		}

		internal static ADObjectId GetRootContainerId(IConfigurationSession scSession)
		{
			return AddressListIdParameter.GetRootContainerId(scSession, null);
		}

		internal static ADObjectId GetRootContainerId(IConfigurationSession scSession, OrganizationId currentOrg)
		{
			if (scSession == null)
			{
				throw new ArgumentNullException("scSession");
			}
			ADObjectId adobjectId;
			if (currentOrg == null || currentOrg.ConfigurationUnit == null)
			{
				adobjectId = scSession.GetOrgContainerId();
			}
			else
			{
				adobjectId = currentOrg.ConfigurationUnit;
			}
			return adobjectId.GetDescendantId(AddressList.RdnAlContainerToOrganization);
		}

		internal static string[] GetCommonNames(string identityString)
		{
			List<string> list = new List<string>();
			string[] array = Regex.Split(identityString, "(\\\\+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text))
				{
					if ('\\' == text[0])
					{
						if (text.Length % 2 == 0)
						{
							stringBuilder.Append(text.Replace("\\\\", "\\"));
						}
						else
						{
							if (stringBuilder.Length > 0)
							{
								list.Add(stringBuilder.ToString());
							}
							stringBuilder.Length = 0;
							stringBuilder.Append(text.Substring(1).Replace("\\\\", "\\"));
						}
					}
					else
					{
						stringBuilder.Append(text);
					}
				}
			}
			if (stringBuilder.Length > 0)
			{
				list.Add(stringBuilder.ToString());
			}
			return list.ToArray();
		}

		internal override IEnumerableFilter<T> GetEnumerableFilter<T>()
		{
			return AddressListIdParameter.AddressListFilter<T>.GetInstance();
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(AddressBookBase))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			IConfigurationSession scSession = session as IConfigurationSession;
			if ("\\" == base.RawIdentity)
			{
				notFoundReason = null;
				ADObjectId rootContainerId = AddressListIdParameter.GetRootContainerId(scSession);
				return EnumerableWrapper<T>.GetWrapper(base.GetADObjectIdObjects<T>(rootContainerId, rootId, subTreeSession, optionalData), this.GetEnumerableFilter<T>());
			}
			return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
		}

		internal override IEnumerable<T> GetObjectsInOrganization<T>(string identityString, ADObjectId rootId, IDirectorySession session, OptionalIdentityData optionalData)
		{
			IConfigurationSession scSession = session as IConfigurationSession;
			string[] commonNames = AddressListIdParameter.GetCommonNames(identityString);
			ADObjectId identity = this.ResolveAddressListId(AddressListIdParameter.GetRootContainerId(scSession), commonNames);
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(base.GetADObjectIdObjects<T>(identity, rootId, session, optionalData));
			if (wrapper.HasElements())
			{
				return wrapper;
			}
			wrapper = EnumerableWrapper<T>.GetWrapper(base.GetObjectsInOrganization<T>(identityString, rootId, session, optionalData));
			if (wrapper.HasElements() || commonNames.Length == 1 || !identityString.Contains("*"))
			{
				return wrapper;
			}
			Queue<ADObjectId> queue = new Queue<ADObjectId>();
			queue.Enqueue(rootId ?? AddressListIdParameter.GetRootContainerId(scSession));
			for (int i = 0; i < commonNames.Length - 1; i++)
			{
				Queue<ADObjectId> queue2 = new Queue<ADObjectId>();
				string name = commonNames[i];
				foreach (ADObjectId rootId2 in queue)
				{
					QueryFilter filter = base.CreateWildcardOrEqualFilter(ADObjectSchema.Name, name);
					IEnumerable<T> enumerable = this.PerformSearch<T>(filter, rootId2, session, false);
					foreach (T t in enumerable)
					{
						queue2.Enqueue((ADObjectId)t.Identity);
					}
				}
				queue = queue2;
			}
			string name2 = commonNames[commonNames.Length - 1];
			List<IEnumerable<T>> list = new List<IEnumerable<T>>();
			foreach (ADObjectId rootId3 in queue)
			{
				QueryFilter filter2 = base.CreateWildcardOrEqualFilter(ADObjectSchema.Name, name2);
				IEnumerable<T> item = base.PerformPrimarySearch<T>(filter2, rootId3, session, false, optionalData);
				list.Add(item);
			}
			return EnumerableWrapper<T>.GetWrapper(list);
		}

		private ADObjectId ResolveAddressListId(ADObjectId rootContainerId, string[] commonName)
		{
			ADObjectId adobjectId = rootContainerId;
			for (int i = 0; i < commonName.Length; i++)
			{
				adobjectId = adobjectId.GetChildId(commonName[i]);
			}
			return adobjectId;
		}

		private class AddressListFilter<T> : IEnumerableFilter<T>
		{
			public static AddressListIdParameter.AddressListFilter<T> GetInstance()
			{
				return AddressListIdParameter.AddressListFilter<T>.addressListFilter;
			}

			public bool AcceptElement(T element)
			{
				AddressBookBase addressBookBase = element as AddressBookBase;
				return addressBookBase != null && !addressBookBase.IsGlobalAddressList && !addressBookBase.IsInSystemAddressListContainer;
			}

			public override bool Equals(object obj)
			{
				return obj is AddressListIdParameter.AddressListFilter<T>;
			}

			public override int GetHashCode()
			{
				return typeof(AddressListIdParameter.AddressListFilter<T>).GetHashCode();
			}

			private static AddressListIdParameter.AddressListFilter<T> addressListFilter = new AddressListIdParameter.AddressListFilter<T>();
		}
	}
}
