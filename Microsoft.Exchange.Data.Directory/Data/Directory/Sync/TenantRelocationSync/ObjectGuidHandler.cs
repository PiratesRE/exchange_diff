using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class ObjectGuidHandler<T> : CustomPropertyHandlerBase
	{
		private bool IsString
		{
			get
			{
				return typeof(T) == typeof(string);
			}
		}

		private Guid GetGuid(object v)
		{
			Guid result;
			if (this.IsString)
			{
				string g = (string)v;
				result = new Guid(g);
			}
			else
			{
				result = new Guid((byte[])v);
			}
			return result;
		}

		private void AddValue(DirectoryAttributeModification mod, Guid v)
		{
			if (this.IsString)
			{
				mod.Add(v.ToString());
				return;
			}
			mod.Add(v.ToByteArray());
		}

		public override List<ADObjectId> EnumerateObjectDependenciesInSource(TenantRelocationSyncTranslator translator, DirectoryAttribute sourceValue)
		{
			List<ADObjectId> list = new List<ADObjectId>();
			object[] values = sourceValue.GetValues(typeof(T));
			foreach (object v in values)
			{
				Guid guid = this.GetGuid(v);
				if (!guid.Equals(EmailAddressPolicy.PolicyGuid) && !Guid.Empty.Equals(guid))
				{
					list.Add(new ADObjectId(guid));
				}
			}
			return list;
		}

		public override void UpdateModifyRequestForTarget(TenantRelocationSyncTranslator translator, DirectoryAttribute sourceValue, ref DirectoryAttributeModification mod)
		{
			object[] values = sourceValue.GetValues(typeof(T));
			foreach (object obj in values)
			{
				Guid guid = this.GetGuid(obj);
				if (guid.Equals(EmailAddressPolicy.PolicyGuid) || Guid.Empty.Equals(guid))
				{
					if (this.IsString)
					{
						mod.Add((string)obj);
					}
					else
					{
						mod.Add((byte[])obj);
					}
				}
				else
				{
					DistinguishedNameMapItem distinguishedNameMapItem = translator.Mappings.LookupByCorrelationGuid(guid);
					if (distinguishedNameMapItem == null)
					{
						this.AddValue(mod, guid);
					}
					else
					{
						this.AddValue(mod, distinguishedNameMapItem.TargetDN.ObjectGuid);
					}
				}
			}
			mod.Name = sourceValue.Name;
			mod.Operation = DirectoryAttributeOperation.Replace;
		}
	}
}
