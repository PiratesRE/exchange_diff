using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class DataTableLoaderCreator : IDataTableLoaderCreator
	{
		public DataTableLoaderCreator(ObjectPickerProfileLoader profileLoader) : this(profileLoader, null)
		{
		}

		public DataTableLoaderCreator(ObjectPickerProfileLoader profileLoader, string profileName)
		{
			if (profileLoader == null)
			{
				throw new ArgumentNullException("profileLoader");
			}
			this.ProfileLoader = profileLoader;
			this.ProfileName = profileName;
		}

		public ObjectPickerProfileLoader ProfileLoader { get; private set; }

		public string ProfileName { get; private set; }

		public Dictionary<string, object> InputValues
		{
			get
			{
				return this.inputValues;
			}
		}

		public DataTableLoader CreateDataTableLoader(string name)
		{
			DataTableLoader dataTableLoader = new DataTableLoader(this.ProfileLoader, string.IsNullOrEmpty(this.ProfileName) ? name : this.ProfileName);
			foreach (string text in this.InputValues.Keys)
			{
				dataTableLoader.InputValue(text, this.InputValues[text]);
			}
			return dataTableLoader;
		}

		private Dictionary<string, object> inputValues = new Dictionary<string, object>();
	}
}
