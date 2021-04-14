using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal abstract class QueryListBase<T> : IQueryList where T : ResultBase
	{
		public void Add(UserResultMapping userResultMapping)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<string, QueryListBase<T>>((long)this.GetHashCode(), "Adding address '{0}' to {1}.", userResultMapping.Mailbox, this);
			string key = userResultMapping.SmtpProxyAddress.ToString();
			T t;
			if (!this.resultDictionary.TryGetValue(key, out t))
			{
				t = this.CreateResult(userResultMapping);
				this.resultDictionary.Add(key, t);
			}
			userResultMapping.Result = t;
		}

		protected abstract T CreateResult(UserResultMapping userResultMapping);

		public abstract void Execute();

		protected Dictionary<string, T> resultDictionary = new Dictionary<string, T>();
	}
}
