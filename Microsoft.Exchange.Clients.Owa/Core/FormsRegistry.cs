using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class FormsRegistry
	{
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string InheritsFrom
		{
			get
			{
				return this.inheritsFrom;
			}
		}

		public string BaseExperience
		{
			get
			{
				return this.baseExperience;
			}
		}

		public ClientMappingList ClientMappingList
		{
			set
			{
				this.clientMappingList = value;
			}
		}

		public bool IsRichClient
		{
			get
			{
				return this.isRichClient;
			}
		}

		public bool HasCustomForm
		{
			get
			{
				return this.hasCustomForm;
			}
		}

		public bool IsCustomRegistry
		{
			get
			{
				return this.isCustomRegistry;
			}
		}

		public IEnumerable<FormKey> CustomizedFormKeys
		{
			get
			{
				List<FormKey> list = new List<FormKey>();
				foreach (FormKey formKey in this.forms.Keys)
				{
					if (this.forms[formKey].IsCustomForm)
					{
						list.Add(formKey);
					}
				}
				return list;
			}
		}

		public override int GetHashCode()
		{
			return this.inheritsFrom.GetHashCode() ^ this.name.GetHashCode();
		}

		public void Initialize(string name, string baseExperience, string inheritsFrom, string path, bool isRichClient)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug((long)this.GetHashCode(), "FormsRegistry.Initialize name = {0}, baseExperience = {1}, inheritsFrom = {2}, path = {3}", new object[]
			{
				name,
				baseExperience,
				inheritsFrom,
				path
			});
			if (inheritsFrom.Length > 0 && baseExperience.Length > 0)
			{
				ExTraceGlobals.FormsRegistryTracer.TraceError<string, string, string>((long)this.GetHashCode(), "A forms registry can not have a base experience and inherit from another registry.  registry = {0}, inheritsFrom = {1}, baseExperience = {2}", name, inheritsFrom, baseExperience);
				throw new OwaInvalidInputException("A forms registry can not have a base experience and inherit from another registry", null, this);
			}
			if (inheritsFrom.Length > 0 && baseExperience.Length > 0)
			{
				ExTraceGlobals.FormsRegistryTracer.TraceError<string>((long)this.GetHashCode(), "A forms registry must have a base experience or inherit from another registry.  registry = {0}", name);
				throw new OwaInvalidInputException("A forms registry must have a base experience or inherit from another registry", null, this);
			}
			this.name = name;
			this.inheritsFrom = inheritsFrom;
			this.baseExperience = baseExperience;
			this.path = path;
			this.isRichClient = isRichClient;
			if (!this.path.EndsWith("/", StringComparison.Ordinal))
			{
				this.path += "/";
			}
			this.isCustomRegistry = (!this.path.EndsWith("Basic/", StringComparison.OrdinalIgnoreCase) && !this.path.EndsWith("Premium/", StringComparison.OrdinalIgnoreCase));
		}

		public void AddForm(FormKey formKey, string form, ulong segmentationFlags)
		{
			if (Utilities.TryParseUri(form) == null)
			{
				form = this.path + form;
			}
			this.AddFormDictionaryEntry(formKey, new FormValue(form, segmentationFlags, this.isCustomRegistry));
		}

		public void AddPreForm(FormKey formKey, string preFormTypeString, ulong segmentationFlags)
		{
			if (formKey == null)
			{
				throw new OwaInvalidInputException("Formkey is empty", null, this);
			}
			if (preFormTypeString == null)
			{
				throw new OwaInvalidInputException("preFormTypeString is empty", null, this);
			}
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug<string>((long)this.GetHashCode(), "FormsRegistry.AddPreForm - looking up type - typestring = ({0})", preFormTypeString);
			Type type = null;
			try
			{
				type = Type.GetType(preFormTypeString, true);
			}
			catch (TypeLoadException)
			{
				throw new OwaInvalidInputException("A forms registry preform type must be inherited from IPreformAction Interface - failed loading Class " + preFormTypeString, null, this);
			}
			catch (TargetInvocationException)
			{
				throw new OwaInvalidInputException("A forms registry preform type must be inherited from IPreformAction Interface - failed loading Class " + preFormTypeString, null, this);
			}
			if (type.IsClass && typeof(IPreFormAction).IsAssignableFrom(type))
			{
				this.AddFormDictionaryEntry(formKey, new FormValue(type, segmentationFlags, this.isCustomRegistry));
				ExTraceGlobals.FormsRegistryDataTracer.TraceDebug<string>((long)this.GetHashCode(), "FormsRegistry.AddPreForm - added type - type = ({0})", type.FullName);
				return;
			}
			string message = "A forms registry preform type must be inherited from IPreformAction Interface - failed loading Class " + preFormTypeString;
			ExTraceGlobals.FormsRegistryTracer.TraceDebug<string>((long)this.GetHashCode(), "FormsRegistry.AddPreForm - failed to add type - type = ({0})", type.FullName);
			throw new OwaInvalidInputException(message, null, this);
		}

		public FormValue LookupForm(FormKey formKey)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug<string>((long)this.GetHashCode(), "FormsRegistry.LookupForm in registry {0}", this.name);
			if (!this.forms.ContainsKey(formKey))
			{
				ExTraceGlobals.FormsRegistryDataTracer.TraceDebug<FormKey>((long)this.GetHashCode(), "FormsRegistry.LookupForm - no form found - key = ({0})", formKey);
				return null;
			}
			FormValue formValue = this.forms[formKey];
			ExTraceGlobals.FormsRegistryDataTracer.TraceDebug<object, FormKey>((long)this.GetHashCode(), "FormsRegistry.LookupForm - found form - form = {0}, key = ({1})", formValue.Value, formKey);
			return formValue;
		}

		public Experience[] LookupExperiences(string application, UserAgentParser.UserAgentVersion version, string platform, ClientControl control)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug((long)this.GetHashCode(), "FormsRegistry.LookupExperiences application = {0}, version = {1}, platform = {2}, control = {3}", new object[]
			{
				application,
				version,
				platform,
				control
			});
			Hashtable hashtable = new Hashtable(1);
			ArrayList arrayList = new ArrayList(1);
			FormsRegistry.LookupExperienceState lookupExperienceState = FormsRegistry.LookupExperienceState.ExactMatch;
			while (FormsRegistry.LookupExperienceState.Done != lookupExperienceState)
			{
				switch (lookupExperienceState)
				{
				case FormsRegistry.LookupExperienceState.ExactMatch:
					lookupExperienceState = FormsRegistry.LookupExperienceState.Control;
					break;
				case FormsRegistry.LookupExperienceState.Control:
					lookupExperienceState = FormsRegistry.LookupExperienceState.Platform;
					if (control == ClientControl.None)
					{
						continue;
					}
					control = ClientControl.None;
					break;
				case FormsRegistry.LookupExperienceState.Platform:
					lookupExperienceState = FormsRegistry.LookupExperienceState.Application;
					platform = string.Empty;
					break;
				case FormsRegistry.LookupExperienceState.Application:
					lookupExperienceState = FormsRegistry.LookupExperienceState.Done;
					application = string.Empty;
					break;
				}
				int i;
				int num;
				if (this.clientMappingList.FindMatchingRange(application, platform, control, version, out i, out num))
				{
					while (i <= num)
					{
						ClientMapping clientMapping = this.clientMappingList[num];
						Experience experience = clientMapping.Experience;
						if (!hashtable.ContainsKey(experience))
						{
							ExTraceGlobals.FormsRegistryDataTracer.TraceDebug<string, ClientMapping>((long)this.GetHashCode(), "Matched experience. name = {0}, client mapping = ({1})", experience.Name, clientMapping);
							hashtable.Add(experience, string.Empty);
							arrayList.Add(experience);
						}
						num--;
					}
				}
			}
			ExTraceGlobals.FormsRegistryTracer.TraceDebug<int>((long)this.GetHashCode(), "FormsRegistry.LookupExperiences - Exit.  Matched {0} Experiences", arrayList.Count);
			return (Experience[])arrayList.ToArray(typeof(Experience));
		}

		private void AddFormDictionaryEntry(FormKey formKey, FormValue formValue)
		{
			if (this.forms.ContainsKey(formKey, FormDictionary.MatchMode.ExactMatch))
			{
				throw new OwaInvalidInputException("Duplicate form registry key exists", null, this);
			}
			this.forms.Add(formKey, formValue);
			this.hasCustomForm = formValue.IsCustomForm;
		}

		public static FastEnumParser ClientControlParser = new FastEnumParser(typeof(ClientControl));

		public static FastEnumParser ApplicationElementParser = new FastEnumParser(typeof(ApplicationElement));

		private string name = string.Empty;

		private string path = string.Empty;

		private string inheritsFrom = string.Empty;

		private string baseExperience = string.Empty;

		private FormDictionary forms = new FormDictionary();

		private ClientMappingList clientMappingList;

		private bool isRichClient;

		private bool hasCustomForm;

		private bool isCustomRegistry;

		private enum LookupExperienceState
		{
			ExactMatch,
			Control,
			Platform,
			Application,
			Done
		}
	}
}
