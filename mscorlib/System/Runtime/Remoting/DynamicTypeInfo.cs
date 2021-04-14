using System;
using System.Security;

namespace System.Runtime.Remoting
{
	[Serializable]
	internal class DynamicTypeInfo : TypeInfo
	{
		[SecurityCritical]
		internal DynamicTypeInfo(RuntimeType typeOfObj) : base(typeOfObj)
		{
		}

		[SecurityCritical]
		public override bool CanCastTo(Type castType, object o)
		{
			return ((MarshalByRefObject)o).IsInstanceOfType(castType);
		}
	}
}
