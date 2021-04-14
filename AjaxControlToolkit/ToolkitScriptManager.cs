using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace AjaxControlToolkit
{
	[Themeable(true)]
	public class ToolkitScriptManager : ScriptManager
	{
		public static ScriptBuckets CacheScriptBuckets
		{
			get
			{
				return ToolkitScriptManager.cacheScriptBuckets;
			}
		}

		[UrlProperty]
		public Uri CombineScriptsHandlerUrl { get; set; }

		public string ScriptsPath { get; set; }

		public string AssemblyVersion { get; set; }

		public string ResourceAssemblyNames
		{
			get
			{
				return this.resourceAssemblyNames;
			}
			set
			{
				this.resourceAssemblyNames = value;
			}
		}

		public ScriptBuckets ScriptsBuckets
		{
			get
			{
				return this.scriptBuckets;
			}
			set
			{
				if (ToolkitScriptManager.cacheScriptBuckets == null)
				{
					this.scriptBuckets = value;
					if (this.scriptBuckets != null)
					{
						if (!string.IsNullOrEmpty(this.ResourceAssemblyNames))
						{
							foreach (string text in this.ResourceAssemblyNames.Split(new char[]
							{
								','
							}))
							{
								Assembly assembly = Assembly.Load(new AssemblyName(text.Trim()));
								ToolkitScriptManager.AddScriptResourceFromAssembly(this.scriptBuckets, assembly);
							}
						}
						this.scriptBuckets.Initialize();
					}
					Interlocked.CompareExchange<ScriptBuckets>(ref ToolkitScriptManager.cacheScriptBuckets, this.scriptBuckets, null);
					return;
				}
				this.scriptBuckets = ToolkitScriptManager.cacheScriptBuckets;
			}
		}

		private static void AddScriptResourceFromAssembly(ScriptBuckets scriptBuckets, Assembly assembly)
		{
			object[] customAttributes = assembly.GetCustomAttributes(typeof(ScriptResourceAttribute), false);
			HashSet<string> hashSet = new HashSet<string>();
			foreach (CombinableScripts combinableScripts in scriptBuckets)
			{
				hashSet.Add(combinableScripts.Alias.ToLowerInvariant());
			}
			foreach (object obj in customAttributes)
			{
				ScriptResourceAttribute scriptResourceAttribute = (ScriptResourceAttribute)obj;
				string typeName = scriptResourceAttribute.TypeName;
				string item = typeName.ToLowerInvariant();
				if (!hashSet.Contains(item))
				{
					hashSet.Add(item);
					scriptBuckets.Add(new CombinableScripts
					{
						Alias = typeName,
						Scripts = new ScriptsEntries
						{
							new ScriptEntry(assembly.FullName, typeName + ".js", false)
						}
					});
				}
			}
		}

		public static void ExpandAndSort(IList<ScriptReference> scriptReferences)
		{
			List<ScriptReference> list = new List<ScriptReference>(scriptReferences.Count);
			List<CombinableScripts> list2 = new List<CombinableScripts>();
			Hashtable hashtable = new Hashtable(StringComparer.OrdinalIgnoreCase);
			foreach (ScriptReference scriptReference in scriptReferences)
			{
				CombinableScripts scriptByName = ToolkitScriptManager.CacheScriptBuckets.GetScriptByName(scriptReference.Name);
				if (scriptByName != null)
				{
					if (hashtable[scriptByName.Alias] == null)
					{
						list2.Add(scriptByName);
						hashtable[scriptByName.Alias] = true;
						if (scriptByName.DependsOn != null && scriptByName.DependsOn.Length > 0)
						{
							Stack stack = new Stack(scriptByName.DependsOn);
							while (stack.Count > 0)
							{
								string text = (string)stack.Pop();
								if (hashtable[text] == null)
								{
									CombinableScripts scriptByAlias = ToolkitScriptManager.CacheScriptBuckets.GetScriptByAlias(text, true);
									list2.Add(scriptByAlias);
									hashtable[text] = true;
									if (scriptByAlias.DependsOn != null && scriptByAlias.DependsOn.Length > 0)
									{
										for (int i = 0; i < scriptByAlias.DependsOn.Length; i++)
										{
											stack.Push(scriptByAlias.DependsOn[i]);
										}
									}
								}
							}
						}
					}
				}
				else
				{
					list.Add(scriptReference);
				}
			}
			if (list2.Count > 0)
			{
				list2.Sort((CombinableScripts left, CombinableScripts right) => right.Rank - left.Rank);
				list.AddRange(from x in list2
				select x.ToScriptReference());
			}
			if (scriptReferences.Count > 1 || scriptReferences.Count != list.Count)
			{
				scriptReferences.Clear();
				foreach (ScriptReference item in list)
				{
					scriptReferences.Add(item);
				}
			}
		}

		public static bool OutputCombineScriptResourcesFile(HttpContext context)
		{
			bool result = false;
			HttpRequest request = context.Request;
			string text = request.QueryString["resources"];
			if (!string.IsNullOrEmpty(text))
			{
				HttpResponse response = context.Response;
				response.ContentType = "application/x-javascript";
				HttpCachePolicy cache = response.Cache;
				cache.SetCacheability(HttpCacheability.Private);
				cache.VaryByParams["resources"] = true;
				cache.VaryByParams["v"] = true;
				cache.VaryByParams["c"] = true;
				cache.SetOmitVaryStar(true);
				cache.SetExpires(DateTime.Now.AddDays(365.0));
				cache.SetValidUntilExpires(true);
				DateTime lastWriteTime = new FileInfo(new Uri(Assembly.GetCallingAssembly().CodeBase).LocalPath).LastWriteTime;
				DateTime lastModified = (DateTime.UtcNow > lastWriteTime.ToUniversalTime()) ? lastWriteTime : DateTime.Now;
				cache.SetLastModified(lastModified);
				Stream stream = response.OutputStream;
				if (!request.Browser.IsBrowser("IE") || 6 < request.Browser.MajorVersion)
				{
					foreach (string b in (request.Headers["Accept-Encoding"] ?? string.Empty).ToUpperInvariant().Split(new char[]
					{
						','
					}))
					{
						if ("GZIP" == b)
						{
							response.AddHeader("Content-encoding", "gzip");
							stream = new GZipStream(stream, CompressionMode.Compress);
							break;
						}
						if ("DEFLATE" == b)
						{
							response.AddHeader("Content-encoding", "deflate");
							stream = new DeflateStream(stream, CompressionMode.Compress);
							break;
						}
					}
				}
				CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
				CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
				try
				{
					string text2 = request.QueryString["c"];
					if (!string.IsNullOrEmpty(text2))
					{
						CultureInfo cultureInfoByIetfLanguageTag = CultureInfo.GetCultureInfoByIetfLanguageTag(text2);
						Thread.CurrentThread.CurrentCulture = (Thread.CurrentThread.CurrentUICulture = cultureInfoByIetfLanguageTag);
					}
				}
				catch (ArgumentException)
				{
				}
				CombinableScripts scriptByAlias = ToolkitScriptManager.CacheScriptBuckets.GetScriptByAlias(HttpUtility.UrlDecode(text), false);
				if (scriptByAlias != null)
				{
					using (StreamWriter streamWriter = new StreamWriter(stream))
					{
						ToolkitScriptManager.WriteScriptsResources(scriptByAlias.Scripts, streamWriter);
						streamWriter.WriteLine("if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();");
					}
					result = true;
				}
				else
				{
					stream.Close();
					stream = null;
				}
				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
			}
			return result;
		}

		public void AddRuntimeScripts(CombinableScripts script)
		{
			if (script != null && this.scriptBuckets.GetScriptByAlias(script.Alias, false) == null)
			{
				this.scriptBuckets.InitializeRuntimeScript(script);
			}
		}

		public void CombineScript(string scriptName)
		{
			base.Scripts.Add(new ScriptReference(this.CreateCombineScriptPath(scriptName)));
		}

		protected static string QuoteString(string value)
		{
			StringBuilder stringBuilder = null;
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			int startIndex = 0;
			int num = 0;
			int i = 0;
			while (i < value.Length)
			{
				char c = value[i];
				if (c == '\r' || c == '\t' || c == '"' || c == '\'' || c == '<' || c == '>' || c == '\\' || c == '\n' || c == '\b' || c == '\f' || c < ' ')
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(value.Length + 5);
					}
					if (num > 0)
					{
						stringBuilder.Append(value, startIndex, num);
					}
					startIndex = i + 1;
					num = 0;
				}
				char c2 = c;
				if (c2 <= '"')
				{
					switch (c2)
					{
					case '\b':
						stringBuilder.Append("\\b");
						break;
					case '\t':
						stringBuilder.Append("\\t");
						break;
					case '\n':
						stringBuilder.Append("\\n");
						break;
					case '\v':
						goto IL_153;
					case '\f':
						stringBuilder.Append("\\f");
						break;
					case '\r':
						stringBuilder.Append("\\r");
						break;
					default:
						if (c2 != '"')
						{
							goto IL_153;
						}
						stringBuilder.Append("\\\"");
						break;
					}
				}
				else
				{
					if (c2 != '\'')
					{
						switch (c2)
						{
						case '<':
						case '>':
							break;
						case '=':
							goto IL_153;
						default:
							if (c2 != '\\')
							{
								goto IL_153;
							}
							stringBuilder.Append("\\\\");
							goto IL_167;
						}
					}
					ToolkitScriptManager.AppendCharAsUnicode(stringBuilder, c);
				}
				IL_167:
				i++;
				continue;
				IL_153:
				if (c < ' ')
				{
					ToolkitScriptManager.AppendCharAsUnicode(stringBuilder, c);
					goto IL_167;
				}
				num++;
				goto IL_167;
			}
			if (stringBuilder == null)
			{
				return value;
			}
			if (num > 0)
			{
				stringBuilder.Append(value, startIndex, num);
			}
			return stringBuilder.ToString();
		}

		protected static void AppendCharAsUnicode(StringBuilder builder, char c)
		{
			builder.Append("\\u");
			builder.AppendFormat(CultureInfo.InvariantCulture, "{0:x4}", new object[]
			{
				(int)c
			});
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.Page.PreRenderComplete += this.OnPagePreRenderComplete;
			this.DataBind();
		}

		protected override void OnResolveScriptReference(ScriptReferenceEventArgs e)
		{
			base.OnResolveScriptReference(e);
			ScriptReference script = e.Script;
			ScriptEntry item = new ScriptEntry(script);
			if (this.ScriptsBuckets != null)
			{
				foreach (CombinableScripts combinableScripts in this.ScriptsBuckets)
				{
					if (combinableScripts.Scripts.Contains(item))
					{
						script.Name = string.Empty;
						script.Assembly = string.Empty;
						script.Path = base.ResolveUrl(this.ScriptsPath + combinableScripts.Alias.ToLower() + ".js");
						if (combinableScripts.HasScriptResources)
						{
							this.AddScriptResourceLink(combinableScripts.Alias);
						}
						break;
					}
				}
			}
		}

		private static void WriteScriptsResources(ScriptsEntries scriptEntries, TextWriter outputWriter)
		{
			outputWriter.WriteLine("//START ");
			foreach (ScriptEntry scriptEntry in scriptEntries)
			{
				if (!scriptEntry.SkipScriptResources)
				{
					Assembly assembly = scriptEntry.LoadAssembly();
					foreach (ScriptResourceAttribute scriptResourceAttribute in assembly.GetCustomAttributes(typeof(ScriptResourceAttribute), false))
					{
						if (scriptResourceAttribute.ScriptName == scriptEntry.Name)
						{
							outputWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}={{", new object[]
							{
								scriptResourceAttribute.TypeName
							}));
							string text = scriptResourceAttribute.ScriptResourceName;
							if (text.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
							{
								text = text.Substring(0, text.Length - 10);
							}
							ResourceManager resourceManager = new ResourceManager(text, assembly);
							using (ResourceSet resourceSet = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true))
							{
								bool flag = true;
								CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
								foreach (object obj in resourceSet)
								{
									DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
									if (!flag)
									{
										outputWriter.Write(",");
									}
									string text2 = (string)dictionaryEntry.Key;
									string @string = resourceManager.GetString(text2, currentUICulture);
									outputWriter.Write(string.Format(CultureInfo.InvariantCulture, "\"{0}\":\"{1}\"", new object[]
									{
										ToolkitScriptManager.QuoteString(text2),
										ToolkitScriptManager.QuoteString(@string)
									}));
									flag = false;
								}
							}
							outputWriter.WriteLine("};");
						}
					}
				}
			}
			outputWriter.WriteLine("//END ");
		}

		private void OnPagePreRenderComplete(object sender, EventArgs e)
		{
			foreach (string text in this.scriptResources)
			{
				string url = this.CreateCombineScriptPath(text);
				this.Page.ClientScript.RegisterClientScriptInclude(text, url);
			}
		}

		private string CreateCombineScriptPath(string scriptResource)
		{
			string text = this.Page.Request.Path;
			if (this.CombineScriptsHandlerUrl != null)
			{
				text = base.ResolveUrl(string.Format("~/{0}/scripts/{1}", this.AssemblyVersion, this.CombineScriptsHandlerUrl.ToString()));
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}?{1}={2}&{3}={4}&{5}={6}", new object[]
			{
				text,
				"resources",
				HttpUtility.UrlEncode(scriptResource),
				"v",
				this.AssemblyVersion,
				"c",
				CultureInfo.CurrentUICulture.IetfLanguageTag
			});
		}

		private void AddScriptResourceLink(string scriptAlias)
		{
			if (!this.scriptResources.Contains(scriptAlias))
			{
				this.scriptResources.Add(scriptAlias);
			}
		}

		private const string CombinedScriptResourcesParamName = "resources";

		private const string VersionParamName = "v";

		private const string CultureParamName = "c";

		private static ScriptBuckets cacheScriptBuckets;

		private string resourceAssemblyNames;

		private ScriptBuckets scriptBuckets;

		private List<string> scriptResources = new List<string>(1);
	}
}
