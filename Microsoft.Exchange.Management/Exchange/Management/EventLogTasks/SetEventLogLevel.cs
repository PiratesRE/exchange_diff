using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EventLogTasks
{
	[Cmdlet("Set", "eventloglevel", SupportsShouldProcess = true)]
	public class SetEventLogLevel : SetTaskBase<EventCategoryObject>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetEventLogLevel(this.Identity.ToString(), this.Level.ToString());
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public ECIdParameter Identity
		{
			get
			{
				return (ECIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public ExEventLog.EventLevel Level
		{
			get
			{
				return (ExEventLog.EventLevel)base.Fields["Level"];
			}
			set
			{
				base.Fields["Level"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			IConfigurable configurable = null;
			try
			{
				configurable = base.GetDataObject(this.Identity);
				this.StampChangesOn(configurable);
			}
			catch (SecurityException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, this.Identity);
			}
			catch (IOException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidData, this.Identity);
			}
			catch (DataSourceOperationException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidData, this.Identity);
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return configurable;
		}

		protected void StampChangesOn(IConfigurable dataObject)
		{
			EventCategoryObject eventCategoryObject = dataObject as EventCategoryObject;
			if (eventCategoryObject == null)
			{
				base.WriteError(new InvalidEventCategoryInputException(), ErrorCategory.InvalidData, this.Identity);
				return;
			}
			eventCategoryObject.EventLevel = this.Level;
		}

		protected override IConfigDataProvider CreateSession()
		{
			return new EventCategorySession((this.Identity != null) ? this.Identity.ToString() : null);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!this.Identity.IsUnique())
			{
				base.WriteError(new NonUniqueEventCategoryInputException(), ErrorCategory.InvalidData, this.Identity);
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 148, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\EventLog\\SetEventLogLevel.cs");
			EventCategoryIdentity eventCategoryIdentity = null;
			if (this.Identity.ToString() != null)
			{
				eventCategoryIdentity = EventCategoryIdentity.Parse(this.Identity.ToString());
			}
			Server obj;
			if (eventCategoryIdentity == null || string.IsNullOrEmpty(eventCategoryIdentity.Server))
			{
				obj = topologyConfigurationSession.FindLocalServer();
			}
			else
			{
				ServerIdParameter serverIdParameter = ServerIdParameter.Parse(eventCategoryIdentity.Server);
				obj = (Server)base.GetDataObject<Server>(serverIdParameter, topologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())));
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			MapiTaskHelper.VerifyIsWithinConfigWriteScope(sessionSettings, obj, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
		}
	}
}
