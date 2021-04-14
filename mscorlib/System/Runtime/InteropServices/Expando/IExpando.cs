﻿using System;
using System.Reflection;

namespace System.Runtime.InteropServices.Expando
{
	[Guid("AFBF15E6-C37C-11d2-B88E-00A0C9B471B8")]
	[ComVisible(true)]
	public interface IExpando : IReflect
	{
		FieldInfo AddField(string name);

		PropertyInfo AddProperty(string name);

		MethodInfo AddMethod(string name, Delegate method);

		void RemoveMember(MemberInfo m);
	}
}
