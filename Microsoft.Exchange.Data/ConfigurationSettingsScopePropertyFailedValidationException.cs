﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsScopePropertyFailedValidationException : ConfigurationSettingsException
	{
		public ConfigurationSettingsScopePropertyFailedValidationException(string name, string value) : base(DataStrings.ConfigurationSettingsScopePropertyFailedValidation(name, value))
		{
			this.name = name;
			this.value = value;
		}

		public ConfigurationSettingsScopePropertyFailedValidationException(string name, string value, Exception innerException) : base(DataStrings.ConfigurationSettingsScopePropertyFailedValidation(name, value), innerException)
		{
			this.name = name;
			this.value = value;
		}

		protected ConfigurationSettingsScopePropertyFailedValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.value = (string)info.GetValue("value", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("value", this.value);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly string name;

		private readonly string value;
	}
}
