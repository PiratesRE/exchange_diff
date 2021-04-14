using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal abstract class GetUserSettingsCommandBase
	{
		internal GetUserSettingsCommandBase(CallContext callContext)
		{
			this.response = callContext.Response;
			this.userResultMappingList = new List<UserResultMapping>(callContext.Users.Count);
			foreach (User user in callContext.Users)
			{
				UserResultMapping item = new UserResultMapping(user.Mailbox, callContext);
				this.userResultMappingList.Add(item);
			}
		}

		internal void Execute()
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<GetUserSettingsCommandBase>((long)this.GetHashCode(), "{0} Execute() called.", this);
			this.queryLists = new List<IQueryList>(this.userResultMappingList.Count);
			this.adQueryListDictionary = new Dictionary<OrganizationId, ADQueryList>(this.userResultMappingList.Count);
			using (IStandardBudget standardBudget = this.AcquireBudget())
			{
				HttpContext.Current.Items["StartBudget"] = standardBudget.ToString();
				GetUserSettingsCommandBase.InitializeBudget(standardBudget);
				foreach (UserResultMapping userResultMapping in this.userResultMappingList)
				{
					if (userResultMapping.IsValidSmtpAddress)
					{
						this.AddToQueryList(userResultMapping, standardBudget);
					}
					else
					{
						ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "Mailbox not valid smtp address for '{0}'.", userResultMapping.Mailbox);
						this.SetInvalidSmtpAddressResult(userResultMapping);
					}
				}
				foreach (IQueryList queryList in this.queryLists)
				{
					queryList.Execute();
				}
				foreach (UserResultMapping userResultMapping2 in this.userResultMappingList)
				{
					if (!(userResultMapping2.Result is InvalidSmtpAddressResult) && !this.IsPostAdQueryAuthorized(userResultMapping2))
					{
						ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "Mailbox not valid smtp address for '{0}' because caller was not authorized to perform query.", userResultMapping2.Mailbox);
						this.SetInvalidSmtpAddressResult(userResultMapping2);
					}
					this.response.UserResponses.Add(userResultMapping2.Result.CreateResponse(standardBudget));
				}
				HttpContext.Current.Items["EndBudget"] = standardBudget.ToString();
			}
		}

		protected virtual bool IsPostAdQueryAuthorized(UserResultMapping userResultMapping)
		{
			return true;
		}

		protected void SetInvalidSmtpAddressResult(UserResultMapping userResultMapping)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "SetInvalidSmtpAddressResult() called for '{0}'.", userResultMapping.Mailbox);
			userResultMapping.Result = new InvalidSmtpAddressResult(userResultMapping);
		}

		protected bool TryGetOrganizationId(UserResultMapping userResultMapping, out OrganizationId organizationId)
		{
			organizationId = DomainToOrganizationIdCache.Singleton.Get(new SmtpDomain(userResultMapping.SmtpAddress.Domain));
			bool flag = organizationId == OrganizationId.ForestWideOrgId || (organizationId != null && ADAccountPartitionLocator.IsKnownPartition(organizationId.PartitionId));
			if (!flag)
			{
				organizationId = null;
			}
			return flag;
		}

		protected void AddToADQueryList(UserResultMapping userResultMapping, OrganizationId organizationId, ADObjectId searchRoot, IBudget budget)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "AddToADQueryList() called for '{0}'.", userResultMapping.Mailbox);
			ADQueryList adqueryList;
			if (!this.adQueryListDictionary.TryGetValue(organizationId, out adqueryList))
			{
				adqueryList = new ADQueryList(organizationId, searchRoot, budget);
				this.adQueryListDictionary.Add(organizationId, adqueryList);
				this.queryLists.Add(adqueryList);
			}
			adqueryList.Add(userResultMapping);
		}

		private static void InitializeBudget(IStandardBudget budget)
		{
			string callerInfo = "GetUserSettingsCommandBase.InitializeBudget";
			budget.CheckOverBudget();
			budget.StartConnection(callerInfo);
			budget.StartLocal(callerInfo, default(TimeSpan));
		}

		protected abstract IStandardBudget AcquireBudget();

		protected abstract void AddToQueryList(UserResultMapping userResultMapping, IBudget budget);

		protected GetUserSettingsResponse response;

		protected List<IQueryList> queryLists;

		protected Dictionary<OrganizationId, ADQueryList> adQueryListDictionary;

		private List<UserResultMapping> userResultMappingList;
	}
}
