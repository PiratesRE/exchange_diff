using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class OptionsRbacEngine
	{
		public OptionsRbacEngine(CallContext callContext, bool forceNewRunspace = false)
		{
			this.runspaceConfig = ExchangeRunspaceConfigurationCache.Singleton.Get(callContext.EffectiveCaller, null, forceNewRunspace);
			this.rbacQueryCache = new Dictionary<string, bool>();
		}

		static OptionsRbacEngine()
		{
			RbacQuery.RegisterQueryProcessor("ClosedCampus", new ClosedCampusQueryProcessor());
			RbacQuery.RegisterQueryProcessor("PopImapDisabled", new PopImapDisabledQueryProcessor());
			RbacQuery.RegisterQueryProcessor("BusinessLiveId", new LiveIdInstanceQueryProcessor(LiveIdInstanceType.Business));
			RbacQuery.RegisterQueryProcessor("ConsumerLiveId", new LiveIdInstanceQueryProcessor(LiveIdInstanceType.Consumer));
			RbacQuery.RegisterQueryProcessor("True", new TrueQueryProcessor());
		}

		public bool EvaluateExpression(string rbacQueryExpression)
		{
			string[] source = rbacQueryExpression.Split(new char[]
			{
				','
			});
			return source.Any((string queryExpressionPart) => this.EvaluateExpressionPart(queryExpressionPart));
		}

		private bool EvaluateExpressionPart(string rbacQueryExpressionPart)
		{
			string[] source = rbacQueryExpressionPart.Split(new char[]
			{
				'+'
			});
			return source.All((string role) => this.IsInSingleRole(role));
		}

		private bool IsInSingleRole(string role)
		{
			if (this.rbacQueryCache.ContainsKey(role))
			{
				return this.rbacQueryCache[role];
			}
			bool flag = new RbacQuery(role).IsInRole(this.runspaceConfig);
			this.rbacQueryCache[role] = flag;
			return flag;
		}

		private Dictionary<string, bool> rbacQueryCache;

		private ExchangeRunspaceConfiguration runspaceConfig;
	}
}
