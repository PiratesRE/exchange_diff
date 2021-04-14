using System;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal enum State
	{
		BeginAcquireTenantLicenses,
		AcquireTenantLicensesCallback,
		AcquireTenantLicensesFailed,
		BeginAcquireRMSTemplate,
		AcquireRMSTemplateCallback,
		AcquireRMSTemplateFailed,
		Encrypted,
		EncryptionFailed,
		UnexpectedExitAcquireTenantLicensesCallback,
		UnexpectedExitAcquireRmsTemplateCallback,
		UnexpectedExceptionInEncryption
	}
}
