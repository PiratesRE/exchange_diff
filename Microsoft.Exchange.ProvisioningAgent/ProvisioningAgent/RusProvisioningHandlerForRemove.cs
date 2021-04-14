using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal sealed class RusProvisioningHandlerForRemove : RUSProvisioningHandler
	{
		private bool DisconnectParameter
		{
			get
			{
				return (SwitchParameter)(base.UserSpecifiedParameters["Disconnect"] ?? new SwitchParameter(false));
			}
		}

		private bool PermanentParameter
		{
			get
			{
				return (SwitchParameter)(base.UserSpecifiedParameters["Disconnect"] ?? new SwitchParameter(false));
			}
		}

		public override bool PreInternalProcessRecord(IConfigurable writeableIConfigurable)
		{
			if (writeableIConfigurable == null)
			{
				throw new ArgumentNullException("writeableIConfigurable");
			}
			ExTraceGlobals.RusTracer.TraceDebug<ObjectId, string>((long)this.GetHashCode(), "RUSProvisioningHandlerForRemove.PreInternalProcessRecord: writeableIConfigurable={0}, TaskName={1}", writeableIConfigurable.Identity, base.TaskName);
			ADRecipient adrecipient = (writeableIConfigurable is ADPresentationObject) ? (((ADPresentationObject)writeableIConfigurable).DataObject as ADRecipient) : (writeableIConfigurable as ADRecipient);
			if (adrecipient == null || adrecipient.OrganizationId == null || adrecipient.OrganizationId.ConfigurationUnit == null)
			{
				return false;
			}
			bool flag = Datacenter.IsMicrosoftHostedOnly(true) && (adrecipient.RecipientType == RecipientType.UserMailbox || adrecipient.RecipientType == RecipientType.MailUser) && !this.DisconnectParameter && !this.PermanentParameter;
			if (flag)
			{
				string text = (base.UserSpecifiedParameters["DomainController"] != null) ? base.UserSpecifiedParameters["DomainController"].ToString() : null;
				string text2 = adrecipient.OriginatingServer;
				if (string.IsNullOrEmpty(text2))
				{
					text2 = ((base.UserSpecifiedParameters["DomainController"] != null) ? base.UserSpecifiedParameters["DomainController"].ToString() : null);
				}
				Trace rusTracer = ExTraceGlobals.RusTracer;
				long id = (long)this.GetHashCode();
				string formatString = "RUSProvisioningHandlerForRemove.UpdateRecipient: recipient={0}, TaskName={1}, ConfigurationDomainController={2}, RecipientDomainController={3}, GlobalCatalog={4}";
				object[] array = new object[5];
				array[0] = adrecipient.Identity;
				array[1] = base.TaskName;
				array[2] = text;
				array[3] = text2;
				rusTracer.TraceDebug(id, formatString, array);
				base.LogMessage(Strings.VerboseUpdateRecipientObject(adrecipient.Id.ToString(), text ?? "<null>", text2 ?? "<null>", "<null>"));
				adrecipient.AddressListMembership.Clear();
				return true;
			}
			return false;
		}

		internal static readonly string[] SupportedTasks = new string[]
		{
			"Remove-Mailbox",
			"Remove-Syncmailbox",
			"Remove-MailUser",
			"Remove-SyncMailUser"
		};
	}
}
