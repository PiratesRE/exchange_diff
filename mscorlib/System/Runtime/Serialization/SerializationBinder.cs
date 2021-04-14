﻿using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	[Serializable]
	public abstract class SerializationBinder
	{
		public virtual void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = null;
			typeName = null;
		}

		public abstract Type BindToType(string assemblyName, string typeName);
	}
}
