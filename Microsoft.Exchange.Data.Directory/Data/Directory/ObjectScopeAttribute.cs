using System;

namespace Microsoft.Exchange.Data.Directory
{
	[AttributeUsage(AttributeTargets.Class)]
	internal sealed class ObjectScopeAttribute : Attribute
	{
		public ObjectScopeAttribute(params ConfigScopes[] applicableScopes)
		{
			this.mainConfigScope = ConfigScopes.None;
			for (int i = 0; i < applicableScopes.Length; i++)
			{
				if (applicableScopes[i] == ConfigScopes.None)
				{
					throw new ArgumentOutOfRangeException("configScopes", "'None' is not a valid ConfigScope.");
				}
			}
			this.applicableScopes = applicableScopes;
		}

		public ObjectScopeAttribute(ConfigScopes configScope)
		{
			if (configScope == ConfigScopes.None)
			{
				throw new ArgumentOutOfRangeException("configScope", "'None' is not a valid ConfigScope.");
			}
			this.mainConfigScope = configScope;
			this.applicableScopes = new ConfigScopes[]
			{
				configScope
			};
		}

		public ConfigScopes ConfigScope
		{
			get
			{
				return this.mainConfigScope;
			}
		}

		public bool HasApplicableConfigScope(ConfigScopes configScope)
		{
			return Array.Exists<ConfigScopes>(this.applicableScopes, (ConfigScopes s) => s == configScope);
		}

		public bool IsTenant
		{
			get
			{
				return this.HasApplicableConfigScope(ConfigScopes.TenantSubTree);
			}
		}

		private ConfigScopes mainConfigScope;

		private ConfigScopes[] applicableScopes;
	}
}
