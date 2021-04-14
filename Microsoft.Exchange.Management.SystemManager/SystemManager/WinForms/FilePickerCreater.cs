using System;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public static class FilePickerCreater
	{
		public static FilePicker CreateAudioFilePicker()
		{
			return new OpenFilePicker
			{
				Filter = string.Format("{0}(*.wav)|*.wav|{1}(*.wma)|*.wma", Strings.WavFileDescription, Strings.WmaFileDescription),
				DefaultExt = "wav"
			};
		}

		public static FilePicker CreateAppFilePicker()
		{
			return new OpenFilePicker
			{
				Filter = string.Format("{0}(*.dll, *.exe, *.cab)|*.dll;*.exe;*.cab", Strings.AppFileDescription)
			};
		}
	}
}
