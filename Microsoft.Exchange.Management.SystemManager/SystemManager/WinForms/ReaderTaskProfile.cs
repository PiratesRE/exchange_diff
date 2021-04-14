using System;
using System.Data;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ReaderTaskProfile : TaskProfileBase
	{
		[DDIDataObjectNameExist]
		public string DataObjectName
		{
			get
			{
				return this.dataObjectName;
			}
			set
			{
				this.dataObjectName = value;
			}
		}

		public object DataObject
		{
			get
			{
				return (base.Runner as Reader).DataObject;
			}
		}

		internal override void Run(CommandInteractionHandler interactionHandler, DataRow row, DataObjectStore store)
		{
			(base.Runner as Reader).Run(interactionHandler, row, store);
		}

		public bool HasPermission()
		{
			return (base.Runner as Reader).HasPermission(base.GetEffectiveParameters());
		}

		private string dataObjectName;
	}
}
