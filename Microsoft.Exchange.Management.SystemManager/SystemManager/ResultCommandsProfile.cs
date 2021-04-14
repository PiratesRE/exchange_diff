using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class ResultCommandsProfile
	{
		public List<ResultsCommandProfile> ResultPaneCommands
		{
			get
			{
				return this.resultPaneCommands;
			}
		}

		public List<ResultsCommandProfile> CustomSelectionCommands
		{
			get
			{
				return this.customSelectionCommands;
			}
		}

		public List<ResultsCommandProfile> DeleteSelectionCommands
		{
			get
			{
				return this.deleteSelectionCommands;
			}
		}

		public List<ResultsCommandProfile> ShowSelectionPropertiesCommands
		{
			get
			{
				return this.showSelectionPropertiesCommands;
			}
		}

		public ResultsCommandProfile GetProfile(Command command)
		{
			ResultsCommandProfile profile = this.GetProfile(this.ResultPaneCommands, command);
			if (profile == null)
			{
				profile = this.GetProfile(this.ResultPaneCommands, command);
			}
			if (profile == null)
			{
				profile = this.GetProfile(this.CustomSelectionCommands, command);
			}
			if (profile == null)
			{
				profile = this.GetProfile(this.DeleteSelectionCommands, command);
			}
			if (profile == null)
			{
				profile = this.GetProfile(this.ShowSelectionPropertiesCommands, command);
			}
			return profile;
		}

		private ResultsCommandProfile GetProfile(List<ResultsCommandProfile> profiles, Command command)
		{
			foreach (ResultsCommandProfile resultsCommandProfile in profiles)
			{
				if (resultsCommandProfile.Command == command)
				{
					return resultsCommandProfile;
				}
			}
			return null;
		}

		private List<ResultsCommandProfile> resultPaneCommands = new List<ResultsCommandProfile>();

		private List<ResultsCommandProfile> customSelectionCommands = new List<ResultsCommandProfile>();

		private List<ResultsCommandProfile> deleteSelectionCommands = new List<ResultsCommandProfile>();

		private List<ResultsCommandProfile> showSelectionPropertiesCommands = new List<ResultsCommandProfile>();
	}
}
