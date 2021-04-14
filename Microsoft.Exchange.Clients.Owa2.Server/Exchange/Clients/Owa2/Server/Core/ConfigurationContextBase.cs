using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class ConfigurationContextBase
	{
		public virtual AttachmentPolicy AttachmentPolicy
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual WebBeaconFilterLevels FilterWebBeaconsAndHtmlForms
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsFeatureEnabled(Feature feature)
		{
			throw new NotImplementedException();
		}

		public virtual ulong GetFeaturesEnabled(Feature feature)
		{
			throw new NotImplementedException();
		}
	}
}
