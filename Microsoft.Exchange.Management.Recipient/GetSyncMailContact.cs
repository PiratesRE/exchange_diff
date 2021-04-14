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
	[OutputType(new Type[]
	{
		typeof(SyncMailContact)
	})]
	[Cmdlet("Get", "SyncMailContact", DefaultParameterSetName = "Identity")]
	public sealed class GetSyncMailContact : GetMailContactBase
	{
		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<SyncMailContactSchema>();
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

		[Parameter(ParameterSetName = "Identity")]
		[Parameter(ParameterSetName = "AnrSet")]
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

		protected override IEnumerable<ADContact> GetPagedData()
		{
			if (base.ParameterSetName == "CookieSet")
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.DataSession, typeof(ADContact), this.InternalFilter, this.RootId, this.DeepSearch));
				base.InternalResultSize = Unlimited<uint>.UnlimitedValue;
				ADPagedReader<ADContact> adpagedReader = (ADPagedReader<ADContact>)base.DataSession.FindPaged<ADContact>(this.InternalFilter, this.RootId, this.DeepSearch, this.InternalSortBy, this.PageSize);
				adpagedReader.Cookie = this.inputCookie.PageCookie;
				return adpagedReader;
			}
			return base.GetPagedData();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			((ADContact)dataObject).BypassModerationCheck = true;
			base.WriteResult(dataObject);
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			if (base.ParameterSetName == "CookieSet")
			{
				SyncTaskHelper.HandleTaskWritePagedResult<ADContact>((IEnumerable<ADContact>)dataObjects, this.inputCookie, ref this.outputCookie, () => base.Stopping, new SyncTaskHelper.OneParameterMethod<bool, IConfigurable>(base.ShouldSkipObject), new SyncTaskHelper.VoidOneParameterMethod<IConfigurable>(this.WriteResult), this.Pages, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
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
			ADContact dataObject2 = (ADContact)dataObject;
			SyncMailContact syncMailContact = new SyncMailContact(dataObject2);
			syncMailContact.propertyBag.SetField(ADRecipientSchema.AcceptMessagesOnlyFrom, SyncTaskHelper.RetrieveFullADObjectId(base.TenantGlobalCatalogSession, syncMailContact.AcceptMessagesOnlyFrom));
			syncMailContact.propertyBag.SetField(ADRecipientSchema.AcceptMessagesOnlyFromDLMembers, SyncTaskHelper.RetrieveFullADObjectId(base.TenantGlobalCatalogSession, syncMailContact.AcceptMessagesOnlyFromDLMembers));
			syncMailContact.propertyBag.SetField(ADRecipientSchema.RejectMessagesFrom, SyncTaskHelper.RetrieveFullADObjectId(base.TenantGlobalCatalogSession, syncMailContact.RejectMessagesFrom));
			syncMailContact.propertyBag.SetField(ADRecipientSchema.RejectMessagesFromDLMembers, SyncTaskHelper.RetrieveFullADObjectId(base.TenantGlobalCatalogSession, syncMailContact.RejectMessagesFromDLMembers));
			if (this.outputCookie != null)
			{
				syncMailContact.propertyBag.SetField(SyncMailContactSchema.Cookie, this.outputCookie.ToBytes());
				if (this.outputCookie.HighWatermark == 0L)
				{
					syncMailContact.propertyBag.SetField(SyncMailContactSchema.EndOfList, true);
				}
			}
			return syncMailContact;
		}

		private SyncCookie inputCookie;

		private SyncCookie outputCookie;
	}
}
