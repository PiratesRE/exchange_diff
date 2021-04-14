using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class EnumParameter : FormletParameter
	{
		public EnumParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, Type objectType, string defaultVal, bool useLocalizedDescription) : this(name, dialogTitle, dialogLabel, defaultVal)
		{
			if (objectType != null)
			{
				Array values = Enum.GetValues(objectType);
				EnumValue[] array = new EnumValue[values.Length];
				for (int i = 0; i < values.Length; i++)
				{
					array[i] = (useLocalizedDescription ? new EnumValue((Enum)values.GetValue(i), true) : new EnumValue((Enum)values.GetValue(i)));
				}
				this.Values = array;
			}
			else
			{
				this.Values = null;
			}
			this.IsFlags = !objectType.GetCustomAttributes(typeof(FlagsAttribute), false).IsNullOrEmpty();
			base.FormletType = typeof(EnumComboBoxModalEditor);
		}

		public EnumParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, Type objectType, string defaultVal) : this(name, dialogTitle, dialogLabel, objectType, defaultVal, false)
		{
		}

		public EnumParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, Type objectType, string defaultVal, bool useLocalizedDescription, LocalizedString noSelectionText) : this(name, dialogTitle, dialogLabel, objectType, defaultVal, false)
		{
			this.noSelectionText = noSelectionText;
		}

		public EnumParameter(string name, LocalizedString displayName, LocalizedString description, string defaultVal) : base(name, displayName, description)
		{
			this.defaultValue = ((defaultVal != null) ? new EnumValue(defaultVal, defaultVal) : null);
			base.FormletType = typeof(EnumComboBoxModalEditor);
		}

		[DataMember]
		public EnumValue DefaultValue
		{
			get
			{
				if (this.defaultValue != null)
				{
					return this.defaultValue;
				}
				if (this.Values != null && this.Values.Length > 0 && !this.IsFlags)
				{
					return this.Values[0];
				}
				return null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public EnumValue[] Values { get; internal set; }

		[DataMember]
		public bool IsFlags { get; private set; }

		private EnumValue defaultValue;
	}
}
