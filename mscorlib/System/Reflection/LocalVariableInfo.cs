using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public class LocalVariableInfo
	{
		[__DynamicallyInvokable]
		protected LocalVariableInfo()
		{
		}

		[__DynamicallyInvokable]
		public override string ToString()
		{
			string text = string.Concat(new object[]
			{
				this.LocalType.ToString(),
				" (",
				this.LocalIndex,
				")"
			});
			if (this.IsPinned)
			{
				text += " (pinned)";
			}
			return text;
		}

		[__DynamicallyInvokable]
		public virtual Type LocalType
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_type;
			}
		}

		[__DynamicallyInvokable]
		public virtual bool IsPinned
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_isPinned != 0;
			}
		}

		[__DynamicallyInvokable]
		public virtual int LocalIndex
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_localIndex;
			}
		}

		private RuntimeType m_type;

		private int m_isPinned;

		private int m_localIndex;
	}
}
