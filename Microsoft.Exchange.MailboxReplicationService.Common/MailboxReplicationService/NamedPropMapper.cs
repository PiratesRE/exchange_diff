using System;
using System.Collections.Generic;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class NamedPropMapper : LookupTable<NamedPropMapper.Mapping>
	{
		public NamedPropMapper(IMailbox mailbox, bool createMappingsIfNeeded)
		{
			this.mailbox = mailbox;
			this.createMappingsIfNeeded = createMappingsIfNeeded;
			this.byId = new NamedPropMapper.PropIdIndex();
			this.byNamedProp = new NamedPropMapper.NamedPropIndex();
			base.RegisterIndex(this.byId);
			base.RegisterIndex(this.byNamedProp);
		}

		public NamedPropMapper.PropIdIndex ById
		{
			get
			{
				return this.byId;
			}
		}

		public NamedPropMapper.NamedPropIndex ByNamedProp
		{
			get
			{
				return this.byNamedProp;
			}
		}

		public PropTag MapNamedProp(NamedPropData npData, PropType propType)
		{
			NamedPropMapper.Mapping mapping = this.ByNamedProp[npData];
			if (mapping != null)
			{
				return PropTagHelper.PropTagFromIdAndType(mapping.PropId, propType);
			}
			return PropTag.Unresolved;
		}

		private IMailbox mailbox;

		private bool createMappingsIfNeeded;

		private NamedPropMapper.PropIdIndex byId;

		private NamedPropMapper.NamedPropIndex byNamedProp;

		public class Mapping
		{
			public Mapping(int propId, NamedPropData npData)
			{
				this.PropId = propId;
				this.NPData = npData;
			}

			public int PropId { get; set; }

			public NamedPropData NPData { get; set; }
		}

		public class PropIdIndex : LookupIndex<int, NamedPropMapper.Mapping>
		{
			protected override ICollection<int> RetrieveKeys(NamedPropMapper.Mapping data)
			{
				return new int[]
				{
					data.PropId
				};
			}

			protected override NamedPropMapper.Mapping[] LookupKeys(int[] keys)
			{
				PropTag[] array = new PropTag[keys.Length];
				for (int i = 0; i < keys.Length; i++)
				{
					array[i] = PropTagHelper.PropTagFromIdAndType(keys[i], PropType.Unspecified);
				}
				NamedPropData[] namesFromIDs = ((NamedPropMapper)base.Owner).mailbox.GetNamesFromIDs(array);
				NamedPropMapper.Mapping[] array2 = new NamedPropMapper.Mapping[namesFromIDs.Length];
				for (int j = 0; j < array.Length; j++)
				{
					if (namesFromIDs[j] == null)
					{
						array2[j] = null;
					}
					else
					{
						array2[j] = new NamedPropMapper.Mapping(keys[j], namesFromIDs[j]);
					}
				}
				return array2;
			}
		}

		public class NamedPropIndex : LookupIndex<NamedPropData, NamedPropMapper.Mapping>
		{
			protected override ICollection<NamedPropData> RetrieveKeys(NamedPropMapper.Mapping data)
			{
				return new NamedPropData[]
				{
					data.NPData
				};
			}

			protected override NamedPropMapper.Mapping[] LookupKeys(NamedPropData[] keys)
			{
				NamedPropMapper namedPropMapper = (NamedPropMapper)base.Owner;
				PropTag[] idsFromNames = namedPropMapper.mailbox.GetIDsFromNames(namedPropMapper.createMappingsIfNeeded, keys);
				NamedPropMapper.Mapping[] array = new NamedPropMapper.Mapping[idsFromNames.Length];
				for (int i = 0; i < idsFromNames.Length; i++)
				{
					int num = idsFromNames[i].Id();
					if (num == 10)
					{
						array[i] = null;
					}
					else
					{
						array[i] = new NamedPropMapper.Mapping(num, keys[i]);
					}
				}
				return array;
			}
		}
	}
}
