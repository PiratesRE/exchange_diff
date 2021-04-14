using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "SyncDeletedRecipient", DefaultParameterSetName = "CookieSet")]
	public sealed class GetSyncDeletedRecipient : GetTaskBase<ADRawEntry>
	{
		[Parameter]
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

		[Parameter]
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

		[Parameter]
		[ValidateNotNullOrEmpty]
		public SyncRecipientType RecipientType
		{
			get
			{
				return (SyncRecipientType)base.Fields["RecipientType"];
			}
			set
			{
				base.Fields["RecipientType"] = value;
			}
		}

		[Parameter]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (!base.Fields.IsChanged("RecipientType"))
				{
					this.RecipientType = SyncRecipientType.All;
				}
				QueryFilter queryFilter = SyncDeleteRecipientFilters.GetFilter(this.RecipientType);
				if (base.CurrentOrganizationId != null && base.CurrentOrganizationId.OrganizationalUnit != null)
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, base.CurrentOrganizationId.OrganizationalUnit)
					});
				}
				else
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						new NotFilter(new ExistsFilter(ADObjectSchema.OrganizationalUnitRoot))
					});
				}
				return new AndFilter(new QueryFilter[]
				{
					queryFilter,
					SyncTaskHelper.GetDeltaFilter(this.inputCookie)
				});
			}
		}

		protected override IEnumerable<ADRawEntry> GetPagedData()
		{
			if (this.Cookie == null)
			{
				DeletedRecipient item = new DeletedRecipient();
				return new List<ADRawEntry>
				{
					item
				};
			}
			PropertyDefinition[] properties = new PropertyDefinition[]
			{
				ADObjectSchema.Id,
				ADObjectSchema.Name,
				ADObjectSchema.DistinguishedName,
				ADObjectSchema.Guid,
				ADObjectSchema.OrganizationalUnitRoot,
				ADObjectSchema.ObjectClass,
				ADObjectSchema.WhenChanged,
				ADObjectSchema.WhenChangedUTC,
				ADObjectSchema.WhenCreated,
				ADObjectSchema.WhenCreatedUTC
			};
			if (this.requireTwoQueries)
			{
				IRecipientSession sessionForSoftDeletedObjects = SoftDeletedTaskHelper.GetSessionForSoftDeletedObjects(base.CurrentOrganizationId, this.DomainController);
				ADObjectId childId = base.CurrentOrganizationId.OrganizationalUnit.GetChildId("OU", "Soft Deleted Objects");
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.IsSoftDeletedByRemove, true);
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					SyncTaskHelper.GetDeltaFilter(this.inputCookie)
				});
				this.reader2 = sessionForSoftDeletedObjects.FindPagedADRawEntry(childId, QueryScope.OneLevel, queryFilter, null, this.PageSize, properties);
				this.reader2.Cookie = this.inputCookie.PageCookie2;
				base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(sessionForSoftDeletedObjects, typeof(ADRawEntry), queryFilter, null, false));
			}
			IRecipientSession recipientSession = (IRecipientSession)base.DataSession;
			ADObjectId deletedObjectsContainer = ADSession.GetDeletedObjectsContainer(recipientSession.GetDomainNamingContext());
			QueryFilter internalFilter = this.InternalFilter;
			recipientSession.SessionSettings.SkipCheckVirtualIndex = true;
			ADPagedReader<ADRawEntry> adpagedReader = recipientSession.FindPagedADRawEntry(deletedObjectsContainer, QueryScope.OneLevel, internalFilter, null, this.PageSize, properties);
			adpagedReader.IncludeDeletedObjects = true;
			adpagedReader.Cookie = this.inputCookie.PageCookie;
			base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.DataSession, typeof(ADRawEntry), this.InternalFilter, null, false));
			return adpagedReader;
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession sessionForDeletedObjects = MailboxTaskHelper.GetSessionForDeletedObjects(this.DomainController, base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
			this.inputCookie = SyncTaskHelper.ResolveSyncCookie(this.Cookie, sessionForDeletedObjects, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
			return sessionForDeletedObjects;
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			SyncTaskHelper.OneParameterMethod<bool, IConfigurable> oneParameterMethod = null;
			SyncTaskHelper.OneParameterMethod<bool, IConfigurable> oneParameterMethod2 = null;
			ADRawEntry adrawEntry = null;
			if (dataObjects is ADPagedReader<ADRawEntry>)
			{
				bool flag;
				if (this.requireTwoQueries)
				{
					IEnumerable<ADRawEntry> dataObjects2 = (IEnumerable<ADRawEntry>)dataObjects;
					IEnumerable<ADRawEntry> dataObjects3 = this.reader2;
					SyncCookie syncCookie = this.inputCookie;
					SyncTaskHelper.ParameterlessMethod<bool> isStopping = () => base.Stopping;
					if (oneParameterMethod == null)
					{
						oneParameterMethod = ((IConfigurable dataObject) => false);
					}
					this.searchNeedsRetry = !SyncTaskHelper.HandleTaskWritePagedResult<ADRawEntry>(dataObjects2, dataObjects3, syncCookie, ref this.outputCookie, isStopping, oneParameterMethod, new SyncTaskHelper.VoidOneParameterMethod<IConfigurable>(this.WriteResult), this.Pages, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError), out flag);
				}
				else
				{
					IEnumerable<ADRawEntry> dataObjects4 = (IEnumerable<ADRawEntry>)dataObjects;
					SyncCookie syncCookie2 = this.inputCookie;
					SyncTaskHelper.ParameterlessMethod<bool> isStopping2 = () => base.Stopping;
					if (oneParameterMethod2 == null)
					{
						oneParameterMethod2 = ((IConfigurable dataObject) => false);
					}
					this.searchNeedsRetry = !SyncTaskHelper.HandleTaskWritePagedResult<ADRawEntry>(dataObjects4, syncCookie2, ref this.outputCookie, isStopping2, oneParameterMethod2, new SyncTaskHelper.VoidOneParameterMethod<IConfigurable>(this.WriteResult), this.Pages, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError), out flag);
				}
				if (!this.searchNeedsRetry && !flag)
				{
					adrawEntry = new DeletedRecipient();
				}
			}
			else
			{
				adrawEntry = ((List<ADRawEntry>)dataObjects)[0];
			}
			if (adrawEntry != null)
			{
				this.outputCookie = new SyncCookie(this.inputCookie.DomainController, this.inputCookie.HighWatermarks, WatermarkMap.Empty, null);
				this.WriteResult(adrawEntry);
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			DeletedRecipient deleteRecipient;
			if (dataObject is DeletedRecipient)
			{
				deleteRecipient = (DeletedRecipient)dataObject;
			}
			else
			{
				ADRawEntry adrawEntry = (ADRawEntry)dataObject;
				this.PackDeletedRecipientInfo(adrawEntry);
				deleteRecipient = new DeletedRecipient(null, adrawEntry.propertyBag);
			}
			SyncDeletedRecipient syncDeletedRecipient = new SyncDeletedRecipient(deleteRecipient);
			if (this.outputCookie != null)
			{
				syncDeletedRecipient.propertyBag.SetField(SyncDeletedObjectSchema.Cookie, this.outputCookie.ToBytes());
				if (this.outputCookie.HighWatermark == 0L)
				{
					syncDeletedRecipient.propertyBag.SetField(SyncDeletedObjectSchema.EndOfList, true);
				}
			}
			return syncDeletedRecipient;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			base.WriteResult(this.ConvertDataObjectToPresentationObject(dataObject));
		}

		protected override void InternalProcessRecord()
		{
			this.requireTwoQueries = (this.Cookie != null && base.CurrentOrganizationId != null && base.CurrentOrganizationId.OrganizationalUnit != null);
			base.InternalProcessRecord();
			int num = 0;
			while (this.searchNeedsRetry)
			{
				num++;
				if (num <= 5)
				{
					base.WriteVerbose(Strings.VerboseGetSyncDeletedRecipientNeedsRetry(num));
					base.InternalProcessRecord();
				}
				else
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorGetSyncDeletedRecipientRetryFailed), ErrorCategory.InvalidOperation, null);
				}
			}
		}

		private void PackDeletedRecipientInfo(ADRawEntry entry)
		{
			string text = (string)entry.propertyBag[ADObjectSchema.DistinguishedName];
			int num = text.IndexOf(",OU=Soft Deleted Objects,", StringComparison.OrdinalIgnoreCase);
			if (-1 != num)
			{
				string text2 = (string)entry.propertyBag[ADObjectSchema.Name];
				string arg = ((Guid)entry.propertyBag[ADObjectSchema.Guid]).ToString();
				string value = string.Format("{0}\nDEL:{1}", text2, arg);
				entry.propertyBag.SetField(ADObjectSchema.Name, value);
				ADObjectId deletedObjectsContainer = ADSession.GetDeletedObjectsContainer(((IRecipientSession)base.DataSession).GetDomainNamingContext());
				string arg2 = AdName.Escape(text2);
				string text3 = string.Format("CN={0}\\0ADEL:{1},{2}", arg2, arg, deletedObjectsContainer.DistinguishedName);
				entry.propertyBag.SetField(ADObjectSchema.DistinguishedName, text3);
				ADObjectId value2 = new ADObjectId(text3);
				entry.propertyBag.SetField(ADObjectSchema.Id, value2);
			}
			entry.propertyBag.SetField(DeletedObjectSchema.IsDeleted, true);
			entry.propertyBag.SetField(DeletedObjectSchema.LastKnownParent, entry.propertyBag[ADObjectSchema.OrganizationalUnitRoot]);
			entry.propertyBag.SetField(ADObjectSchema.OrganizationId, base.CurrentOrganizationId);
		}

		private const int InvalidPagedSearchMaxRetryCount = 5;

		private bool searchNeedsRetry;

		private SyncCookie inputCookie;

		private SyncCookie outputCookie;

		private bool requireTwoQueries;

		private ADPagedReader<ADRawEntry> reader2;
	}
}
