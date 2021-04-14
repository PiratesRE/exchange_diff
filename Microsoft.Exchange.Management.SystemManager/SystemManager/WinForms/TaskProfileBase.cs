using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class TaskProfileBase
	{
		public TaskProfileBase()
		{
		}

		public TaskProfileBase(string name, RunnerBase runner)
		{
			this.name = name;
			this.runner = runner;
		}

		public ParameterProfileList ParameterProfileList
		{
			get
			{
				return this.parameterProfileList;
			}
			set
			{
				this.parameterProfileList = value;
			}
		}

		[DDIMandatoryValue]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public RunnerBase Runner
		{
			get
			{
				return this.runner;
			}
			set
			{
				this.runner = value;
			}
		}

		public void AddParameterProfile(ParameterProfile profile)
		{
			this.parameterProfileList.Add(profile);
		}

		public bool IsRunnable(DataRow row, DataObjectStore store)
		{
			return this.runner.IsRunnable(row, store);
		}

		public void BuildParameters(DataRow row, DataObjectStore store)
		{
			this.runner.BuildParameters(row, store, this.GetEffectiveParameters());
		}

		[DefaultValue(false)]
		public bool IgnoreException { get; set; }

		internal abstract void Run(CommandInteractionHandler interactionHandler, DataRow row, DataObjectStore store);

		public IList<ParameterProfile> GetEffectiveParameters()
		{
			IList<ParameterProfile> list = new List<ParameterProfile>();
			foreach (ParameterProfile parameterProfile in this.ParameterProfileList)
			{
				if (WinformsHelper.IsCurrentOrganizationAllowed(parameterProfile.OrganizationTypes))
				{
					list.Add(parameterProfile);
				}
			}
			return list;
		}

		private ParameterProfileList parameterProfileList = new ParameterProfileList();

		private RunnerBase runner;

		private string name;
	}
}
