using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class RunspaceServerSettingsPresentationObject : ConfigurableObject, IConfigurable, ICloneable, IEquatable<RunspaceServerSettingsPresentationObject>
	{
		public RunspaceServerSettingsPresentationObject() : base(new SimpleProviderPropertyBag())
		{
			this.RecipientViewRoot = string.Empty;
		}

		internal RunspaceServerSettingsPresentationObject(RunspaceServerSettings serverSettings) : base(new SimpleProviderPropertyBag())
		{
			if (serverSettings == null)
			{
				throw new ArgumentNullException("The value of parameter serverSettings is null!");
			}
			this.UserPreferredGlobalCatalog = serverSettings.UserPreferredGlobalCatalog;
			this.DefaultGlobalCatalog = serverSettings.GetSingleDefaultGlobalCatalog();
			this.DefaultGlobalCatalogsForAllForests = RunspaceServerSettingsPresentationObject.ConvertStringDictionaryToMVP<string, Fqdn>(serverSettings.PreferredGlobalCatalogs);
			this.UserPreferredConfigurationDomainController = serverSettings.UserConfigurationDomainController;
			this.DefaultConfigurationDomainController = serverSettings.GetSingleDefaultConfigurationDomainController();
			this.DefaultConfigurationDomainControllersForAllForests = RunspaceServerSettingsPresentationObject.ConvertStringDictionaryToMVP<string, Fqdn>(serverSettings.ConfigurationDomainControllers);
			this.userServerPerDomain = RunspaceServerSettingsPresentationObject.ConvertStringDictionaryToMVP<ADObjectId, Fqdn>(serverSettings.UserServerPerDomain);
			this.UserPreferredDomainControllers = new MultiValuedProperty<Fqdn>(serverSettings.UserPreferredDomainControllers);
			this.DefaultPreferredDomainControllers = new MultiValuedProperty<Fqdn>(serverSettings.PreferredDomainControllers);
			if (serverSettings.RecipientViewRoot == null)
			{
				this.RecipientViewRoot = string.Empty;
			}
			else
			{
				this.RecipientViewRoot = serverSettings.RecipientViewRoot.ToCanonicalName();
			}
			this.ViewEntireForest = serverSettings.ViewEntireForest;
			this.WriteOriginatingChangeTimestamp = serverSettings.WriteOriginatingChangeTimestamp;
			this.WriteShadowProperties = serverSettings.WriteShadowProperties;
			this.rawServerSettings = serverSettings;
		}

		public Fqdn DefaultGlobalCatalog
		{
			get
			{
				return (Fqdn)this[RunspaceServerSettingsPresentationObjectSchema.DefaultGlobalCatalog];
			}
			set
			{
				this[RunspaceServerSettingsPresentationObjectSchema.DefaultGlobalCatalog] = value;
			}
		}

		public MultiValuedProperty<string> PreferredDomainControllerForDomain
		{
			get
			{
				return this.userServerPerDomain;
			}
		}

		public Fqdn DefaultConfigurationDomainController
		{
			get
			{
				return (Fqdn)this[RunspaceServerSettingsPresentationObjectSchema.DefaultConfigurationDomainController];
			}
			set
			{
				this[RunspaceServerSettingsPresentationObjectSchema.DefaultConfigurationDomainController] = value;
			}
		}

		public MultiValuedProperty<Fqdn> DefaultPreferredDomainControllers
		{
			get
			{
				return (MultiValuedProperty<Fqdn>)this[RunspaceServerSettingsPresentationObjectSchema.DefaultPreferredDomainControllers];
			}
			set
			{
				this[RunspaceServerSettingsPresentationObjectSchema.DefaultPreferredDomainControllers] = value;
			}
		}

		public Fqdn UserPreferredGlobalCatalog
		{
			get
			{
				return (Fqdn)this[RunspaceServerSettingsPresentationObjectSchema.UserPreferredGlobalCatalog];
			}
			set
			{
				this[RunspaceServerSettingsPresentationObjectSchema.UserPreferredGlobalCatalog] = value;
			}
		}

		public Fqdn UserPreferredConfigurationDomainController
		{
			get
			{
				return (Fqdn)this[RunspaceServerSettingsPresentationObjectSchema.UserPreferredConfigurationDomainController];
			}
			set
			{
				this[RunspaceServerSettingsPresentationObjectSchema.UserPreferredConfigurationDomainController] = value;
			}
		}

		public MultiValuedProperty<Fqdn> UserPreferredDomainControllers
		{
			get
			{
				return (MultiValuedProperty<Fqdn>)this[RunspaceServerSettingsPresentationObjectSchema.UserPreferredDomainControllers];
			}
			set
			{
				this[RunspaceServerSettingsPresentationObjectSchema.UserPreferredDomainControllers] = value;
			}
		}

		public MultiValuedProperty<string> DefaultConfigurationDomainControllersForAllForests
		{
			get
			{
				return (MultiValuedProperty<string>)this[RunspaceServerSettingsPresentationObjectSchema.DefaultConfigurationDomainControllersForAllForests];
			}
			set
			{
				this[RunspaceServerSettingsPresentationObjectSchema.DefaultConfigurationDomainControllersForAllForests] = value;
			}
		}

		public MultiValuedProperty<string> DefaultGlobalCatalogsForAllForests
		{
			get
			{
				return (MultiValuedProperty<string>)this[RunspaceServerSettingsPresentationObjectSchema.DefaultGlobalCatalogsForAllForests];
			}
			set
			{
				this[RunspaceServerSettingsPresentationObjectSchema.DefaultGlobalCatalogsForAllForests] = value;
			}
		}

		public string RecipientViewRoot
		{
			get
			{
				return (string)this[RunspaceServerSettingsPresentationObjectSchema.RecipientViewRoot];
			}
			set
			{
				this[RunspaceServerSettingsPresentationObjectSchema.RecipientViewRoot] = value;
				this[RunspaceServerSettingsPresentationObjectSchema.ViewEntireForest] = string.IsNullOrEmpty(value);
			}
		}

		public bool ViewEntireForest
		{
			get
			{
				return (bool)this[RunspaceServerSettingsPresentationObjectSchema.ViewEntireForest];
			}
			set
			{
				this[RunspaceServerSettingsPresentationObjectSchema.ViewEntireForest] = value;
				if (value)
				{
					this[RunspaceServerSettingsPresentationObjectSchema.RecipientViewRoot] = string.Empty;
				}
			}
		}

		public bool WriteOriginatingChangeTimestamp
		{
			get
			{
				return (bool)this[RunspaceServerSettingsPresentationObjectSchema.WriteOriginatingChangeTimestamp];
			}
			set
			{
				this[RunspaceServerSettingsPresentationObjectSchema.WriteOriginatingChangeTimestamp] = value;
			}
		}

		public bool WriteShadowProperties
		{
			get
			{
				return (bool)this[RunspaceServerSettingsPresentationObjectSchema.WriteShadowProperties];
			}
			set
			{
				this[RunspaceServerSettingsPresentationObjectSchema.WriteShadowProperties] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return RunspaceServerSettingsPresentationObject.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal RunspaceServerSettings RawServerSettings
		{
			get
			{
				return this.rawServerSettings;
			}
		}

		public bool Equals(RunspaceServerSettingsPresentationObject other)
		{
			if (other == null)
			{
				return false;
			}
			if (!((this.UserPreferredGlobalCatalog != null) ? this.UserPreferredGlobalCatalog.Equals(other.UserPreferredGlobalCatalog) : (other.UserPreferredGlobalCatalog == null)) || !((this.DefaultGlobalCatalog != null) ? this.DefaultGlobalCatalog.Equals(other.DefaultGlobalCatalog) : (other.DefaultGlobalCatalog == null)) || !((this.DefaultConfigurationDomainController != null) ? this.DefaultConfigurationDomainController.Equals(other.DefaultConfigurationDomainController) : (other.DefaultConfigurationDomainController == null)) || !((this.UserPreferredConfigurationDomainController != null) ? this.UserPreferredConfigurationDomainController.Equals(other.UserPreferredConfigurationDomainController) : (other.UserPreferredConfigurationDomainController == null)) || !RunspaceServerSettingsPresentationObject.SequenceEquals(this.DefaultPreferredDomainControllers, other.DefaultPreferredDomainControllers) || !RunspaceServerSettingsPresentationObject.SequenceEquals(this.UserPreferredDomainControllers, other.UserPreferredDomainControllers) || !(this.RecipientViewRoot == other.RecipientViewRoot) || this.ViewEntireForest != other.ViewEntireForest || this.WriteOriginatingChangeTimestamp != other.WriteOriginatingChangeTimestamp || this.WriteShadowProperties != other.WriteShadowProperties)
			{
				return false;
			}
			if (this.rawServerSettings == null)
			{
				return other.rawServerSettings == null;
			}
			return this.rawServerSettings.Equals(other.rawServerSettings);
		}

		internal static bool SequenceEquals(IList firstList, IList secondList)
		{
			if (firstList.Count != secondList.Count)
			{
				return false;
			}
			for (int i = 0; i < firstList.Count; i++)
			{
				if (!firstList[i].Equals(secondList[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static MultiValuedProperty<string> ConvertStringDictionaryToMVP<TKey, TValue>(IDictionary<TKey, TValue> hash)
		{
			List<string> list = new List<string>(hash.Keys.Count);
			foreach (KeyValuePair<TKey, TValue> keyValuePair in hash)
			{
				List<string> list2 = list;
				string format = "<{0}, {1}>";
				TKey key = keyValuePair.Key;
				object arg = key.ToString();
				TValue value = keyValuePair.Value;
				list2.Add(string.Format(format, arg, value.ToString()));
			}
			return new MultiValuedProperty<string>(list);
		}

		private static RunspaceServerSettingsPresentationObjectSchema schema = ObjectSchema.GetInstance<RunspaceServerSettingsPresentationObjectSchema>();

		private MultiValuedProperty<string> userServerPerDomain;

		private RunspaceServerSettings rawServerSettings;
	}
}
