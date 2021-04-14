using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ADSpamScanSetting : ADObject
	{
		public ADSpamScanSetting()
		{
		}

		internal ADSpamScanSetting(IConfigurationSession session, string tenantId)
		{
			this.m_Session = session;
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal ADSpamScanSetting(string tenantId)
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		public override ObjectId Identity
		{
			get
			{
				return base.Id;
			}
		}

		public ObjectId ConfigurationId
		{
			get
			{
				return (ObjectId)this[ADSpamScanSettingSchema.ConfigurationIdProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.ConfigurationIdProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte ActionTypeId
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.ActionTypeIdProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.ActionTypeIdProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Parameter
		{
			get
			{
				return (string)this[ADSpamScanSettingSchema.ParameterProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.ParameterProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmImage
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmImageProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmImageProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmEmpty
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmEmptyProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmEmptyProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmScript
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmScriptProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmScriptProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmIframe
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmIframeProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmIframeProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmObject
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmObjectProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmObjectProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmEmbed
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmEmbedProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmEmbedProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmForm
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmFormProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmFormProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmWebBugs
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmWebBugsProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmWebBugsProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmWordList
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmWordListProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmWordListProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmUrlNumericIP
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmUrlNumericIPProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmUrlNumericIPProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmUrlRedirect
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmUrlRedirectProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmUrlRedirectProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmWebsite
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmWebsiteProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmWebsiteProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmSpfFail
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmSpfFailProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmSpfFailProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmSpfFromFail
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmSpfFromFailProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmSpfFromFailProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte CsfmNdrBackscatter
		{
			get
			{
				return (byte)this[ADSpamScanSettingSchema.CsfmNdrBackScatterProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmNdrBackScatterProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SpamScanFlags Flags
		{
			get
			{
				return (SpamScanFlags)this[ADSpamScanSettingSchema.FlagsProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.FlagsProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CsfmTestBccAddress
		{
			get
			{
				return (string)this[ADSpamScanSettingSchema.CsfmTestBccAddressProp];
			}
			set
			{
				this[ADSpamScanSettingSchema.CsfmTestBccAddressProp] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADSpamScanSetting.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADSpamScanSetting.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override bool ShouldValidatePropertyLinkInSameOrganization(ADPropertyDefinition property)
		{
			return false;
		}

		internal override void InitializeSchema()
		{
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
		}

		private static readonly ADSpamScanSettingSchema schema = ObjectSchema.GetInstance<ADSpamScanSettingSchema>();

		private static string mostDerivedClass = "ADSpamScanSetting";
	}
}
