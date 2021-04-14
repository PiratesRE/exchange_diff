using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Get", "FailedMSOSyncObject", DefaultParameterSetName = "Identity")]
	public sealed class GetFailedMSOSyncObject : GetObjectWithIdentityTaskBase<FailedMSOSyncObjectIdParameter, FailedMSOSyncObject>
	{
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
				MonadFilter monadFilter = new MonadFilter(value, this, this.FilterableObjectSchema);
				this.inputFilter = monadFilter.InnerFilter;
				base.Fields["Filter"] = value;
			}
		}

		internal ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<FailedMSOSyncObjectSchema>();
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter internalFilter = base.InternalFilter;
				if (this.inputFilter == null)
				{
					return internalFilter;
				}
				if (internalFilter != null)
				{
					return new AndFilter(new QueryFilter[]
					{
						internalFilter,
						this.inputFilter
					});
				}
				return this.inputFilter;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return ForwardSyncDataAccessHelper.GetRootId(this.Identity);
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return this.Identity == null || !this.Identity.IsServiceInstanceDefinied;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return ForwardSyncDataAccessHelper.CreateSession(false);
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			base.WriteResult(new FailedMSOSyncObjectPresentationObject((FailedMSOSyncObject)dataObject));
		}

		private QueryFilter inputFilter;
	}
}
