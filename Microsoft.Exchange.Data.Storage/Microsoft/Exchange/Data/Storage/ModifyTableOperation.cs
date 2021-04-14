using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ModifyTableOperation
	{
		public ModifyTableOperation(ModifyTableOperationType operationType, PropValue[] properties)
		{
			EnumValidator.ThrowIfInvalid<ModifyTableOperationType>(operationType, "operationType");
			this.operationType = operationType;
			this.properties = properties;
		}

		public ModifyTableOperationType Operation
		{
			get
			{
				return this.operationType;
			}
		}

		public PropValue[] Properties
		{
			get
			{
				return this.properties;
			}
		}

		private readonly ModifyTableOperationType operationType;

		private readonly PropValue[] properties;
	}
}
