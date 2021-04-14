using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory
{
	internal class SidBudgetKey : LookupBudgetKey
	{
		public SidBudgetKey(SecurityIdentifier sid, BudgetType budgetType, bool isServiceAccount, ADSessionSettings settings) : base(budgetType, isServiceAccount)
		{
			if (sid == null)
			{
				throw new ArgumentNullException("sid");
			}
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}
			this.NtAccount = SidToAccountMap.Singleton.Get(sid).ToString();
			this.SessionSettings = settings;
			if (sid.AccountDomainSid == null && (sid.IsWellKnown(WellKnownSidType.LocalSystemSid) || sid.IsWellKnown(WellKnownSidType.NetworkServiceSid)))
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug((long)this.GetHashCode(), "[SidBudgetKey.ctor] Using domain sid for local computer account.");
				this.Sid = SidBudgetKey.localMachineSid.Member;
			}
			else
			{
				this.Sid = sid;
			}
			this.cachedToString = this.GetCachedToString();
			this.cachedHashCode = (base.BudgetType.GetHashCode() ^ this.Sid.GetHashCode() ^ base.IsServiceAccountBudget.GetHashCode());
		}

		public ADSessionSettings SessionSettings { get; private set; }

		public SecurityIdentifier Sid { get; private set; }

		public string NtAccount { get; private set; }

		public override string ToString()
		{
			return this.cachedToString;
		}

		public override int GetHashCode()
		{
			return this.cachedHashCode;
		}

		public override bool Equals(object obj)
		{
			SidBudgetKey sidBudgetKey = obj as SidBudgetKey;
			return !(sidBudgetKey == null) && (sidBudgetKey.BudgetType == base.BudgetType && sidBudgetKey.Sid == this.Sid) && sidBudgetKey.IsServiceAccountBudget == base.IsServiceAccountBudget;
		}

		internal override IThrottlingPolicy InternalLookup()
		{
			IRecipientSession session = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, this.SessionSettings, 201, "InternalLookup", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\throttling\\SidBudgetKey.cs");
			return base.ADRetryLookup(delegate
			{
				MiniRecipient recipient = session.FindMiniRecipientBySid<MiniRecipient>(this.Sid, null);
				return this.GetPolicyForRecipient(recipient);
			});
		}

		private string GetCachedToString()
		{
			string text = base.BudgetType.ToString();
			StringBuilder stringBuilder = new StringBuilder(this.NtAccount.Length + text.Length + 11);
			stringBuilder.Append("Sid~");
			stringBuilder.Append(this.NtAccount);
			stringBuilder.Append("~");
			stringBuilder.Append(text);
			stringBuilder.Append("~");
			stringBuilder.Append(base.IsServiceAccountBudget ? "true" : "false");
			return stringBuilder.ToString();
		}

		private readonly string cachedToString;

		private readonly int cachedHashCode;

		private static LazyMember<SecurityIdentifier> localMachineSid = new LazyMember<SecurityIdentifier>(delegate()
		{
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 44, "localMachineSid", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\throttling\\SidBudgetKey.cs");
			session.UseConfigNC = false;
			session.UseGlobalCatalog = true;
			SecurityIdentifier result = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADComputer adcomputer = session.FindLocalComputer();
				if (adcomputer != null)
				{
					result = adcomputer.Sid;
					return;
				}
				ExTraceGlobals.ClientThrottlingTracer.TraceError(0L, "[SidBudgetKe.LocalMachineSidInitializer] FindLocalComputer returned null.  Using local machine sid instead.");
			});
			if (!adoperationResult.Succeeded)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceError<Exception>(0L, "[SidBudgetKey.LocalMachineSidInitializer] Domain computer lookup failed. Using local machine sid instead.  Exception: {0}", adoperationResult.Exception);
			}
			return result ?? new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
		});
	}
}
