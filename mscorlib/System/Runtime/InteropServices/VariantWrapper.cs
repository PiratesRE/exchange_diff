using System;

namespace System.Runtime.InteropServices
{
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class VariantWrapper
	{
		[__DynamicallyInvokable]
		public VariantWrapper(object obj)
		{
			this.m_WrappedObject = obj;
		}

		[__DynamicallyInvokable]
		public object WrappedObject
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_WrappedObject;
			}
		}

		private object m_WrappedObject;
	}
}
