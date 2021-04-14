using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[ComVisible(true)]
	public sealed class LCIDConversionAttribute : Attribute
	{
		public LCIDConversionAttribute(int lcid)
		{
			this._val = lcid;
		}

		public int Value
		{
			get
			{
				return this._val;
			}
		}

		internal int _val;
	}
}
