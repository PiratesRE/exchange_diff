using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	public abstract class GetSubscriptionBase<TSubscription> : GetTenantADObjectWithIdentityTaskBase<AggregationSubscriptionIdParameter, TSubscription> where TSubscription : IConfigurable, new()
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter IncludeReport
		{
			get
			{
				return (SwitchParameter)(base.Fields[GetSubscriptionBase<TSubscription>.ParameterIncludeReport] ?? false);
			}
			set
			{
				base.Fields[GetSubscriptionBase<TSubscription>.ParameterIncludeReport] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AggregationType AggregationType
		{
			get
			{
				return (AggregationType)base.Fields["AggregationType"];
			}
			set
			{
				base.Fields["AggregationType"] = value;
			}
		}

		protected override Unlimited<uint> DefaultResultSize
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		protected abstract AggregationSubscriptionType IdentityType { get; }

		protected virtual AggregationType AggregationTypeValue
		{
			get
			{
				if (!base.Fields.IsModified("AggregationType"))
				{
					return AggregationType.Aggregation;
				}
				return this.AggregationType;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			PimSubscriptionProxy pimSubscriptionProxy = dataObject as PimSubscriptionProxy;
			if (pimSubscriptionProxy != null)
			{
				pimSubscriptionProxy.NeedSuppressingPiiData = base.NeedSuppressingPiiData;
			}
			base.WriteResult(dataObject);
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 130, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Aggregation\\GetSubscriptionBase.cs");
			if (this.Mailbox == null)
			{
				if (this.Identity != null && this.Identity.MailboxIdParameter != null)
				{
					this.Mailbox = this.Identity.MailboxIdParameter;
				}
				else
				{
					ADObjectId adObjectId;
					if (!base.TryGetExecutingUserId(out adObjectId))
					{
						throw new ExecutingUserPropertyNotFoundException("executingUserid");
					}
					this.Mailbox = new MailboxIdParameter(adObjectId);
				}
			}
			ADUser aduser = (ADUser)base.GetDataObject<ADUser>(this.Mailbox, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorUserNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorUserNotUnique(this.Mailbox.ToString())));
			IRecipientSession session = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(tenantOrRootOrgRecipientSession, aduser.OrganizationId, true);
			AggregationSubscriptionDataProvider aggregationSubscriptionDataProvider = null;
			try
			{
				aggregationSubscriptionDataProvider = SubscriptionConfigDataProviderFactory.Instance.CreateSubscriptionDataProvider(this.AggregationTypeValue, AggregationTaskType.Get, session, aduser);
				aggregationSubscriptionDataProvider.LoadReport = this.IncludeReport;
			}
			catch (MailboxFailureException exception)
			{
				this.WriteDebugInfoAndError(exception, ErrorCategory.InvalidArgument, this.Mailbox);
			}
			return aggregationSubscriptionDataProvider;
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			base.InternalStateReset();
			this.identityDefined = (this.Identity != null);
			if (!this.identityDefined)
			{
				this.Identity = new AggregationSubscriptionIdParameter();
			}
			this.Identity.SubscriptionType = new AggregationSubscriptionType?(this.IdentityType);
			this.Identity.AggregationType = new AggregationType?(this.AggregationTypeValue);
			if (base.Fields.IsModified("SubscriptionType") && !base.Fields.IsModified("AggregationType"))
			{
				this.Identity.AggregationType = new AggregationType?(AggregationType.All);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			try
			{
				if (this.identityDefined && this.Identity.IsUniqueIdentity)
				{
					TSubscription tsubscription = (TSubscription)((object)base.GetDataObject(this.Identity));
					PimSubscriptionProxy pimSubscriptionProxy = tsubscription as PimSubscriptionProxy;
					if (pimSubscriptionProxy != null)
					{
						pimSubscriptionProxy.SetDebug(base.IsDebugOn || base.IsVerboseOn);
					}
					this.WriteResult(tsubscription);
				}
				else
				{
					LocalizedString? localizedString;
					IEnumerable<TSubscription> dataObjects = base.GetDataObjects(this.Identity, base.OptionalIdentityData, out localizedString);
					foreach (TSubscription tsubscription2 in dataObjects)
					{
						PimSubscriptionProxy pimSubscriptionProxy2 = tsubscription2 as PimSubscriptionProxy;
						if (pimSubscriptionProxy2 != null)
						{
							pimSubscriptionProxy2.SetDebug(base.IsDebugOn || base.IsVerboseOn);
						}
					}
					this.WriteResult<TSubscription>(dataObjects);
					if (!base.HasErrors && base.WriteObjectCount == 0U && localizedString != null)
					{
						this.WriteDebugInfoAndError(new ManagementObjectNotFoundException(localizedString.Value), ErrorCategory.InvalidData, null);
					}
				}
			}
			finally
			{
				this.WriteDebugInfo();
			}
			TaskLogger.LogExit();
		}

		protected void WriteDebugInfoAndError(Exception exception, ErrorCategory category, object target)
		{
			this.WriteDebugInfo();
			base.WriteError(exception, category, target);
		}

		protected void WriteDebugInfo()
		{
			if (base.IsDebugOn)
			{
				base.WriteDebug(CommonLoggingHelper.SyncLogSession.GetBlackBoxText());
			}
			CommonLoggingHelper.SyncLogSession.ClearBlackBox();
		}

		public static readonly string ParameterIncludeReport = "IncludeReport";

		private bool identityDefined;
	}
}
