using System;
using System.ServiceModel;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class BulkEditUtility
	{
		public static string ConfirmationText
		{
			get
			{
				switch (BulkEditUtility.GetBulkEditPageId())
				{
				case BulkEditPageId.EnableOWA:
					return Strings.EnableOWAConfirmation;
				case BulkEditPageId.DisableOWA:
					return Strings.DisableOWAConfirmation;
				case BulkEditPageId.EnableEAS:
					return Strings.EnableEASConfirmation;
				case BulkEditPageId.DisableEAS:
					return Strings.DisableEASConfirmation;
				case BulkEditPageId.EnablePOP3:
					return Strings.EnablePOP3Confirmation;
				case BulkEditPageId.DisablePOP3:
					return Strings.DisablePOP3Confirmation;
				case BulkEditPageId.EnableIMAP:
					return Strings.EnableIMAPConfirmation;
				case BulkEditPageId.DisableIMAP:
					return Strings.DisableIMAPConfirmation;
				case BulkEditPageId.EnableMAPI:
					return Strings.EnableMAPIConfirmation;
				case BulkEditPageId.DisableMAPI:
					return Strings.DisableMAPIConfirmation;
				case BulkEditPageId.EnableArchiveDC:
					return Strings.EnableArchiveConfirmation;
				case BulkEditPageId.DisableArchive:
					return Strings.DisableArchiveConfirmation;
				default:
					return string.Empty;
				}
			}
		}

		public static string PageTitle
		{
			get
			{
				switch (BulkEditUtility.GetBulkEditPageId())
				{
				case BulkEditPageId.EnableOWA:
					return Strings.BulkEditEnableOWATitle;
				case BulkEditPageId.DisableOWA:
					return Strings.BulkEditDisableOWATitle;
				case BulkEditPageId.EnableEAS:
					return Strings.BulkEditEnableEASTitle;
				case BulkEditPageId.DisableEAS:
					return Strings.BulkEditDisableEASTitle;
				case BulkEditPageId.EnablePOP3:
					return Strings.BulkEditEnablePOP3Title;
				case BulkEditPageId.DisablePOP3:
					return Strings.BulkEditDisablePOP3Title;
				case BulkEditPageId.EnableIMAP:
					return Strings.BulkEditEnableIMAPTitle;
				case BulkEditPageId.DisableIMAP:
					return Strings.BulkEditDisableIMAPTitle;
				case BulkEditPageId.EnableMAPI:
					return Strings.BulkEditEnableMAPITitle;
				case BulkEditPageId.DisableMAPI:
					return Strings.BulkEditDisableMAPITitle;
				case BulkEditPageId.EnableArchiveDC:
					return Strings.BulkEditEnableArchiveTitle;
				case BulkEditPageId.DisableArchive:
					return Strings.BulkEditDisableArchiveTitle;
				case BulkEditPageId.AddDelegation:
					return Strings.BulkEditAddDelegationTitle;
				case BulkEditPageId.RemoveDelegation:
					return Strings.BulkEditRemoveDelegationTitle;
				default:
					return string.Empty;
				}
			}
		}

		public static string PageCaption
		{
			get
			{
				switch (BulkEditUtility.GetBulkEditPageId())
				{
				case BulkEditPageId.EnableOWA:
					return Strings.BulkEditEnableOWACaption;
				case BulkEditPageId.DisableOWA:
					return Strings.BulkEditDisableOWACaption;
				case BulkEditPageId.EnableEAS:
					return Strings.BulkEditEnableEASCaption;
				case BulkEditPageId.DisableEAS:
					return Strings.BulkEditDisableEASCaption;
				case BulkEditPageId.EnablePOP3:
					return Strings.BulkEditEnablePOP3Caption;
				case BulkEditPageId.DisablePOP3:
					return Strings.BulkEditDisablePOP3Caption;
				case BulkEditPageId.EnableIMAP:
					return Strings.BulkEditEnableIMAPCaption;
				case BulkEditPageId.DisableIMAP:
					return Strings.BulkEditDisableIMAPCaption;
				case BulkEditPageId.EnableMAPI:
					return Strings.BulkEditEnableMAPICaption;
				case BulkEditPageId.DisableMAPI:
					return Strings.BulkEditDisableMAPICaption;
				case BulkEditPageId.EnableArchiveDC:
					return Strings.BulkEditEnableArchiveCaption;
				case BulkEditPageId.DisableArchive:
					return Strings.BulkEditDisableArchiveCaption;
				case BulkEditPageId.AddDelegation:
					return Strings.BulkEditAddDelegationCaption;
				case BulkEditPageId.RemoveDelegation:
					return Strings.BulkEditRemoveDelegationCaption;
				default:
					return string.Empty;
				}
			}
		}

		public static string ServiceUrl
		{
			get
			{
				switch (BulkEditUtility.GetBulkEditPageId())
				{
				case BulkEditPageId.EnableOWA:
					return BulkEditUtility.ComposeServiceUrl("BulkEditMailbox", "BulkEnableOWAWorkFlow");
				case BulkEditPageId.DisableOWA:
					return BulkEditUtility.ComposeServiceUrl("BulkEditMailbox", "BulkDisableOWAWorkFlow");
				case BulkEditPageId.EnableEAS:
					return BulkEditUtility.ComposeServiceUrl("BulkEditMailbox", "BulkEnableEASWorkFlow");
				case BulkEditPageId.DisableEAS:
					return BulkEditUtility.ComposeServiceUrl("BulkEditMailbox", "BulkDisableEASWorkFlow");
				case BulkEditPageId.EnablePOP3:
					return BulkEditUtility.ComposeServiceUrl("BulkEditMailbox", "BulkEnablePOP3WorkFlow");
				case BulkEditPageId.DisablePOP3:
					return BulkEditUtility.ComposeServiceUrl("BulkEditMailbox", "BulkDisablePOP3WorkFlow");
				case BulkEditPageId.EnableIMAP:
					return BulkEditUtility.ComposeServiceUrl("BulkEditMailbox", "BulkEnableIMAPWorkFlow");
				case BulkEditPageId.DisableIMAP:
					return BulkEditUtility.ComposeServiceUrl("BulkEditMailbox", "BulkDisableIMAPWorkFlow");
				case BulkEditPageId.EnableMAPI:
					return BulkEditUtility.ComposeServiceUrl("BulkEditMailbox", "BulkEnableMAPIWorkFlow");
				case BulkEditPageId.DisableMAPI:
					return BulkEditUtility.ComposeServiceUrl("BulkEditMailbox", "BulkDisableMAPIWorkFlow");
				case BulkEditPageId.EnableArchiveDC:
					return BulkEditUtility.ComposeServiceUrl("BulkEditMailbox", "BulkEnableArchiveDCWorkFlow");
				case BulkEditPageId.DisableArchive:
					return BulkEditUtility.ComposeServiceUrl("BulkEditMailbox", "BulkDisableArchiveWorkFlow");
				default:
					return string.Empty;
				}
			}
		}

		public static string ComposeServiceUrl(string schema, string workflow)
		{
			return "../DDI/DDIService.svc?schema=" + schema + "&Workflow=" + workflow;
		}

		private static BulkEditPageId GetBulkEditPageId()
		{
			string text = HttpContext.Current.Request.QueryString["functionaltype"];
			int num;
			BulkEditPageId result;
			if (!int.TryParse(text, out num) && Enum.TryParse<BulkEditPageId>(text, out result))
			{
				return result;
			}
			throw new FaultException("The query string functionaltype = " + HttpContext.Current.Request.QueryString["functionaltype"] + " could not be understood.");
		}
	}
}
