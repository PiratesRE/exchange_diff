using System;
using System.Runtime.InteropServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class AttributeUsageAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public AttributeUsageAttribute(AttributeTargets validOn)
		{
			this.m_attributeTarget = validOn;
		}

		internal AttributeUsageAttribute(AttributeTargets validOn, bool allowMultiple, bool inherited)
		{
			this.m_attributeTarget = validOn;
			this.m_allowMultiple = allowMultiple;
			this.m_inherited = inherited;
		}

		[__DynamicallyInvokable]
		public AttributeTargets ValidOn
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_attributeTarget;
			}
		}

		[__DynamicallyInvokable]
		public bool AllowMultiple
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_allowMultiple;
			}
			[__DynamicallyInvokable]
			set
			{
				this.m_allowMultiple = value;
			}
		}

		[__DynamicallyInvokable]
		public bool Inherited
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_inherited;
			}
			[__DynamicallyInvokable]
			set
			{
				this.m_inherited = value;
			}
		}

		internal AttributeTargets m_attributeTarget = AttributeTargets.All;

		internal bool m_allowMultiple;

		internal bool m_inherited = true;

		internal static AttributeUsageAttribute Default = new AttributeUsageAttribute(AttributeTargets.All);
	}
}
