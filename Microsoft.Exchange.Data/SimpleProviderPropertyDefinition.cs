using System;
using System.Collections;
using System.Reflection;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class SimpleProviderPropertyDefinition : ProviderPropertyDefinition
	{
		public PropertyDefinitionFlags Flags
		{
			get
			{
				return (PropertyDefinitionFlags)this.flags;
			}
		}

		public override bool IsMultivalued
		{
			get
			{
				return (this.Flags & PropertyDefinitionFlags.MultiValued) != PropertyDefinitionFlags.None;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return (this.Flags & PropertyDefinitionFlags.ReadOnly) != PropertyDefinitionFlags.None;
			}
		}

		public override bool IsFilterOnly
		{
			get
			{
				return (this.Flags & PropertyDefinitionFlags.FilterOnly) != PropertyDefinitionFlags.None;
			}
		}

		public override bool IsMandatory
		{
			get
			{
				return (this.Flags & PropertyDefinitionFlags.Mandatory) != PropertyDefinitionFlags.None;
			}
		}

		public override bool IsCalculated
		{
			get
			{
				return (this.Flags & PropertyDefinitionFlags.Calculated) != PropertyDefinitionFlags.None;
			}
		}

		public override bool PersistDefaultValue
		{
			get
			{
				return (this.Flags & PropertyDefinitionFlags.PersistDefaultValue) != PropertyDefinitionFlags.None;
			}
		}

		public override bool IsWriteOnce
		{
			get
			{
				return (this.Flags & PropertyDefinitionFlags.WriteOnce) != PropertyDefinitionFlags.None;
			}
		}

		public override bool IsBinary
		{
			get
			{
				return (this.Flags & PropertyDefinitionFlags.Binary) != PropertyDefinitionFlags.None;
			}
		}

		public override bool IsTaskPopulated
		{
			get
			{
				return (this.Flags & PropertyDefinitionFlags.TaskPopulated) != PropertyDefinitionFlags.None;
			}
		}

		private bool IsKnownFailure(Type type)
		{
			return "Microsoft.Exchange.Data.Schedule" == type.FullName || "Microsoft.Exchange.Common.ScheduleInterval[]" == type.FullName || "Microsoft.Exchange.Data.Storage.Management.ADRecipientOrAddress[]" == type.FullName || "System.Collections.Generic.List`1[[Microsoft.Exchange.Monitoring.MonitoringEvent, Microsoft.Exchange.Configuration.ObjectModel, Version=15.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]]" == type.FullName || "Microsoft.Exchange.Data.Directory.SystemConfiguration.MessageClassification[]" == type.FullName || "Microsoft.Exchange.Data.ContentAggregation.AggregationSubscriptionIdentity[]" == type.FullName || "System.DirectoryServices.ActiveDirectoryRights[]" == type.FullName || "Microsoft.Exchange.Configuration.Tasks.ExtendedRightIdParameter[]" == type.FullName || "Microsoft.Exchange.Configuration.Tasks.ADSchemaObjectIdParameter[]" == type.FullName || "Microsoft.Exchange.Management.RecipientTasks.MailboxRights[]" == type.FullName || "Microsoft.Exchange.Transport.Sync.Common.Subscription.AggregationSubscriptionIdentity[]" == type.FullName || "Microsoft.Exchange.MessagingPolicies.Rules.Tasks.TransportRulePredicate[]" == type.FullName || "Microsoft.Exchange.MessagingPolicies.Rules.Tasks.TransportRuleAction[]" == type.FullName || "Microsoft.Exchange.Configuration.Tasks.RecipientIdParameter[]" == type.FullName || "Microsoft.Exchange.Data.Word[]" == type.FullName || "Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Pattern[]" == type.FullName || "Microsoft.Exchange.Data.SmtpAddress[]" == type.FullName || "System.String[]" == type.FullName || "Microsoft.Exchange.Management.Tracking.RecipientTrackingEvent[]" == type.FullName;
		}

		internal SimpleProviderPropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints, ProviderPropertyDefinition[] supportingProperties, CustomFilterBuilderDelegate customFilterBuilderDelegate, GetterDelegate getterDelegate, SetterDelegate setterDelegate) : base(name, versionAdded, type, defaultValue, readConstraints, writeConstraints, supportingProperties, customFilterBuilderDelegate, getterDelegate, setterDelegate)
		{
			this.flags = (int)flags;
			Type left = base.Type.GetTypeInfo().IsGenericType ? base.Type.GetTypeInfo().GetGenericTypeDefinition() : null;
			if (typeof(ICollection).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) && typeof(byte[]) != type && !this.IsKnownFailure(type))
			{
				if (this.IsMultivalued)
				{
					throw new ArgumentException(base.Name + ": Only specify the element type for MultiValued properties. Type: " + type.FullName, "type");
				}
				throw new ArgumentException(base.Name + ": Instead of specifying a collection type, please mark the property definition as MultiValued and specify the element type as the type Type: " + type.FullName, "type");
			}
			else
			{
				if (this.IsMultivalued && base.DefaultValue != null)
				{
					throw new ArgumentException(base.Name + ": Multivalued properties should not have default value", "defaultValue");
				}
				if (base.DefaultValue == null && this.PersistDefaultValue)
				{
					throw new ArgumentException(base.Name + ": Cannot persist default value when no default value is specified.", "defaultValue");
				}
				if (this.IsTaskPopulated)
				{
					if (this.IsFilterOnly)
					{
						throw new ArgumentException(base.Name + ": TaskPopulated properties are not supported as FilterOnly properties at this time.", "flags");
					}
					if (this.IsCalculated)
					{
						throw new ArgumentException(base.Name + ": TaskPopulated properties should not be marked as calculated.", "flags");
					}
					if (this.IsReadOnly || this.IsWriteOnce)
					{
						throw new ArgumentException(base.Name + ": TaskPopulated properties must be modifiable from within the task and cannot be marked ReadOnly or WriteOnce.", "flags");
					}
				}
				if (this.IsWriteOnce && this.IsReadOnly)
				{
					throw new ArgumentException(base.Name + ": Properties cannot be marked as both ReadOnly and WriteOnce.", "flags");
				}
				if (this.IsMandatory && this.IsFilterOnly)
				{
					throw new ArgumentException(base.Name + ": Mandatory properties should not be marked FilterOnly.", "flags");
				}
				if (this.IsFilterOnly && this.IsCalculated)
				{
					throw new ArgumentException(base.Name + ": Calculated properties should not be marked as FilterOnly.", "flags");
				}
				if (!this.IsMultivalued && !this.IsFilterOnly && !this.IsCalculated)
				{
					Type underlyingType = Nullable.GetUnderlyingType(type);
					if (base.Type.GetTypeInfo().IsValueType && left != typeof(Nullable<>) && base.DefaultValue == null)
					{
						throw new ArgumentException(base.Name + ": Value type properties must have default value.", "defaultValue");
					}
					if (left == typeof(Nullable<>) && base.DefaultValue != null)
					{
						throw new ArgumentException(base.Name + ": Default value for a property of Nullable type must be 'null'.", "defaultValue");
					}
					if (null != underlyingType && underlyingType.GetTypeInfo().IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(Unlimited<>))
					{
						throw new ArgumentException(base.Name + ": Properties cannot be both Unlimited and Nullable.", "type");
					}
					if (left == typeof(Unlimited<>) && !(bool)base.Type.GetTypeInfo().GetDeclaredProperty("IsUnlimited").GetValue(base.DefaultValue, null))
					{
						throw new ArgumentException(base.Name + ": Default value for a property of Unlimited type must be 'unlimited'.", "defaultValue");
					}
					if (left == typeof(Unlimited<>) && this.PersistDefaultValue)
					{
						throw new ArgumentException(base.Name + ": Cannot persist default value for Unlimited type.", "flags");
					}
				}
				if (this.IsCalculated != (base.GetterDelegate != null))
				{
					throw new ArgumentException(base.Name + ": Calculated properties must have GetterDelegate, non-calculated ones must not", "getterDelegate");
				}
				if ((this.IsCalculated && !this.IsReadOnly) != (base.SetterDelegate != null))
				{
					throw new ArgumentException(base.Name + ": Writable calculated properties must have SetterDelegate, non-calculated & readonly ones must not", "setterDelegate");
				}
				if (this.IsCalculated != (base.SupportingProperties.Count != 0))
				{
					throw new ArgumentException(base.Name + ": Calculated properties must have supporting properties, non-calculated ones must not", "supportingProperties");
				}
				if (this.IsReadOnly && writeConstraints.Length > 0)
				{
					throw new ArgumentException(base.Name + ": Readonly properties should not have write-only constraints", "writeConstraints");
				}
				if (base.DefaultValue != null && !this.PersistDefaultValue && !this.IsCalculated && !this.IsTaskPopulated && (base.Type == typeof(int) || base.Type == typeof(uint) || base.Type == typeof(long) || base.Type == typeof(ulong) || base.Type == typeof(ByteQuantifiedSize) || base.Type == typeof(EnhancedTimeSpan) || base.Type == typeof(DateTime)))
				{
					throw new ArgumentException(string.Format("Property {0} has type {1} and a default value that is not persisted", base.Name, base.Type.ToString()), "flags");
				}
				if (this.IsCalculated && base.Type.GetTypeInfo().IsValueType && left != typeof(Nullable<>) && base.DefaultValue == null && !this.IsMultivalued)
				{
					throw new ArgumentException(string.Format("Calculated property {0} has type {1} and no default value", base.Name, base.Type.ToString()), "flags");
				}
				return;
			}
		}

		public SimpleProviderPropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints) : this(name, versionAdded, type, flags, defaultValue, readConstraints, writeConstraints, SimpleProviderPropertyDefinition.None, null, null, null)
		{
		}

		public override bool Equals(ProviderPropertyDefinition other)
		{
			return object.ReferenceEquals(other, this) || (base.Equals(other) && (other as SimpleProviderPropertyDefinition).Flags == this.Flags);
		}

		public new static SimpleProviderPropertyDefinition[] None = new SimpleProviderPropertyDefinition[0];

		private readonly int flags;
	}
}
