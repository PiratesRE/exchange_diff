using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public abstract class PopImapAdConfiguration : ADEmailTransport
	{
		public PopImapAdConfiguration()
		{
		}

		public abstract string ProtocolName { get; }

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPBinding> UnencryptedOrTLSBindings
		{
			get
			{
				return (MultiValuedProperty<IPBinding>)this[PopImapAdConfigurationSchema.UnencryptedOrTLSBindings];
			}
			set
			{
				this[PopImapAdConfigurationSchema.UnencryptedOrTLSBindings] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPBinding> SSLBindings
		{
			get
			{
				return (MultiValuedProperty<IPBinding>)this[PopImapAdConfigurationSchema.SSLBindings];
			}
			set
			{
				this[PopImapAdConfigurationSchema.SSLBindings] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ProtocolConnectionSettings> InternalConnectionSettings
		{
			get
			{
				return (MultiValuedProperty<ProtocolConnectionSettings>)this[PopImapAdConfigurationSchema.InternalConnectionSettings];
			}
			set
			{
				this[PopImapAdConfigurationSchema.InternalConnectionSettings] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ProtocolConnectionSettings> ExternalConnectionSettings
		{
			get
			{
				return (MultiValuedProperty<ProtocolConnectionSettings>)this[PopImapAdConfigurationSchema.ExternalConnectionSettings];
			}
			set
			{
				this[PopImapAdConfigurationSchema.ExternalConnectionSettings] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string X509CertificateName
		{
			get
			{
				return (string)this[PopImapAdConfigurationSchema.X509CertificateName];
			}
			set
			{
				this[PopImapAdConfigurationSchema.X509CertificateName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Banner
		{
			get
			{
				return (string)this[PopImapAdConfigurationSchema.Banner];
			}
			set
			{
				this[PopImapAdConfigurationSchema.Banner] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LoginOptions LoginType
		{
			get
			{
				return (LoginOptions)this[PopImapAdConfigurationSchema.LoginType];
			}
			set
			{
				this[PopImapAdConfigurationSchema.LoginType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan AuthenticatedConnectionTimeout
		{
			get
			{
				return (EnhancedTimeSpan)this[PopImapAdConfigurationSchema.AuthenticatedConnectionTimeout];
			}
			set
			{
				this[PopImapAdConfigurationSchema.AuthenticatedConnectionTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan PreAuthenticatedConnectionTimeout
		{
			get
			{
				return (EnhancedTimeSpan)this[PopImapAdConfigurationSchema.PreAuthenticatedConnectionTimeout];
			}
			set
			{
				this[PopImapAdConfigurationSchema.PreAuthenticatedConnectionTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxConnections
		{
			get
			{
				return (int)this[PopImapAdConfigurationSchema.MaxConnections];
			}
			set
			{
				this[PopImapAdConfigurationSchema.MaxConnections] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxConnectionFromSingleIP
		{
			get
			{
				return (int)this[PopImapAdConfigurationSchema.MaxConnectionFromSingleIP];
			}
			set
			{
				this[PopImapAdConfigurationSchema.MaxConnectionFromSingleIP] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxConnectionsPerUser
		{
			get
			{
				return (int)this[PopImapAdConfigurationSchema.MaxConnectionsPerUser];
			}
			set
			{
				this[PopImapAdConfigurationSchema.MaxConnectionsPerUser] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MimeTextFormat MessageRetrievalMimeFormat
		{
			get
			{
				return (MimeTextFormat)this[PopImapAdConfigurationSchema.MessageRetrievalMimeFormat];
			}
			set
			{
				this[PopImapAdConfigurationSchema.MessageRetrievalMimeFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int ProxyTargetPort
		{
			get
			{
				return (int)this[PopImapAdConfigurationSchema.ProxyTargetPort];
			}
			set
			{
				this[PopImapAdConfigurationSchema.ProxyTargetPort] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CalendarItemRetrievalOptions CalendarItemRetrievalOption
		{
			get
			{
				return (CalendarItemRetrievalOptions)this[PopImapAdConfigurationSchema.CalendarItemRetrievalOption];
			}
			set
			{
				this[PopImapAdConfigurationSchema.CalendarItemRetrievalOption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri OwaServerUrl
		{
			get
			{
				return (Uri)this[PopImapAdConfigurationSchema.OwaServerUrl];
			}
			set
			{
				this[PopImapAdConfigurationSchema.OwaServerUrl] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EnableExactRFC822Size
		{
			get
			{
				return (bool)this[PopImapAdConfigurationSchema.EnableExactRFC822Size];
			}
			set
			{
				this[PopImapAdConfigurationSchema.EnableExactRFC822Size] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LiveIdBasicAuthReplacement
		{
			get
			{
				return (bool)this[PopImapAdConfigurationSchema.LiveIdBasicAuthReplacement];
			}
			set
			{
				this[PopImapAdConfigurationSchema.LiveIdBasicAuthReplacement] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SuppressReadReceipt
		{
			get
			{
				return (bool)this[PopImapAdConfigurationSchema.SuppressReadReceipt];
			}
			set
			{
				this[PopImapAdConfigurationSchema.SuppressReadReceipt] = value;
			}
		}

		public abstract int MaxCommandSize { get; set; }

		[Parameter(Mandatory = false)]
		public bool ProtocolLogEnabled
		{
			get
			{
				return (bool)this[PopImapAdConfigurationSchema.ProtocolLogEnabled];
			}
			set
			{
				this[PopImapAdConfigurationSchema.ProtocolLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EnforceCertificateErrors
		{
			get
			{
				return (bool)this[PopImapAdConfigurationSchema.EnforceCertificateErrors];
			}
			set
			{
				this[PopImapAdConfigurationSchema.EnforceCertificateErrors] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LogFileLocation
		{
			get
			{
				return (string)this[PopImapAdConfigurationSchema.LogFileLocation];
			}
			set
			{
				this[PopImapAdConfigurationSchema.LogFileLocation] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LogFileRollOver LogFileRollOverSettings
		{
			get
			{
				return (LogFileRollOver)this[PopImapAdConfigurationSchema.LogFileRollOverSettings];
			}
			set
			{
				this[PopImapAdConfigurationSchema.LogFileRollOverSettings] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> LogPerFileSizeQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[PopImapAdConfigurationSchema.LogPerFileSizeQuota];
			}
			set
			{
				this[PopImapAdConfigurationSchema.LogPerFileSizeQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExtendedProtectionTokenCheckingMode ExtendedProtectionPolicy
		{
			get
			{
				return (ExtendedProtectionTokenCheckingMode)this[PopImapAdConfigurationSchema.ExtendedProtectionPolicy];
			}
			set
			{
				this[PopImapAdConfigurationSchema.ExtendedProtectionPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EnableGSSAPIAndNTLMAuth
		{
			get
			{
				return !(bool)this[PopImapAdConfigurationSchema.EnableGSSAPIAndNTLMAuth];
			}
			set
			{
				this[PopImapAdConfigurationSchema.EnableGSSAPIAndNTLMAuth] = !value;
			}
		}

		internal static ExchangeObjectVersion MinimumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal static ObjectId GetRootId(Server server, string protocolName)
		{
			return server.Id.GetChildId("Protocols").GetChildId(protocolName);
		}

		internal static TResult FindOne<TResult>(ITopologyConfigurationSession session) where TResult : PopImapAdConfiguration, new()
		{
			Server server = session.FindLocalServer();
			return PopImapAdConfiguration.FindOne<TResult>(session, server);
		}

		internal static TResult FindOne<TResult>(ITopologyConfigurationSession session, string serverFqdn) where TResult : PopImapAdConfiguration, new()
		{
			Server server = session.FindServerByFqdn(serverFqdn);
			return PopImapAdConfiguration.FindOne<TResult>(session, server);
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal List<IPBinding> GetBindings()
		{
			List<IPBinding> list = new List<IPBinding>(this.UnencryptedOrTLSBindings.Count + this.SSLBindings.Count);
			list.AddRange(this.UnencryptedOrTLSBindings);
			list.AddRange(this.SSLBindings);
			return list;
		}

		internal string DisplayString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PropertyDefinition propertyDefinition in this.Schema.AllProperties)
			{
				object obj = this[propertyDefinition];
				if (obj is MultiValuedProperty<IPBinding>)
				{
					stringBuilder.AppendFormat("{0}: {{", propertyDefinition.Name);
					foreach (IPBinding arg in ((MultiValuedProperty<IPBinding>)obj))
					{
						stringBuilder.AppendFormat("{0}, ", arg);
					}
					stringBuilder.Append("}\r\n");
				}
				else if (obj is MultiValuedProperty<ProtocolConnectionSettings>)
				{
					stringBuilder.AppendFormat("{0}: {{", propertyDefinition.Name);
					foreach (ProtocolConnectionSettings arg2 in ((MultiValuedProperty<ProtocolConnectionSettings>)obj))
					{
						stringBuilder.AppendFormat("{0}, ", arg2);
					}
					stringBuilder.Append("}\r\n");
				}
				else
				{
					stringBuilder.AppendFormat("{0}: {1}\r\n", propertyDefinition.Name, obj);
				}
			}
			return stringBuilder.ToString();
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.CalendarItemRetrievalOption == CalendarItemRetrievalOptions.Custom && this.OwaServerUrl == null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExArgumentNullException(PopImapAdConfigurationSchema.OwaServerUrl.Name), PopImapAdConfigurationSchema.OwaServerUrl, this.OwaServerUrl));
			}
			if (!string.IsNullOrEmpty(this.X509CertificateName) && !PopImapAdConfiguration.IsValidProtocolCertificate(this.X509CertificateName))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InvalidCertificateName(this.X509CertificateName), PopImapAdConfigurationSchema.X509CertificateName, this.X509CertificateName));
			}
			if (!this.LogPerFileSizeQuota.IsUnlimited && this.LogPerFileSizeQuota.Value > ByteQuantifiedSize.Zero && this.LogPerFileSizeQuota.Value < ByteQuantifiedSize.FromMB(1UL))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExArgumentOutOfRangeException(PopImapAdConfigurationSchema.LogPerFileSizeQuota.Name, this.LogPerFileSizeQuota), PopImapAdConfigurationSchema.LogPerFileSizeQuota, this.LogPerFileSizeQuota));
			}
		}

		private static bool IsValidProtocolCertificate(string certificateName)
		{
			return Dns.IsValidName(certificateName);
		}

		private static TResult FindOne<TResult>(ITopologyConfigurationSession session, Server server) where TResult : PopImapAdConfiguration, new()
		{
			TResult tresult = Activator.CreateInstance<TResult>();
			string protocolName = tresult.ProtocolName;
			ObjectId rootId = PopImapAdConfiguration.GetRootId(server, protocolName);
			if (rootId == null)
			{
				return default(TResult);
			}
			TResult[] array = session.Find<TResult>(rootId as ADObjectId, QueryScope.OneLevel, null, null, 2);
			if (array == null || array.Length <= 0)
			{
				return default(TResult);
			}
			return array[0];
		}

		internal enum PopImapFlag
		{
			MessageRetrievalSortOrder,
			ShowHiddenFoldersEnabled = 0,
			EnableExactRFC822Size,
			LiveIdBasicAuthReplacement,
			SuppressReadReceipt = 4,
			EnableGSSAPIAndNTLMAuth = 8
		}
	}
}
