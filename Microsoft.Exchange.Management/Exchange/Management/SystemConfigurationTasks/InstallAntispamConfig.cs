using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class InstallAntispamConfig<TDataObject> : NewFixedNameSystemConfigurationObjectTask<TDataObject> where TDataObject : MessageHygieneAgentConfig, new()
	{
		protected abstract string CanonicalName { get; }

		protected override IConfigurable PrepareDataObject()
		{
			TDataObject tdataObject = (TDataObject)((object)base.PrepareDataObject());
			tdataObject.SetId((IConfigurationSession)base.DataSession, this.CanonicalName);
			return tdataObject;
		}

		protected override void InternalProcessRecord()
		{
			TDataObject[] array = this.ConfigurationSession.Find<TDataObject>(base.RootOrgContainerId, QueryScope.SubTree, null, null, 2);
			if (array == null || array.Length == 0)
			{
				base.InternalProcessRecord();
			}
		}
	}
}
