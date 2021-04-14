using System;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WorkingHoursAdapter<TObject> : MailboxManagementDataAdapter<TObject> where TObject : UserConfigurationObject, IWorkingHours, new()
	{
		public WorkingHoursAdapter(MailboxSession session) : base(session, string.Empty)
		{
		}

		protected override void InternalFill(TObject configObject)
		{
			base.InternalFill(configObject);
			StorageWorkingHours storageWorkingHours = StorageWorkingHours.LoadFrom(base.Session, base.Session.GetDefaultFolderId(DefaultFolderType.Calendar));
			if (storageWorkingHours == null)
			{
				return;
			}
			configObject.WorkDays = storageWorkingHours.DaysOfWeek;
			configObject.WorkingHoursStartTime = TimeSpan.FromMinutes((double)storageWorkingHours.StartTimeInMinutes);
			configObject.WorkingHoursEndTime = TimeSpan.FromMinutes((double)storageWorkingHours.EndTimeInMinutes);
			configObject.WorkingHoursTimeZone = new ExTimeZoneValue(storageWorkingHours.TimeZone);
		}

		protected override void InternalSave(TObject configObj)
		{
			base.InternalSave(configObj);
			StoreObjectId defaultFolderId = base.Session.GetDefaultFolderId(DefaultFolderType.Calendar);
			StorageWorkingHours storageWorkingHours = StorageWorkingHours.LoadFrom(base.Session, defaultFolderId);
			if (storageWorkingHours == null)
			{
				ExTimeZone timeZone = (configObj.WorkingHoursTimeZone != null) ? configObj.WorkingHoursTimeZone.ExTimeZone : ExTimeZone.CurrentTimeZone;
				storageWorkingHours = StorageWorkingHours.Create(timeZone, (int)configObj.WorkDays, (int)configObj.WorkingHoursStartTime.TotalMinutes, (int)configObj.WorkingHoursEndTime.TotalMinutes);
			}
			else
			{
				if (configObj.IsModified(WorkingHoursSchema.WorkingHoursTimeZone))
				{
					storageWorkingHours.TimeZone = configObj.WorkingHoursTimeZone.ExTimeZone;
				}
				if (configObj.IsModified(WorkingHoursSchema.WorkDays) || configObj.IsModified(WorkingHoursSchema.WorkingHoursStartTime) || configObj.IsModified(WorkingHoursSchema.WorkingHoursEndTime))
				{
					storageWorkingHours.UpdateWorkingPeriod(configObj.WorkDays, (int)configObj.WorkingHoursStartTime.TotalMinutes, (int)configObj.WorkingHoursEndTime.TotalMinutes);
				}
			}
			try
			{
				storageWorkingHours.SaveTo(base.Session, defaultFolderId);
			}
			catch (ObjectExistedException)
			{
				storageWorkingHours.SaveTo(base.Session, defaultFolderId);
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<WorkingHoursAdapter<TObject>>(this);
		}
	}
}
