using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	internal sealed class SimplePropertyDefinition : ProviderPropertyDefinition
	{
		public SimplePropertyDefinition(string name, Type type, object defaultValue) : base(name, ExchangeObjectVersion.Exchange2010, type, defaultValue)
		{
		}

		public override bool IsMultivalued
		{
			get
			{
				return false;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
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

		public override bool IsMandatory
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

		public override bool IsWriteOnce
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
	}
}
