using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetTaskBase<TDataObject> : DataAccessTask<TDataObject> where TDataObject : IConfigurable, new()
	{
		protected virtual TDataObject DataObject { get; set; }

		internal virtual IReferenceErrorReporter ReferenceErrorReporter
		{
			get
			{
				if (this.directReferenceErrorReporter == null)
				{
					this.directReferenceErrorReporter = new DirectReferenceErrorReporter(new Task.ErrorLoggerDelegate(base.WriteError));
				}
				return this.directReferenceErrorReporter;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				try
				{
					if (disposing)
					{
						IDisposable disposable = this.DataObject as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
					this.disposed = true;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				this.DataObject = (TDataObject)((object)this.PrepareDataObject());
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, (ErrorCategory)1002, null);
			}
			if (this.DataObject == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(null, typeof(TDataObject).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, null);
			}
			this.ProvisioningUpdateConfigurationObject();
			TDataObject dataObject = this.DataObject;
			string id;
			if (dataObject.Identity != null)
			{
				TDataObject dataObject2 = this.DataObject;
				id = dataObject2.Identity.ToString();
			}
			else
			{
				id = "$null";
			}
			base.WriteVerbose(Strings.VerboseTaskProcessingObject(id));
			base.Validate(this.DataObject);
			TaskLogger.LogExit();
		}

		protected override void PostInternalValidate()
		{
			TaskLogger.LogEnter();
			base.PostInternalValidate();
			this.ReferenceErrorReporter.ReportError(new Task.ErrorLoggerDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		protected abstract IConfigurable PrepareDataObject();

		protected override void InternalProvisioningValidation()
		{
			ProvisioningValidationError[] array = ProvisioningLayer.Validate(this, this.ConvertDataObjectToPresentationObject(this.DataObject));
			if (array != null && array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					ProvisioningValidationException exception = new ProvisioningValidationException(array[i].Description, array[i].AgentName, array[i].Exception);
					this.WriteError(exception, (ErrorCategory)array[i].ErrorCategory, null, array.Length - 1 == i);
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			try
			{
				base.Validate(this.DataObject);
				if (base.HasErrors)
				{
					return;
				}
				TDataObject dataObject = this.DataObject;
				if (dataObject.Identity != null)
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(this.DataObject, base.DataSession, typeof(TDataObject)));
				}
				using (TaskPerformanceData.SaveResult.StartRequestTimer())
				{
					base.DataSession.Save(this.DataObject);
				}
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, (ErrorCategory)1002, null);
			}
			finally
			{
				TDataObject dataObject2 = this.DataObject;
				if (dataObject2.Identity != null)
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
				}
			}
			TaskLogger.LogExit();
		}

		protected virtual string ResolveIdentityString(ObjectId identity)
		{
			if (identity == null)
			{
				return null;
			}
			ADObjectId adobjectId = identity as ADObjectId;
			if (adobjectId != null)
			{
				return adobjectId.DistinguishedName ?? identity.ToString();
			}
			return identity.ToString();
		}

		protected virtual void ProvisioningUpdateConfigurationObject()
		{
			ProvisioningLayer.UpdateAffectedIConfigurable(this, this.DataObject, true);
		}

		protected virtual void ResolveLocalSecondaryIdentities()
		{
		}

		private bool disposed;

		private DirectReferenceErrorReporter directReferenceErrorReporter;
	}
}
