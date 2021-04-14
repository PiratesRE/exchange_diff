using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Security;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[Serializable]
	internal sealed class BinaryMethodCallMessage
	{
		[SecurityCritical]
		internal BinaryMethodCallMessage(string uri, string methodName, string typeName, Type[] instArgs, object[] args, object methodSignature, LogicalCallContext callContext, object[] properties)
		{
			this._methodName = methodName;
			this._typeName = typeName;
			if (args == null)
			{
				args = new object[0];
			}
			this._inargs = args;
			this._args = args;
			this._instArgs = instArgs;
			this._methodSignature = methodSignature;
			if (callContext == null)
			{
				this._logicalCallContext = new LogicalCallContext();
			}
			else
			{
				this._logicalCallContext = callContext;
			}
			this._properties = properties;
		}

		public string MethodName
		{
			get
			{
				return this._methodName;
			}
		}

		public string TypeName
		{
			get
			{
				return this._typeName;
			}
		}

		public Type[] InstantiationArgs
		{
			get
			{
				return this._instArgs;
			}
		}

		public object MethodSignature
		{
			get
			{
				return this._methodSignature;
			}
		}

		public object[] Args
		{
			get
			{
				return this._args;
			}
		}

		public LogicalCallContext LogicalCallContext
		{
			[SecurityCritical]
			get
			{
				return this._logicalCallContext;
			}
		}

		public bool HasProperties
		{
			get
			{
				return this._properties != null;
			}
		}

		internal void PopulateMessageProperties(IDictionary dict)
		{
			foreach (DictionaryEntry dictionaryEntry in this._properties)
			{
				dict[dictionaryEntry.Key] = dictionaryEntry.Value;
			}
		}

		private object[] _inargs;

		private string _methodName;

		private string _typeName;

		private object _methodSignature;

		private Type[] _instArgs;

		private object[] _args;

		[SecurityCritical]
		private LogicalCallContext _logicalCallContext;

		private object[] _properties;
	}
}
