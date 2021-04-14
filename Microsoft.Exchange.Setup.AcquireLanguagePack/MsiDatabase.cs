using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal sealed class MsiDatabase : MsiBase
	{
		public MsiDatabase(string fileName)
		{
			this.OpenDatabase(fileName);
		}

		public List<string> ExtractCabs(string toPath)
		{
			ValidationHelper.ThrowIfNullOrEmpty(toPath, "toPath");
			Directory.CreateDirectory(toPath);
			List<string> result;
			using (MsiView msiView = this.OpenView("SELECT `Name`, `Data` FROM `_Streams`"))
			{
				List<string> list = new List<string>();
				for (;;)
				{
					try
					{
						using (MsiRecord msiRecord = msiView.FetchNextRecord())
						{
							string @string = msiRecord.GetString(1U);
							if (!string.IsNullOrEmpty(@string) && !@string.StartsWith("\u0005", StringComparison.InvariantCultureIgnoreCase))
							{
								msiRecord.SaveStream(2U, Path.Combine(toPath, @string));
								list.Add(@string);
							}
						}
					}
					catch (MsiException ex)
					{
						if (ex.ErrorCode == 259U)
						{
							result = list;
							break;
						}
						throw;
					}
				}
			}
			return result;
		}

		public string QueryProperty(string tableName, string propertyName)
		{
			ValidationHelper.ThrowIfNullOrEmpty(tableName, "tableName");
			ValidationHelper.ThrowIfNullOrEmpty(propertyName, "propertyName");
			string result = string.Empty;
			using (MsiView msiView = this.OpenView(string.Format("SELECT `Value` FROM `{0}` WHERE `Property`='{1}'", tableName, propertyName)))
			{
				using (MsiRecord msiRecord = msiView.FetchNextRecord())
				{
					result = msiRecord.GetString(1U);
				}
			}
			return result;
		}

		private MsiView OpenView(string query)
		{
			return new MsiView(this, query);
		}

		private void OpenDatabase(string fileName)
		{
			if (!MsiHelper.IsMsiFileExtension(fileName) && !MsiHelper.IsMspFileExtension(fileName))
			{
				throw new MsiException(Strings.WrongFileType("msiFilePath"));
			}
			SafeMsiHandle handle;
			uint errorCode = MsiNativeMethods.OpenDatabase(fileName, (IntPtr)(MsiHelper.IsMspFileExtension(fileName) ? MsiDatabase.MsiDbOpenPatchFile : MsiDatabase.MsiDbOpenReadonly), out handle);
			MsiHelper.ThrowIfNotSuccess(errorCode);
			base.Handle = handle;
		}

		private const string SkippedFilesIndicator = "\u0005";

		private const string DataQuery = "SELECT `Name`, `Data` FROM `_Streams`";

		private const string PropertyQuery = "SELECT `Value` FROM `{0}` WHERE `Property`='{1}'";

		private static readonly int MsiDbOpenReadonly = 0;

		private static readonly int MsiDbOpenPatchFile = MsiDatabase.MsiDbOpenReadonly + 32;
	}
}
