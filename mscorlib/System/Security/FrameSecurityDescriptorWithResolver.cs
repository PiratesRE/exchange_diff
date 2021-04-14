using System;
using System.Reflection.Emit;

namespace System.Security
{
	internal class FrameSecurityDescriptorWithResolver : FrameSecurityDescriptor
	{
		public DynamicResolver Resolver
		{
			get
			{
				return this.m_resolver;
			}
		}

		private DynamicResolver m_resolver;
	}
}
