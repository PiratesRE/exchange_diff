using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class TestMigrationServerAvailabilityOutcome : ConfigurableObject
	{
		public TestMigrationServerAvailabilityOutcome() : base(new SimpleProviderPropertyBag())
		{
		}

		public TestMigrationServerAvailabilityResult Result
		{
			get
			{
				return (TestMigrationServerAvailabilityResult)this[TestMigrationServerAvailabilityOutcome.TestMigrationServerAvailabilityOutcomeSchema.ResultProperty];
			}
			set
			{
				this[TestMigrationServerAvailabilityOutcome.TestMigrationServerAvailabilityOutcomeSchema.ResultProperty] = value;
			}
		}

		public LocalizedString Message
		{
			get
			{
				return (LocalizedString)this[TestMigrationServerAvailabilityOutcome.TestMigrationServerAvailabilityOutcomeSchema.MessageProperty];
			}
			set
			{
				this[TestMigrationServerAvailabilityOutcome.TestMigrationServerAvailabilityOutcomeSchema.MessageProperty] = value;
			}
		}

		public ExchangeConnectionSettings ConnectionSettings
		{
			get
			{
				return (ExchangeConnectionSettings)this[TestMigrationServerAvailabilityOutcome.TestMigrationServerAvailabilityOutcomeSchema.ConnectionSettings];
			}
			set
			{
				this[TestMigrationServerAvailabilityOutcome.TestMigrationServerAvailabilityOutcomeSchema.ConnectionSettings] = value;
			}
		}

		public bool SupportsCutover
		{
			get
			{
				return (bool)this[TestMigrationServerAvailabilityOutcome.TestMigrationServerAvailabilityOutcomeSchema.SupportsCutover];
			}
			set
			{
				this[TestMigrationServerAvailabilityOutcome.TestMigrationServerAvailabilityOutcomeSchema.SupportsCutover] = value;
			}
		}

		public string ErrorDetail
		{
			get
			{
				return (string)this[TestMigrationServerAvailabilityOutcome.TestMigrationServerAvailabilityOutcomeSchema.ErrorDetail];
			}
			set
			{
				this[TestMigrationServerAvailabilityOutcome.TestMigrationServerAvailabilityOutcomeSchema.ErrorDetail] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<TestMigrationServerAvailabilityOutcome.TestMigrationServerAvailabilityOutcomeSchema>();
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private new ObjectId Identity
		{
			get
			{
				return base.Identity;
			}
		}

		public override string ToString()
		{
			return this.Result.ToString();
		}

		internal static TestMigrationServerAvailabilityOutcome Create(TestMigrationServerAvailabilityResult result, bool supportsCutover, LocalizedString message, string errorDetail)
		{
			return new TestMigrationServerAvailabilityOutcome
			{
				Result = result,
				SupportsCutover = supportsCutover,
				Message = message,
				ErrorDetail = errorDetail
			};
		}

		internal static TestMigrationServerAvailabilityOutcome Create(TestMigrationServerAvailabilityResult result, bool supportsCutover, ExchangeConnectionSettings connectionSettings)
		{
			TestMigrationServerAvailabilityOutcome testMigrationServerAvailabilityOutcome = new TestMigrationServerAvailabilityOutcome();
			testMigrationServerAvailabilityOutcome.Result = result;
			testMigrationServerAvailabilityOutcome.SupportsCutover = supportsCutover;
			if (connectionSettings != null)
			{
				testMigrationServerAvailabilityOutcome.ConnectionSettings = (ExchangeConnectionSettings)connectionSettings.CloneForPresentation();
			}
			return testMigrationServerAvailabilityOutcome;
		}

		private class TestMigrationServerAvailabilityOutcomeSchema : SimpleProviderObjectSchema
		{
			public static readonly ProviderPropertyDefinition MessageProperty = new SimpleProviderPropertyDefinition("MessageProperty", ExchangeObjectVersion.Exchange2010, typeof(LocalizedString), PropertyDefinitionFlags.TaskPopulated, LocalizedString.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition ResultProperty = new SimpleProviderPropertyDefinition("ResultProperty", ExchangeObjectVersion.Exchange2010, typeof(TestMigrationServerAvailabilityResult), PropertyDefinitionFlags.TaskPopulated, TestMigrationServerAvailabilityResult.Failed, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition ConnectionSettings = new SimpleProviderPropertyDefinition("ConnectionSettings", ExchangeObjectVersion.Exchange2010, typeof(ExchangeConnectionSettings), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition SupportsCutover = new SimpleProviderPropertyDefinition("SupportsCutover", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition ErrorDetail = new SimpleProviderPropertyDefinition("ErrorDetail", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
