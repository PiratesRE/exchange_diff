using System;

namespace Microsoft.Exchange.ExchangeSystem
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
	public sealed class PointerTypeAttribute : Attribute
	{
		public PointerTypeAttribute(string pointerType)
		{
			this.pointerType = pointerType;
		}

		public string InherentType
		{
			get
			{
				return this.pointerType;
			}
		}

		private readonly string pointerType;
	}
}
