using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class PolicyConfiguration : ConfigurationBase
	{
		private PolicyConfiguration()
		{
		}

		internal static PolicyConfiguration GetPolicyConfigurationFromAD(IConfigurationSession session, ADObjectId owaMailboxPolicyId)
		{
			OwaMailboxPolicy[] array = session.Find<OwaMailboxPolicy>(owaMailboxPolicyId, QueryScope.Base, null, null, 1);
			OwaMailboxPolicy owaMailboxPolicy = null;
			if (array != null && array.Length == 1)
			{
				owaMailboxPolicy = array[0];
			}
			if (owaMailboxPolicy == null)
			{
				ExTraceGlobals.PolicyConfigurationTracer.TraceError<ADObjectId>(0L, LocalizedStrings.GetNonEncoded(246332502), owaMailboxPolicyId);
				return null;
			}
			PolicyConfiguration policyConfiguration = new PolicyConfiguration();
			policyConfiguration.PhoneticSupportEnabled = owaMailboxPolicy.PhoneticSupportEnabled;
			AttachmentPolicy.Level treatUnknownTypeAs = ConfigurationBase.AttachmentActionToLevel(new AttachmentBlockingActions?(owaMailboxPolicy.ActionForUnknownFileAndMIMETypes));
			AttachmentPolicy attachmentPolicy = new AttachmentPolicy(owaMailboxPolicy.BlockedFileTypes.ToArray(), owaMailboxPolicy.BlockedMimeTypes.ToArray(), owaMailboxPolicy.ForceSaveFileTypes.ToArray(), owaMailboxPolicy.ForceSaveMimeTypes.ToArray(), owaMailboxPolicy.AllowedFileTypes.ToArray(), owaMailboxPolicy.AllowedMimeTypes.ToArray(), treatUnknownTypeAs, owaMailboxPolicy.DirectFileAccessOnPublicComputersEnabled, owaMailboxPolicy.DirectFileAccessOnPrivateComputersEnabled, owaMailboxPolicy.ForceWebReadyDocumentViewingFirstOnPublicComputers, owaMailboxPolicy.ForceWebReadyDocumentViewingFirstOnPrivateComputers, owaMailboxPolicy.WebReadyDocumentViewingOnPublicComputersEnabled, owaMailboxPolicy.WebReadyDocumentViewingOnPrivateComputersEnabled, owaMailboxPolicy.WebReadyFileTypes.ToArray(), owaMailboxPolicy.WebReadyMimeTypes.ToArray(), owaMailboxPolicy.WebReadyDocumentViewingSupportedFileTypes.ToArray(), owaMailboxPolicy.WebReadyDocumentViewingSupportedMimeTypes.ToArray(), owaMailboxPolicy.WebReadyDocumentViewingForAllSupportedTypes);
			policyConfiguration.AttachmentPolicy = attachmentPolicy;
			policyConfiguration.DefaultClientLanguage = owaMailboxPolicy.DefaultClientLanguage;
			policyConfiguration.LogonAndErrorLanguage = owaMailboxPolicy.LogonAndErrorLanguage;
			policyConfiguration.DefaultTheme = owaMailboxPolicy.DefaultTheme;
			policyConfiguration.SetPhotoURL = owaMailboxPolicy.SetPhotoURL;
			int segmentationBits = (int)owaMailboxPolicy[OwaMailboxPolicySchema.ADMailboxFolderSet];
			int segmentationBits2 = (int)owaMailboxPolicy[OwaMailboxPolicySchema.ADMailboxFolderSet2];
			policyConfiguration.SegmentationFlags = Utilities.SetSegmentationFlags(segmentationBits, segmentationBits2);
			policyConfiguration.OutboundCharset = owaMailboxPolicy.OutboundCharset;
			policyConfiguration.UseGB18030 = owaMailboxPolicy.UseGB18030;
			policyConfiguration.UseISO885915 = owaMailboxPolicy.UseISO885915;
			policyConfiguration.InstantMessagingType = ((owaMailboxPolicy.InstantMessagingType != null) ? owaMailboxPolicy.InstantMessagingType.Value : InstantMessagingTypeOptions.None);
			return policyConfiguration;
		}
	}
}
