using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AjaxControlToolkit
{
	public class ScriptBuckets : Collection<CombinableScripts>
	{
		public CombinableScripts GetScriptByAlias(string alias, bool thrownOnNotFound = false)
		{
			CombinableScripts result = null;
			if (!this.Alias2Script.TryGetValue(alias.ToLowerInvariant(), out result) && thrownOnNotFound)
			{
				throw new InvalidOperationException(string.Format("Script alias '{0}' cannot be found.", alias));
			}
			return result;
		}

		public CombinableScripts GetScriptByName(string name)
		{
			CombinableScripts result = null;
			this.Name2Script.TryGetValue(name.ToLowerInvariant(), out result);
			return result;
		}

		internal void Initialize()
		{
			Dictionary<string, CombinableScripts> dictionary = this.Alias2Script;
			Dictionary<string, CombinableScripts> dictionary2 = this.Name2Script;
			foreach (CombinableScripts combinableScripts in this)
			{
				if (combinableScripts.Rank == 0)
				{
					this.IncreaseScriptRank(combinableScripts);
				}
			}
		}

		internal void InitializeRuntimeScript(CombinableScripts script)
		{
			if (this.GetScriptByAlias(script.Alias, false) == null)
			{
				base.Add(script);
				this.alias2Script = null;
				this.name2Script = null;
				this.Initialize();
			}
		}

		private void IncreaseScriptRank(CombinableScripts script)
		{
			script.Rank++;
			if (script.Rank > base.Count)
			{
				throw new InvalidOperationException("Circular reference detected in CombinableScripts.DependsOn.");
			}
			if (script.DependsOn != null && script.DependsOn.Length > 0)
			{
				for (int i = 0; i < script.DependsOn.Length; i++)
				{
					CombinableScripts scriptByAlias = this.GetScriptByAlias(script.DependsOn[i], true);
					if (scriptByAlias.Rank <= script.Rank)
					{
						scriptByAlias.Rank = script.Rank;
						this.IncreaseScriptRank(scriptByAlias);
					}
				}
			}
		}

		private Dictionary<string, CombinableScripts> Name2Script
		{
			get
			{
				if (this.name2Script == null)
				{
					Dictionary<string, CombinableScripts> dictionary = new Dictionary<string, CombinableScripts>(base.Count * 2);
					foreach (CombinableScripts combinableScripts in this)
					{
						foreach (ScriptEntry scriptEntry in combinableScripts.Scripts)
						{
							dictionary.Add(scriptEntry.Name.ToLowerInvariant(), combinableScripts);
						}
					}
					this.name2Script = dictionary;
				}
				return this.name2Script;
			}
		}

		private Dictionary<string, CombinableScripts> Alias2Script
		{
			get
			{
				if (this.alias2Script == null)
				{
					Dictionary<string, CombinableScripts> dictionary = new Dictionary<string, CombinableScripts>(base.Count);
					foreach (CombinableScripts combinableScripts in this)
					{
						dictionary.Add(combinableScripts.Alias.ToLowerInvariant(), combinableScripts);
					}
					this.alias2Script = dictionary;
				}
				return this.alias2Script;
			}
		}

		private Dictionary<string, CombinableScripts> name2Script;

		private Dictionary<string, CombinableScripts> alias2Script;
	}
}
