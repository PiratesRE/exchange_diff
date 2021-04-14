using System;
using System.IO;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	internal class FileExistsCondition : Condition
	{
		public FileExistsCondition(string fileName)
		{
			this.fileName = fileName;
		}

		public override bool Verify()
		{
			TaskLogger.LogEnter();
			if (this.FileName == null || string.Empty == this.FileName)
			{
				throw new ConditionInitializationException("FileName", this);
			}
			bool result = File.Exists(this.FileName);
			TaskLogger.LogExit();
			return result;
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		private string fileName;
	}
}
