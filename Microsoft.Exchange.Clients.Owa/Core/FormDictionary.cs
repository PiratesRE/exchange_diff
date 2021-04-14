using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class FormDictionary
	{
		public FormDictionary() : this(FormDictionary.MatchMode.DowngradeMatch)
		{
		}

		public FormDictionary(FormDictionary.MatchMode matchMode)
		{
			this.matchMode = matchMode;
			this.formDictionary = new Dictionary<FormKey, FormValue>();
		}

		public IEnumerable<FormKey> Keys
		{
			get
			{
				return this.formDictionary.Keys;
			}
		}

		public bool ContainsKey(FormKey formKey)
		{
			return this.FindMatchingKeyClass(formKey, this.matchMode) != null;
		}

		public bool ContainsKey(FormKey formKey, FormDictionary.MatchMode matchMode)
		{
			return this.FindMatchingKeyClass(formKey, matchMode) != null;
		}

		public void Add(FormKey formKey, FormValue formValue)
		{
			this.formDictionary.Add(formKey, formValue);
		}

		public FormValue this[FormKey formKey]
		{
			get
			{
				string text = this.FindMatchingKeyClass(formKey, this.matchMode);
				if (text == null)
				{
					return null;
				}
				string @class = formKey.Class;
				formKey.Class = text;
				FormValue result = this.formDictionary[formKey];
				formKey.Class = @class;
				return result;
			}
		}

		private string FindMatchingKeyClass(FormKey formKey, FormDictionary.MatchMode matchMode)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug((long)this.GetHashCode(), "FormDictionary.FindMatchingKeyClass");
			if (this.formDictionary.ContainsKey(formKey))
			{
				ExTraceGlobals.FormsRegistryDataTracer.TraceDebug<string>((long)this.GetHashCode(), "FindMatchingKeyClass found exact key match, MessageClass = '{0}'", formKey.Class);
				return formKey.Class;
			}
			string text = null;
			if (matchMode == FormDictionary.MatchMode.DowngradeMatch)
			{
				ExTraceGlobals.FormsRegistryTracer.TraceDebug<string>((long)this.GetHashCode(), "FormDictionary is doing the down-grade lookup, MessageClass = '{0}'", formKey.Class);
				string @class = formKey.Class;
				string text2 = null;
				int num = @class.LastIndexOf('.');
				int num2;
				if (@class.StartsWith("REPORT.", StringComparison.OrdinalIgnoreCase))
				{
					num2 = @class.IndexOf('.', "REPORT.".Length);
					text2 = @class.Substring(num);
				}
				else
				{
					num2 = @class.IndexOf('.');
				}
				if (num2 < 0 || num2 == num)
				{
					return null;
				}
				string text3 = formKey.Class = @class.Substring(0, num);
				while (!this.formDictionary.ContainsKey(formKey))
				{
					num2 = ((text2 == null) ? text3.IndexOf('.') : text3.IndexOf('.', "REPORT.".Length));
					num = text3.LastIndexOf('.');
					if (num2 >= 0 && num2 != num)
					{
						formKey.Class = text3.Substring(0, num);
						text3 = formKey.Class;
						if (text2 == null)
						{
							continue;
						}
						string class2 = formKey.Class;
						formKey.Class += text2;
						if (!this.formDictionary.ContainsKey(formKey))
						{
							formKey.Class = class2;
							continue;
						}
						text = formKey.Class;
					}
					IL_17C:
					formKey.Class = @class;
					goto IL_183;
				}
				text = formKey.Class;
				goto IL_17C;
			}
			IL_183:
			ExTraceGlobals.FormsRegistryDataTracer.TraceDebug<string>((long)this.GetHashCode(), "FormDictionary is returning the down-graded MessageClass: '{0}'", text);
			return text;
		}

		private Dictionary<FormKey, FormValue> formDictionary;

		private FormDictionary.MatchMode matchMode;

		public enum MatchMode
		{
			DowngradeMatch,
			ExactMatch
		}
	}
}
