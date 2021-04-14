using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "SyncDynamicDistributionGroup", DefaultParameterSetName = "Identity")]
	[OutputType(new Type[]
	{
		typeof(SyncDynamicDistributionGroup)
	})]
	public sealed class GetSyncDynamicDistributionGroup : GetDynamicDistributionGroupBase
	{
		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<SyncDynamicDistributionGroupSchema>();
			}
		}

		[Parameter(ParameterSetName = "CookieSet")]
		public byte[] Cookie
		{
			get
			{
				return (byte[])base.Fields["Cookie"];
			}
			set
			{
				base.Fields["Cookie"] = value;
			}
		}

		[Parameter(ParameterSetName = "CookieSet")]
		[ValidateRange(1, 2147483647)]
		public int Pages
		{
			get
			{
				return (int)(base.Fields["Pages"] ?? int.MaxValue);
			}
			set
			{
				base.Fields["Pages"] = value;
			}
		}

		[Parameter(ParameterSetName = "AnrSet")]
		[Parameter(ParameterSetName = "ManagedBySet")]
		[Parameter(ParameterSetName = "Identity")]
		public new Unlimited<uint> ResultSize
		{
			get
			{
				return base.ResultSize;
			}
			set
			{
				base.ResultSize = value;
			}
		}

		[Parameter(ParameterSetName = "Identity")]
		[Parameter(ParameterSetName = "AnrSet")]
		[Parameter(ParameterSetName = "ManagedBySet")]
		public new SwitchParameter ReadFromDomainController
		{
			get
			{
				return base.ReadFromDomainController;
			}
			set
			{
				base.ReadFromDomainController = value;
			}
		}

		[Parameter(ParameterSetName = "AnrSet")]
		[Parameter(ParameterSetName = "ManagedBySet")]
		[Parameter(ParameterSetName = "Identity")]
		public new SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return base.IgnoreDefaultScope;
			}
			set
			{
				base.IgnoreDefaultScope = value;
			}
		}

		[Parameter(ParameterSetName = "AnrSet")]
		[Parameter(ParameterSetName = "ManagedBySet")]
		[Parameter(ParameterSetName = "Identity")]
		public new string SortBy
		{
			get
			{
				return base.SortBy;
			}
			set
			{
				base.SortBy = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeSoftDeletedObjects
		{
			get
			{
				return (SwitchParameter)(base.Fields["SoftDeletedObject"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SoftDeletedObject"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.CreateSession();
			if (this.IncludeSoftDeletedObjects.IsPresent)
			{
				recipientSession.SessionSettings.IncludeSoftDeletedObjects = true;
			}
			if (base.ParameterSetName == "CookieSet")
			{
				recipientSession.UseGlobalCatalog = true;
				this.inputCookie = SyncTaskHelper.ResolveSyncCookie(this.Cookie, recipientSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			return recipientSession;
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (base.ParameterSetName == "CookieSet")
				{
					return new AndFilter(new QueryFilter[]
					{
						base.InternalFilter,
						SyncTaskHelper.GetDeltaFilter(this.inputCookie)
					});
				}
				return base.InternalFilter;
			}
		}

		protected override IEnumerable<ADDynamicGroup> GetPagedData()
		{
			if (base.ParameterSetName == "CookieSet")
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.DataSession, typeof(ADDynamicGroup), this.InternalFilter, this.RootId, this.DeepSearch));
				base.InternalResultSize = Unlimited<uint>.UnlimitedValue;
				ADPagedReader<ADDynamicGroup> adpagedReader = (ADPagedReader<ADDynamicGroup>)base.DataSession.FindPaged<ADDynamicGroup>(this.InternalFilter, this.RootId, this.DeepSearch, this.InternalSortBy, this.PageSize);
				adpagedReader.Cookie = this.inputCookie.PageCookie;
				return adpagedReader;
			}
			return base.GetPagedData();
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			if (base.ParameterSetName == "CookieSet")
			{
				SyncTaskHelper.HandleTaskWritePagedResult<ADDynamicGroup>((IEnumerable<ADDynamicGroup>)dataObjects, this.inputCookie, ref this.outputCookie, () => base.Stopping, new SyncTaskHelper.OneParameterMethod<bool, IConfigurable>(base.ShouldSkipObject), new SyncTaskHelper.VoidOneParameterMethod<IConfigurable>(this.WriteResult), this.Pages, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
				return;
			}
			base.WriteResult<T>(dataObjects);
		}

		protected override bool ShouldSkipObject(IConfigurable dataObject)
		{
			return !(base.ParameterSetName == "CookieSet") && base.ShouldSkipObject(dataObject);
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			ADDynamicGroup dataObject2 = (ADDynamicGroup)dataObject;
			SyncDynamicDistributionGroup syncDynamicDistributionGroup = new SyncDynamicDistributionGroup(dataObject2);
			if (this.outputCookie != null)
			{
				syncDynamicDistributionGroup.propertyBag.SetField(SyncDynamicDistributionGroupSchema.Cookie, this.outputCookie.ToBytes());
				if (this.outputCookie.HighWatermark == 0L)
				{
					syncDynamicDistributionGroup.propertyBag.SetField(SyncDynamicDistributionGroupSchema.EndOfList, true);
				}
			}
			return syncDynamicDistributionGroup;
		}

		private SyncCookie inputCookie;

		private SyncCookie outputCookie;
	}
}
