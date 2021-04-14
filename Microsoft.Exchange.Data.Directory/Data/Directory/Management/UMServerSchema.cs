using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class UMServerSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ActiveDirectoryServerSchema>();
		}

		public static readonly ADPropertyDefinition Status = ServerSchema.Status;

		public static readonly ADPropertyDefinition Languages = ServerSchema.Languages;

		public static readonly ADPropertyDefinition MaxCallsAllowed = ServerSchema.MaxCallsAllowed;

		public static readonly ADPropertyDefinition DialPlans = ServerSchema.DialPlans;

		public static readonly ADPropertyDefinition GrammarGenerationSchedule = ServerSchema.GrammarGenerationSchedule;

		public static readonly ADPropertyDefinition ExternalHostFqdn = ServerSchema.ExternalHostFqdn;

		public static readonly ADPropertyDefinition SipTcpListeningPort = ActiveDirectoryServerSchema.SipTcpListeningPort;

		public static readonly ADPropertyDefinition SipTlsListeningPort = ActiveDirectoryServerSchema.SipTlsListeningPort;

		public static readonly ADPropertyDefinition ExternalServiceFqdn = ActiveDirectoryServerSchema.ExternalServiceFqdn;

		public static readonly ADPropertyDefinition UMPodRedirectTemplate = ServerSchema.UMPodRedirectTemplate;

		public static readonly ADPropertyDefinition UMForwardingAddressTemplate = ServerSchema.UMForwardingAddressTemplate;

		public static readonly ADPropertyDefinition UMStartupMode = ServerSchema.UMStartupMode;

		public static readonly ADPropertyDefinition UMCertificateThumbprint = ServerSchema.UMCertificateThumbprint;

		public static readonly ADPropertyDefinition SIPAccessService = ServerSchema.SIPAccessService;

		public static readonly ADPropertyDefinition IrmLogPath = ServerSchema.IrmLogPath;

		public static readonly ADPropertyDefinition IrmLogMaxAge = ServerSchema.IrmLogMaxAge;

		public static readonly ADPropertyDefinition IrmLogMaxDirectorySize = ServerSchema.IrmLogMaxDirectorySize;

		public static readonly ADPropertyDefinition IrmLogMaxFileSize = ServerSchema.IrmLogMaxFileSize;

		public static readonly ADPropertyDefinition IrmLogEnabled = ServerSchema.IrmLogEnabled;

		public static readonly ADPropertyDefinition IPAddressFamilyConfigurable = ActiveDirectoryServerSchema.IPAddressFamilyConfigurable;

		public static readonly ADPropertyDefinition IPAddressFamily = ActiveDirectoryServerSchema.IPAddressFamily;
	}
}
