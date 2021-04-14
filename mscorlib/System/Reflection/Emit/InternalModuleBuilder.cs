using System;

namespace System.Reflection.Emit
{
	internal sealed class InternalModuleBuilder : RuntimeModule
	{
		private InternalModuleBuilder()
		{
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is InternalModuleBuilder)
			{
				return this == obj;
			}
			return obj.Equals(this);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
