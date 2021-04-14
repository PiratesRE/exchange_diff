using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public sealed class Watermarks
	{
		public Watermarks(string directory)
		{
			this.directory = directory;
			this.mappings = Watermark.LoadWatermarksFromDirectory(directory);
		}

		public string Directory
		{
			get
			{
				return this.directory;
			}
		}

		public Watermark Get(string jobName)
		{
			Watermark latestWatermark;
			if (!this.mappings.TryGetValue(jobName, out latestWatermark))
			{
				Logger.LogInformationMessage("No watermark found for '{0}' job, defaulting.", new object[]
				{
					jobName
				});
				latestWatermark = Watermark.LatestWatermark;
			}
			return latestWatermark;
		}

		private readonly string directory;

		private IDictionary<string, Watermark> mappings;
	}
}
