using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationMexAgentsComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationMexAgentsComponent() : base("MexAgents")
		{
			base.Add(new VariantConfigurationSection("MexAgents.settings.ini", "TrustedMailAgents_CrossPremisesHeadersPreserved", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MexAgents.settings.ini", "TrustedMailAgents_AcceptAnyRecipientOnPremises", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MexAgents.settings.ini", "TrustedMailAgents_StampOriginatorOrgForMsitConnector", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MexAgents.settings.ini", "TrustedMailAgents_HandleCrossPremisesProbe", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MexAgents.settings.ini", "TrustedMailAgents_CheckOutboundDeliveryTypeSmtpConnector", typeof(IFeature), false));
		}

		public VariantConfigurationSection TrustedMailAgents_CrossPremisesHeadersPreserved
		{
			get
			{
				return base["TrustedMailAgents_CrossPremisesHeadersPreserved"];
			}
		}

		public VariantConfigurationSection TrustedMailAgents_AcceptAnyRecipientOnPremises
		{
			get
			{
				return base["TrustedMailAgents_AcceptAnyRecipientOnPremises"];
			}
		}

		public VariantConfigurationSection TrustedMailAgents_StampOriginatorOrgForMsitConnector
		{
			get
			{
				return base["TrustedMailAgents_StampOriginatorOrgForMsitConnector"];
			}
		}

		public VariantConfigurationSection TrustedMailAgents_HandleCrossPremisesProbe
		{
			get
			{
				return base["TrustedMailAgents_HandleCrossPremisesProbe"];
			}
		}

		public VariantConfigurationSection TrustedMailAgents_CheckOutboundDeliveryTypeSmtpConnector
		{
			get
			{
				return base["TrustedMailAgents_CheckOutboundDeliveryTypeSmtpConnector"];
			}
		}
	}
}
