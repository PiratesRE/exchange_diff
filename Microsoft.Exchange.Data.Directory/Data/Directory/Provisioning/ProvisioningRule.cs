using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	internal abstract class ProvisioningRule : IProvisioningRule
	{
		public ICollection<Type> TargetObjectTypes
		{
			get
			{
				return this.targetObjectTypes;
			}
		}

		public ProvisioningContext Context
		{
			get
			{
				return this.context;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Context");
				}
				this.context = value;
			}
		}

		public ProvisioningRule(Type[] targetObjectTypes)
		{
			if (targetObjectTypes == null || targetObjectTypes.Length == 0)
			{
				throw new ArgumentNullException("targetObjectTypes");
			}
			this.targetObjectTypes = new ReadOnlyCollection<Type>(targetObjectTypes);
		}

		private ReadOnlyCollection<Type> targetObjectTypes;

		private ProvisioningContext context;
	}
}
