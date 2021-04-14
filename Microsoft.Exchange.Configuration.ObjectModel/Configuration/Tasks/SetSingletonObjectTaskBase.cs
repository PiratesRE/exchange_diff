using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetSingletonObjectTaskBase<TDataObject> : SetObjectTaskBase<TDataObject, TDataObject> where TDataObject : IConfigurable, new()
	{
		protected virtual QueryFilter InternalFilter
		{
			get
			{
				return null;
			}
		}

		protected virtual bool DeepSearch
		{
			get
			{
				return false;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			TaskLogger.LogEnter();
			IConfigurable[] array = null;
			base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.DataSession, typeof(TDataObject), this.InternalFilter, this.RootId, this.DeepSearch));
			try
			{
				array = base.DataSession.Find<TDataObject>(this.InternalFilter, this.RootId, this.DeepSearch, null);
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, (ErrorCategory)1002, null);
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
			}
			if (array == null)
			{
				array = new IConfigurable[0];
			}
			IConfigurable result = null;
			switch (array.Length)
			{
			case 0:
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(null, typeof(TDataObject).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, null);
				break;
			case 1:
				result = array[0];
				break;
			default:
				base.WriteError(new ManagementObjectAmbiguousException(Strings.ExceptionObjectAmbiguous(typeof(TDataObject).ToString())), (ErrorCategory)1003, null);
				break;
			}
			TaskLogger.LogExit();
			return result;
		}
	}
}
