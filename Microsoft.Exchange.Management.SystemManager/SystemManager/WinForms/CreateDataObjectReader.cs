using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CreateDataObjectReader : Reader
	{
		public CreateDataObjectReader()
		{
		}

		public CreateDataObjectReader(Type type)
		{
			this.Type = type;
		}

		public CreateDataObjectReader(string typeString) : this(ObjectSchemaLoader.GetTypeByString(typeString))
		{
		}

		public override object DataObject
		{
			get
			{
				return this.dataObject;
			}
		}

		public Type Type { get; set; }

		public override void Run(object interactionHandler, DataRow row, DataObjectStore store)
		{
			this.dataObject = this.Type.GetConstructor(new Type[0]).Invoke(new object[0]);
			foreach (ParameterProfile parameterProfile in this.paramInfos)
			{
				if (parameterProfile.IsRunnable(row))
				{
					object obj = row[parameterProfile.Reference];
					if (DBNull.Value.Equals(obj))
					{
						obj = null;
					}
					this.Type.GetProperty(parameterProfile.Name).SetValue(this.dataObject, obj, null);
				}
			}
		}

		public override void BuildParameters(DataRow row, DataObjectStore store, IList<ParameterProfile> paramInfos)
		{
			base.BuildParameters(row, store, paramInfos);
			this.paramInfos = paramInfos;
		}

		private object dataObject;

		private IList<ParameterProfile> paramInfos;
	}
}
