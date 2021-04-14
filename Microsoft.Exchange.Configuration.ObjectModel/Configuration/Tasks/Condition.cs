using System;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public abstract class Condition
	{
		public abstract bool Verify();

		public void VerifyAndThrow(LocalizedString ls)
		{
			this.VerifyAndThrow(new LocalizedException(ls));
		}

		public void VerifyAndThrow(LocalizedException le)
		{
			if (!this.Verify())
			{
				throw le;
			}
		}

		public bool Match(Condition conditionToMatch)
		{
			TaskLogger.LogEnter(new object[]
			{
				this,
				conditionToMatch
			});
			if (conditionToMatch == null)
			{
				throw new ArgumentNullException("conditionToMatch");
			}
			bool result = false;
			if (base.GetType() != conditionToMatch.GetType())
			{
				TaskLogger.Trace(Strings.LogConditionMatchingTypeMismacth(base.GetType(), conditionToMatch.GetType()));
			}
			else
			{
				foreach (PropertyInfo propertyInfo in base.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
				{
					object obj = propertyInfo.GetGetMethod().Invoke(this, null);
					object obj2 = propertyInfo.GetGetMethod().Invoke(conditionToMatch, null);
					TaskLogger.Trace("{0}.{1}: {2} ?=? {3}", new object[]
					{
						base.GetType().FullName,
						propertyInfo.Name,
						obj,
						obj2
					});
					if ((obj != null || obj2 != null) && (obj == null || !obj.Equals(obj2)))
					{
						TaskLogger.Trace(Strings.LogConditionMatchingPropertyMismatch(base.GetType(), propertyInfo.Name, obj, obj2));
						goto IL_106;
					}
				}
				result = true;
			}
			IL_106:
			TaskLogger.LogExit();
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.GetType().ToString());
			stringBuilder.Append("(");
			bool flag = true;
			foreach (PropertyInfo propertyInfo in base.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if (!flag)
				{
					stringBuilder.Append(", ");
				}
				string name = propertyInfo.Name;
				object obj = propertyInfo.GetGetMethod().Invoke(this, null);
				stringBuilder.Append(name);
				stringBuilder.Append("=");
				stringBuilder.Append((obj == null) ? "null" : obj.ToString());
				flag = false;
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		internal ConditionAttribute Role
		{
			get
			{
				ExAssert.RetailAssert(null != this.role, "The condition role was not yet set.");
				return this.role;
			}
			set
			{
				ExAssert.RetailAssert(null != value, "Cannot set the condition role to null.");
				ExAssert.RetailAssert(null == this.role, "The condition role is already set and cannot be changed.");
				this.role = value;
			}
		}

		private ConditionAttribute role;
	}
}
