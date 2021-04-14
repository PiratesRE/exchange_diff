using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class MiniClientAccessServerOrArray : MiniObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return MiniClientAccessServerOrArray.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				throw new InvalidADObjectOperationException(DirectoryStrings.ExceptionMostDerivedOnBase("MiniClientAccessServerOrArray"));
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return MiniClientAccessServerOrArray.implicitFilter;
			}
		}

		public string Fqdn
		{
			get
			{
				return (string)this[MiniClientAccessServerOrArraySchema.Fqdn];
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[MiniClientAccessServerOrArraySchema.ExchangeLegacyDN];
			}
		}

		public string Site
		{
			get
			{
				return (string)this[MiniClientAccessServerOrArraySchema.Site];
			}
		}

		public ADObjectId ServerSite
		{
			get
			{
				return (ADObjectId)this[MiniClientAccessServerOrArraySchema.Site];
			}
			internal set
			{
				this[MiniClientAccessServerOrArraySchema.Site] = value;
			}
		}

		public bool IsClientAccessArray
		{
			get
			{
				return (bool)this[MiniClientAccessServerOrArraySchema.IsClientAccessArray];
			}
		}

		public bool IsClientAccessServer
		{
			get
			{
				return (bool)this[ServerSchema.IsClientAccessServer];
			}
		}

		private static readonly QueryFilter implicitFilter = QueryFilter.OrTogether(new QueryFilter[]
		{
			QueryFilter.AndTogether(new QueryFilter[]
			{
				new ClientAccessArray().ImplicitFilter,
				ClientAccessArray.PriorTo15ExchangeObjectVersionFilter
			}),
			QueryFilter.AndTogether(new QueryFilter[]
			{
				new Server().ImplicitFilter,
				new BitMaskOrFilter(ServerSchema.CurrentServerRole, 6UL)
			})
		});

		private static MiniClientAccessServerOrArraySchema schema = ObjectSchema.GetInstance<MiniClientAccessServerOrArraySchema>();
	}
}
