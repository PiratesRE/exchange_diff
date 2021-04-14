using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[DataContract]
	public class FormSettings
	{
		[DataMember]
		public FormSettings.FormSettingsType SettingsType { get; set; }

		[DataMember]
		public string SourceLocation { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int RequestedHeight { get; set; }

		[DataContract]
		public enum FormSettingsType
		{
			[EnumMember]
			ItemRead,
			[EnumMember]
			ItemEdit
		}
	}
}
