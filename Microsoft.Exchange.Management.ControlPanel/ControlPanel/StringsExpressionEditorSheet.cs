using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI.Design;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class StringsExpressionEditorSheet : ExpressionEditorSheet
	{
		internal static void AddExpressionGroup(string prefix, Type stringsType, Type idsType)
		{
			if (StringsExpressionEditorSheet.stringsExpressionGroups.ContainsKey(prefix))
			{
				throw new ArgumentException("String expression group is already provided for :" + prefix);
			}
			StringsExpressionEditorSheet.stringsExpressionGroups[prefix] = new StringsExpressionGroup(stringsType, idsType);
		}

		public StringsExpressionEditorSheet(string expression, IServiceProvider serviceProvider) : base(serviceProvider)
		{
			string[] array = expression.Split(new char[]
			{
				'.'
			});
			if (array != null && array.Length == 2 && !string.IsNullOrEmpty(array[0]) && !string.IsNullOrEmpty(array[1]))
			{
				StringsExpressionGroup stringsExpressionGroup = StringsExpressionEditorSheet.stringsExpressionGroups[array[0]];
				if (stringsExpressionGroup == null)
				{
					throw new Exception("Unsupported String Expression Group: " + array[0]);
				}
				this.Group = stringsExpressionGroup;
				this.StringID = array[1];
			}
			if (string.IsNullOrEmpty(this.StringID))
			{
				this.StringID = expression;
			}
		}

		public override bool IsValid
		{
			get
			{
				bool result;
				try
				{
					Enum.Parse(this.Group.IdsType, this.StringID);
					result = true;
				}
				catch (ArgumentException)
				{
					result = false;
				}
				return result;
			}
		}

		public override string GetExpression()
		{
			return this.stringID;
		}

		[DefaultValue("")]
		[DisplayName("String Name")]
		[TypeConverter(typeof(StringsExpressionEditorSheet.StringIDTypeConverter))]
		[Description("The ID of the string to show. The string cannot have parameters.")]
		public string StringID
		{
			get
			{
				return this.stringID;
			}
			set
			{
				this.stringID = (value ?? string.Empty);
			}
		}

		public StringsExpressionGroup Group
		{
			get
			{
				return this.group ?? StringsExpressionEditorSheet.defaultGroup;
			}
			set
			{
				this.group = value;
			}
		}

		public string Evaluate()
		{
			MethodInfo method = this.Group.StringsType.GetMethod("GetLocalizedString", BindingFlags.Static | BindingFlags.Public);
			if (method != null)
			{
				return method.Invoke(null, new object[]
				{
					Enum.Parse(this.Group.IdsType, this.StringID)
				}) as string;
			}
			return string.Empty;
		}

		private static readonly StringsExpressionGroup defaultGroup = new StringsExpressionGroup(typeof(Strings), typeof(Strings.IDs));

		private static Dictionary<string, StringsExpressionGroup> stringsExpressionGroups = new Dictionary<string, StringsExpressionGroup>
		{
			{
				string.Empty,
				new StringsExpressionGroup(typeof(Strings), typeof(Strings.IDs))
			},
			{
				"Client",
				new StringsExpressionGroup(typeof(ClientStrings), typeof(ClientStrings.IDs))
			},
			{
				"OwaOption",
				new StringsExpressionGroup(typeof(OwaOptionStrings), typeof(OwaOptionStrings.IDs))
			},
			{
				"OwaOptionClient",
				new StringsExpressionGroup(typeof(OwaOptionClientStrings), typeof(OwaOptionClientStrings.IDs))
			}
		};

		private string stringID = string.Empty;

		private StringsExpressionGroup group;

		private class StringIDTypeConverter : TypeConverter
		{
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				if (StringsExpressionEditorSheet.StringIDTypeConverter.standardValues == null)
				{
					ArrayList arrayList = new ArrayList();
					foreach (KeyValuePair<string, StringsExpressionGroup> keyValuePair in StringsExpressionEditorSheet.stringsExpressionGroups)
					{
						string key = keyValuePair.Key;
						string[] names = Enum.GetNames(keyValuePair.Value.IdsType);
						if (string.Empty == key)
						{
							arrayList.AddRange(names);
						}
						else
						{
							foreach (string arg in names)
							{
								arrayList.Add(key + '.' + arg);
							}
						}
					}
					StringsExpressionEditorSheet.StringIDTypeConverter.standardValues = new TypeConverter.StandardValuesCollection(arrayList.ToArray());
				}
				return StringsExpressionEditorSheet.StringIDTypeConverter.standardValues;
			}

			private static TypeConverter.StandardValuesCollection standardValues;
		}
	}
}
