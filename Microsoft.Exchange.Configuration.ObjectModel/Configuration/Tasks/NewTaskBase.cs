using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class NewTaskBase<TDataObject> : SetTaskBase<TDataObject> where TDataObject : IConfigurable, new()
	{
		public NewTaskBase()
		{
			this.bindingInstance = ((default(TDataObject) == null) ? Activator.CreateInstance<TDataObject>() : default(TDataObject));
			this.InitializeDataObject(this.bindingInstance);
		}

		protected override TDataObject DataObject
		{
			get
			{
				if (base.Stage == TaskStage.ProcessRecord && base.DataObject != null)
				{
					return base.DataObject;
				}
				return this.bindingInstance;
			}
			set
			{
				if (base.Stage == TaskStage.ProcessRecord)
				{
					base.DataObject = value;
					return;
				}
				this.bindingInstance = value;
			}
		}

		protected virtual void InitializeDataObject(TDataObject dataObject)
		{
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			base.DataObject = ((default(TDataObject) == null) ? Activator.CreateInstance<TDataObject>() : default(TDataObject));
			if (base.IsProvisioningLayerAvailable)
			{
				ProvisioningLayer.ProvisionDefaultProperties(this, this.ConvertDataObjectToPresentationObject((default(TDataObject) == null) ? Activator.CreateInstance<TDataObject>() : default(TDataObject)), this.ConvertDataObjectToPresentationObject(base.DataObject), false);
				this.ValidateProvisionedProperties(base.DataObject);
			}
			this.InitializeDataObject(base.DataObject);
			TDataObject dataObject = base.DataObject;
			dataObject.CopyChangesFrom(this.bindingInstance);
		}

		protected virtual bool SkipWriteResult
		{
			get
			{
				return false;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			base.InternalProcessRecord();
			if (!base.HasErrors && !this.SkipWriteResult)
			{
				this.WriteResult();
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			return base.DataObject;
		}

		protected virtual void ValidateProvisionedProperties(IConfigurable dataObject)
		{
		}

		protected virtual void WriteResult()
		{
			object[] array = new object[1];
			object[] array2 = array;
			int num = 0;
			TDataObject dataObject = this.DataObject;
			array2[num] = dataObject.Identity;
			TaskLogger.LogEnter(array);
			TDataObject dataObject2 = this.DataObject;
			base.WriteVerbose(TaskVerboseStringHelper.GetReadObjectVerboseString(dataObject2.Identity, base.DataSession, typeof(TDataObject)));
			IConfigurable configurable = null;
			try
			{
				using (TaskPerformanceData.ReadResult.StartRequestTimer())
				{
					IConfigDataProvider dataSession = base.DataSession;
					TDataObject dataObject3 = this.DataObject;
					configurable = dataSession.Read<TDataObject>(dataObject3.Identity);
				}
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
			}
			if (configurable == null)
			{
				TDataObject dataObject4 = this.DataObject;
				Exception exception = new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.ResolveIdentityString(dataObject4.Identity), typeof(TDataObject).ToString(), (base.DataSession != null) ? base.DataSession.Source : null));
				ErrorCategory category = (ErrorCategory)1003;
				TDataObject dataObject5 = this.DataObject;
				base.WriteError(exception, category, dataObject5.Identity);
			}
			using (TaskPerformanceData.WriteResult.StartRequestTimer())
			{
				this.WriteResult(configurable);
			}
			TaskLogger.LogExit();
		}

		protected virtual void WriteResult(IConfigurable result)
		{
			TaskLogger.LogEnter(new object[]
			{
				result.Identity
			});
			base.WriteObject(result);
			TaskLogger.LogExit();
		}

		private TDataObject bindingInstance;
	}
}
