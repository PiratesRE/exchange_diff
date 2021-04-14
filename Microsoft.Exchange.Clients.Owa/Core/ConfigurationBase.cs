using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public abstract class ConfigurationBase
	{
		public AttachmentPolicy AttachmentPolicy
		{
			get
			{
				return this.attachmentPolicy;
			}
			protected set
			{
				this.attachmentPolicy = value;
			}
		}

		internal AuthenticationMethod InternalAuthenticationMethod
		{
			get
			{
				return this.internalAuthenticationMethod;
			}
			set
			{
				this.internalAuthenticationMethod = value;
			}
		}

		internal AuthenticationMethod ExternalAuthenticationMethod
		{
			get
			{
				return this.externalAuthenticationMethod;
			}
			set
			{
				this.externalAuthenticationMethod = value;
			}
		}

		internal Uri Exchange2003Url
		{
			get
			{
				return this.exchange2003Url;
			}
			set
			{
				this.exchange2003Url = value;
			}
		}

		internal LegacyRedirectTypeOptions LegacyRedirectType
		{
			get
			{
				return this.legacyRedirectType;
			}
			set
			{
				this.legacyRedirectType = value;
			}
		}

		public int DefaultClientLanguage
		{
			get
			{
				return this.defaultClientLanguage;
			}
			protected set
			{
				this.defaultClientLanguage = value;
			}
		}

		public int LogonAndErrorLanguage
		{
			get
			{
				return this.logonAndErrorLanguage;
			}
			protected set
			{
				this.logonAndErrorLanguage = value;
			}
		}

		public bool PhoneticSupportEnabled
		{
			get
			{
				return this.phoneticSupportEnabled;
			}
			protected set
			{
				this.phoneticSupportEnabled = value;
			}
		}

		public ulong SegmentationFlags
		{
			get
			{
				return this.segmentationFlags;
			}
			protected set
			{
				this.segmentationFlags = value;
			}
		}

		public string DefaultTheme
		{
			get
			{
				return this.defaultTheme;
			}
			protected set
			{
				this.defaultTheme = value;
			}
		}

		public string SetPhotoURL
		{
			get
			{
				return this.setPhotoURL;
			}
			protected set
			{
				this.setPhotoURL = value;
			}
		}

		public bool UseGB18030
		{
			get
			{
				return this.useGB18030;
			}
			protected set
			{
				this.useGB18030 = value;
			}
		}

		public bool UseISO885915
		{
			get
			{
				return this.useISO885915;
			}
			protected set
			{
				this.useISO885915 = value;
			}
		}

		public OutboundCharsetOptions OutboundCharset
		{
			get
			{
				return this.outboundCharset;
			}
			protected set
			{
				this.outboundCharset = value;
			}
		}

		public InstantMessagingTypeOptions InstantMessagingType
		{
			get
			{
				return this.instantMessagingType;
			}
			protected set
			{
				this.instantMessagingType = value;
			}
		}

		internal static AuthenticationMethod GetAuthenticationMethod(object authenticationMethodObject)
		{
			if (authenticationMethodObject is AuthenticationMethodFlags)
			{
				AuthenticationMethod authenticationMethod = (AuthenticationMethod)authenticationMethodObject;
				if (EnumValidator.IsValidValue<AuthenticationMethod>(authenticationMethod))
				{
					return authenticationMethod;
				}
			}
			return AuthenticationMethod.None;
		}

		protected static AttachmentPolicy.Level AttachmentActionToLevel(AttachmentBlockingActions? action)
		{
			AttachmentBlockingActions valueOrDefault = action.GetValueOrDefault();
			if (action != null)
			{
				switch (valueOrDefault)
				{
				case AttachmentBlockingActions.Allow:
					return AttachmentPolicy.Level.Allow;
				case AttachmentBlockingActions.ForceSave:
					return AttachmentPolicy.Level.ForceSave;
				case AttachmentBlockingActions.Block:
					return AttachmentPolicy.Level.Block;
				}
			}
			return AttachmentPolicy.Level.Block;
		}

		private AttachmentPolicy attachmentPolicy;

		private int defaultClientLanguage;

		private int logonAndErrorLanguage;

		private bool phoneticSupportEnabled;

		private string defaultTheme;

		private string setPhotoURL;

		private bool useGB18030;

		private bool useISO885915;

		private OutboundCharsetOptions outboundCharset = OutboundCharsetOptions.AutoDetect;

		private ulong segmentationFlags;

		private InstantMessagingTypeOptions instantMessagingType;

		private LegacyRedirectTypeOptions legacyRedirectType;

		private AuthenticationMethod internalAuthenticationMethod;

		private AuthenticationMethod externalAuthenticationMethod;

		private Uri exchange2003Url;
	}
}
