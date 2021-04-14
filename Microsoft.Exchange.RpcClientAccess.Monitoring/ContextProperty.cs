using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class ContextProperty
	{
		protected ContextProperty(Guid identity, ContextProperty.DeclarationOptions options)
		{
			this.Identity = identity;
			this.Options = options;
		}

		public string Name { get; internal set; }

		public abstract Type Type { get; }

		public ContextProperty.AccessMode AllowedAccessMode
		{
			get
			{
				return this.Options.AccessMode;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is ContextProperty && ((ContextProperty)obj).Identity == this.Identity;
		}

		public override int GetHashCode()
		{
			return this.Identity.GetHashCode();
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal abstract bool TryGetDefaultValue(out object defaultValue);

		protected readonly Guid Identity;

		protected readonly ContextProperty.DeclarationOptions Options;

		[Flags]
		internal enum AccessMode
		{
			None = 0,
			Get = 1,
			Set = 2
		}

		protected struct DeclarationOptions
		{
			public ContextProperty.AccessMode AccessMode;
		}
	}
}
