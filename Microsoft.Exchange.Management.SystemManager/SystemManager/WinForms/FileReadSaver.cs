using System;
using System.Data;
using System.IO;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class FileReadSaver : FileSaver
	{
		public override void UpdateWorkUnits(DataRow row)
		{
			base.UpdateWorkUnits(row);
			string path = row[base.FilePathParameterName].ToString();
			this.workUnit.Text = Strings.FileReadTaskWorkUnitText;
			this.workUnit.Description = Strings.FileReadTaskWorkUnitDescription(path);
		}

		public override void Run(object interactionHandler, DataRow row, DataObjectStore store)
		{
			this.SavedResults.Clear();
			base.OnStart();
			string path = (string)row[base.FilePathParameterName];
			try
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream))
					{
						byte[] fileData = binaryReader.ReadBytes((int)fileStream.Length);
						this.SavedResults.Add(new BinaryFileObject(Path.GetFileName(path), fileData));
						base.OnComplete(true, null);
					}
				}
			}
			catch (FileNotFoundException exception)
			{
				base.OnComplete(false, exception);
			}
			catch (UnauthorizedAccessException exception2)
			{
				base.OnComplete(false, exception2);
			}
			catch (DirectoryNotFoundException exception3)
			{
				base.OnComplete(false, exception3);
			}
			catch (PathTooLongException exception4)
			{
				base.OnComplete(false, exception4);
			}
		}
	}
}
