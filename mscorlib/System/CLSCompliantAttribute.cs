using System;
using System.Runtime.InteropServices;

namespace System
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class CLSCompliantAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public CLSCompliantAttribute(bool isCompliant)
		{
			this.m_compliant = isCompliant;
		}

		[__DynamicallyInvokable]
		public bool IsCompliant
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_compliant;
			}
		}

		private bool m_compliant;
	}
}
