using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Serializable]
	public sealed class ExportableClassificationRuleCollectionPresentationObject : ClassificationRuleCollectionPresentationObject
	{
		private ExportableClassificationRuleCollectionPresentationObject(TransportRule transportRule) : base(transportRule)
		{
		}

		internal new static ExportableClassificationRuleCollectionPresentationObject Create(TransportRule transportRule)
		{
			if (transportRule == null)
			{
				throw new ArgumentNullException("transportRule");
			}
			ExportableClassificationRuleCollectionPresentationObject exportableClassificationRuleCollectionPresentationObject = new ExportableClassificationRuleCollectionPresentationObject(transportRule);
			exportableClassificationRuleCollectionPresentationObject.Initialize();
			return exportableClassificationRuleCollectionPresentationObject;
		}

		internal byte[] Export()
		{
			if (!this.IsExportable)
			{
				throw new InvalidOperationException(Strings.ClassificationRuleCollectionExportEncyrptedProhibited);
			}
			TransportRule storedRuleCollection = base.StoredRuleCollection;
			ExAssert.RetailAssert(storedRuleCollection != null, "The classification rule collection presentation object should store reference to its backing transport rule instance.");
			byte[] result;
			Exception ex;
			bool condition = ClassificationDefinitionUtils.TryUncompressXmlBytes(storedRuleCollection.ReplicationSignature, out result, out ex);
			ExAssert.RetailAssert(condition, "Decompression of classification rule collection must succeed since the presentation was initialized successfully before. Details: {0}", new object[]
			{
				(ex != null) ? ex.ToString() : string.Empty
			});
			return result;
		}

		public bool IsExportable
		{
			get
			{
				return !base.IsEncrypted;
			}
		}

		public byte[] SerializedClassificationRuleCollection
		{
			get
			{
				byte[] result;
				try
				{
					result = this.Export();
				}
				catch (InvalidOperationException)
				{
					result = null;
				}
				return result;
			}
		}

		private new Guid Guid
		{
			get
			{
				return base.Guid;
			}
		}

		private new Guid ImmutableId
		{
			get
			{
				return base.ImmutableId;
			}
		}
	}
}
