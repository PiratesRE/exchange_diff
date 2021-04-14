using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class RegistryObjectId : ObjectId
	{
		public RegistryObjectId(string registryFolderPath) : this(registryFolderPath, null)
		{
		}

		public RegistryObjectId(string registryFolderPath, string registryFolderName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("registryFolderPath", registryFolderPath);
			this.RegistryKeyPath = registryFolderPath;
			this.Name = registryFolderName;
		}

		public string RegistryKeyPath { get; private set; }

		public string Name { get; private set; }

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = ((this.Name != null) ? string.Format("{0}\\{1}", this.RegistryKeyPath, this.Name) : this.RegistryKeyPath);
			}
			return this.toString;
		}

		public override bool Equals(object obj)
		{
			RegistryObjectId registryObjectId = obj as RegistryObjectId;
			return registryObjectId != null && (object.ReferenceEquals(this, registryObjectId) || (string.Equals(this.RegistryKeyPath, registryObjectId.RegistryKeyPath, StringComparison.OrdinalIgnoreCase) && string.Equals(this.Name, registryObjectId.Name, StringComparison.OrdinalIgnoreCase)));
		}

		public override int GetHashCode()
		{
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.RegistryKeyPath.GetHashCode() ^ this.Name.GetHashCode();
			}
			return this.RegistryKeyPath.GetHashCode();
		}

		public override byte[] GetBytes()
		{
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			Stream stream = new MemoryStream();
			binaryFormatter.Serialize(stream, this.RegistryKeyPath);
			binaryFormatter.Serialize(stream, this.Name);
			byte[] array = new byte[stream.Length];
			stream.Read(array, 0, (int)stream.Length);
			stream.Close();
			return array;
		}

		private string toString;
	}
}
