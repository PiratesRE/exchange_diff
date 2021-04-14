﻿using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	[Serializable]
	internal class SerializationMonkey : ISerializable, IFieldInfo
	{
		[SecurityCritical]
		internal SerializationMonkey(SerializationInfo info, StreamingContext ctx)
		{
			this._obj.RootSetObjectData(info, ctx);
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_Method"));
		}

		public string[] FieldNames
		{
			[SecurityCritical]
			get
			{
				return this.fieldNames;
			}
			[SecurityCritical]
			set
			{
				this.fieldNames = value;
			}
		}

		public Type[] FieldTypes
		{
			[SecurityCritical]
			get
			{
				return this.fieldTypes;
			}
			[SecurityCritical]
			set
			{
				this.fieldTypes = value;
			}
		}

		internal ISerializationRootObject _obj;

		internal string[] fieldNames;

		internal Type[] fieldTypes;
	}
}
