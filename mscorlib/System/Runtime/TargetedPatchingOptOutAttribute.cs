using System;

namespace System.Runtime
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class TargetedPatchingOptOutAttribute : Attribute
	{
		public TargetedPatchingOptOutAttribute(string reason)
		{
			this.m_reason = reason;
		}

		public string Reason
		{
			get
			{
				return this.m_reason;
			}
		}

		private TargetedPatchingOptOutAttribute()
		{
		}

		private string m_reason;
	}
}
