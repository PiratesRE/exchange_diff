using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal class ConditionalRegistrationCache
	{
		public static Action<ConditionalRegistration> TESTHOOK_PersistRegistration { get; set; }

		public static Action<string> TESTHOOK_DeleteRegistration { get; set; }

		private ConditionalRegistrationCache()
		{
			this.cache = new ExactTimeoutCache<string, BaseConditionalRegistration>(new RemoveItemDelegate<string, BaseConditionalRegistration>(this.HandleRemoveRegistration), null, null, 1000, false);
		}

		public int Count
		{
			get
			{
				return this.cache.Count;
			}
		}

		public bool PropertyIsActive(PropertyDefinition propDef)
		{
			bool result;
			lock (this.activePropertiesLock)
			{
				int num = 0;
				this.activeProperties.TryGetValue(propDef, out num);
				result = (num > 0);
			}
			return result;
		}

		internal List<ConditionalResults> Evaluate(IReadOnlyPropertyBag propertyBag)
		{
			List<BaseConditionalRegistration> values = this.cache.Values;
			if (values == null || values.Count == 0)
			{
				return null;
			}
			List<ConditionalResults> list = null;
			foreach (BaseConditionalRegistration baseConditionalRegistration in values)
			{
				ConditionalResults conditionalResults = baseConditionalRegistration.Evaluate(propertyBag);
				if (conditionalResults != null)
				{
					if (list == null)
					{
						list = new List<ConditionalResults>();
					}
					list.Add(conditionalResults);
					ConditionalRegistration conditionalRegistration = baseConditionalRegistration as ConditionalRegistration;
					if (conditionalRegistration != null)
					{
						int currentHits = conditionalRegistration.CurrentHits;
						if (currentHits >= conditionalRegistration.MaxHits)
						{
							ExTraceGlobals.DiagnosticHandlersTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "[ConditionalRegistrationCache.Evaluate] Removing entry '{0}' because current hits {1} exceeds MaxHits {2}.", conditionalRegistration.Cookie, currentHits, conditionalRegistration.MaxHits);
							this.cache.Remove(baseConditionalRegistration.Cookie);
						}
					}
				}
			}
			return list;
		}

		internal void Register(ConditionalRegistration registration)
		{
			TimeSpan expiration = (ConditionalRegistrationCache.MaxActiveRegistrationTimeEntry.Value < registration.TimeToLive) ? ConditionalRegistrationCache.MaxActiveRegistrationTimeEntry.Value : registration.TimeToLive;
			if (this.cache.TryAddAbsolute(registration.Cookie, registration, expiration))
			{
				this.UpdateActiveProperties(registration, true);
				if (ConditionalRegistrationCache.TESTHOOK_PersistRegistration == null)
				{
					ConditionalRegistrationLog.Save(registration);
					return;
				}
				ConditionalRegistrationCache.TESTHOOK_PersistRegistration(registration);
			}
		}

		private void UpdateActiveProperties(BaseConditionalRegistration newRegistration, bool adding)
		{
			lock (this.activePropertiesLock)
			{
				foreach (PropertyDefinition key in newRegistration.PropertiesToFetch)
				{
					int num;
					this.activeProperties.TryGetValue(key, out num);
					num += (adding ? 1 : -1);
					if (num <= 0)
					{
						num = 0;
					}
					this.activeProperties[key] = num;
				}
				foreach (PropertyDefinition key2 in newRegistration.QueryFilter.FilterProperties())
				{
					int num2;
					this.activeProperties.TryGetValue(key2, out num2);
					num2 += (adding ? 1 : -1);
					if (num2 <= 0)
					{
						num2 = 0;
					}
					this.activeProperties[key2] = num2;
				}
			}
		}

		internal void Register(PersistentConditionalRegistration persistentRegistration)
		{
			if (this.cache.TryAddAbsolute(persistentRegistration.Cookie, persistentRegistration, DateTime.MaxValue))
			{
				this.UpdateActiveProperties(persistentRegistration, true);
			}
		}

		internal void GetRegistrationMetadata(string userIdentity, out List<BaseConditionalRegistration> active, out List<ConditionalRegistrationLog.ConditionalRegistrationHitMetadata> hits)
		{
			if (string.IsNullOrEmpty(userIdentity))
			{
				active = this.cache.Values;
				hits = ConditionalRegistrationLog.GetHitsMetadata("");
				return;
			}
			active = (from s in this.cache.Values
			where s.User == userIdentity
			select s).ToList<BaseConditionalRegistration>();
			hits = ConditionalRegistrationLog.GetHitsMetadata(userIdentity);
		}

		internal void GetRegistrationMetadata(string userIdentity, string cookie, out BaseConditionalRegistration reg, out ConditionalRegistrationLog.ConditionalRegistrationHitMetadata hit)
		{
			reg = this.GetRegistration(cookie);
			hit = ConditionalRegistrationLog.GetHitsForCookie(userIdentity, cookie);
		}

		internal void Clear()
		{
			this.cache.Clear();
			this.activeProperties.Clear();
		}

		internal bool Remove(string cookie)
		{
			if (ConditionalRegistrationCache.TESTHOOK_DeleteRegistration == null)
			{
				ConditionalRegistrationLog.DeleteRegistration(cookie);
			}
			else
			{
				ConditionalRegistrationCache.TESTHOOK_DeleteRegistration(cookie);
			}
			BaseConditionalRegistration baseConditionalRegistration = this.cache.Remove(cookie.ToString());
			if (baseConditionalRegistration != null)
			{
				this.UpdateActiveProperties(baseConditionalRegistration, false);
			}
			return baseConditionalRegistration != null;
		}

		internal List<string> GetAllKeys()
		{
			return this.cache.Keys;
		}

		internal List<BaseConditionalRegistration> GetAllValues()
		{
			return this.cache.Values;
		}

		internal BaseConditionalRegistration GetRegistration(string cookie)
		{
			BaseConditionalRegistration result;
			this.cache.TryGetValue(cookie.ToString(), out result);
			return result;
		}

		private void HandleRemoveRegistration(string key, BaseConditionalRegistration value, RemoveReason reason)
		{
			ExTraceGlobals.DiagnosticHandlersTracer.TraceDebug<string, RemoveReason, string>((long)this.GetHashCode(), "[ConditionalRegistrationCache.HandleRemoveRegistration] Cookie: {0} was removed for reason {1}.  Description: '{2}'", key, reason, value.Description ?? "<NULL>");
			if (ConditionalRegistrationCache.TESTHOOK_DeleteRegistration == null)
			{
				ConditionalRegistrationLog.DeleteRegistration(key);
			}
			else
			{
				ConditionalRegistrationCache.TESTHOOK_DeleteRegistration(key);
			}
			this.UpdateActiveProperties(value, false);
			if (value.OnExpired != null)
			{
				value.OnExpired(value, reason);
			}
		}

		private static TimeSpanAppSettingsEntry MaxActiveRegistrationTimeEntry = new TimeSpanAppSettingsEntry("MaxActiveRegistrationTime", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(1440.0), ExTraceGlobals.DiagnosticHandlersTracer);

		public static readonly ConditionalRegistrationCache Singleton = new ConditionalRegistrationCache();

		private ExactTimeoutCache<string, BaseConditionalRegistration> cache;

		private object activePropertiesLock = new object();

		private Dictionary<PropertyDefinition, int> activeProperties = new Dictionary<PropertyDefinition, int>();
	}
}
