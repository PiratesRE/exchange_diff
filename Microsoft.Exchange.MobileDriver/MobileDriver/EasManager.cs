using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class EasManager : IMobileServiceManager
	{
		public EasManager(EasSelector selector)
		{
			this.Selector = selector;
		}

		IMobileServiceSelector IMobileServiceManager.Selector
		{
			get
			{
				return this.Selector;
			}
		}

		public bool CapabilityPerRecipientSupported
		{
			get
			{
				return false;
			}
		}

		public EasSelector Selector { get; private set; }

		private static EasCapability Capability { get; set; } = new EasCapability(PartType.Short | PartType.Concatenated, 255, new CodingSupportability[]
		{
			new CodingSupportability(CodingScheme.GsmDefault, 160, 153),
			new CodingSupportability(CodingScheme.Unicode, 70, 67)
		}, FeatureSupportability.None);

		public EasCapability GetCapabilityForRecipient(MobileRecipient recipient)
		{
			if (recipient != null)
			{
				throw new ArgumentException("recipient");
			}
			return EasManager.Capability;
		}

		MobileServiceCapability IMobileServiceManager.GetCapabilityForRecipient(MobileRecipient recipient)
		{
			return this.GetCapabilityForRecipient(recipient);
		}
	}
}
