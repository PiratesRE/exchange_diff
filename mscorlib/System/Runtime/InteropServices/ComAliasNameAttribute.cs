using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	[ComVisible(true)]
	public sealed class ComAliasNameAttribute : Attribute
	{
		public ComAliasNameAttribute(string alias)
		{
			this._val = alias;
		}

		public string Value
		{
			get
			{
				return this._val;
			}
		}

		internal string _val;
	}
}
