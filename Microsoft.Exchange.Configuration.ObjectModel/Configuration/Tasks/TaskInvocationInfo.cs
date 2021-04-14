using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class TaskInvocationInfo
	{
		public string CommandName { get; private set; }

		public string InvocationName { get; private set; }

		public bool IsCmdletInvokedWithoutPSFramework { get; set; }

		public string DisplayName
		{
			get
			{
				if (this.CommandName != null)
				{
					return this.CommandName;
				}
				return this.InvocationName;
			}
		}

		public string ScriptName { get; private set; }

		public string RootScriptName { get; private set; }

		public string ShellHostName { get; private set; }

		public PropertyBag Fields { get; private set; }

		public string ParameterSetName { get; set; }

		public PropertyBag UserSpecifiedParameters { get; private set; }

		public bool IsInternalOrigin { get; private set; }

		public bool IsVerboseOn
		{
			get
			{
				return TaskLogger.IsFileLoggingEnabled || this.isVerboseOn;
			}
			set
			{
				this.isVerboseOn = value;
			}
		}

		public bool IsDebugOn { get; set; }

		public TaskInvocationInfo(string commandName, string invocationName, string scriptName, string rootScriptName, bool isInternalOrigin, Dictionary<string, object> parameters, PropertyBag fields, string shellHostName)
		{
			this.CommandName = commandName;
			this.InvocationName = invocationName;
			this.ScriptName = scriptName;
			this.RootScriptName = rootScriptName;
			this.IsInternalOrigin = isInternalOrigin;
			if (parameters != null)
			{
				this.UpdateSpecifiedParameters(parameters);
			}
			this.Fields = fields;
			this.ShellHostName = shellHostName;
		}

		public static TaskInvocationInfo CreateForDirectTaskInvocation(string commandName)
		{
			PropertyBag fields = new PropertyBag();
			return new TaskInvocationInfo(commandName, commandName, null, null, false, null, fields, "PSDirectInvoke")
			{
				IsCmdletInvokedWithoutPSFramework = true,
				UserSpecifiedParameters = new PropertyBag()
			};
		}

		public void UpdateSpecifiedParameters(Dictionary<string, object> boundParameters)
		{
			this.UserSpecifiedParameters = TaskInvocationInfo.GetUserSpecifiedParameters(boundParameters);
		}

		public void AddToUserSpecifiedParameter(object key, object value)
		{
			PropertyDefinition propertyDefinition = key as PropertyDefinition;
			if (propertyDefinition != null)
			{
				string name = propertyDefinition.Name;
				this.UserSpecifiedParameters[name] = value;
				return;
			}
			this.UserSpecifiedParameters[key] = value;
		}

		private static PropertyBag GetUserSpecifiedParameters(Dictionary<string, object> boundParameters)
		{
			PropertyBag propertyBag = new PropertyBag(boundParameters.Count);
			foreach (KeyValuePair<string, object> keyValuePair in boundParameters)
			{
				propertyBag.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return propertyBag;
		}

		private bool isVerboseOn;
	}
}
