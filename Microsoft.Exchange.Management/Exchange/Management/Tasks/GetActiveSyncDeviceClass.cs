using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "ActiveSyncDeviceClass", DefaultParameterSetName = "Identity")]
	public sealed class GetActiveSyncDeviceClass : GetMultitenancySystemConfigurationObjectTask<ActiveSyncDeviceClassIdParameter, ActiveSyncDeviceClass>
	{
		[Parameter]
		public string SortBy
		{
			get
			{
				return (string)base.Fields["SortBy"];
			}
			set
			{
				base.Fields["SortBy"] = (string.IsNullOrEmpty(value) ? null : value);
				this.internalSortBy = QueryHelper.GetSortBy(this.SortBy, GetActiveSyncDeviceClass.SortPropertiesArray);
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public string Filter
		{
			get
			{
				return (string)base.Fields["Filter"];
			}
			set
			{
				MonadFilter monadFilter = new MonadFilter(value, this, GetActiveSyncDeviceClass.FilterableObjectSchema);
				this.inputFilter = monadFilter.InnerFilter;
				base.OptionalIdentityData.AdditionalFilter = monadFilter.InnerFilter;
				base.Fields["Filter"] = value;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return this.ConstructQueryFilterWithCustomFilter(this.inputFilter);
			}
		}

		protected override SortBy InternalSortBy
		{
			get
			{
				return this.internalSortBy;
			}
		}

		private QueryFilter ConstructQueryFilterWithCustomFilter(QueryFilter customFilter)
		{
			QueryFilter internalFilter = base.InternalFilter;
			if (internalFilter == null)
			{
				return customFilter;
			}
			if (customFilter == null)
			{
				return internalFilter;
			}
			return new AndFilter(new QueryFilter[]
			{
				internalFilter,
				customFilter
			});
		}

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ActiveSyncDeviceClassSchema.DeviceType,
			ActiveSyncDeviceClassSchema.DeviceModel,
			ActiveSyncDeviceClassSchema.LastUpdateTime
		};

		private static readonly ActiveSyncDeviceClassSchema FilterableObjectSchema = ObjectSchema.GetInstance<ActiveSyncDeviceClassSchema>();

		private SortBy internalSortBy;

		private QueryFilter inputFilter;
	}
}
