using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal sealed class InterceptorAgentRulesCache
	{
		private InterceptorAgentRulesCache()
		{
			this.cache = new ConfigurationLoader<IList<InterceptorAgentRule>, InterceptorAgentRulesCache.Builder>(Components.TransportAppConfig.ADPolling.InterceptorRulesReloadInterval);
			this.cache.Changed += this.OnRulesChanged;
			Components.ConfigChanged += this.cache.Reload;
		}

		public static InterceptorAgentRulesCache Instance
		{
			get
			{
				return InterceptorAgentRulesCache.instance;
			}
		}

		internal void Load()
		{
			if (Monitor.TryEnter(this.syncObject, TimeSpan.Zero))
			{
				try
				{
					if (!this.loaded)
					{
						this.cache.Load();
						this.loaded = true;
					}
				}
				finally
				{
					Monitor.Exit(this.syncObject);
				}
			}
		}

		internal void Reload()
		{
			if (Monitor.TryEnter(this.syncObject, TimeSpan.Zero))
			{
				try
				{
					this.cache.Reload(null, null);
				}
				finally
				{
					Monitor.Exit(this.syncObject);
				}
			}
		}

		internal void RegisterCache(FilteredRuleCache filteredRuleCache)
		{
			ArgumentValidator.ThrowIfNull("filteredRuleCache", filteredRuleCache);
			lock (this.ruleCacheGuard)
			{
				this.ruleCaches.Add(new WeakReference(filteredRuleCache));
				if (this.loaded)
				{
					this.Reload();
				}
				else
				{
					this.Load();
				}
			}
		}

		private void OnRulesChanged(IList<InterceptorAgentRule> allRulesFromAd)
		{
			List<InterceptorAgentRule> list = new List<InterceptorAgentRule>();
			lock (this.ruleCacheGuard)
			{
				List<WeakReference> list2 = new List<WeakReference>();
				foreach (WeakReference weakReference in this.ruleCaches)
				{
					FilteredRuleCache filteredRuleCache = weakReference.Target as FilteredRuleCache;
					if (filteredRuleCache == null)
					{
						list2.Add(weakReference);
					}
					else
					{
						filteredRuleCache.UpdateCache(allRulesFromAd);
						list.AddRange(filteredRuleCache.Rules);
					}
				}
				foreach (WeakReference item in list2)
				{
					this.ruleCaches.Remove(item);
				}
			}
			this.TraceNewConfiguration(list);
		}

		private void TraceNewConfiguration(IEnumerable<InterceptorAgentRule> activeRules)
		{
			if (activeRules == null || !activeRules.Any<InterceptorAgentRule>())
			{
				return;
			}
			bool rulesEmpty = true;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<rules>\n");
			foreach (InterceptorAgentRule interceptorAgentRule in activeRules)
			{
				rulesEmpty = false;
				interceptorAgentRule.ToString(stringBuilder);
			}
			stringBuilder.Append("</rules>");
			this.TraceNewConfiguration(stringBuilder.ToString(), rulesEmpty);
		}

		private void TraceNewConfiguration(string configuration, bool rulesEmpty)
		{
			ExTraceGlobals.InterceptorAgentTracer.TraceDebug((long)this.GetHashCode(), "New configuration: " + configuration);
			if (!rulesEmpty)
			{
				Util.EventLog.LogEvent(TransportEventLogConstants.Tuple_InterceptorAgentConfigurationReplaced, null, new object[]
				{
					configuration
				});
			}
		}

		private static readonly InterceptorAgentRulesCache instance = new InterceptorAgentRulesCache();

		private readonly object syncObject = new object();

		private readonly ConfigurationLoader<IList<InterceptorAgentRule>, InterceptorAgentRulesCache.Builder> cache;

		private readonly List<WeakReference> ruleCaches = new List<WeakReference>();

		private readonly object ruleCacheGuard = new object();

		private bool loaded;

		internal class Builder : ConfigurationLoader<IList<InterceptorAgentRule>, InterceptorAgentRulesCache.Builder>.SimpleBuilder<InterceptorRule>
		{
			public override void LoadData(ITopologyConfigurationSession session, QueryScope scope = QueryScope.SubTree)
			{
				ArgumentValidator.ThrowIfNull("session", session);
				ADObjectId orgContainerId = session.GetOrgContainerId();
				if (orgContainerId == null)
				{
					throw new OrgContainerNotFoundException();
				}
				ADObjectId childId = orgContainerId.GetChildId("Transport Settings");
				if (childId == null)
				{
					throw new EndpointContainerNotFoundException("Transport Settings");
				}
				ADObjectId childId2 = childId.GetChildId("Interceptor Rules");
				if (childId2 == null)
				{
					throw new EndpointContainerNotFoundException("Interceptor Rules");
				}
				base.RootId = childId2;
				base.LoadData(session, QueryScope.OneLevel);
			}

			protected override IList<InterceptorAgentRule> BuildCache(List<InterceptorRule> configObjects)
			{
				if (configObjects == null || configObjects.Count == 0)
				{
					return new InterceptorAgentRule[0];
				}
				List<InterceptorAgentRule> list = new List<InterceptorAgentRule>();
				List<InterceptorAgentRule> list2 = new List<InterceptorAgentRule>();
				List<InterceptorAgentRule> list3 = new List<InterceptorAgentRule>();
				foreach (InterceptorRule interceptorRule in configObjects)
				{
					InterceptorAgentRulesCache.Builder.InterceptorRuleScope ruleScope = InterceptorAgentRulesCache.Builder.GetRuleScope(interceptorRule);
					Version version;
					if (ruleScope != InterceptorAgentRulesCache.Builder.InterceptorRuleScope.None && Version.TryParse(interceptorRule.Version, out version) && version.Major <= InterceptorAgentRule.Version.Major)
					{
						try
						{
							InterceptorAgentRule interceptorAgentRule = InterceptorAgentRule.CreateRuleFromXml(interceptorRule.Xml);
							if (!interceptorAgentRule.MatchRoleAndServerVersion(Components.Configuration.ProcessTransportRole, Components.Configuration.LocalServer.TransportServer.AdminDisplayVersion))
							{
								ExTraceGlobals.InterceptorAgentTracer.TraceDebug(0L, "Skipping rule as process role and server version does not match");
							}
							else
							{
								interceptorAgentRule.SetPropertiesFromAdObjet(interceptorRule);
								switch (ruleScope)
								{
								case InterceptorAgentRulesCache.Builder.InterceptorRuleScope.Server:
									list.Add(interceptorAgentRule);
									break;
								case InterceptorAgentRulesCache.Builder.InterceptorRuleScope.Dag:
								case InterceptorAgentRulesCache.Builder.InterceptorRuleScope.Site:
									list2.Add(interceptorAgentRule);
									break;
								case InterceptorAgentRulesCache.Builder.InterceptorRuleScope.Forest:
									list3.Add(interceptorAgentRule);
									break;
								default:
									throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "InterceptorRuleScope {0} not expected", new object[]
									{
										ruleScope
									}));
								}
							}
						}
						catch (MissingFieldException arg)
						{
							ExTraceGlobals.InterceptorAgentTracer.TraceError<MissingFieldException>(0L, "Cannot parse rule from xml. Error: {0}", arg);
						}
						catch (InvalidOperationException arg2)
						{
							ExTraceGlobals.InterceptorAgentTracer.TraceError<InvalidOperationException>(0L, "Cannot parse rule from xml. Error: {0}", arg2);
						}
						catch (ArgumentException arg3)
						{
							ExTraceGlobals.InterceptorAgentTracer.TraceError<ArgumentException>(0L, "Cannot parse rule from xml. Error: {0}", arg3);
						}
						catch (FormatException arg4)
						{
							ExTraceGlobals.InterceptorAgentTracer.TraceError<FormatException>(0L, "Cannot parse rule from xml. Error: {0}", arg4);
						}
					}
				}
				IOrderedEnumerable<InterceptorAgentRule> collection = from rule in list
				orderby rule.WhenCreatedUtc.Value descending
				select rule;
				IOrderedEnumerable<InterceptorAgentRule> collection2 = from rule in list2
				orderby rule.WhenCreatedUtc.Value descending
				select rule;
				IOrderedEnumerable<InterceptorAgentRule> collection3 = from rule in list3
				orderby rule.WhenCreatedUtc.Value descending
				select rule;
				List<InterceptorAgentRule> list4 = new List<InterceptorAgentRule>(list.Count + list2.Count + list3.Count);
				list4.AddRange(collection);
				list4.AddRange(collection2);
				list4.AddRange(collection3);
				return list4;
			}

			private static InterceptorAgentRulesCache.Builder.InterceptorRuleScope GetRuleScope(InterceptorRule rule)
			{
				InterceptorAgentRulesCache.Builder.InterceptorRuleScope interceptorRuleScope = InterceptorAgentRulesCache.Builder.InterceptorRuleScope.None;
				DateTime expireTimeUtc = rule.ExpireTimeUtc;
				if (expireTimeUtc < DateTime.UtcNow)
				{
					return InterceptorAgentRulesCache.Builder.InterceptorRuleScope.None;
				}
				if (rule.Target != null && rule.Target.Count > 0)
				{
					using (MultiValuedProperty<ADObjectId>.Enumerator enumerator = rule.Target.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ADObjectId id = enumerator.Current;
							if (Components.Configuration.LocalServer.TransportServer.Id.Equals(id))
							{
								interceptorRuleScope = InterceptorAgentRulesCache.Builder.InterceptorRuleScope.Server;
							}
							else if (Components.Configuration.LocalServer.TransportServer.ServerSite != null && Components.Configuration.LocalServer.TransportServer.ServerSite.Equals(id))
							{
								interceptorRuleScope = InterceptorAgentRulesCache.Builder.InterceptorRuleScope.Site;
							}
							else if (Components.Configuration.LocalServer.TransportServer.DatabaseAvailabilityGroup != null && Components.Configuration.LocalServer.TransportServer.DatabaseAvailabilityGroup.Equals(id))
							{
								interceptorRuleScope = InterceptorAgentRulesCache.Builder.InterceptorRuleScope.Dag;
							}
							if (interceptorRuleScope == InterceptorAgentRulesCache.Builder.InterceptorRuleScope.Server)
							{
								break;
							}
						}
						return interceptorRuleScope;
					}
				}
				interceptorRuleScope = InterceptorAgentRulesCache.Builder.InterceptorRuleScope.Forest;
				return interceptorRuleScope;
			}

			private const string TransportSettingsAdContainer = "Transport Settings";

			private const string InterceptorRulesAdContainer = "Interceptor Rules";

			private enum InterceptorRuleScope
			{
				None,
				Server,
				Dag,
				Site,
				Forest
			}
		}
	}
}
