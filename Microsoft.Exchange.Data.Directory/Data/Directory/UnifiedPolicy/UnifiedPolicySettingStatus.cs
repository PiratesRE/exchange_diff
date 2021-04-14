using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.UnifiedPolicy
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class UnifiedPolicySettingStatus : ADConfigurationObject
	{
		public UnifiedPolicySettingStatus()
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return UnifiedPolicySettingStatus.mostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return UnifiedPolicySettingStatus.schema;
			}
		}

		public string SettingType
		{
			get
			{
				return this[UnifiedPolicySettingStatusSchema.SettingType] as string;
			}
			set
			{
				this[UnifiedPolicySettingStatusSchema.SettingType] = value;
			}
		}

		public Guid? ParentObjectId
		{
			get
			{
				return new Guid?((Guid)this[UnifiedPolicySettingStatusSchema.ParentObjectId]);
			}
			set
			{
				this[UnifiedPolicySettingStatusSchema.ParentObjectId] = value;
			}
		}

		public string Container
		{
			get
			{
				return this[UnifiedPolicySettingStatusSchema.Container] as string;
			}
			set
			{
				this[UnifiedPolicySettingStatusSchema.Container] = value;
			}
		}

		public Guid ObjectVersion
		{
			get
			{
				return (Guid)this[UnifiedPolicySettingStatusSchema.ObjectVersion];
			}
			set
			{
				this[UnifiedPolicySettingStatusSchema.ObjectVersion] = value;
			}
		}

		public int ErrorCode
		{
			get
			{
				return (int)this[UnifiedPolicySettingStatusSchema.ErrorCode];
			}
			set
			{
				this[UnifiedPolicySettingStatusSchema.ErrorCode] = value;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this[UnifiedPolicySettingStatusSchema.ErrorMessage] as string;
			}
			set
			{
				this[UnifiedPolicySettingStatusSchema.ErrorMessage] = value;
			}
		}

		public DateTime WhenProcessedUTC
		{
			get
			{
				return (DateTime)this[UnifiedPolicySettingStatusSchema.WhenProcessedUTC];
			}
			set
			{
				this[UnifiedPolicySettingStatusSchema.WhenProcessedUTC] = value;
			}
		}

		internal StatusMode ObjectStatus
		{
			get
			{
				return (StatusMode)this[UnifiedPolicySettingStatusSchema.ObjectStatus];
			}
			set
			{
				this[UnifiedPolicySettingStatusSchema.ObjectStatus] = value;
			}
		}

		public string AdditionalDiagnostics
		{
			get
			{
				return (string)this[UnifiedPolicySettingStatusSchema.AdditionalDiagnostics];
			}
			set
			{
				this[UnifiedPolicySettingStatusSchema.AdditionalDiagnostics] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		private static readonly UnifiedPolicySettingStatusSchema schema = ObjectSchema.GetInstance<UnifiedPolicySettingStatusSchema>();

		private static string mostDerivedClass = "msExchUnifiedPolicySettingStatus";
	}
}
