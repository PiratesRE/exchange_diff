using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class TransportRule : ADConfigurationObject
	{
		public TransportRule()
		{
			this.WhenChangedUTCCopy = null;
		}

		public TransportRule(string ruleName, ADObjectId ruleCollectionId)
		{
			this.WhenChangedUTCCopy = null;
			base.SetId(ruleCollectionId.GetChildId(ruleName));
		}

		public DateTime? WhenChangedUTCCopy
		{
			get
			{
				DateTime? whenChangedUTC = base.WhenChangedUTC;
				if (whenChangedUTC == null)
				{
					return this.whenChangedUTCCopy;
				}
				return new DateTime?(whenChangedUTC.GetValueOrDefault());
			}
			set
			{
				this.whenChangedUTCCopy = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return TransportRule.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return TransportRule.mostDerivedClass;
			}
		}

		public string Xml
		{
			get
			{
				return (string)this[TransportRuleSchema.Xml];
			}
			internal set
			{
				this[TransportRuleSchema.Xml] = value;
			}
		}

		public int Priority
		{
			get
			{
				return (int)this[TransportRuleSchema.Priority];
			}
			set
			{
				this[TransportRuleSchema.Priority] = value;
			}
		}

		public Guid ImmutableId
		{
			get
			{
				Guid guid = (Guid)this[TransportRuleSchema.ImmutableId];
				if (guid == Guid.Empty)
				{
					guid = ((base.Id == null) ? Guid.Empty : base.Id.ObjectGuid);
				}
				return guid;
			}
			internal set
			{
				this[TransportRuleSchema.ImmutableId] = value;
			}
		}

		private static TransportRuleSchema schema = ObjectSchema.GetInstance<TransportRuleSchema>();

		private static string mostDerivedClass = "msExchTransportRule";

		private DateTime? whenChangedUTCCopy;
	}
}
