using System;
using System.Reflection;

namespace System.Runtime.Serialization
{
	[Serializable]
	internal class MemberHolder
	{
		internal MemberHolder(Type type, StreamingContext ctx)
		{
			this.memberType = type;
			this.context = ctx;
		}

		public override int GetHashCode()
		{
			return this.memberType.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is MemberHolder))
			{
				return false;
			}
			MemberHolder memberHolder = (MemberHolder)obj;
			return memberHolder.memberType == this.memberType && memberHolder.context.State == this.context.State;
		}

		internal MemberInfo[] members;

		internal Type memberType;

		internal StreamingContext context;
	}
}
