using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationUserSkippedItem : ConfigurableObject
	{
		public MigrationUserSkippedItem() : base(new SimplePropertyBag(SimpleProviderObjectSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
			base.ResetChangeTracking();
		}

		public string Kind
		{
			get
			{
				return (string)this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.Kind];
			}
			internal set
			{
				this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.Kind] = value;
			}
		}

		public string FolderName
		{
			get
			{
				return (string)this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.FolderName];
			}
			internal set
			{
				this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.FolderName] = value;
			}
		}

		public string Sender
		{
			get
			{
				return (string)this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.Sender];
			}
			internal set
			{
				this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.Sender] = value;
			}
		}

		public string Recipient
		{
			get
			{
				return (string)this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.Recipient];
			}
			internal set
			{
				this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.Recipient] = value;
			}
		}

		public string Subject
		{
			get
			{
				return (string)this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.Subject];
			}
			internal set
			{
				this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.Subject] = value;
			}
		}

		public string MessageClass
		{
			get
			{
				return (string)this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.MessageClass];
			}
			internal set
			{
				this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.MessageClass] = value;
			}
		}

		public int? MessageSize
		{
			get
			{
				return (int?)this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.MessageSize];
			}
			internal set
			{
				this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.MessageSize] = value;
			}
		}

		public DateTime? DateSent
		{
			get
			{
				return (DateTime?)this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.DateSent];
			}
			internal set
			{
				this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.DateSent] = value;
			}
		}

		public DateTime? DateReceived
		{
			get
			{
				return (DateTime?)this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.DateReceived];
			}
			internal set
			{
				this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.DateReceived] = value;
			}
		}

		public string Failure
		{
			get
			{
				return (string)this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.Failure];
			}
			internal set
			{
				this[MigrationUserSkippedItem.MigrationUserSkippedItemSchema.Failure] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MigrationUserSkippedItem.schema;
			}
		}

		public override string ToString()
		{
			return ServerStrings.MigrationUserSkippedItemString(this.FolderName, this.Subject);
		}

		private static MigrationUserSkippedItem.MigrationUserSkippedItemSchema schema = ObjectSchema.GetInstance<MigrationUserSkippedItem.MigrationUserSkippedItemSchema>();

		internal class MigrationUserSkippedItemSchema : SimpleProviderObjectSchema
		{
			public static readonly ProviderPropertyDefinition Kind = new SimpleProviderPropertyDefinition("Kind", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition FolderName = new SimpleProviderPropertyDefinition("FolderName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition Sender = new SimpleProviderPropertyDefinition("Sender", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition Recipient = new SimpleProviderPropertyDefinition("Recipient", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition Subject = new SimpleProviderPropertyDefinition("Subject", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition MessageClass = new SimpleProviderPropertyDefinition("MessageClass", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition Failure = new SimpleProviderPropertyDefinition("Failure", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition DateSent = new SimpleProviderPropertyDefinition("DateSent", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition DateReceived = new SimpleProviderPropertyDefinition("DateReceived", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition MessageSize = new SimpleProviderPropertyDefinition("MessageSize", ExchangeObjectVersion.Exchange2010, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
