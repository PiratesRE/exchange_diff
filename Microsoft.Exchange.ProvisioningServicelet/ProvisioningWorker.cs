using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.ProvisioningServicelet;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Servicelets.Provisioning.Messages;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	public class ProvisioningWorker
	{
		internal ProvisioningWorker(List<ProvisioningInfo> data, ProvisioningAgentContext agentContext)
		{
			MigrationUtil.ThrowOnNullArgument(data, "data");
			MigrationUtil.ThrowOnNullArgument(agentContext, "agentContext");
			this.provisioningInfoQueue = data.ToArray();
			this.provisionedObjectsQueue = new List<ProvisionedObject>();
			this.agentContext = agentContext;
			this.completed = false;
		}

		internal bool Completed
		{
			get
			{
				return this.completed;
			}
		}

		internal ProvisioningAgentContext ProvisioningAgentContext
		{
			get
			{
				return this.agentContext;
			}
		}

		internal void Work(object o)
		{
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				for (int i = 0; i < this.provisioningInfoQueue.Length; i++)
				{
					ProvisioningInfo provisioningInfo = this.provisioningInfoQueue[i];
					if (!provisioningInfo.Canceled)
					{
						ProvisioningAgent provisioningAgent = this.CreateProvisioningAgent(provisioningInfo.Data);
						ProvisionedObject item = new ProvisionedObject(provisioningInfo.ItemId, provisioningInfo.JobId, provisioningInfo.Data.ProvisioningType);
						if (provisioningAgent != null)
						{
							provisioningAgent.Work();
							if (provisioningAgent.Error == null)
							{
								item.Error = string.Empty;
								item.Succeeded = true;
								item.TimeAttempted = provisioningAgent.TimeStarted;
								item.TimeFinished = provisioningAgent.TimeFinished;
							}
							else
							{
								item.IsRetryable = !provisioningAgent.IsUserInputError;
								item.TimeAttempted = provisioningAgent.TimeStarted;
								item.Error = provisioningAgent.Error.Message;
								item.Succeeded = false;
							}
							item.MailboxData = provisioningAgent.MailboxData;
							item.GroupMemberProvisioned = provisioningAgent.GroupMemberProvisioned;
							item.GroupMemberSkipped = provisioningAgent.GroupMemberSkipped;
							lock (this.provisionedObjectsQueue)
							{
								this.provisionedObjectsQueue.Add(item);
							}
							provisioningAgent.Dispose();
						}
						else
						{
							item.Succeeded = false;
							item.Error = Strings.UnknownProvisioningType;
							lock (this.provisionedObjectsQueue)
							{
								this.provisionedObjectsQueue.Add(item);
							}
						}
					}
				}
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3269864765U);
			}, (object exception) => true, ReportOptions.None);
			this.completed = true;
			this.agentContext.EventLog.LogEvent(MSExchangeProvisioningEventLogConstants.Tuple_OneProvisioningRoundCompleted, string.Empty, new object[]
			{
				this.agentContext.TenantOrganization
			});
		}

		internal void Cancel(ObjectId id)
		{
			for (int i = 0; i < this.provisioningInfoQueue.Length; i++)
			{
				if (this.provisioningInfoQueue[i].ItemId == id)
				{
					this.provisioningInfoQueue[i].Canceled = true;
					return;
				}
			}
		}

		internal bool IsItemCanceled(ObjectId id)
		{
			return this.provisioningInfoQueue.Any((ProvisioningInfo x) => x.ItemId == id && x.Canceled);
		}

		internal bool ItemCompleted(ObjectId id)
		{
			bool result;
			lock (this.provisionedObjectsQueue)
			{
				result = this.provisionedObjectsQueue.Any((ProvisionedObject x) => x.ItemId == id);
			}
			return result;
		}

		internal ProvisionedObject GetCompletedItem(ObjectId id)
		{
			ProvisionedObject result;
			lock (this.provisionedObjectsQueue)
			{
				result = this.provisionedObjectsQueue.First((ProvisionedObject x) => x.ItemId == id);
			}
			return result;
		}

		private ProvisioningAgent CreateProvisioningAgent(IProvisioningData data)
		{
			switch (data.ProvisioningType)
			{
			case ProvisioningType.User:
				if (data.Component == ProvisioningComponent.ExchangeMigration)
				{
					return new ExchangeUserProvisioningAgent(data, this.agentContext);
				}
				if (data.Component == ProvisioningComponent.StagedExchangeMigration)
				{
					return new StagedExchangeUserProvisioningAgent(data, this.agentContext);
				}
				if (data.Component == ProvisioningComponent.BulkProvision)
				{
					return new BulkProvisionUserProvisioningAgent(data, this.agentContext);
				}
				return null;
			case ProvisioningType.Contact:
				return new ContactProvisioningAgent(data, this.agentContext);
			case ProvisioningType.Group:
				return new GroupProvisioningAgent(data, this.agentContext);
			case ProvisioningType.GroupMember:
				return new MemberProvisioningAgent(data, this.agentContext);
			case ProvisioningType.UserUpdate:
				return new UserUpdateProvisioningAgent(data, this.agentContext);
			case ProvisioningType.ContactUpdate:
				return new ContactUpdateProvisioningAgent(data, this.agentContext);
			case ProvisioningType.MailEnabledUser:
				return new MeuProvisioningAgent(data, this.agentContext);
			case ProvisioningType.MailEnabledUserUpdate:
				return new MeuUpdateProvisioningAgent(data, this.agentContext);
			case ProvisioningType.XO1User:
				return new XO1UserProvisioningAgent(data, this.agentContext);
			}
			return null;
		}

		private ProvisioningInfo[] provisioningInfoQueue;

		private List<ProvisionedObject> provisionedObjectsQueue;

		private ProvisioningAgentContext agentContext;

		private bool completed;
	}
}
