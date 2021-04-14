using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	public sealed class TestDataCenterDKMAccessResult : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return TestDataCenterDKMAccessResult.Schema;
			}
		}

		public TestDataCenterDKMAccessResult(bool aclStateIsGood, string aclStateDetails) : base(new SimpleProviderPropertyBag())
		{
			this.AclStateIsGood = aclStateIsGood;
			this.AclStateDetails = aclStateDetails;
		}

		public bool AclStateIsGood
		{
			get
			{
				return (bool)this[TestDataCenterDKMAccessResultSchema.AclStateIsGood];
			}
			private set
			{
				this[TestDataCenterDKMAccessResultSchema.AclStateIsGood] = value;
			}
		}

		public string AclStateDetails
		{
			get
			{
				return (string)this[TestDataCenterDKMAccessResultSchema.AclStateDetails];
			}
			private set
			{
				this[TestDataCenterDKMAccessResultSchema.AclStateDetails] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly TestDataCenterDKMAccessResultSchema Schema = ObjectSchema.GetInstance<TestDataCenterDKMAccessResultSchema>();
	}
}
