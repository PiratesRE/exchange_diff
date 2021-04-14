using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
	[ComVisible(true)]
	public sealed class AutomationProxyAttribute : Attribute
	{
		public AutomationProxyAttribute(bool val)
		{
			this._val = val;
		}

		public bool Value
		{
			get
			{
				return this._val;
			}
		}

		internal bool _val;
	}
}
