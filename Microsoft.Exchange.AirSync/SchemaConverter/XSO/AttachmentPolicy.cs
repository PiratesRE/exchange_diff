using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class AttachmentPolicy
	{
		internal AttachmentPolicy(string[] blockFileTypes, string[] blockMimeTypes, string[] forceSaveFileTypes, string[] forceSaveMimeTypes, string[] allowFileTypes, string[] allowMimeTypes, bool alwaysBlock, bool blockOnPublicComputers, AttachmentPolicy.Level treatUnknownTypeAs)
		{
			this.alwaysBlock = alwaysBlock;
			this.blockOnPublicComputers = blockOnPublicComputers;
			this.treatUnknownTypeAs = treatUnknownTypeAs;
			AttachmentPolicy.LoadDictionary(out this.fileTypeLevels, blockFileTypes, forceSaveFileTypes, allowFileTypes);
			AttachmentPolicy.LoadDictionary(out this.mimeTypeLevels, blockMimeTypes, forceSaveMimeTypes, allowMimeTypes);
		}

		public static int MaxEmbeddedDepth
		{
			get
			{
				return 4;
			}
		}

		public bool AlwaysBlock
		{
			get
			{
				return this.alwaysBlock;
			}
		}

		public bool BlockOnPublicComputers
		{
			get
			{
				return this.blockOnPublicComputers;
			}
		}

		public AttachmentPolicy.Level TreatUnknownTypeAs
		{
			get
			{
				return this.treatUnknownTypeAs;
			}
		}

		public AttachmentPolicy.Level GetLevel(string attachmentType, AttachmentPolicy.TypeSignifier typeSignifier)
		{
			AirSyncDiagnostics.Assert(attachmentType != null);
			AttachmentPolicy.Level result = AttachmentPolicy.Level.Unknown;
			switch (typeSignifier)
			{
			case AttachmentPolicy.TypeSignifier.File:
				result = AttachmentPolicy.FindLevel(this.fileTypeLevels, attachmentType);
				break;
			case AttachmentPolicy.TypeSignifier.Mime:
				result = AttachmentPolicy.FindLevel(this.mimeTypeLevels, attachmentType);
				break;
			}
			return result;
		}

		private static AttachmentPolicy.Level FindLevel(SortedDictionary<string, AttachmentPolicy.Level> dictionary, string attachmentType)
		{
			AttachmentPolicy.Level result;
			if (!dictionary.TryGetValue(attachmentType, out result))
			{
				return AttachmentPolicy.Level.Unknown;
			}
			return result;
		}

		private static void LoadDictionary(out SortedDictionary<string, AttachmentPolicy.Level> dictionary, string[] block, string[] forceSave, string[] allow)
		{
			string[][] array = new string[3][];
			AttachmentPolicy.Level[] array2 = new AttachmentPolicy.Level[3];
			array[1] = block;
			array[2] = forceSave;
			array[0] = allow;
			array2[1] = AttachmentPolicy.Level.Block;
			array2[2] = AttachmentPolicy.Level.ForceSave;
			array2[0] = AttachmentPolicy.Level.Allow;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					array[i] = new string[0];
				}
			}
			dictionary = new SortedDictionary<string, AttachmentPolicy.Level>(StringComparer.OrdinalIgnoreCase);
			for (int j = 0; j < 2; j++)
			{
				for (int k = 0; k < array[j].Length; k++)
				{
					if (!dictionary.ContainsKey(array[j][k]))
					{
						dictionary.Add(array[j][k], array2[j]);
					}
				}
			}
		}

		private const int MaxEmbeddedDepthConstant = 4;

		private bool alwaysBlock;

		private bool blockOnPublicComputers;

		private SortedDictionary<string, AttachmentPolicy.Level> fileTypeLevels;

		private SortedDictionary<string, AttachmentPolicy.Level> mimeTypeLevels;

		private AttachmentPolicy.Level treatUnknownTypeAs;

		public enum Level
		{
			Block = 1,
			ForceSave,
			Allow,
			Unknown
		}

		public enum TypeSignifier
		{
			File,
			Mime
		}

		private enum LevelPrecedence
		{
			First,
			Allow = 0,
			Block,
			ForceSave,
			Last = 2
		}
	}
}
