using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class GetObjectTask<TIdentity, TDataObject> : GetObjectWithIdentityTaskBase<TIdentity, TDataObject> where TIdentity : IIdentityParameter where TDataObject : ConfigObject, new()
	{
		protected GetObjectTask()
		{
			this.FindOne = false;
		}

		protected override IConfigDataProvider CreateSession()
		{
			DataSourceManager[] dataSourceManagers = DataSourceManager.GetDataSourceManagers(typeof(TDataObject), "Identity");
			return dataSourceManagers[0];
		}

		[Parameter]
		public string Expression
		{
			get
			{
				return (string)base.Fields["Expression"];
			}
			set
			{
				base.Fields["Expression"] = value;
			}
		}

		[Parameter]
		public bool FindOne
		{
			get
			{
				return (bool)base.Fields["FindOne"];
			}
			set
			{
				base.Fields["FindOne"] = value;
			}
		}

		protected override void InternalValidate()
		{
			if (this.Expression == null)
			{
				this.Expression = "";
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!string.IsNullOrEmpty(this.Expression))
			{
				if (this.FindOne)
				{
					IConfigurable[] array = ((DataSourceManager)base.DataSession).Find(typeof(TDataObject), this.Expression, false, null);
					if (array != null && array.Length > 0)
					{
						this.WriteResult(array[0]);
					}
				}
				else
				{
					IConfigurable[] array2 = ((DataSourceManager)base.DataSession).Find(typeof(TDataObject), this.Expression, true, null);
					if (array2 != null)
					{
						for (int i = 0; i < array2.Length; i++)
						{
							this.WriteResult(array2[i]);
						}
					}
				}
			}
			else
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}
	}
}
