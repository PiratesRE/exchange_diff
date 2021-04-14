using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class SerializationHelper
	{
		internal static string Serialize(object o)
		{
			AutoAttendantSettingsSerializer autoAttendantSettingsSerializer = new AutoAttendantSettingsSerializer();
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(stringBuilder);
			autoAttendantSettingsSerializer.Serialize(stringWriter, o);
			stringWriter.Flush();
			stringWriter.Close();
			return stringBuilder.ToString();
		}

		internal static object Deserialize(string text, Type t)
		{
			AutoAttendantSettingsSerializer autoAttendantSettingsSerializer = new AutoAttendantSettingsSerializer();
			StringReader stringReader = new StringReader(text);
			object result = autoAttendantSettingsSerializer.Deserialize(stringReader);
			stringReader.Close();
			return result;
		}
	}
}
