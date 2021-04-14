using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public sealed class UnmanagedFunctionPointerAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public UnmanagedFunctionPointerAttribute(CallingConvention callingConvention)
		{
			this.m_callingConvention = callingConvention;
		}

		[__DynamicallyInvokable]
		public CallingConvention CallingConvention
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_callingConvention;
			}
		}

		private CallingConvention m_callingConvention;

		[__DynamicallyInvokable]
		public CharSet CharSet;

		[__DynamicallyInvokable]
		public bool BestFitMapping;

		[__DynamicallyInvokable]
		public bool ThrowOnUnmappableChar;

		[__DynamicallyInvokable]
		public bool SetLastError;
	}
}
