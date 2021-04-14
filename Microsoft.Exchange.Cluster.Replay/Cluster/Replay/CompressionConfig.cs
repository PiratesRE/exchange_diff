using System;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Cluster.Replay
{
	[Serializable]
	public class CompressionConfig
	{
		public CompressionConfig.CompressionProvider Provider { get; set; }

		public CoconetConfig CoconetConfig { get; set; }

		internal CompressionConfig()
		{
			this.Provider = CompressionConfig.CompressionProvider.None;
		}

		internal static CompressionConfig Deserialize(string configXml, out Exception ex)
		{
			ex = null;
			try
			{
				if (!string.IsNullOrEmpty(configXml))
				{
					return (CompressionConfig)SerializationUtil.XmlToObject(configXml, typeof(CompressionConfig));
				}
			}
			catch (InvalidOperationException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			catch (SerializationException ex4)
			{
				ex = ex4;
			}
			return new CompressionConfig();
		}

		public override string ToString()
		{
			if (this.Provider == CompressionConfig.CompressionProvider.None)
			{
				return "None";
			}
			if (this.Provider == CompressionConfig.CompressionProvider.Xpress)
			{
				return "Xpress";
			}
			return string.Format("Coconet (DictionarySize={0}, SampleRate={1}, LzOption={2}", this.CoconetConfig.DictionarySize, this.CoconetConfig.SampleRate, this.CoconetConfig.LzOption);
		}

		public enum CompressionProvider
		{
			None,
			Xpress,
			Coconet
		}
	}
}
