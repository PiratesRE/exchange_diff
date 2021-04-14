using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.Security.Permissions;
using System.ServiceModel;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.PowerShell.RbacHostingTools.Asp.Net;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class DataSourceService
	{
		protected DataSourceService() : this(DataSourceService.UserRunspaces)
		{
		}

		protected DataSourceService(RunspaceMediator runspaceMediator)
		{
			this.runspaceMediator = runspaceMediator;
		}

		protected PowerShellResults<L> GetList<L, F>(string getCmdlet, F filter, SortOptions sort, string defaultSortProperty) where F : WebServiceParameters, new()
		{
			PSCommand psCommand = new PSCommand().AddCommand(getCmdlet);
			return this.GetList<L, F>(psCommand, filter, sort, defaultSortProperty);
		}

		protected PowerShellResults<L> GetList<L, F>(PSCommand psCommand, F filter, SortOptions sort, string defaultSortProperty) where F : WebServiceParameters, new()
		{
			if (string.IsNullOrEmpty(defaultSortProperty))
			{
				throw new FaultException(new ArgumentNullException("defaultSortProperty").Message);
			}
			if (sort == null)
			{
				sort = new SortOptions();
				sort.Direction = SortDirection.Ascending;
				sort.PropertyName = defaultSortProperty;
			}
			return this.GetList<L, F>(psCommand, filter, sort);
		}

		protected PowerShellResults<L> GetList<L, F>(string getCmdlet, F filter, SortOptions sort) where F : WebServiceParameters, new()
		{
			PSCommand psCommand = new PSCommand().AddCommand(getCmdlet);
			return this.GetList<L, F>(psCommand, filter, sort);
		}

		protected PowerShellResults<L> GetList<L, F>(PSCommand psCommand, F filter, SortOptions sort) where F : WebServiceParameters, new()
		{
			EcpPerfCounters.WebServiceGetList.Increment();
			Func<L[], L[]> func = (sort != null) ? sort.GetSortFunction<L>() : null;
			F f;
			if ((f = filter) == null)
			{
				f = Activator.CreateInstance<F>();
			}
			filter = f;
			PowerShellResults<L> powerShellResults = this.CoreInvoke<L>(psCommand, null, null, filter);
			if (func != null)
			{
				powerShellResults.Output = func(powerShellResults.Output);
			}
			ResultSizeFilter resultSizeFilter = filter as ResultSizeFilter;
			if (resultSizeFilter != null && powerShellResults.HasWarnings)
			{
				for (int i = 0; i < powerShellResults.Warnings.Length; i++)
				{
					if (powerShellResults.Warnings[i] == Strings.WarningMoreResultsAvailable || powerShellResults.Warnings[i] == Strings.WarningDefaultResultSizeReached(resultSizeFilter.ResultSize.ToString()))
					{
						powerShellResults.Warnings[i] = ClientStrings.ListViewMoreResultsWarning;
					}
				}
			}
			return powerShellResults;
		}

		protected PowerShellResults<O> GetObject<O>(string getCmdlet, Identity identity)
		{
			return this.GetObject<O>(new PSCommand().AddCommand(getCmdlet), identity);
		}

		protected PowerShellResults<L> GetObjectForList<L>(string getCmdlet, Identity identity)
		{
			return this.GetObject<L>(getCmdlet, identity);
		}

		protected PowerShellResults<L> GetObjectForList<L>(PSCommand psCommand, Identity identity)
		{
			return this.GetObject<L>(psCommand, identity);
		}

		protected PowerShellResults<O> GetObject<O>(PSCommand psCommand, Identity identity)
		{
			EcpPerfCounters.WebServiceGetObject.Increment();
			identity.FaultIfNull();
			PowerShellResults<O> powerShellResults = this.CoreInvoke<O>(psCommand, identity.ToPipelineInput(), identity, null);
			if (powerShellResults.Output.Length > 1)
			{
				throw new SecurityException(Strings.ErrorManagementObjectAmbiguous(identity.DisplayName));
			}
			return powerShellResults;
		}

		protected PowerShellResults<O> GetObject<O>(string getCmdlet)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand(getCmdlet);
			return this.GetObject<O>(pscommand);
		}

		protected PowerShellResults<O> GetObject<O>(PSCommand psCommand)
		{
			EcpPerfCounters.WebServiceGetObject.Increment();
			return psCommand.Invoke(this.runspaceMediator, null, null);
		}

		protected PowerShellResults Invoke(PSCommand psCommand)
		{
			return this.Invoke(psCommand, Identity.FromExecutingUserId(), null);
		}

		protected PowerShellResults InvokeAsync(PSCommand psCommand, Action<PowerShellResults> onCompleted)
		{
			return AsyncServiceManager.InvokeAsync(() => this.Invoke(psCommand), onCompleted, RbacPrincipal.Current.UniqueName, AsyncTaskType.Default, psCommand.ToTraceString());
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "*")]
		public PowerShellResults<JsonDictionary<object>> GetProgress(string progressId)
		{
			return AsyncServiceManager.GetProgress(progressId);
		}

		protected PowerShellResults Invoke(PSCommand psCommand, Identity translationIdentity, WebServiceParameters parameters)
		{
			if (null == translationIdentity)
			{
				throw new ArgumentNullException("translationIdentity");
			}
			return this.CoreInvoke(psCommand, null, translationIdentity, parameters);
		}

		protected PowerShellResults Invoke(PSCommand psCommand, Identity[] identities, WebServiceParameters parameters)
		{
			identities.FaultIfNullOrEmpty();
			Identity translationIdentity = (identities.Length == 1) ? identities[0] : null;
			return this.CoreInvoke(psCommand, identities.ToIdParameters(), translationIdentity, parameters);
		}

		protected PowerShellResults<O> InvokeAndGetObject<O>(PSCommand psCommand, Identity[] identities, WebServiceParameters parameters) where O : BaseRow
		{
			PowerShellResults<O> powerShellResults = new PowerShellResults<O>();
			powerShellResults.MergeErrors(this.Invoke(psCommand, identities, parameters));
			if (powerShellResults.Succeeded && identities.Length <= 5)
			{
				IGetObjectForListService<O> getObjectForListService = this as IGetObjectForListService<O>;
				Func<Identity, PowerShellResults<O>> func;
				if (getObjectForListService != null)
				{
					func = ((Identity x) => getObjectForListService.GetObjectForList(x));
				}
				else
				{
					IGetObjectService<O> getObjectService = this as IGetObjectService<O>;
					if (getObjectService == null)
					{
						throw new Exception("Either IGetObjectForListService or IGetObjectService must be implemented for single row refresh.");
					}
					func = ((Identity x) => getObjectService.GetObject(x));
				}
				PowerShellResults<O> powerShellResults2 = new PowerShellResults<O>();
				try
				{
					for (int i = 0; i < identities.Length; i++)
					{
						powerShellResults2.MergeAll(func(identities[i]));
						if (powerShellResults2.Failed)
						{
							break;
						}
					}
				}
				catch (SecurityException)
				{
					if (powerShellResults2.HasValue)
					{
						throw;
					}
				}
				if (powerShellResults2.SucceededWithValue)
				{
					powerShellResults.MergeAll(powerShellResults2);
				}
			}
			return powerShellResults;
		}

		protected PowerShellResults<O> Invoke<O>(PSCommand psCommand)
		{
			return this.CoreInvoke<O>(psCommand, null, null, null);
		}

		protected PowerShellResults<L> NewObject<L, C>(string newCmdlet, C properties) where C : WebServiceParameters
		{
			EcpPerfCounters.WebServiceNewObject.Increment();
			properties.FaultIfNull();
			return this.CoreInvoke<L>(new PSCommand().AddCommand(newCmdlet), null, null, properties);
		}

		protected PowerShellResults RemoveObjects(string removeCmdlet, Identity identity, Identity[] identities, string parameterNameForIdentities, WebServiceParameters parameters)
		{
			EcpPerfCounters.WebServiceRemoveObject.Increment();
			PowerShellResults powerShellResults = new PowerShellResults();
			foreach (Identity identity2 in identities)
			{
				PSCommand psCommand = new PSCommand().AddCommand(removeCmdlet).AddParameter("Identity", identity).AddParameter(parameterNameForIdentities, identity2.RawIdentity);
				powerShellResults.MergeErrors(this.Invoke(psCommand, identity, parameters));
			}
			return powerShellResults;
		}

		protected PowerShellResults RemoveObjects(string removeCmdlet, Identity[] identities, WebServiceParameters parameters)
		{
			return this.RemoveObjects(new PSCommand().AddCommand(removeCmdlet), identities, parameters);
		}

		protected PowerShellResults RemoveObjects(PSCommand psCommand, Identity[] identities, WebServiceParameters parameters)
		{
			EcpPerfCounters.WebServiceRemoveObject.Increment();
			return this.Invoke(psCommand, identities, parameters);
		}

		protected PowerShellResults<O> SetObject<O, U>(string setCmdlet, Identity identity, U properties) where U : SetObjectProperties
		{
			return this.SetObject<O, U, O>(setCmdlet, identity, properties, identity);
		}

		protected PowerShellResults<L> SetObject<O, U, L>(string setCmdlet, Identity identity, U properties) where O : L where U : SetObjectProperties
		{
			return this.SetObject<O, U, L>(setCmdlet, identity, properties, identity);
		}

		protected PowerShellResults<L> SetObject<O, U, L>(string setCmdlet, Identity identity, U properties, Identity identityForGetCmdlet) where O : L where U : SetObjectProperties
		{
			EcpPerfCounters.WebServiceSetObject.Increment();
			identity.FaultIfNull();
			properties.FaultIfNull();
			PowerShellResults<L> powerShellResults = new PowerShellResults<L>();
			properties.IgnoreNullOrEmpty = false;
			if (properties.Any<KeyValuePair<string, object>>())
			{
				powerShellResults = this.CoreInvoke<L>(new PSCommand().AddCommand(setCmdlet), identity.ToPipelineInput(), identity, properties);
			}
			if (powerShellResults.Succeeded && null != identityForGetCmdlet)
			{
				PowerShellResults<L> powerShellResults2 = null;
				if (properties.ReturnObjectType == ReturnObjectTypes.Full && this is IGetObjectService<O>)
				{
					IGetObjectService<O> getObjectService = this as IGetObjectService<O>;
					PowerShellResults<O> @object = getObjectService.GetObject(identityForGetCmdlet);
					powerShellResults2 = new PowerShellResults<L>();
					powerShellResults2.MergeOutput(@object.Output.Cast<L>().ToArray<L>());
					powerShellResults2.MergeErrors<O>(@object);
				}
				else if (properties.ReturnObjectType == ReturnObjectTypes.PartialForList && this is IGetObjectForListService<L>)
				{
					IGetObjectForListService<L> getObjectForListService = this as IGetObjectForListService<L>;
					powerShellResults2 = getObjectForListService.GetObjectForList(identityForGetCmdlet);
				}
				if (powerShellResults2 != null)
				{
					powerShellResults.MergeAll(powerShellResults2);
				}
			}
			return powerShellResults;
		}

		private PowerShellResults CoreInvoke(PSCommand psCommand, IEnumerable pipelineInput, Identity translationIdentity, WebServiceParameters parameters)
		{
			return new PowerShellResults(this.CoreInvoke<PSObject>(psCommand, pipelineInput, translationIdentity, parameters));
		}

		private PowerShellResults<T> CoreInvoke<T>(PSCommand psCommand, IEnumerable pipelineInput, Identity translationIdentity, WebServiceParameters parameters)
		{
			PowerShellResults<T> powerShellResults = psCommand.Invoke(this.runspaceMediator, pipelineInput, parameters);
			powerShellResults.TranslationIdentity = translationIdentity;
			return powerShellResults;
		}

		private const int SingleRowRefreshThreshold = 5;

		public static RunspaceMediator UserRunspaces = new RunspaceMediator(new EcpRunspaceFactory(new RbacInitialSessionStateFactory(), EcpHost.Factory), new EcpRunspaceCache());

		private RunspaceMediator runspaceMediator;
	}
}
