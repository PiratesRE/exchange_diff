using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class OwaClientStrings : OwaPageCached
	{
		public void RenderVariable(string variableValue, string variableName)
		{
			RenderingUtilities.RenderStringVariable(base.SanitizingResponse, variableName, variableValue);
		}

		public void RenderVariable(Strings.IDs stringID, string variableName)
		{
			RenderingUtilities.RenderStringVariable(base.SanitizingResponse, variableName, stringID);
		}

		private void CheckDuplicatedStringID(Strings.IDs stringID)
		{
			if (this.stringIDList == null)
			{
				this.stringIDList = new List<Strings.IDs>();
			}
			this.stringIDList.Add(stringID);
		}

		private void CheckDuplicatedVariableName(string variableName)
		{
			if (this.variableNameList == null)
			{
				this.variableNameList = new List<string>();
			}
			this.variableNameList.Add(variableName);
		}

		public const string HotKeyCtrl = "Ctrl";

		public const string HotKeyAlt = "Alt";

		public const string HotKeyMacCtrl = "MacCtrl";

		public const string HotKeyShift = "Shift";

		public const string HotKeyTab = "Tab";

		private List<Strings.IDs> stringIDList;

		private List<string> variableNameList;
	}
}
