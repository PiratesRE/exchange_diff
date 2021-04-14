using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	[DataContract(Name = "Watermark")]
	public sealed class Watermark
	{
		public Watermark(DateTime timestamp)
		{
			this.timestamp = timestamp;
		}

		[IgnoreDataMember]
		public static Watermark LatestWatermark
		{
			get
			{
				return Watermark.latestWatermark;
			}
		}

		[IgnoreDataMember]
		public DateTime Timestamp
		{
			get
			{
				return this.timestamp;
			}
		}

		public static Watermark Load(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				throw new ArgumentNullException("filename");
			}
			Watermark result = null;
			using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(Watermark));
				result = (Watermark)dataContractSerializer.ReadObject(fileStream);
			}
			return result;
		}

		public static IDictionary<string, Watermark> LoadWatermarksFromDirectory(string watermarksDirectory)
		{
			if (string.IsNullOrEmpty(watermarksDirectory))
			{
				throw new ArgumentNullException("watermarksDirectory");
			}
			string[] files = Directory.GetFiles(watermarksDirectory, string.Format("{0}_Watermark.xml", "*"));
			Dictionary<string, Watermark> dictionary = new Dictionary<string, Watermark>();
			foreach (string text in files)
			{
				string fileName = Path.GetFileName(text);
				int num = fileName.LastIndexOf(string.Format("{0}_Watermark.xml", string.Empty));
				if (num != -1)
				{
					string text2 = fileName.Substring(0, num);
					if (!string.IsNullOrEmpty(text2))
					{
						try
						{
							Watermark watermark = Watermark.Load(text);
							if (watermark == null)
							{
								watermark = Watermark.LatestWatermark;
							}
							dictionary[text2] = watermark;
						}
						catch (FileNotFoundException)
						{
							Logger.LogErrorMessage("Unable to open watermark file '{0}' which was expected to exist.", new object[]
							{
								text
							});
						}
						catch (SerializationException ex)
						{
							Logger.LogErrorMessage("Unable to de-serialize the watermark file '{0}'. Exception: {1}", new object[]
							{
								text,
								ex
							});
						}
						catch (XmlException ex2)
						{
							Logger.LogErrorMessage("Unable to parse XML of the watermark file '{0}'. Exception: {1}", new object[]
							{
								text,
								ex2
							});
						}
						catch (Exception ex3)
						{
							Logger.LogErrorMessage("An unexpected exception was caught while trying to read a watermark. Exception: {0}", new object[]
							{
								ex3
							});
						}
					}
				}
			}
			return dictionary;
		}

		public override bool Equals(object obj)
		{
			Watermark watermark = obj as Watermark;
			return watermark != null && this.timestamp.Equals(watermark.timestamp);
		}

		public override int GetHashCode()
		{
			return this.timestamp.GetHashCode();
		}

		public void Save(FileStream fileStream)
		{
			try
			{
				if (fileStream == null)
				{
					throw new ArgumentNullException("fileStream");
				}
			}
			finally
			{
				fileStream.SetLength(0L);
				DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(Watermark));
				dataContractSerializer.WriteObject(fileStream, this);
				fileStream.Flush();
			}
		}

		[IgnoreDataMember]
		public const string WatermarkFileNameFormat = "{0}_Watermark.xml";

		private static readonly Watermark latestWatermark = new Watermark(DateTime.UtcNow);

		[DataMember(Name = "Timestamp", IsRequired = true)]
		private readonly DateTime timestamp;
	}
}
