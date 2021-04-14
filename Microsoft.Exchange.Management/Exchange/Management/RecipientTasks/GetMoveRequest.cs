using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "MoveRequest", DefaultParameterSetName = "Identity")]
	public sealed class GetMoveRequest : GetRecipientBase<MoveRequestIdParameter, ADUser>
	{
		public GetMoveRequest()
		{
			this.targetDatabase = null;
			this.sourceDatabase = null;
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.NotEqual, ADUserSchema.MailboxMoveStatus, RequestStatus.None),
				new ExistsFilter(ADUserSchema.MailboxMoveStatus)
			});
			if (base.OptionalIdentityData.AdditionalFilter == null)
			{
				base.OptionalIdentityData.AdditionalFilter = queryFilter;
				return;
			}
			base.OptionalIdentityData.AdditionalFilter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				base.OptionalIdentityData.AdditionalFilter
			});
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		[ValidateNotNull]
		public DatabaseIdParameter TargetDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["TargetDatabase"];
			}
			set
			{
				base.Fields["TargetDatabase"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public DatabaseIdParameter SourceDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["SourceDatabase"];
			}
			set
			{
				base.Fields["SourceDatabase"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public RequestStatus MoveStatus
		{
			get
			{
				return (RequestStatus)(base.Fields["MoveStatus"] ?? RequestStatus.None);
			}
			set
			{
				base.Fields["MoveStatus"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public Fqdn RemoteHostName
		{
			get
			{
				return (Fqdn)base.Fields["RemoteHostName"];
			}
			set
			{
				base.Fields["RemoteHostName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public string BatchName
		{
			get
			{
				return (string)base.Fields["BatchName"];
			}
			set
			{
				base.Fields["BatchName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public bool Protect
		{
			get
			{
				return (bool)(base.Fields["Protect"] ?? false);
			}
			set
			{
				base.Fields["Protect"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public bool Offline
		{
			get
			{
				return (bool)(base.Fields["Offline"] ?? false);
			}
			set
			{
				base.Fields["Offline"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public bool Suspend
		{
			get
			{
				return (bool)(base.Fields["Suspend"] ?? false);
			}
			set
			{
				base.Fields["Suspend"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public bool SuspendWhenReadyToComplete
		{
			get
			{
				return (bool)(base.Fields["SuspendWhenReadyToComplete"] ?? false);
			}
			set
			{
				base.Fields["SuspendWhenReadyToComplete"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public bool HighPriority
		{
			get
			{
				return (bool)(base.Fields["HighPriority"] ?? false);
			}
			set
			{
				base.Fields["HighPriority"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "Filtering", ValueFromPipeline = true)]
		[ValidateNotNull]
		public new AccountPartitionIdParameter AccountPartition
		{
			get
			{
				return base.AccountPartition;
			}
			set
			{
				base.AccountPartition = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public RequestFlags Flags
		{
			get
			{
				return (RequestFlags)(base.Fields["Flags"] ?? RequestFlags.None);
			}
			set
			{
				base.Fields["Flags"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter IncludeSoftDeletedObjects
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeSoftDeletedObjects"] ?? false);
			}
			set
			{
				base.Fields["IncludeSoftDeletedObjects"] = value;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<MoveRequestUserSchema>();
			}
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetMoveRequest.SortPropertiesArray;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				List<QueryFilter> list = new List<QueryFilter>();
				QueryFilter internalFilter = base.InternalFilter;
				if (internalFilter != null)
				{
					list.Add(internalFilter);
				}
				if (this.targetDatabase != null)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.MailboxMoveTargetMDB, ((MailboxDatabase)this.targetDatabase).Id));
				}
				if (this.sourceDatabase != null)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.MailboxMoveSourceMDB, ((MailboxDatabase)this.sourceDatabase).Id));
				}
				if (!this.IsFieldSet("MoveStatus"))
				{
					list.Add(new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.NotEqual, ADUserSchema.MailboxMoveStatus, RequestStatus.None),
						new ExistsFilter(ADUserSchema.MailboxMoveStatus)
					}));
				}
				else if (this.MoveStatus != RequestStatus.None)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.MailboxMoveStatus, this.MoveStatus));
				}
				else
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCannotFindMoveRequestWithStatusNone), ErrorCategory.InvalidArgument, this.MoveStatus);
				}
				if (this.IsFieldSet("RemoteHostName"))
				{
					if (string.IsNullOrEmpty(this.RemoteHostName))
					{
						list.Add(QueryFilter.NotFilter(new ExistsFilter(ADUserSchema.MailboxMoveRemoteHostName)));
					}
					else
					{
						list.Add(new TextFilter(ADUserSchema.MailboxMoveRemoteHostName, this.RemoteHostName, MatchOptions.WildcardString, MatchFlags.IgnoreCase));
					}
				}
				if (this.IsFieldSet("BatchName"))
				{
					if (string.IsNullOrEmpty(this.BatchName))
					{
						list.Add(new NotFilter(new ExistsFilter(ADUserSchema.MailboxMoveBatchName)));
					}
					else
					{
						list.Add(new TextFilter(ADUserSchema.MailboxMoveBatchName, this.BatchName, MatchOptions.WildcardString, MatchFlags.IgnoreCase));
					}
				}
				if (this.IsFieldSet("Protect"))
				{
					QueryFilter queryFilter = new BitMaskAndFilter(ADUserSchema.MailboxMoveFlags, 32UL);
					if (this.Protect)
					{
						list.Add(queryFilter);
					}
					else
					{
						list.Add(new NotFilter(queryFilter));
					}
				}
				if (this.IsFieldSet("Offline"))
				{
					QueryFilter queryFilter2 = new BitMaskAndFilter(ADUserSchema.MailboxMoveFlags, 16UL);
					if (this.Offline)
					{
						list.Add(queryFilter2);
					}
					else
					{
						list.Add(new NotFilter(queryFilter2));
					}
				}
				if (this.IsFieldSet("Suspend"))
				{
					QueryFilter queryFilter3 = new BitMaskAndFilter(ADUserSchema.MailboxMoveFlags, 256UL);
					if (this.Suspend)
					{
						list.Add(queryFilter3);
					}
					else
					{
						list.Add(new NotFilter(queryFilter3));
					}
				}
				if (this.IsFieldSet("SuspendWhenReadyToComplete"))
				{
					QueryFilter queryFilter4 = new BitMaskAndFilter(ADUserSchema.MailboxMoveFlags, 512UL);
					if (this.SuspendWhenReadyToComplete)
					{
						list.Add(queryFilter4);
					}
					else
					{
						list.Add(new NotFilter(queryFilter4));
					}
				}
				if (this.IsFieldSet("HighPriority"))
				{
					QueryFilter queryFilter5 = new BitMaskAndFilter(ADUserSchema.MailboxMoveFlags, 128UL);
					if (this.HighPriority)
					{
						list.Add(queryFilter5);
					}
					else
					{
						list.Add(new NotFilter(queryFilter5));
					}
				}
				if (this.IsFieldSet("Flags"))
				{
					QueryFilter item = new BitMaskAndFilter(ADUserSchema.MailboxMoveFlags, (ulong)((long)this.Flags));
					list.Add(item);
				}
				return new AndFilter(list.ToArray());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.CreateSession();
			if (this.IncludeSoftDeletedObjects)
			{
				recipientSession.SessionSettings.IncludeSoftDeletedObjects = true;
			}
			return recipientSession;
		}

		private new string Anr
		{
			get
			{
				return base.Anr;
			}
			set
			{
				base.Anr = value;
			}
		}

		private new string Filter
		{
			get
			{
				return base.Filter;
			}
			set
			{
				base.Filter = value;
			}
		}

		private new SwitchParameter ReadFromDomainController
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

		private new SwitchParameter IgnoreDefaultScope
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

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.TargetDatabase != null)
			{
				this.TargetDatabase.AllowLegacy = true;
				this.targetDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.TargetDatabase, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.TargetDatabase.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.TargetDatabase.ToString())));
			}
			if (this.SourceDatabase != null)
			{
				this.SourceDatabase.AllowLegacy = true;
				this.sourceDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.SourceDatabase, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.SourceDatabase.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.SourceDatabase.ToString())));
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return MoveRequest.FromDataObject((ADUser)dataObject);
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			if (!base.ParameterSetName.Equals("Identity"))
			{
				base.WriteResult(dataObject);
				return;
			}
			ADUser aduser = (ADUser)dataObject;
			if (aduser.MailboxMoveStatus != RequestStatus.None)
			{
				base.WriteResult(dataObject);
				return;
			}
			this.WriteError(new ManagementObjectNotFoundException(Strings.ErrorUserNotBeingMoved(dataObject.ToString())), ErrorCategory.InvalidArgument, this.Identity, false);
		}

		private bool IsFieldSet(string fieldName)
		{
			return base.Fields.IsChanged(fieldName) || base.Fields.IsModified(fieldName);
		}

		public const string ParameterTargetDatabase = "TargetDatabase";

		public const string ParameterSourceDatabase = "SourceDatabase";

		public const string ParameterMoveStatus = "MoveStatus";

		public const string ParameterRemoteHostName = "RemoteHostName";

		public const string ParameterBatchName = "BatchName";

		public const string ParameterProtect = "Protect";

		public const string ParameterOffline = "Offline";

		public const string ParameterHighPriority = "HighPriority";

		public const string ParameterFlags = "Flags";

		public const string ParameterSuspend = "Suspend";

		public const string ParameterSuspendWhenReadyToComplete = "SuspendWhenReadyToComplete";

		public const string ParameterIncludeSoftDeletedObjects = "IncludeSoftDeletedObjects";

		public const string FiltersSet = "Filtering";

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			MailEnabledRecipientSchema.DisplayName,
			MailEnabledRecipientSchema.Alias
		};

		private ADObject targetDatabase;

		private ADObject sourceDatabase;
	}
}
