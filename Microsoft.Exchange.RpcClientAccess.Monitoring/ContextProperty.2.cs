using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ContextProperty<TValue> : ContextProperty
	{
		private ContextProperty(Func<object> defaultValueDelegate) : base(Guid.NewGuid(), default(ContextProperty.DeclarationOptions))
		{
			this.defaultValueDelegate = defaultValueDelegate;
		}

		private ContextProperty(ContextProperty<TValue> parentProperty, ContextProperty.DeclarationOptions options) : base(parentProperty.Identity, options)
		{
			base.Name = parentProperty.Name;
			this.defaultValueDelegate = parentProperty.defaultValueDelegate;
		}

		public override Type Type
		{
			get
			{
				return typeof(TValue);
			}
		}

		public static ContextProperty<TValue> Declare(Func<TValue> defaultValueDelegate)
		{
			return new ContextProperty<TValue>((defaultValueDelegate != null) ? (() => defaultValueDelegate()) : null);
		}

		public static ContextProperty<TValue> Declare(TValue defaultValue)
		{
			return ContextProperty<TValue>.Declare(() => defaultValue);
		}

		public static ContextProperty<TValue> Declare()
		{
			return ContextProperty<TValue>.Declare(null);
		}

		public ContextProperty<TValue> GetOnly()
		{
			ContextProperty.DeclarationOptions options = this.Options;
			options.AccessMode = ContextProperty.AccessMode.Get;
			return new ContextProperty<TValue>(this, options);
		}

		public ContextProperty<TValue> SetOnly()
		{
			ContextProperty.DeclarationOptions options = this.Options;
			options.AccessMode = ContextProperty.AccessMode.Set;
			return new ContextProperty<TValue>(this, options);
		}

		internal override bool TryGetDefaultValue(out object defaultValue)
		{
			if (this.defaultValueDelegate != null)
			{
				defaultValue = (TValue)((object)this.defaultValueDelegate());
				return true;
			}
			defaultValue = null;
			return false;
		}

		private readonly Func<object> defaultValueDelegate;
	}
}
