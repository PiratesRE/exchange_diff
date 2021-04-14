using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoCategoriesProperty : XsoMultiValuedStringProperty
	{
		public XsoCategoriesProperty() : base(ItemSchema.Categories)
		{
		}

		private MasterCategoryList MasterCategoryList
		{
			get
			{
				if (this.masterCategoryList == null)
				{
					try
					{
						try
						{
							this.masterCategoryList = Command.CurrentCommand.MailboxSession.GetMasterCategoryList();
						}
						catch (CorruptDataException arg)
						{
							AirSyncDiagnostics.TraceDebug<CorruptDataException>(ExTraceGlobals.XsoTracer, this, "Failed to get MCL, exception: {0}", arg);
							Command.CurrentCommand.MailboxSession.DeleteMasterCategoryList();
							this.masterCategoryList = Command.CurrentCommand.MailboxSession.GetMasterCategoryList();
						}
					}
					catch (LocalizedException arg2)
					{
						AirSyncDiagnostics.TraceDebug<LocalizedException>(ExTraceGlobals.XsoTracer, this, "Failed to load MCL, exception: {0}", arg2);
						Command.CurrentCommand.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "FailedToLoadMCL");
					}
				}
				return this.masterCategoryList;
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			base.InternalCopyFromModified(srcProperty);
			IMultivaluedProperty<string> multivaluedProperty = srcProperty as IMultivaluedProperty<string>;
			foreach (string name in multivaluedProperty)
			{
				this.CheckAddCategory(name);
			}
			try
			{
				if (this.needToSaveMCL && this.MasterCategoryList != null)
				{
					this.MasterCategoryList.Save();
					this.needToSaveMCL = false;
				}
			}
			catch (LocalizedException arg)
			{
				AirSyncDiagnostics.TraceDebug<LocalizedException>(ExTraceGlobals.XsoTracer, this, "Failed to save MCL, exception: {0}", arg);
				Command.CurrentCommand.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "FailedToSaveMCL");
			}
		}

		private void CheckAddCategory(string name)
		{
			int hashCode = name.GetHashCode();
			if (!Command.CurrentCommand.SyncStatusSyncData.ContainsClientCategoryHash(hashCode))
			{
				Command.CurrentCommand.SyncStatusSyncData.AddClientCategoryHash(hashCode);
				if (this.MasterCategoryList != null && !this.MasterCategoryList.Contains(name))
				{
					Category item = Category.Create(name, 9, true);
					this.MasterCategoryList.Add(item);
					this.needToSaveMCL = true;
				}
				if (!Command.CurrentCommand.ShouldSaveSyncStatus)
				{
					throw new InvalidOperationException(string.Format("ShouldSaveSyncStatus should be true. Current command is {0}.", Command.CurrentCommand.GetType().ToString()));
				}
			}
		}

		private const int DefaultColor = 9;

		private MasterCategoryList masterCategoryList;

		private bool needToSaveMCL;
	}
}
