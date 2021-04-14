using System;
using System.Runtime.InteropServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class ObsoleteAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public ObsoleteAttribute()
		{
			this._message = null;
			this._error = false;
		}

		[__DynamicallyInvokable]
		public ObsoleteAttribute(string message)
		{
			this._message = message;
			this._error = false;
		}

		[__DynamicallyInvokable]
		public ObsoleteAttribute(string message, bool error)
		{
			this._message = message;
			this._error = error;
		}

		[__DynamicallyInvokable]
		public string Message
		{
			[__DynamicallyInvokable]
			get
			{
				return this._message;
			}
		}

		[__DynamicallyInvokable]
		public bool IsError
		{
			[__DynamicallyInvokable]
			get
			{
				return this._error;
			}
		}

		private string _message;

		private bool _error;
	}
}
