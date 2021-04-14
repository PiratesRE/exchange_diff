using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class GetObjectWithIdentityTaskBase<TIdentity, TDataObject> : GetTaskBase<TDataObject> where TIdentity : IIdentityParameter where TDataObject : IConfigurable, new()
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public virtual TIdentity Identity
		{
			get
			{
				return (TIdentity)((object)base.Fields["Identity"]);
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new GetTaskWithIdentityModuleFactory();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			if (this.Identity != null)
			{
				LocalizedString? localizedString;
				IEnumerable<TDataObject> dataObjects = base.GetDataObjects<TDataObject>(this.Identity, base.DataSession, this.RootId, base.OptionalIdentityData, out localizedString);
				dataObjects = this.RescopeSessionByResultObjects<TDataObject>(dataObjects);
				this.WriteResult<TDataObject>(dataObjects);
				if (!base.HasErrors && base.WriteObjectCount == 0U)
				{
					LocalizedString? localizedString2 = localizedString;
					LocalizedString message;
					if (localizedString2 == null)
					{
						TIdentity identity = this.Identity;
						message = base.GetErrorMessageObjectNotFound(identity.ToString(), typeof(TDataObject).ToString(), (base.DataSession != null) ? base.DataSession.Source : null);
					}
					else
					{
						message = localizedString2.GetValueOrDefault();
					}
					base.WriteError(new ManagementObjectNotFoundException(message), (ErrorCategory)1003, null);
				}
			}
			else
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private IEnumerable<TObject> RescopeSessionByResultObjects<TObject>(IEnumerable<TObject> dataObjects) where TObject : IConfigurable, new()
		{
			if (typeof(ADObject).IsAssignableFrom(typeof(TObject)) && base.DataSession is IDirectorySession)
			{
				List<TObject> list = new List<TObject>(dataObjects);
				if (list.Count > 0)
				{
					ADObject adobject = list[0] as ADObject;
					IDirectorySession directorySession = base.DataSession as IDirectorySession;
					if (directorySession != null && adobject != null && adobject.OrganizationId != null && TaskHelper.ShouldUnderscopeDataSessionToOrganization(directorySession, adobject))
					{
						base.UnderscopeDataSession(adobject.OrganizationId);
						base.CurrentOrganizationId = adobject.OrganizationId;
					}
				}
				dataObjects = list;
			}
			return dataObjects;
		}
	}
}
