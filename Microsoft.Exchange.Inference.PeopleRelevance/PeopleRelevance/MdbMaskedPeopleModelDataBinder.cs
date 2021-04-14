using System;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	internal class MdbMaskedPeopleModelDataBinder : MdbPeopleBaseModelDataBinder<MaskedPeopleModelItem>
	{
		public MdbMaskedPeopleModelDataBinder(MailboxSession session) : base("Inference.MaskedPeopleModel", session)
		{
		}

		protected override Version MinimumSupportedVersion
		{
			get
			{
				return MaskedPeopleModelItem.MinimumSupportedVersion;
			}
		}

		internal override Stream GetModelStreamFromUserConfig(UserConfiguration config)
		{
			return config.GetXmlStream();
		}

		protected override UserConfigurationTypes GetUserConfigurationType()
		{
			return UserConfigurationTypes.XML;
		}

		protected override void WriteModelData(DataContractSerializer serializer, Stream stream, MaskedPeopleModelItem modelData)
		{
			serializer.WriteObject(stream, modelData);
			stream.SetLength(stream.Position);
		}

		protected override MaskedPeopleModelItem ReadModelData(DataContractSerializer serializer, Stream stream)
		{
			return serializer.ReadObject(stream) as MaskedPeopleModelItem;
		}
	}
}
