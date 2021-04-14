using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CustomPropertyRule : PropertyRule
	{
		public CustomPropertyRule(string name, Func<ICorePropertyBag, bool> writeEnforceDelegate, params PropertyReference[] references) : this(name, null, writeEnforceDelegate, references)
		{
			this.writeEnforceDelegate = writeEnforceDelegate;
		}

		public CustomPropertyRule(string name, Action<ILocationIdentifierSetter> onSetWriteEnforceLocationIdentifier, Func<ICorePropertyBag, bool> writeEnforceDelegate, params PropertyReference[] references) : base(name, onSetWriteEnforceLocationIdentifier, references)
		{
			this.writeEnforceDelegate = writeEnforceDelegate;
		}

		protected override bool WriteEnforceRule(ICorePropertyBag propertyBag)
		{
			return this.writeEnforceDelegate(propertyBag);
		}

		private readonly Func<ICorePropertyBag, bool> writeEnforceDelegate;
	}
}
