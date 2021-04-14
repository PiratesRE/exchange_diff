using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	internal abstract class SettingsScopeFilterSchema : SimpleProviderObjectSchema
	{
		public static SettingsScopeFilterSchema GetSchemaInstance(IConfigSchema schema)
		{
			if (schema == null)
			{
				return ObjectSchema.GetInstance<SettingsScopeFilterSchema.UntypedSettingsScopeFilterSchema>();
			}
			Type schemaType = typeof(SettingsScopeFilterSchema.TypedSettingsScopeFilterSchema<>).MakeGenericType(new Type[]
			{
				schema.GetType()
			});
			SettingsScopeFilterSchema settingsScopeFilterSchema = (SettingsScopeFilterSchema)ObjectSchema.GetInstance(schemaType);
			settingsScopeFilterSchema.InitializeScopeProperties(schema);
			return settingsScopeFilterSchema;
		}

		public virtual PropertyDefinition LookupSchemaProperty(string propName)
		{
			return base[propName];
		}

		protected virtual void InitializeScopeProperties(IConfigSchema schema)
		{
		}

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition ServerGuid = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<Guid?>("ServerGuid", (ISettingsContext c) => c.ServerGuid);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition ServerName = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<string>("ServerName", (ISettingsContext c) => c.ServerName);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition ServerVersion = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<Version>("ServerVersion", delegate(ISettingsContext c)
		{
			if (!(c.ServerVersion != null))
			{
				return null;
			}
			return c.ServerVersion;
		});

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition ServerRole = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<string>("ServerRole", (ISettingsContext c) => c.ServerRole);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition ProcessName = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<string>("ProcessName", (ISettingsContext c) => c.ProcessName);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition DagOrServerGuid = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<Guid?>("DagOrServerGuid", (ISettingsContext c) => c.DagOrServerGuid);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition DatabaseGuid = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<Guid?>("DatabaseGuid", (ISettingsContext c) => c.DatabaseGuid);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition DatabaseName = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<string>("DatabaseName", (ISettingsContext c) => c.DatabaseName);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition DatabaseVersion = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<Version>("DatabaseVersion", (ISettingsContext c) => c.DatabaseVersion);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition OrganizationName = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<string>("OrganizationName", (ISettingsContext c) => c.OrganizationName);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition OrganizationVersion = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<ExchangeObjectVersion>("OrganizationVersion", (ISettingsContext c) => c.OrganizationVersion);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition MailboxGuid = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<Guid?>("MailboxGuid", (ISettingsContext c) => c.MailboxGuid);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition Hour = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<int>("Hour", (ISettingsContext c) => DateTime.UtcNow.Hour);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition Minute = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<int>("Minute", (ISettingsContext c) => DateTime.UtcNow.Minute);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition DayOfWeek = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<DayOfWeek>("DayOfWeek", (ISettingsContext c) => DateTime.UtcNow.DayOfWeek);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition Day = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<int>("Day", (ISettingsContext c) => DateTime.UtcNow.Day);

		public static readonly SettingsScopeFilterSchema.ScopeFilterPropertyDefinition Month = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<int>("Month", (ISettingsContext c) => DateTime.UtcNow.Month);

		private class TypedSettingsScopeFilterSchema<TConfig> : SettingsScopeFilterSchema where TConfig : IConfigSchema
		{
			protected override void InitializeScopeProperties(IConfigSchema schema)
			{
				if (this.scopePropertiesInitialized)
				{
					return;
				}
				lock (this.locker)
				{
					if (!this.scopePropertiesInitialized)
					{
						if (schema.ScopeSchema != null)
						{
							List<PropertyDefinition> list = new List<PropertyDefinition>(base.AllProperties);
							foreach (string name in schema.ScopeSchema.Settings)
							{
								ConfigurationProperty scopeProperty = schema.ScopeSchema.GetConfigurationProperty(name, null);
								list.Add(new SettingsScopeFilterSchema.ScopeFilterPropertyDefinition(scopeProperty.Name, scopeProperty.Type, scopeProperty.DefaultValue, delegate(ISettingsContext ctx)
								{
									object result;
									if (!ExchangeConfigurationSection.TryConvertFromInvariantString(scopeProperty, ctx.GetGenericProperty(scopeProperty.Name), out result))
									{
										result = null;
									}
									return result;
								}));
							}
							base.AllProperties = new ReadOnlyCollection<PropertyDefinition>(list);
							base.InitializePropertyCollections();
						}
						this.scopePropertiesInitialized = true;
					}
				}
			}

			private bool scopePropertiesInitialized;

			private object locker = new object();
		}

		private class UntypedSettingsScopeFilterSchema : SettingsScopeFilterSchema
		{
			public override PropertyDefinition LookupSchemaProperty(string propName)
			{
				PropertyDefinition propertyDefinition = base.LookupSchemaProperty(propName);
				if (propertyDefinition == null && !this.namedPropInstances.TryGetValue(propName, out propertyDefinition))
				{
					lock (this.namedPropInstancesLocker)
					{
						if (!this.namedPropInstances.TryGetValue(propName, out propertyDefinition))
						{
							propertyDefinition = new SettingsScopeFilterSchema.TypedScopeFilterPropertyDefinition<string>(propName, (ISettingsContext ctx) => ctx.GetGenericProperty(propName));
							this.namedPropInstances[propName] = propertyDefinition;
						}
					}
				}
				return propertyDefinition;
			}

			private Dictionary<string, PropertyDefinition> namedPropInstances = new Dictionary<string, PropertyDefinition>(StringComparer.OrdinalIgnoreCase);

			private object namedPropInstancesLocker = new object();
		}

		public class ScopeFilterPropertyDefinition : SimpleProviderPropertyDefinition
		{
			public ScopeFilterPropertyDefinition(string propertyName, Type propertyType, object defaultValue, Func<ISettingsContext, object> retrieveValueDelegate) : base(propertyName, ExchangeObjectVersion.Exchange2010, propertyType, (defaultValue == null) ? PropertyDefinitionFlags.None : PropertyDefinitionFlags.PersistDefaultValue, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None)
			{
				this.retrieveValueDelegate = retrieveValueDelegate;
			}

			public object RetrieveValue(ISettingsContext ctx)
			{
				if (ctx == null)
				{
					return null;
				}
				return this.retrieveValueDelegate(ctx);
			}

			private Func<ISettingsContext, object> retrieveValueDelegate;
		}

		private class TypedScopeFilterPropertyDefinition<T> : SettingsScopeFilterSchema.ScopeFilterPropertyDefinition
		{
			public TypedScopeFilterPropertyDefinition(string propertyName, Func<ISettingsContext, object> retrieveValueDelegate) : base(propertyName, typeof(T), default(T), retrieveValueDelegate)
			{
			}
		}
	}
}
