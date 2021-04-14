using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PropertyBagContext
	{
		public PropertyBagContext()
		{
		}

		public PropertyBagContext(PropertyBagContext other)
		{
			if (other == null)
			{
				return;
			}
			this.session = other.session;
			this.coreState = other.coreState;
			this.coreObject = other.coreObject;
			this.storeObject = other.storeObject;
			this.isValidationDisabled = other.isValidationDisabled;
		}

		public void Copy(PropertyBagContext other)
		{
			if (other == null)
			{
				return;
			}
			this.session = other.session;
			this.coreState = other.coreState;
			this.coreObject = other.coreObject;
			this.storeObject = other.storeObject;
			this.isValidationDisabled = other.isValidationDisabled;
		}

		public StoreSession Session
		{
			get
			{
				return this.session;
			}
			set
			{
				this.session = value;
			}
		}

		public ICoreObject CoreObject
		{
			get
			{
				return this.coreObject;
			}
			set
			{
				this.coreState = null;
				this.coreObject = value;
			}
		}

		public ICoreState CoreState
		{
			get
			{
				if (this.coreState != null)
				{
					return this.coreState;
				}
				return this.CoreObject;
			}
			set
			{
				this.coreState = value;
			}
		}

		public StoreObject StoreObject
		{
			get
			{
				return this.storeObject;
			}
			set
			{
				this.storeObject = value;
			}
		}

		internal bool IsValidationDisabled
		{
			get
			{
				return this.isValidationDisabled;
			}
			set
			{
				this.isValidationDisabled = value;
			}
		}

		internal AutomaticContactLinkingAction AutomaticContactLinkingAction { get; set; }

		private StoreSession session;

		private ICoreObject coreObject;

		private ICoreState coreState;

		private StoreObject storeObject;

		private bool isValidationDisabled;
	}
}
