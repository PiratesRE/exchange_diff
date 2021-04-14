using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class SimpleScopeSet<TScope>
	{
		public SimpleScopeSet(TScope recipientReadScope, TScope recipientWriteScope, TScope configReadScope, TScope configWriteScope)
		{
			this.recipientReadScope = recipientReadScope;
			this.recipientWriteScope = recipientWriteScope;
			this.configReadScope = configReadScope;
			this.configWriteScope = configWriteScope;
		}

		public TScope RecipientReadScope
		{
			get
			{
				return this.recipientReadScope;
			}
		}

		public TScope RecipientWriteScope
		{
			get
			{
				return this.recipientWriteScope;
			}
		}

		public TScope ConfigReadScope
		{
			get
			{
				return this.configReadScope;
			}
		}

		public TScope ConfigWriteScope
		{
			get
			{
				return this.configWriteScope;
			}
		}

		public TScope this[ScopeLocation scopeLocation]
		{
			get
			{
				switch (scopeLocation)
				{
				case ScopeLocation.RecipientRead:
					return this.recipientReadScope;
				case ScopeLocation.RecipientWrite:
					return this.recipientWriteScope;
				case ScopeLocation.ConfigRead:
					return this.configReadScope;
				case ScopeLocation.ConfigWrite:
					return this.configWriteScope;
				default:
					throw new ArgumentException("scopeLocation");
				}
			}
		}

		public bool Equals(SimpleScopeSet<TScope> obj)
		{
			if (obj == null)
			{
				return false;
			}
			bool flag;
			if (this.ConfigReadScope == null)
			{
				flag = (null == obj.ConfigReadScope);
			}
			else
			{
				TScope tscope = this.ConfigReadScope;
				flag = tscope.Equals(obj.ConfigReadScope);
			}
			if (this.ConfigWriteScope == null)
			{
				flag = (flag && null == obj.ConfigWriteScope);
			}
			else
			{
				bool flag2;
				if (flag)
				{
					TScope tscope2 = this.ConfigWriteScope;
					flag2 = tscope2.Equals(obj.ConfigWriteScope);
				}
				else
				{
					flag2 = false;
				}
				flag = flag2;
			}
			if (this.RecipientReadScope == null)
			{
				flag = (flag && null == obj.RecipientReadScope);
			}
			else
			{
				bool flag3;
				if (flag)
				{
					TScope tscope3 = this.RecipientReadScope;
					flag3 = tscope3.Equals(obj.RecipientReadScope);
				}
				else
				{
					flag3 = false;
				}
				flag = flag3;
			}
			if (this.RecipientWriteScope == null)
			{
				flag = (flag && null == obj.RecipientWriteScope);
			}
			else
			{
				bool flag4;
				if (flag)
				{
					TScope tscope4 = this.RecipientWriteScope;
					flag4 = tscope4.Equals(obj.RecipientWriteScope);
				}
				else
				{
					flag4 = false;
				}
				flag = flag4;
			}
			return flag;
		}

		private TScope recipientReadScope;

		private TScope recipientWriteScope;

		private TScope configReadScope;

		private TScope configWriteScope;
	}
}
