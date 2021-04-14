using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	[Serializable]
	internal class AdminPropertyDefinition : ProviderPropertyDefinition
	{
		public AdminPropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints) : base(name, versionAdded, type, defaultValue, readConstraints, writeConstraints)
		{
		}

		public AdminPropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, object defaultValue, bool mandatory, bool multiValued, bool readOnly, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints) : base(name, versionAdded, type, defaultValue, readConstraints, writeConstraints)
		{
			this.isMandatory = mandatory;
			this.isMultivalued = multiValued;
			this.isReadOnly = readOnly;
		}

		public override bool IsMandatory
		{
			get
			{
				return this.isMandatory;
			}
		}

		public override bool IsMultivalued
		{
			get
			{
				return this.isMultivalued;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		public override bool IsCalculated
		{
			get
			{
				return false;
			}
		}

		public override bool IsFilterOnly
		{
			get
			{
				return false;
			}
		}

		public override bool IsWriteOnce
		{
			get
			{
				return false;
			}
		}

		public override bool PersistDefaultValue
		{
			get
			{
				return false;
			}
		}

		public override bool IsBinary
		{
			get
			{
				return false;
			}
		}

		private bool isMandatory;

		private bool isMultivalued;

		private bool isReadOnly;
	}
}
