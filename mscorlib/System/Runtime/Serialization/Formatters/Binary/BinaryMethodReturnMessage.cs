using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Security;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[Serializable]
	internal class BinaryMethodReturnMessage
	{
		[SecurityCritical]
		internal BinaryMethodReturnMessage(object returnValue, object[] args, Exception e, LogicalCallContext callContext, object[] properties)
		{
			this._returnValue = returnValue;
			if (args == null)
			{
				args = new object[0];
			}
			this._outargs = args;
			this._args = args;
			this._exception = e;
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

		public Exception Exception
		{
			get
			{
				return this._exception;
			}
		}

		public object ReturnValue
		{
			get
			{
				return this._returnValue;
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

		private object[] _outargs;

		private Exception _exception;

		private object _returnValue;

		private object[] _args;

		[SecurityCritical]
		private LogicalCallContext _logicalCallContext;

		private object[] _properties;
	}
}
