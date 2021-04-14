using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal abstract class XsoNestedProperty : XsoProperty, INestedProperty
	{
		public XsoNestedProperty(INestedData nestedData) : base(null)
		{
			this.nestedData = nestedData;
		}

		public XsoNestedProperty(INestedData nestedData, PropertyType type) : base(null, type)
		{
			this.nestedData = nestedData;
		}

		public XsoNestedProperty(INestedData nestedData, PropertyDefinition[] prefetchProps) : base(null, prefetchProps)
		{
			this.nestedData = nestedData;
		}

		public XsoNestedProperty(INestedData nestedData, PropertyType type, PropertyDefinition[] prefetchProps) : base(null, type, prefetchProps)
		{
			this.nestedData = nestedData;
		}

		public virtual INestedData NestedData
		{
			get
			{
				return this.nestedData;
			}
			set
			{
				this.nestedData = value;
			}
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			this.nestedData.SubProperties.Clear();
		}

		public override void Bind(StoreObject item)
		{
			this.Unbind();
			base.Bind(item);
		}

		public override void Unbind()
		{
			this.nestedData.SubProperties.Clear();
			base.Unbind();
		}

		private INestedData nestedData;
	}
}
