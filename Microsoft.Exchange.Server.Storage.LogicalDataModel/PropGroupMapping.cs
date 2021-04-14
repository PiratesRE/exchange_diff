using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public struct PropGroupMapping
	{
		public PropGroupMapping(Context context, Mailbox mailbox)
		{
			this.mappingId = MessagePropGroups.CurrentGroupMappingId;
			this.groups = new StorePropTag[MessagePropGroups.NumberedGroupLists.Length][];
			for (int i = 0; i < this.groups.Length; i++)
			{
				this.groups[i] = new StorePropTag[MessagePropGroups.NumberedGroupLists[i].Length];
				for (int j = 0; j < this.groups[i].Length; j++)
				{
					StorePropInfo storePropInfo = MessagePropGroups.NumberedGroupLists[i][j];
					ushort propId;
					if (storePropInfo.IsNamedProperty)
					{
						StoreNamedPropInfo objB;
						mailbox.NamedPropertyMap.GetNumberFromName(context, storePropInfo.PropName, true, mailbox.QuotaInfo, out propId, out objB);
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(object.ReferenceEquals(storePropInfo, objB), "unexpected scenario for a named props");
					}
					else
					{
						propId = storePropInfo.PropId;
					}
					this.groups[i][j] = new StorePropTag(propId, storePropInfo.PropType, storePropInfo, ObjectType.Message);
				}
			}
		}

		public int MappingId
		{
			get
			{
				return this.mappingId;
			}
		}

		public int Count
		{
			get
			{
				if (this.groups != null)
				{
					return this.groups.Length;
				}
				return 0;
			}
		}

		public StorePropTag[] this[int groupIndex]
		{
			get
			{
				return this.groups[groupIndex];
			}
		}

		private int mappingId;

		private StorePropTag[][] groups;
	}
}
