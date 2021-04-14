using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[OutputType(new Type[]
	{
		typeof(SyncMailbox)
	})]
	[Cmdlet("Get", "SyncMailbox", DefaultParameterSetName = "Identity")]
	public sealed class GetSyncMailbox : GetMailboxOrSyncMailbox
	{
		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<SyncMailboxSchema>();
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

		[ValidateRange(1, 2147483647)]
		[Parameter(ParameterSetName = "CookieSet")]
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

		[Parameter(ParameterSetName = "Identity")]
		[Parameter(ParameterSetName = "MailboxPlanSet")]
		[Parameter(ParameterSetName = "ServerSet")]
		[Parameter(ParameterSetName = "AnrSet")]
		[Parameter(ParameterSetName = "DatabaseSet")]
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

		[Parameter(ParameterSetName = "DatabaseSet")]
		[Parameter(ParameterSetName = "ServerSet")]
		[Parameter(ParameterSetName = "MailboxPlanSet")]
		[Parameter(ParameterSetName = "AnrSet")]
		[Parameter(ParameterSetName = "Identity")]
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
		[Parameter(ParameterSetName = "MailboxPlanSet")]
		[Parameter(ParameterSetName = "DatabaseSet")]
		[Parameter(ParameterSetName = "Identity")]
		[Parameter(ParameterSetName = "ServerSet")]
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
		[Parameter(ParameterSetName = "DatabaseSet")]
		[Parameter(ParameterSetName = "Identity")]
		[Parameter(ParameterSetName = "MailboxPlanSet")]
		[Parameter(ParameterSetName = "ServerSet")]
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

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.CreateSession();
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

		protected override IEnumerable<ADUser> GetPagedData()
		{
			base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.DataSession, typeof(ADUser), this.InternalFilter, this.RootId, this.DeepSearch));
			if (base.ParameterSetName == "CookieSet")
			{
				base.InternalResultSize = Unlimited<uint>.UnlimitedValue;
				ADPagedReader<ADUser> adpagedReader = (ADPagedReader<ADUser>)base.DataSession.FindPaged<ADUser>(this.InternalFilter, this.RootId, this.DeepSearch, this.InternalSortBy, this.PageSize);
				adpagedReader.Cookie = this.inputCookie.PageCookie;
				return adpagedReader;
			}
			return base.GetPagedData();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			((ADUser)dataObject).BypassModerationCheck = true;
			base.WriteResult(dataObject);
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			if (base.ParameterSetName == "CookieSet")
			{
				SyncTaskHelper.HandleTaskWritePagedResult<ADUser>((IEnumerable<ADUser>)dataObjects, this.inputCookie, ref this.outputCookie, () => base.Stopping, new SyncTaskHelper.OneParameterMethod<bool, IConfigurable>(base.ShouldSkipObject), new SyncTaskHelper.VoidOneParameterMethod<IConfigurable>(this.WriteResult), this.Pages, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
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
			ADUser dataObject2 = (ADUser)dataObject;
			SyncMailbox syncMailbox = new SyncMailbox(dataObject2);
			syncMailbox.propertyBag.SetField(ADRecipientSchema.AcceptMessagesOnlyFrom, SyncTaskHelper.RetrieveFullADObjectId(base.TenantGlobalCatalogSession, syncMailbox.AcceptMessagesOnlyFrom));
			syncMailbox.propertyBag.SetField(ADRecipientSchema.AcceptMessagesOnlyFromDLMembers, SyncTaskHelper.RetrieveFullADObjectId(base.TenantGlobalCatalogSession, syncMailbox.AcceptMessagesOnlyFromDLMembers));
			syncMailbox.propertyBag.SetField(ADRecipientSchema.RejectMessagesFrom, SyncTaskHelper.RetrieveFullADObjectId(base.TenantGlobalCatalogSession, syncMailbox.RejectMessagesFrom));
			syncMailbox.propertyBag.SetField(ADRecipientSchema.RejectMessagesFromDLMembers, SyncTaskHelper.RetrieveFullADObjectId(base.TenantGlobalCatalogSession, syncMailbox.RejectMessagesFromDLMembers));
			if (syncMailbox.MailboxPlan != null)
			{
				ADUser aduser = base.ProvisioningCache.TryAddAndGetOrganizationDictionaryValue<ADUser, ADObjectId>(CannedProvisioningCacheKeys.CacheKeyMailboxPlanId, base.CurrentOrganizationId, syncMailbox.MailboxPlan, () => (ADUser)this.GetDataObject<ADUser>(new MailboxPlanIdParameter(syncMailbox.MailboxPlan), this.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxPlanNotFound(syncMailbox.MailboxPlan.ToString())), new LocalizedString?(Strings.ErrorMailboxPlanNotUnique(syncMailbox.MailboxPlan.ToString()))));
				syncMailbox.propertyBag.SetField(SyncMailboxSchema.MailboxPlanName, aduser.DisplayName);
			}
			if (this.outputCookie != null)
			{
				syncMailbox.propertyBag.SetField(SyncMailboxSchema.Cookie, this.outputCookie.ToBytes());
				if (this.outputCookie.HighWatermark == 0L)
				{
					syncMailbox.propertyBag.SetField(SyncMailboxSchema.EndOfList, true);
				}
			}
			syncMailbox.ResetChangeTracking();
			return syncMailbox;
		}

		private SyncCookie inputCookie;

		private SyncCookie outputCookie;
	}
}
