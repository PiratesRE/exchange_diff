using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess.Handler;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal sealed class SessionAdaptor : ISession
	{
		public SessionAdaptor(StoreSession session)
		{
			this.session = session;
		}

		public bool TryResolveToNamedProperty(PropertyTag propertyTag, out NamedProperty namedProperty)
		{
			if (propertyTag.IsNamedProperty)
			{
				this.PropertyIdList[0] = (ushort)propertyTag.PropertyId;
				NamedPropertyDefinition.NamedPropertyKey[] namesFromIds = NamedPropConverter.GetNamesFromIds(this.session, this.PropertyIdList);
				namedProperty = namesFromIds[0].ToNamedProperty();
				return namedProperty != null;
			}
			namedProperty = null;
			return false;
		}

		public bool TryResolveFromNamedProperty(NamedProperty namedProperty, ref PropertyTag propertyTag)
		{
			NamedPropertyDefinition.NamedPropertyKey namedPropertyKey = namedProperty.ToNamedPropertyKey();
			if (namedPropertyKey != null)
			{
				this.NamedPropertyList[0] = namedPropertyKey;
				ushort[] idsFromNames = NamedPropConverter.GetIdsFromNames(this.session, true, this.NamedPropertyList);
				propertyTag = new PropertyTag((PropertyId)idsFromNames[0], propertyTag.PropertyType);
				return true;
			}
			return false;
		}

		private List<ushort> PropertyIdList
		{
			get
			{
				if (this.propertyIdList == null)
				{
					ushort[] collection = new ushort[1];
					this.propertyIdList = new List<ushort>(collection);
				}
				return this.propertyIdList;
			}
		}

		private List<NamedPropertyDefinition.NamedPropertyKey> NamedPropertyList
		{
			get
			{
				if (this.namedPropertyList == null)
				{
					NamedPropertyDefinition.NamedPropertyKey[] collection = new NamedPropertyDefinition.NamedPropertyKey[1];
					this.namedPropertyList = new List<NamedPropertyDefinition.NamedPropertyKey>(collection);
				}
				return this.namedPropertyList;
			}
		}

		private readonly StoreSession session;

		private List<ushort> propertyIdList;

		private List<NamedPropertyDefinition.NamedPropertyKey> namedPropertyList;
	}
}
