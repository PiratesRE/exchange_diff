using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class GetTaskBase<TDataObject> : DataAccessTask<TDataObject> where TDataObject : IConfigurable, new()
	{
		protected internal uint WriteObjectCount
		{
			get
			{
				return this.matchCount;
			}
		}

		protected virtual QueryFilter InternalFilter
		{
			get
			{
				return null;
			}
		}

		protected virtual bool DeepSearch
		{
			get
			{
				return false;
			}
		}

		protected virtual SortBy InternalSortBy
		{
			get
			{
				return null;
			}
		}

		protected virtual Unlimited<uint> DefaultResultSize
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		protected Unlimited<uint> InternalResultSize
		{
			get
			{
				Unlimited<uint>? unlimited = this.internalResultSize;
				if (unlimited == null)
				{
					return this.DefaultResultSize;
				}
				return unlimited.GetValueOrDefault();
			}
			set
			{
				this.internalResultSize = new Unlimited<uint>?(value);
			}
		}

		protected virtual int PageSize
		{
			get
			{
				return 0;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new GetTaskBaseModuleFactory();
		}

		protected void IncreaseMatchCount()
		{
			this.matchCount += 1U;
		}

		internal override IRecipientSession CreateTenantGlobalCatalogSession(ADSessionSettings sessionSettings)
		{
			if (sessionSettings.ConfigScopes == ConfigScopes.TenantLocal)
			{
				return base.CreateTenantGlobalCatalogSession(ADSessionSettings.RescopeToSubtree(sessionSettings));
			}
			return base.CreateTenantGlobalCatalogSession(sessionSettings);
		}

		internal override IConfigurationSession CreateConfigurationSession(ADSessionSettings sessionSettings)
		{
			if (sessionSettings.ConfigScopes == ConfigScopes.TenantLocal)
			{
				return base.CreateConfigurationSession(ADSessionSettings.RescopeToSubtree(sessionSettings));
			}
			return base.CreateConfigurationSession(sessionSettings);
		}

		[Obsolete("Use WriteResult(IConfigurable dataObject) instead.")]
		internal new void WriteObject(object sendToPipeline)
		{
			base.WriteObject(sendToPipeline);
		}

		[Obsolete("Use WriteResult<T>(IEnumerable<T> dataObjects) instead.")]
		internal new void WriteObject(object sendToPipeline, bool enumerateCollection)
		{
			base.WriteObject(sendToPipeline, enumerateCollection);
		}

		internal virtual void AdjustPageSize(IPageInformation pageInfo)
		{
		}

		protected string RequestQueryFilterInGetTasks
		{
			get
			{
				return (string)base.CurrentTaskContext.Items["Log_RequestQueryFilterInGetTasks"];
			}
			set
			{
				base.CurrentTaskContext.Items["Log_RequestQueryFilterInGetTasks"] = value;
			}
		}

		protected string InternalQueryFilterInGetTasks
		{
			get
			{
				return (string)base.CurrentTaskContext.Items["Log_InternalQueryFilterInGetTasks"];
			}
			set
			{
				base.CurrentTaskContext.Items["Log_InternalQueryFilterInGetTasks"] = value;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			this.matchCount = 0U;
			TaskLogger.LogExit();
		}

		protected virtual IEnumerable<TDataObject> GetPagedData()
		{
			QueryFilter internalFilter = this.InternalFilter;
			base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.DataSession, typeof(TDataObject), internalFilter, this.RootId, this.DeepSearch));
			return base.DataSession.FindPaged<TDataObject>(internalFilter, this.RootId, this.DeepSearch, this.InternalSortBy, this.PageSize);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				IEnumerable<TDataObject> pagedData = this.GetPagedData();
				ADPagedReader<TDataObject> adpagedReader = pagedData as ADPagedReader<TDataObject>;
				if (adpagedReader != null)
				{
					this.RequestQueryFilterInGetTasks = adpagedReader.LdapFilter;
					base.WriteVerbose(Strings.VerboseRequestFilterInGetTask(this.RequestQueryFilterInGetTasks));
				}
				if (this.InternalFilter != null)
				{
					this.InternalQueryFilterInGetTasks = this.InternalFilter.ToString();
					base.WriteVerbose(Strings.VerboseInternalQueryFilterInGetTasks(this.InternalQueryFilterInGetTasks));
				}
				this.pageInfo = (pagedData as IPageInformation);
				this.WriteResult<TDataObject>(pagedData);
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, (ErrorCategory)1002, null);
			}
			TaskLogger.LogExit();
		}

		protected virtual void WriteResult(IConfigurable dataObject)
		{
			if (dataObject is PagedPositionInfo)
			{
				base.WriteObject(dataObject);
				return;
			}
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			if (this.InternalResultSize.IsUnlimited || this.InternalResultSize.Value > this.matchCount)
			{
				ConfigurableObject configurableObject = dataObject as ConfigurableObject;
				if (configurableObject != null)
				{
					configurableObject.InitializeSchema();
				}
				ValidationError[] array = dataObject.Validate();
				base.WriteObject(dataObject);
				if (array != null && array.Length > 0)
				{
					if (dataObject.Identity != null)
					{
						this.WriteWarning(Strings.ErrorObjectHasValidationErrorsWithId(dataObject.Identity));
					}
					else
					{
						this.WriteWarning(Strings.ErrorObjectHasValidationErrors);
					}
					foreach (ValidationError validationError in array)
					{
						this.WriteWarning(validationError.Description);
					}
				}
			}
			else if (this.InternalResultSize.Value == this.matchCount)
			{
				if (this.internalResultSize == null)
				{
					this.WriteWarning(Strings.WarningDefaultResultSizeReached(this.InternalResultSize.Value.ToString()));
				}
				else
				{
					this.WriteWarning(Strings.WarningMoreResultsAvailable);
				}
			}
			this.matchCount += 1U;
			TaskLogger.LogExit();
		}

		protected virtual void WriteResult<T>(IEnumerable<T> dataObjects) where T : IConfigurable
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObjects
			});
			if (dataObjects != null)
			{
				using (IEnumerator<T> enumerator = dataObjects.GetEnumerator())
				{
					bool flag = false;
					if (!base.Stopping)
					{
						if (!this.InternalResultSize.IsUnlimited)
						{
							if (this.InternalResultSize.Value < this.matchCount)
							{
								goto IL_163;
							}
						}
						try
						{
							flag = enumerator.MoveNext();
						}
						finally
						{
							base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
						}
						base.WriteVerbose(Strings.VerboseWriteResultSize(this.InternalResultSize.ToString()));
					}
					IL_163:
					while (!base.Stopping && (this.InternalResultSize.IsUnlimited || this.InternalResultSize.Value >= this.matchCount) && flag)
					{
						if (!this.InternalResultSize.IsUnlimited && this.InternalResultSize.Value == this.matchCount + 1U && this.pageInfo != null && this.pageInfo.MorePagesAvailable != null && this.pageInfo.MorePagesAvailable.Value)
						{
							this.WriteResult(enumerator.Current);
						}
						IConfigurable dataObject = enumerator.Current;
						this.WriteResult(dataObject);
						if (!base.Stopping && (this.InternalResultSize.IsUnlimited || this.InternalResultSize.Value >= this.matchCount))
						{
							if (this.pageInfo != null)
							{
								this.AdjustPageSize(this.pageInfo);
							}
							flag = enumerator.MoveNext();
						}
					}
				}
			}
			TaskLogger.LogExit();
		}

		private const string RelatedCmdletDataRedactionConfigFilePath = "ClientAccess\\PowerShell-Proxy\\CmdletDataRedaction.xml";

		private const int MicroDelayWriteCount = 50;

		private Unlimited<uint>? internalResultSize;

		private uint matchCount;

		private IPageInformation pageInfo;
	}
}
