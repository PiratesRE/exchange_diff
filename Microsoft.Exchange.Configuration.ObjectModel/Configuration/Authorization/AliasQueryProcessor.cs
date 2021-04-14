using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal sealed class AliasQueryProcessor : RbacQuery.RbacQueryProcessor, INamedQueryProcessor
	{
		public AliasQueryProcessor(string roleName, string key)
		{
			if (string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("roleName");
			}
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			this.Name = roleName;
			this.key = key;
			this.ProcessKey();
		}

		public string Name { get; private set; }

		public sealed override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			bool value = false;
			foreach (List<RbacQuery.RbacQueryProcessor> list in this.orQueries)
			{
				bool flag = true;
				foreach (RbacQuery.RbacQueryProcessor rbacQueryProcessor in list)
				{
					if (!rbacQueryProcessor.IsInRole(rbacConfiguration))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					value = true;
					break;
				}
			}
			return new bool?(value);
		}

		private void ProcessKey()
		{
			string[] array = this.key.Split(AliasQueryProcessor.commaSeparator, StringSplitOptions.RemoveEmptyEntries);
			this.orQueries = new List<List<RbacQuery.RbacQueryProcessor>>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(AliasQueryProcessor.plusSeparator, StringSplitOptions.RemoveEmptyEntries);
				List<RbacQuery.RbacQueryProcessor> list = new List<RbacQuery.RbacQueryProcessor>(array2.Length);
				this.orQueries.Add(list);
				foreach (string arg in array2)
				{
					RbacQuery.RbacQueryProcessor item;
					if (!RbacQuery.WellKnownQueryProcessors.TryGetValue(arg, out item))
					{
						throw new ArgumentException(string.Format("Key '{0}' contains a not recongized query: '{1}'. Make sure you only register this alias after all parts had been registered.", this.key, arg));
					}
					list.Add(item);
				}
			}
		}

		private static char[] commaSeparator = new char[]
		{
			','
		};

		private static char[] plusSeparator = new char[]
		{
			'+'
		};

		private readonly string key;

		private List<List<RbacQuery.RbacQueryProcessor>> orQueries;
	}
}
