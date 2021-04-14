using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public sealed class AssemblyVersionAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public AssemblyVersionAttribute(string version)
		{
			this.m_version = version;
		}

		[__DynamicallyInvokable]
		public string Version
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_version;
			}
		}

		private string m_version;
	}
}
