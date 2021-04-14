using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System
{
	internal class ConfigTreeParser : BaseConfigHandler
	{
		internal ConfigNode Parse(string fileName, string configPath)
		{
			return this.Parse(fileName, configPath, false);
		}

		[SecuritySafeCritical]
		internal ConfigNode Parse(string fileName, string configPath, bool skipSecurityStuff)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			this.fileName = fileName;
			if (configPath[0] == '/')
			{
				this.treeRootPath = configPath.Substring(1).Split(new char[]
				{
					'/'
				});
				this.pathDepth = this.treeRootPath.Length - 1;
				this.bNoSearchPath = false;
			}
			else
			{
				this.treeRootPath = new string[1];
				this.treeRootPath[0] = configPath;
				this.bNoSearchPath = true;
			}
			if (!skipSecurityStuff)
			{
				new FileIOPermission(FileIOPermissionAccess.Read, Path.GetFullPathInternal(fileName)).Demand();
			}
			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
			try
			{
				base.RunParser(fileName);
			}
			catch (FileNotFoundException)
			{
				throw;
			}
			catch (DirectoryNotFoundException)
			{
				throw;
			}
			catch (UnauthorizedAccessException)
			{
				throw;
			}
			catch (FileLoadException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				string invalidSyntaxMessage = this.GetInvalidSyntaxMessage();
				throw new ApplicationException(invalidSyntaxMessage, innerException);
			}
			return this.rootNode;
		}

		public override void NotifyEvent(ConfigEvents nEvent)
		{
		}

		public override void BeginChildren(int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength)
		{
			if (!this.parsing && !this.bNoSearchPath && this.depth == this.searchDepth + 1 && string.Compare(text, this.treeRootPath[this.searchDepth], StringComparison.Ordinal) == 0)
			{
				this.searchDepth++;
			}
		}

		public override void EndChildren(int fEmpty, int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength)
		{
			this.lastProcessed = text;
			this.lastProcessedEndElement = true;
			if (this.parsing)
			{
				if (this.currentNode == this.rootNode)
				{
					this.parsing = false;
				}
				this.currentNode = this.currentNode.Parent;
				return;
			}
			if (nType == ConfigNodeType.Element)
			{
				if (this.depth == this.searchDepth && string.Compare(text, this.treeRootPath[this.searchDepth - 1], StringComparison.Ordinal) == 0)
				{
					this.searchDepth--;
					this.depth--;
					return;
				}
				this.depth--;
			}
		}

		public override void Error(int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength)
		{
		}

		public override void CreateNode(int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength)
		{
			if (nType != ConfigNodeType.Element)
			{
				if (nType == ConfigNodeType.PCData && this.currentNode != null)
				{
					this.currentNode.Value = text;
				}
				return;
			}
			this.lastProcessed = text;
			this.lastProcessedEndElement = false;
			if (!this.parsing && (!this.bNoSearchPath || string.Compare(text, this.treeRootPath[0], StringComparison.OrdinalIgnoreCase) != 0) && (this.depth != this.searchDepth || this.searchDepth != this.pathDepth || string.Compare(text, this.treeRootPath[this.pathDepth], StringComparison.OrdinalIgnoreCase) != 0))
			{
				this.depth++;
				return;
			}
			this.parsing = true;
			ConfigNode configNode = this.currentNode;
			this.currentNode = new ConfigNode(text, configNode);
			if (this.rootNode == null)
			{
				this.rootNode = this.currentNode;
				return;
			}
			configNode.AddChild(this.currentNode);
		}

		public override void CreateAttribute(int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength)
		{
			if (!this.parsing)
			{
				return;
			}
			if (nType == ConfigNodeType.Attribute)
			{
				this.attributeEntry = this.currentNode.AddAttribute(text, "");
				this.key = text;
				return;
			}
			if (nType == ConfigNodeType.PCData)
			{
				this.currentNode.ReplaceAttribute(this.attributeEntry, this.key, text);
				return;
			}
			string invalidSyntaxMessage = this.GetInvalidSyntaxMessage();
			throw new ApplicationException(invalidSyntaxMessage);
		}

		private string GetInvalidSyntaxMessage()
		{
			string text = null;
			if (this.lastProcessed != null)
			{
				text = (this.lastProcessedEndElement ? "</" : "<") + this.lastProcessed + ">";
			}
			return Environment.GetResourceString("XML_Syntax_InvalidSyntaxInFile", new object[]
			{
				this.fileName,
				text
			});
		}

		private ConfigNode rootNode;

		private ConfigNode currentNode;

		private string fileName;

		private int attributeEntry;

		private string key;

		private string[] treeRootPath;

		private bool parsing;

		private int depth;

		private int pathDepth;

		private int searchDepth;

		private bool bNoSearchPath;

		private string lastProcessed;

		private bool lastProcessedEndElement;
	}
}
