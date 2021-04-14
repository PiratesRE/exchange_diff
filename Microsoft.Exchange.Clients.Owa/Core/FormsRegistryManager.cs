using System;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class FormsRegistryManager
	{
		public static bool HasCustomForm
		{
			get
			{
				return FormsRegistryManager.hasCustomForm;
			}
		}

		private FormsRegistryManager()
		{
		}

		public static void Initialize(string directory)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug<string>(0L, "FormsRegistryManager.Initialize directory = {0}", directory);
			FormsRegistryLoader formsRegistryLoader = new FormsRegistryLoader();
			try
			{
				formsRegistryLoader.LoadRegistries(directory + "forms/");
			}
			catch (OwaInvalidInputException innerException)
			{
				throw new OwaFormsRegistryInitializationException("Unable to initialize forms registries", innerException);
			}
			FormsRegistryManager.baseExperienceClientMappingList = formsRegistryLoader.BaseClientMappings;
			FormsRegistryManager.isLoaded = true;
			FormsRegistryManager.hasCustomForm = formsRegistryLoader.HasCustomForm;
			ExTraceGlobals.FormsRegistryTracer.TraceDebug(0L, "FormsRegistryManager initialized succesfully.");
		}

		public static bool IsLoaded
		{
			get
			{
				return FormsRegistryManager.isLoaded;
			}
		}

		public static Experience[] LookupExperiences(string application, UserAgentParser.UserAgentVersion version, string platform, ClientControl control, bool isRichClientFeatureOn)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug(0L, "FormsRegistryManager.LookupExperiences application = {0}, version = {1}, platform = {2}, control = {3}", new object[]
			{
				application,
				version,
				platform,
				control
			});
			int num;
			int num2;
			if (!FormsRegistryManager.baseExperienceClientMappingList.FindMatchingRange(application, platform, control, version, out num, out num2))
			{
				return null;
			}
			FormsRegistry formsRegistry = null;
			if (isRichClientFeatureOn)
			{
				formsRegistry = FormsRegistryManager.baseExperienceClientMappingList[num2].Experience.FormsRegistry;
			}
			else
			{
				for (int i = num2; i >= num; i--)
				{
					formsRegistry = FormsRegistryManager.baseExperienceClientMappingList[i].Experience.FormsRegistry;
					if (!formsRegistry.IsRichClient)
					{
						break;
					}
					formsRegistry = null;
				}
				if (formsRegistry == null)
				{
					return null;
				}
			}
			ExTraceGlobals.FormsRegistryDataTracer.TraceDebug<string>(0L, "Matched registry = {0}", formsRegistry.Name);
			return formsRegistry.LookupExperiences(application, version, platform, control);
		}

		public static FormValue LookupForm(FormKey formKey, Experience[] experiences)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug<FormKey>(0L, "FormsRegistryManager.LookupForm key = {0}", formKey);
			if (experiences == null)
			{
				throw new ArgumentNullException("experiences", "There must be at least one experience provided");
			}
			if (experiences.Length == 0)
			{
				throw new ArgumentOutOfRangeException("experiences", "There must be at least one experience provided");
			}
			FormValue formValue = FormsRegistryManager.LookupFormInExperiences(formKey, experiences);
			if (formValue == null)
			{
				ExTraceGlobals.FormsRegistryTracer.TraceDebug(0L, "Downgrading state to wildcard");
				formKey.State = string.Empty;
				formValue = FormsRegistryManager.LookupFormInExperiences(formKey, experiences);
				if (formValue == null)
				{
					ExTraceGlobals.FormsRegistryTracer.TraceDebug(0L, "Downgrading action to wildcard");
					string action = formKey.Action;
					formKey.Action = string.Empty;
					formValue = FormsRegistryManager.LookupFormInExperiences(formKey, experiences);
					if (formValue == null)
					{
						ExTraceGlobals.FormsRegistryTracer.TraceDebug(0L, "Downgrading class to wildcard");
						formKey.Class = string.Empty;
						formKey.Action = action;
						ExTraceGlobals.FormsRegistryTracer.TraceDebug(0L, "Restoring action");
						formValue = FormsRegistryManager.LookupFormInExperiences(formKey, experiences);
						if (formValue == null)
						{
							formKey.Action = string.Empty;
							ExTraceGlobals.FormsRegistryTracer.TraceDebug(0L, "Downgrading action to wildcard");
							formValue = FormsRegistryManager.LookupFormInExperiences(formKey, experiences);
						}
					}
				}
			}
			return formValue;
		}

		private static FormValue LookupFormInExperiences(FormKey formKey, Experience[] experiences)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug<FormKey>(0L, "FormsRegistryManager.LookupFormInExperiences key = {0}", formKey);
			FormValue formValue = null;
			int num = experiences.Length;
			for (int i = 0; i < num; i++)
			{
				Experience experience = experiences[i];
				formKey.Experience = experience.Name;
				formValue = experience.FormsRegistry.LookupForm(formKey);
				if (formValue != null)
				{
					break;
				}
			}
			return formValue;
		}

		private const string RegistryFolder = "forms/";

		private static ClientMappingList baseExperienceClientMappingList;

		private static bool isLoaded;

		private static bool hasCustomForm;
	}
}
