﻿using System;
using System.Configuration.Assemblies;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyAlgorithmIdAttribute : Attribute
	{
		public AssemblyAlgorithmIdAttribute(AssemblyHashAlgorithm algorithmId)
		{
			this.m_algId = (uint)algorithmId;
		}

		[CLSCompliant(false)]
		public AssemblyAlgorithmIdAttribute(uint algorithmId)
		{
			this.m_algId = algorithmId;
		}

		[CLSCompliant(false)]
		public uint AlgorithmId
		{
			get
			{
				return this.m_algId;
			}
		}

		private uint m_algId;
	}
}
