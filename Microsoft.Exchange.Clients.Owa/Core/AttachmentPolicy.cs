using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class AttachmentPolicy
	{
		internal AttachmentPolicy(string[] blockFileTypes, string[] blockMimeTypes, string[] forceSaveFileTypes, string[] forceSaveMimeTypes, string[] allowFileTypes, string[] allowMimeTypes, AttachmentPolicy.Level treatUnknownTypeAs, bool directFileAccessOnPublicComputersEnabled, bool directFileAccessOnPrivateComputersEnabled, bool forceWebReadyDocumentViewingFirstOnPublicComputers, bool forceWebReadyDocumentViewingFirstOnPrivateComputers, bool webReadyDocumentViewingOnPublicComputersEnabled, bool webReadyDocumentViewingOnPrivateComputersEnabled, string[] webReadyFileTypes, string[] webReadyMimeTypes, string[] webReadyDocumentViewingSupportedFileTypes, string[] webReadyDocumentViewingSupportedMimeTypes, bool webReadyDocumentViewingForAllSupportedTypes)
		{
			this.treatUnknownTypeAs = treatUnknownTypeAs;
			this.directFileAccessOnPublicComputersEnabled = directFileAccessOnPublicComputersEnabled;
			this.directFileAccessOnPrivateComputersEnabled = directFileAccessOnPrivateComputersEnabled;
			this.forceWebReadyDocumentViewingFirstOnPublicComputers = forceWebReadyDocumentViewingFirstOnPublicComputers;
			this.forceWebReadyDocumentViewingFirstOnPrivateComputers = forceWebReadyDocumentViewingFirstOnPrivateComputers;
			this.webReadyDocumentViewingOnPublicComputersEnabled = webReadyDocumentViewingOnPublicComputersEnabled;
			this.webReadyDocumentViewingOnPrivateComputersEnabled = webReadyDocumentViewingOnPrivateComputersEnabled;
			this.webReadyFileTypes = webReadyFileTypes;
			Array.Sort<string>(this.webReadyFileTypes);
			this.webReadyMimeTypes = webReadyMimeTypes;
			Array.Sort<string>(this.webReadyMimeTypes);
			this.webReadyDocumentViewingSupportedFileTypes = webReadyDocumentViewingSupportedFileTypes;
			Array.Sort<string>(this.webReadyDocumentViewingSupportedFileTypes);
			this.webReadyDocumentViewingSupportedMimeTypes = webReadyDocumentViewingSupportedMimeTypes;
			Array.Sort<string>(this.webReadyDocumentViewingSupportedMimeTypes);
			this.webReadyDocumentViewingForAllSupportedTypes = webReadyDocumentViewingForAllSupportedTypes;
			this.fileTypeLevels = AttachmentPolicy.LoadDictionary(blockFileTypes, forceSaveFileTypes, allowFileTypes);
			this.mimeTypeLevels = AttachmentPolicy.LoadDictionary(blockMimeTypes, forceSaveMimeTypes, allowMimeTypes);
		}

		private static SortedDictionary<string, AttachmentPolicy.Level> LoadDictionary(string[] block, string[] forceSave, string[] allow)
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
			SortedDictionary<string, AttachmentPolicy.Level> sortedDictionary = new SortedDictionary<string, AttachmentPolicy.Level>(StringComparer.OrdinalIgnoreCase);
			for (int j = 0; j <= 2; j++)
			{
				for (int k = 0; k < array[j].Length; k++)
				{
					string key = array[j][k];
					if (!sortedDictionary.ContainsKey(key))
					{
						sortedDictionary.Add(key, array2[j]);
					}
				}
			}
			return sortedDictionary;
		}

		public AttachmentPolicy.Level GetLevel(string attachmentType, AttachmentPolicy.TypeSignifier typeSignifier)
		{
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

		public bool Contains(string key, AttachmentPolicy.LookupTypeSignifer signifer)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			switch (signifer)
			{
			case AttachmentPolicy.LookupTypeSignifer.FileArray:
				return Array.BinarySearch<string>(this.webReadyFileTypes, key, StringComparer.OrdinalIgnoreCase) >= 0;
			case AttachmentPolicy.LookupTypeSignifer.MimeArray:
				return Array.BinarySearch<string>(this.webReadyMimeTypes, key, StringComparer.OrdinalIgnoreCase) >= 0;
			case AttachmentPolicy.LookupTypeSignifer.SupportedFileArray:
				return Array.BinarySearch<string>(this.webReadyDocumentViewingSupportedFileTypes, key, StringComparer.OrdinalIgnoreCase) >= 0;
			case AttachmentPolicy.LookupTypeSignifer.SupportedMimeArray:
				return Array.BinarySearch<string>(this.webReadyDocumentViewingSupportedMimeTypes, key, StringComparer.OrdinalIgnoreCase) >= 0;
			default:
				return false;
			}
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

		public static int MaxEmbeddedDepth
		{
			get
			{
				return 16;
			}
		}

		public AttachmentPolicy.Level TreatUnknownTypeAs
		{
			get
			{
				return this.treatUnknownTypeAs;
			}
		}

		public bool DirectFileAccessEnabled
		{
			get
			{
				if (UserContextManager.GetUserContext().IsPublicLogon)
				{
					return this.directFileAccessOnPublicComputersEnabled;
				}
				return this.directFileAccessOnPrivateComputersEnabled;
			}
		}

		public bool WebReadyDocumentViewingEnable
		{
			get
			{
				if (UserContextManager.GetUserContext().IsPublicLogon)
				{
					return this.webReadyDocumentViewingOnPublicComputersEnabled;
				}
				return this.webReadyDocumentViewingOnPrivateComputersEnabled;
			}
		}

		public bool ForceWebReadyDocumentViewingFirst
		{
			get
			{
				if (UserContextManager.GetUserContext().IsPublicLogon)
				{
					return this.forceWebReadyDocumentViewingFirstOnPublicComputers;
				}
				return this.forceWebReadyDocumentViewingFirstOnPrivateComputers;
			}
		}

		public bool WebReadyDocumentViewingForAllSupportedTypes
		{
			get
			{
				return this.webReadyDocumentViewingForAllSupportedTypes;
			}
		}

		public string[] WebReadyFileTypes
		{
			get
			{
				return this.webReadyFileTypes;
			}
		}

		public string[] WebReadyDocumentViewingSupportedFileTypes
		{
			get
			{
				return this.webReadyDocumentViewingSupportedFileTypes;
			}
		}

		public SortedDictionary<string, AttachmentPolicy.Level>.Enumerator FileTypeLevels
		{
			get
			{
				return this.fileTypeLevels.GetEnumerator();
			}
		}

		public SortedDictionary<string, AttachmentPolicy.Level>.Enumerator MimeTypeLevels
		{
			get
			{
				return this.mimeTypeLevels.GetEnumerator();
			}
		}

		private const int MaxEmbeddedMessageDepth = 16;

		private AttachmentPolicy.Level treatUnknownTypeAs;

		private bool directFileAccessOnPrivateComputersEnabled;

		private bool directFileAccessOnPublicComputersEnabled;

		private bool webReadyDocumentViewingOnPrivateComputersEnabled;

		private bool webReadyDocumentViewingOnPublicComputersEnabled;

		private bool forceWebReadyDocumentViewingFirstOnPrivateComputers;

		private bool forceWebReadyDocumentViewingFirstOnPublicComputers;

		private bool webReadyDocumentViewingForAllSupportedTypes;

		private string[] webReadyFileTypes;

		private string[] webReadyMimeTypes;

		private string[] webReadyDocumentViewingSupportedMimeTypes;

		private string[] webReadyDocumentViewingSupportedFileTypes;

		private SortedDictionary<string, AttachmentPolicy.Level> fileTypeLevels;

		private SortedDictionary<string, AttachmentPolicy.Level> mimeTypeLevels;

		public enum TypeSignifier
		{
			File,
			Mime
		}

		public enum LookupTypeSignifer
		{
			FileArray,
			MimeArray,
			SupportedFileArray,
			SupportedMimeArray
		}

		public enum Level
		{
			None,
			Block,
			ForceSave,
			Allow,
			Unknown
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
