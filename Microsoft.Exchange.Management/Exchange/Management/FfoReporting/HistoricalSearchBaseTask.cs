using System;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.FfoReporting.Providers;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting
{
	public abstract class HistoricalSearchBaseTask : Task
	{
		protected HistoricalSearchBaseTask(string componentName, string assemblyType)
		{
			this.componentName = componentName;
			this.assemblyType = assemblyType;
			this.configDataProvider = new Lazy<IConfigDataProvider>(() => ServiceLocator.Current.GetService<IAuthenticationProvider>().CreateConfigSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId));
		}

		[Parameter(Mandatory = false, Position = 0)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		internal IConfigDataProvider ConfigSession
		{
			get
			{
				return this.configDataProvider.Value;
			}
		}

		protected sealed override void InternalProcessRecord()
		{
			SystemProbe.Trace(this.componentName, SystemProbe.Status.Pass, "Entering InternalProcessRecord", new object[0]);
			try
			{
				base.InternalProcessRecord();
				Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.ManagementHelper");
				Type type = assembly.GetType(this.assemblyType);
				MethodInfo method = type.GetMethod("InternalProcessRecordHelper", BindingFlags.Static | BindingFlags.Public, null, new Type[]
				{
					base.GetType()
				}, null);
				method.Invoke(null, new object[]
				{
					this
				});
			}
			catch (TargetInvocationException ex)
			{
				SystemProbe.Trace(this.componentName, SystemProbe.Status.Fail, "TargetInvocationException in InternalProcessRecord: {0}", new object[]
				{
					ex.ToString()
				});
				if (ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				throw;
			}
			catch (Exception ex2)
			{
				SystemProbe.Trace(this.componentName, SystemProbe.Status.Fail, "Unhandled Exception in InternalProcessRecord: {0}", new object[]
				{
					ex2.ToString()
				});
				throw;
			}
			SystemProbe.Trace(this.componentName, SystemProbe.Status.Pass, "Exiting InternalProcessRecord", new object[0]);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.Organization == null)
			{
				if (base.CurrentOrganizationId.OrganizationalUnit == null)
				{
					throw new ArgumentException(Strings.InvalidOrganization);
				}
				this.Organization = new OrganizationIdParameter(base.CurrentOrganizationId.OrganizationalUnit.Name);
			}
			ServiceLocator.Current.GetService<IAuthenticationProvider>().ResolveOrganizationId(this.Organization, this);
		}

		private const string helperAssemblyName = "Microsoft.Exchange.Hygiene.ManagementHelper";

		private readonly string componentName = string.Empty;

		private readonly string assemblyType = string.Empty;

		private Lazy<IConfigDataProvider> configDataProvider;
	}
}
