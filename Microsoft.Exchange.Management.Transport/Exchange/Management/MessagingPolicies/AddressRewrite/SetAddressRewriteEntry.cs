using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MessagingPolicies.AddressRewrite
{
	[Cmdlet("set", "addressrewriteentry", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class SetAddressRewriteEntry : SetSystemConfigurationObjectTask<AddressRewriteEntryIdParameter, AddressRewriteEntry>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAddressrewriteentry(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.CreateSession();
			configurationSession.UseConfigNC = false;
			return configurationSession;
		}

		protected override ObjectId RootId
		{
			get
			{
				return Utils.RootId;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				AddressRewriteEntry dataObject = this.DataObject;
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				configurationSession.UseConfigNC = false;
				Utils.ValidateEntryAddresses(dataObject.InternalAddress, dataObject.ExternalAddress, dataObject.OutboundOnly, dataObject.ExceptionList, configurationSession, new Guid?(dataObject.Guid));
			}
			catch (ArgumentException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (FormatException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}
	}
}
