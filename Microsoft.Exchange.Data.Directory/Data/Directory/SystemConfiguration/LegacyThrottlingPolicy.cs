using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class LegacyThrottlingPolicy : ADConfigurationObject
	{
		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return LegacyThrottlingPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return LegacyThrottlingPolicy.mostDerivedClass;
			}
		}

		internal bool IsDefault
		{
			get
			{
				return ((bool?)this[LegacyThrottlingPolicySchema.IsDefaultPolicy]) ?? false;
			}
			set
			{
				this[LegacyThrottlingPolicySchema.IsDefaultPolicy] = value;
			}
		}

		internal LegacyThrottlingPolicySettings AnonymousThrottlingPolicyStateSettings
		{
			get
			{
				return (LegacyThrottlingPolicySettings)this[LegacyThrottlingPolicySchema.AnonymousThrottlingPolicyStateSettings];
			}
			set
			{
				this[LegacyThrottlingPolicySchema.AnonymousThrottlingPolicyStateSettings] = value;
			}
		}

		internal LegacyThrottlingPolicySettings EasThrottlingPolicyStateSettings
		{
			get
			{
				return (LegacyThrottlingPolicySettings)this[LegacyThrottlingPolicySchema.EasThrottlingPolicyStateSettings];
			}
			set
			{
				this[LegacyThrottlingPolicySchema.EasThrottlingPolicyStateSettings] = value;
			}
		}

		internal LegacyThrottlingPolicySettings EwsThrottlingPolicyStateSettings
		{
			get
			{
				return (LegacyThrottlingPolicySettings)this[LegacyThrottlingPolicySchema.EwsThrottlingPolicyStateSettings];
			}
			set
			{
				this[LegacyThrottlingPolicySchema.EwsThrottlingPolicyStateSettings] = value;
			}
		}

		internal LegacyThrottlingPolicySettings ImapThrottlingPolicyStateSettings
		{
			get
			{
				return (LegacyThrottlingPolicySettings)this[LegacyThrottlingPolicySchema.ImapThrottlingPolicyStateSettings];
			}
			set
			{
				this[LegacyThrottlingPolicySchema.ImapThrottlingPolicyStateSettings] = value;
			}
		}

		internal LegacyThrottlingPolicySettings OwaThrottlingPolicyStateSettings
		{
			get
			{
				return (LegacyThrottlingPolicySettings)this[LegacyThrottlingPolicySchema.OwaThrottlingPolicyStateSettings];
			}
			set
			{
				this[LegacyThrottlingPolicySchema.OwaThrottlingPolicyStateSettings] = value;
			}
		}

		internal LegacyThrottlingPolicySettings PopThrottlingPolicyStateSettings
		{
			get
			{
				return (LegacyThrottlingPolicySettings)this[LegacyThrottlingPolicySchema.PopThrottlingPolicyStateSettings];
			}
			set
			{
				this[LegacyThrottlingPolicySchema.PopThrottlingPolicyStateSettings] = value;
			}
		}

		internal LegacyThrottlingPolicySettings PowershellThrottlingPolicyStateSettings
		{
			get
			{
				return (LegacyThrottlingPolicySettings)this[LegacyThrottlingPolicySchema.PowershellThrottlingPolicyStateSettings];
			}
			set
			{
				this[LegacyThrottlingPolicySchema.PowershellThrottlingPolicyStateSettings] = value;
			}
		}

		internal LegacyThrottlingPolicySettings RcaThrottlingPolicyStateSettings
		{
			get
			{
				return (LegacyThrottlingPolicySettings)this[LegacyThrottlingPolicySchema.RcaThrottlingPolicyStateSettings];
			}
			set
			{
				this[LegacyThrottlingPolicySchema.RcaThrottlingPolicyStateSettings] = value;
			}
		}

		internal LegacyThrottlingPolicySettings GeneralThrottlingPolicyStateSettings
		{
			get
			{
				return (LegacyThrottlingPolicySettings)this[LegacyThrottlingPolicySchema.GeneralThrottlingPolicyStateSettings];
			}
			set
			{
				this[LegacyThrottlingPolicySchema.GeneralThrottlingPolicyStateSettings] = value;
			}
		}

		internal static LegacyThrottlingPolicy GetLegacyThrottlingPolicy(ThrottlingPolicy policy)
		{
			LegacyThrottlingPolicy legacyThrottlingPolicy;
			if (policy.ObjectState == ObjectState.New)
			{
				legacyThrottlingPolicy = new LegacyThrottlingPolicy();
				legacyThrottlingPolicy[ADObjectSchema.ObjectState] = ObjectState.Unchanged;
				legacyThrottlingPolicy.ResetChangeTracking();
			}
			else
			{
				legacyThrottlingPolicy = LegacyThrottlingPolicy.LoadLegacyThrottlingPolicyFromAD(policy);
			}
			return legacyThrottlingPolicy;
		}

		internal void SetIdAndName(ThrottlingPolicy policy)
		{
			base.SetId(policy.Id);
			this.propertyBag.ResetChangeTracking(ADObjectSchema.Id);
			base.Name = policy.Id.Name;
			this.propertyBag.ResetChangeTracking(ADObjectSchema.Name);
			base.OrganizationId = policy.OrganizationId;
			this.propertyBag.ResetChangeTracking(ADObjectSchema.OrganizationId);
		}

		private static LegacyThrottlingPolicy LoadLegacyThrottlingPolicyFromAD(ThrottlingPolicy policy)
		{
			LegacyThrottlingPolicy[] array = policy.Session.Find<LegacyThrottlingPolicy>(policy.Id, QueryScope.Base, null, null, 2);
			if (array == null || array.Length != 1)
			{
				throw new InvalidOperationException("Could not read throttling policy " + policy.Id);
			}
			return array[0];
		}

		private static void AppendSettingsString(StringBuilder sb, string name, LegacyThrottlingPolicySettings settings)
		{
			if (settings != null && !string.IsNullOrEmpty(settings.ToString()))
			{
				if (sb.Length > 0)
				{
					sb.Append(", ");
				}
				sb.Append("Legacy/" + name + ":" + settings.ToString());
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			LegacyThrottlingPolicy.AppendSettingsString(stringBuilder, "Anonymous", this.AnonymousThrottlingPolicyStateSettings);
			LegacyThrottlingPolicy.AppendSettingsString(stringBuilder, "Eas", this.EasThrottlingPolicyStateSettings);
			LegacyThrottlingPolicy.AppendSettingsString(stringBuilder, "Ews", this.EwsThrottlingPolicyStateSettings);
			LegacyThrottlingPolicy.AppendSettingsString(stringBuilder, "Owa", this.OwaThrottlingPolicyStateSettings);
			LegacyThrottlingPolicy.AppendSettingsString(stringBuilder, "Pop", this.PopThrottlingPolicyStateSettings);
			LegacyThrottlingPolicy.AppendSettingsString(stringBuilder, "Imap", this.ImapThrottlingPolicyStateSettings);
			LegacyThrottlingPolicy.AppendSettingsString(stringBuilder, "PowerShell", this.PowershellThrottlingPolicyStateSettings);
			LegacyThrottlingPolicy.AppendSettingsString(stringBuilder, "Rca", this.RcaThrottlingPolicyStateSettings);
			LegacyThrottlingPolicy.AppendSettingsString(stringBuilder, "General", this.GeneralThrottlingPolicyStateSettings);
			return stringBuilder.ToString();
		}

		internal void CloneThrottlingSettingsTo(LegacyThrottlingPolicy policy)
		{
			policy.AnonymousThrottlingPolicyStateSettings = this.AnonymousThrottlingPolicyStateSettings;
			policy.EasThrottlingPolicyStateSettings = this.EasThrottlingPolicyStateSettings;
			policy.EwsThrottlingPolicyStateSettings = this.EwsThrottlingPolicyStateSettings;
			policy.ImapThrottlingPolicyStateSettings = this.ImapThrottlingPolicyStateSettings;
			policy.OwaThrottlingPolicyStateSettings = this.OwaThrottlingPolicyStateSettings;
			policy.PopThrottlingPolicyStateSettings = this.PopThrottlingPolicyStateSettings;
			policy.PowershellThrottlingPolicyStateSettings = this.PowershellThrottlingPolicyStateSettings;
			policy.RcaThrottlingPolicyStateSettings = this.RcaThrottlingPolicyStateSettings;
			policy.GeneralThrottlingPolicyStateSettings = this.GeneralThrottlingPolicyStateSettings;
			if (policy.Session == null)
			{
				policy.m_Session = base.Session;
			}
		}

		internal void UpgradeThrottlingSettingsTo(ThrottlingPolicy policy)
		{
			foreach (KeyValuePair<ADPropertyDefinition, ADPropertyDefinition> keyValuePair in LegacyThrottlingPolicy.CommonThrottlingPolicySettingMapping)
			{
				this.CopyIfValueExists(policy, keyValuePair.Key, keyValuePair.Value);
			}
		}

		private void CopyIfValueExists(ThrottlingPolicy policy, ADPropertyDefinition property, ADPropertyDefinition legacyProperty)
		{
			string text = (string)this[legacyProperty];
			if (!string.IsNullOrEmpty(text))
			{
				Unlimited<uint> unlimited = ThrottlingPolicyBaseSettings.ParseValue(text);
				policy[property] = unlimited;
			}
		}

		private static readonly Dictionary<ADPropertyDefinition, ADPropertyDefinition> CommonThrottlingPolicySettingMapping = new Dictionary<ADPropertyDefinition, ADPropertyDefinition>
		{
			{
				ThrottlingPolicySchema.AnonymousMaxConcurrency,
				LegacyThrottlingPolicySchema.AnonymousMaxConcurrency
			},
			{
				ThrottlingPolicySchema.EasMaxConcurrency,
				LegacyThrottlingPolicySchema.EasMaxConcurrency
			},
			{
				ThrottlingPolicySchema.EasMaxDevices,
				LegacyThrottlingPolicySchema.EasMaxDevices
			},
			{
				ThrottlingPolicySchema.EasMaxDeviceDeletesPerMonth,
				LegacyThrottlingPolicySchema.EasMaxDeviceDeletesPerMonth
			},
			{
				ThrottlingPolicySchema.EwsMaxConcurrency,
				LegacyThrottlingPolicySchema.EwsMaxConcurrency
			},
			{
				ThrottlingPolicySchema.EwsMaxSubscriptions,
				LegacyThrottlingPolicySchema.EwsMaxSubscriptions
			},
			{
				ThrottlingPolicySchema.ImapMaxConcurrency,
				LegacyThrottlingPolicySchema.ImapMaxConcurrency
			},
			{
				ThrottlingPolicySchema.OwaMaxConcurrency,
				LegacyThrottlingPolicySchema.OwaMaxConcurrency
			},
			{
				ThrottlingPolicySchema.PopMaxConcurrency,
				LegacyThrottlingPolicySchema.PopMaxConcurrency
			},
			{
				ThrottlingPolicySchema.PowerShellMaxConcurrency,
				LegacyThrottlingPolicySchema.PowershellMaxConcurrency
			},
			{
				ThrottlingPolicySchema.PowerShellMaxTenantConcurrency,
				LegacyThrottlingPolicySchema.PowershellMaxTenantConcurrency
			},
			{
				ThrottlingPolicySchema.PowerShellMaxCmdlets,
				LegacyThrottlingPolicySchema.PowerShellMaxCmdlets
			},
			{
				ThrottlingPolicySchema.PowerShellMaxCmdletsTimePeriod,
				LegacyThrottlingPolicySchema.PowershellMaxCmdletsTimePeriod
			},
			{
				ThrottlingPolicySchema.PowerShellMaxCmdletQueueDepth,
				LegacyThrottlingPolicySchema.PowershellMaxCmdletQueueDepth
			},
			{
				ThrottlingPolicySchema.ExchangeMaxCmdlets,
				LegacyThrottlingPolicySchema.ExchangeMaxCmdlets
			},
			{
				ThrottlingPolicySchema.PowerShellMaxDestructiveCmdlets,
				LegacyThrottlingPolicySchema.PowershellMaxDestructiveCmdlets
			},
			{
				ThrottlingPolicySchema.PowerShellMaxDestructiveCmdletsTimePeriod,
				LegacyThrottlingPolicySchema.PowershellMaxDestructiveCmdletsTimePeriod
			},
			{
				ThrottlingPolicySchema.RcaMaxConcurrency,
				LegacyThrottlingPolicySchema.RcaMaxConcurrency
			},
			{
				ThrottlingPolicySchema.CpaMaxConcurrency,
				LegacyThrottlingPolicySchema.CpaMaxConcurrency
			},
			{
				ThrottlingPolicySchema.MessageRateLimit,
				LegacyThrottlingPolicySchema.MessageRateLimit
			},
			{
				ThrottlingPolicySchema.RecipientRateLimit,
				LegacyThrottlingPolicySchema.RecipientRateLimit
			},
			{
				ThrottlingPolicySchema.ForwardeeLimit,
				LegacyThrottlingPolicySchema.ForwardeeLimit
			},
			{
				ThrottlingPolicySchema.DiscoveryMaxConcurrency,
				LegacyThrottlingPolicySchema.DiscoveryMaxConcurrency
			},
			{
				ThrottlingPolicySchema.DiscoveryMaxMailboxes,
				LegacyThrottlingPolicySchema.DiscoveryMaxMailboxes
			},
			{
				ThrottlingPolicySchema.DiscoveryMaxKeywords,
				LegacyThrottlingPolicySchema.DiscoveryMaxKeywords
			}
		};

		private static LegacyThrottlingPolicySchema schema = ObjectSchema.GetInstance<LegacyThrottlingPolicySchema>();

		private static string mostDerivedClass = "msExchThrottlingPolicy";
	}
}
