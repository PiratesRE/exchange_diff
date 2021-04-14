using System;
using System.Data;
using System.IO;
using System.Security;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class FileSaveSaver : FileSaver
	{
		public FileSaveSaver()
		{
			this.FileDataParameterName = "FileData";
		}

		[DDIDataColumnExist]
		public string FileDataParameterName { get; set; }

		public override void UpdateWorkUnits(DataRow row)
		{
			base.UpdateWorkUnits(row);
			string path = row[base.FilePathParameterName].ToString();
			this.workUnit.Text = Strings.FileSaveTaskWorkUnitText;
			this.workUnit.Description = Strings.FileSaveTaskWorkUnitDescription(path);
		}

		public override void Run(object interactionHandler, DataRow row, DataObjectStore store)
		{
			base.OnStart();
			string path = (string)row[base.FilePathParameterName];
			try
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Create))
				{
					byte[] array = (byte[])row[this.FileDataParameterName];
					fileStream.Write(array, 0, array.Length);
					base.OnComplete(true, null);
				}
			}
			catch (UnauthorizedAccessException exception)
			{
				base.OnComplete(false, exception);
			}
			catch (DirectoryNotFoundException exception2)
			{
				base.OnComplete(false, exception2);
			}
			catch (PathTooLongException exception3)
			{
				base.OnComplete(false, exception3);
			}
			catch (SecurityException exception4)
			{
				base.OnComplete(false, exception4);
			}
			catch (IOException exception5)
			{
				base.OnComplete(false, exception5);
			}
		}
	}
}
