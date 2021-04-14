using System;

namespace Microsoft.Exchange.Data.ContentTypes.Internal
{
	internal abstract class ValueTypeContainer
	{
		protected ValueTypeContainer()
		{
			this.Reset();
		}

		public void SetValueTypeParameter(string value)
		{
			this.valueTypeParameter = value;
			this.isValueTypeInitialized = false;
		}

		public void SetPropertyName(string value)
		{
			this.propertyName = value;
			this.isValueTypeInitialized = false;
		}

		public abstract bool IsTextType { get; }

		public abstract bool CanBeMultivalued { get; }

		public abstract bool CanBeCompound { get; }

		public bool IsInitialized
		{
			get
			{
				return this.propertyName != null;
			}
		}

		public virtual void Reset()
		{
			this.valueTypeParameter = null;
			this.propertyName = null;
			this.isValueTypeInitialized = false;
		}

		protected string valueTypeParameter;

		protected string propertyName;

		protected bool isValueTypeInitialized;
	}
}
