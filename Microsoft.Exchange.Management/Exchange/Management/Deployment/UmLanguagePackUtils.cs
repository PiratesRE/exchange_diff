using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.Management.Deployment
{
	public static class UmLanguagePackUtils
	{
		private static TargetPlatform GetTargetPlatform()
		{
			if (IntPtr.Size != 8)
			{
				return TargetPlatform.X86;
			}
			return TargetPlatform.X64;
		}

		private static Dictionary<CultureInfo, UmLanguagePack> BuildSupportedUmLanguages()
		{
			Dictionary<CultureInfo, UmLanguagePack> dictionary = new Dictionary<CultureInfo, UmLanguagePack>();
			CultureInfo cultureInfo = new CultureInfo("ca-ES");
			UmLanguagePack value = new UmLanguagePack(cultureInfo.Name, new Guid("8bf38132-c5c7-4c66-bc8e-9b3df8280dab"), new Guid("8bf38164-c5c7-4c66-bc8e-9b3df8280dab"), new Guid("55d56947-b976-4e27-822b-e87feffb35f2"), new Guid("33970c23-6d7a-4827-a843-1286ac9b0399"), new Guid("674881b1-6a04-4bce-aa6b-85e8a3494631"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("ca");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("da-DK");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("893d0032-9182-44c9-80a6-b80166ad772d"), new Guid("893d0064-9182-44c9-80a6-b80166ad772d"), new Guid("18b4b2e0-6a0d-4bac-99eb-843f2c290e07"), new Guid("ce89ff12-9d18-421b-b1f2-20a802c330d4"), new Guid("d05fa8b5-7ac9-4844-b406-2de575fb4d9e"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("da");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("de-DE");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("8ef42e32-6de1-47e6-a07f-0e77b83f98a6"), new Guid("8ef42e64-6de1-47e6-a07f-0e77b83f98a6"), new Guid("955f43d9-38c4-4c22-bee3-1a6c63f968fa"), new Guid("4ca929a0-9076-4ff9-a8d1-508215a352a3"), new Guid("acfcc7b5-c028-40ae-a5f5-9778b41f22a2"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("de");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("en-AU");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("56fdb432-0cd2-434c-bd42-41b9fe06e9a7"), new Guid("56fdb464-0cd2-434c-bd42-41b9fe06e9a7"), new Guid("fa19a2b8-9a24-49b0-a51c-cf4a6b4b2b62"), new Guid("d35327f1-4aa1-46c7-9a1c-e808d2107e28"), new Guid("c58363bd-6cb1-447f-9fee-5f9542981d15"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("en-CA");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("6a0d2a32-9cf1-4b18-b0d2-6a72f0df4c7c"), new Guid("6a0d2a64-9cf1-4b18-b0d2-6a72f0df4c7c"), new Guid("0c96ed3f-83e2-4917-89dc-7837dc775fec"), new Guid("5cbdad0b-9803-49cc-af24-b5dcbb72d555"), new Guid("6483cae5-a44c-4cc4-8dd2-4f73c00471ec"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("en-GB");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("d0ff1f32-221f-42ba-81bd-a06e3b6c8a91"), new Guid("d0ff1f64-221f-42ba-81bd-a06e3b6c8a91"), new Guid("e0d13850-f97c-4b30-9f05-862299ce8da5"), new Guid("557c3d30-9590-4db4-8c7e-1e22f9b5d7ef"), new Guid("9f1b2d5b-e203-4a4f-9ebd-af04489ee058"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("en-IN");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("b4823232-eb6e-4343-a03e-0f8807757b8b"), new Guid("b4823264-eb6e-4343-a03e-0f8807757b8b"), new Guid("3b06ac90-de68-44a9-95eb-0a3c1af1514f"), new Guid("fa675732-f4b9-485b-9911-59c312fa98b8"), new Guid("a9ad7528-7979-4472-8d61-a9f2994c8aad"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("en-US");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("CEF60932-21AE-47E0-93C6-611AA8941B7F"), new Guid("CEF60964-21AE-47E0-93C6-611AA8941B7F"), new Guid("66d57636-bd4b-402f-9e7d-5e89c28c8136"), new Guid("b07da010-66cf-40a7-908f-f6482219c57f"), new Guid("8466eaed-7024-4aee-9d13-f3a55b98d114"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("en");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("es-ES");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("70ae4e32-1944-4c7f-9ddd-55ebc65abc4b"), new Guid("70ae4e64-1944-4c7f-9ddd-55ebc65abc4b"), new Guid("5d4a25b6-3a4e-409b-90fa-ede99e2006b4"), new Guid("1599e1fd-ae5b-46ba-8297-baed95be202e"), new Guid("8a732901-9531-4cc2-8d5b-9cba1d8de4fd"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("es");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("es-MX");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("4b78e932-d62f-4cda-b26e-59f7e4e460cc"), new Guid("4b78e964-d62f-4cda-b26e-59f7e4e460cc"), new Guid("be94188a-ca4f-4ac7-a1b3-52d37882c30d"), new Guid("98da3e6f-1076-4ade-937b-7db8df458a62"), new Guid("01c2594b-fa78-4c33-a9b7-6090a5ef7e90"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("fi-FI");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("acde0332-f85b-475d-8a8d-30ebcd002c81"), new Guid("acde0364-f85b-475d-8a8d-30ebcd002c81"), new Guid("e3b7dbc7-7551-4e61-9b0d-fe660cffc4fc"), new Guid("d904cd78-b52d-42ea-9925-7430a04a7e4f"), new Guid("b9e961a1-9724-4a01-b3de-bb380375c777"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("fi");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("fr-CA");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("62e30632-4a9e-4bed-b91c-ba26c4ea96a6"), new Guid("62e30664-4a9e-4bed-b91c-ba26c4ea96a6"), new Guid("58de670f-4977-4a23-9d2e-8c82a2072920"), new Guid("6b4b5941-4b59-43d7-ae71-efc0ba7a3816"), new Guid("7418aec8-8d0f-4242-aec5-b9a67c403686"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("fr-FR");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("690ee732-68dc-4890-a836-6275f4afaac6"), new Guid("690ee764-68dc-4890-a836-6275f4afaac6"), new Guid("4d2ddb98-1fe6-4cfe-bcfd-efe27ff24fae"), new Guid("1019f3eb-8352-4001-8d83-b722727d2e7b"), new Guid("9b9d928f-97d5-4d95-9a71-ee9b1805bade"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("fr");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("it-IT");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("a68f8f32-5afd-4ff7-8864-f99f31d256a8"), new Guid("a68f8f64-5afd-4ff7-8864-f99f31d256a8"), new Guid("9267d7e7-5872-4cb1-b4e3-377f4ca272d0"), new Guid("dd57f4a3-f65f-4df0-b2f2-598ddb2c43cd"), new Guid("71c2443b-9b9d-47b5-a54b-f09ed52f29aa"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("it");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("ja-JP");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("e8510832-f1ed-4224-9219-265b395fc47d"), new Guid("e8510864-f1ed-4224-9219-265b395fc47d"), new Guid("a06f3ea5-7c55-4505-8982-534ba05f49be"), new Guid("aeb24132-04cb-44ab-8add-05040e59d26f"), new Guid("c84f5451-2c75-48e0-b2bd-773a096c655a"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("ja");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("ko-KR");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("1aef8932-d585-4ac2-a27d-eaa3b0bd9ccb"), new Guid("1aef8964-d585-4ac2-a27d-eaa3b0bd9ccb"), new Guid("1d8f6891-9b7f-4f08-a54e-c568d8c33276"), new Guid("5ca6ae2e-07be-4554-9ebf-65f0ecd7eb63"), new Guid("db25c483-67d0-417a-953f-09264e85561c"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("ko");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("nb-NO");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("0fb69632-7689-44f7-9aef-9877e2c73cdf"), new Guid("0fb69664-7689-44f7-9aef-9877e2c73cdf"), new Guid("49b7e67f-5e62-4988-a4f4-6c54b9e814eb"), new Guid("7d11b115-5f51-4df0-be68-bba49338a80a"), new Guid("b57d764e-32aa-48de-baf6-e9004ab9e1de"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("no");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("nl-NL");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("3d9a6f32-54be-4858-a16f-c7e2d426b880"), new Guid("3d9a6f64-54be-4858-a16f-c7e2d426b880"), new Guid("2cbab07e-4865-40f0-9d6a-efa350420166"), new Guid("3b4c9ec5-ca12-48d4-93d0-7019c746e12f"), new Guid("d1885f96-20f9-4a7a-a8c3-133b32f4d930"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("nl");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("pl-PL");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("117cff32-0870-4d85-a04a-60b6b858213e"), new Guid("117cff64-0870-4d85-a04a-60b6b858213e"), new Guid("befb9378-5e88-4266-8eb1-c92869449885"), new Guid("25a42d4b-074b-409e-bd33-3903f1d02652"), new Guid("6f2accd6-b1bc-410d-80bd-0abb24d8d880"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("pl");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("pt-BR");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("e84da432-b5c8-40a3-a4f7-b39cea757ff6"), new Guid("e84da464-b5c8-40a3-a4f7-b39cea757ff6"), new Guid("f6b5eb21-0abf-487c-b9a9-d9db259c4403"), new Guid("c68ed6f5-4660-415c-b28e-d728c48e51fe"), new Guid("4812e2fc-5a25-4220-bf5a-d50238b63151"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("pt");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("pt-PT");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("8262b932-e6e9-4640-b5c5-bbe3a6dad608"), new Guid("8262b964-e6e9-4640-b5c5-bbe3a6dad608"), new Guid("dafe30c6-c638-4505-9372-2ecd1a1b317c"), new Guid("5a92ded3-1874-4147-bfd2-857186a5eb85"), new Guid("cd7c1faf-67ac-4965-8581-e65ac4117867"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("ru-RU");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("70527a32-82db-4049-9c86-ec191d1e11a7"), new Guid("70527a64-82db-4049-9c86-ec191d1e11a7"), new Guid("9419b7ea-6a4b-4a57-8e2a-3bdd4676118f"), new Guid("7f7a2c24-cb9c-4983-85be-147f789c8bab"), new Guid("90aee28f-2fde-4ed6-9d36-58c6329e77fb"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("ru");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("sv-SE");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("75d54a32-bc87-4e05-b930-319884653310"), new Guid("75d54a64-bc87-4e05-b930-319884653310"), new Guid("12c43d71-15a1-4f83-9d4d-e3134ae6ffd6"), new Guid("7eb1dee0-8a60-4665-9d17-8749631580c7"), new Guid("1351792b-d8f3-4da3-b8d8-b8d005ab3d10"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("sv");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("zh-CN");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("99252332-4cd8-47f4-9213-8802112bfe5c"), new Guid("99252364-4cd8-47f4-9213-8802112bfe5c"), new Guid("bad2a75a-1708-47ba-a498-20890d2c78a7"), new Guid("3ddb708a-6403-466a-8720-efad37ac66fc"), new Guid("44b3785f-9f8b-46a9-ad46-21b7ac49d086"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("zh-HANS");
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("zh-HK");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("778b3232-bb6b-4117-a3a7-f8327e2e0a82"), new Guid("778b3264-bb6b-4117-a3a7-f8327e2e0a82"), new Guid("6baa03f9-b2e5-40eb-8871-703ff0046e9d"), new Guid("811f8906-f800-4d38-a845-9eeb226e6e45"), new Guid("0625f1d2-84a0-490c-afb1-7db67cb893d6"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("zh-TW");
			value = new UmLanguagePack(cultureInfo.Name, new Guid("77740132-4a4c-4e2d-8353-7cbef3b37430"), new Guid("77740164-4a4c-4e2d-8353-7cbef3b37430"), new Guid("28292b72-cf8a-4915-a5f5-07ff1e44c6f5"), new Guid("10daf7df-c84f-444a-aa49-e27eb6d4a16c"), new Guid("6d38b9e1-d13d-439d-8309-000b55d704d8"));
			dictionary.Add(cultureInfo, value);
			cultureInfo = new CultureInfo("zh-HANT");
			dictionary.Add(cultureInfo, value);
			return dictionary;
		}

		public static UmLanguagePack GetUmLanguagePack(CultureInfo language)
		{
			UmLanguagePack result;
			try
			{
				result = UmLanguagePackUtils.umLanguages[language];
			}
			catch (KeyNotFoundException innerException)
			{
				throw new UnSupportedUMLanguageException(language.Name, innerException);
			}
			return result;
		}

		public static string GetTeleProductCode(string language)
		{
			CultureInfo language2 = new CultureInfo(language);
			UmLanguagePack umLanguagePack = UmLanguagePackUtils.GetUmLanguagePack(language2);
			return umLanguagePack.TeleProductCode.ToString("B");
		}

		public static readonly TargetPlatform CurrentTargetPlatform = UmLanguagePackUtils.GetTargetPlatform();

		private static Dictionary<CultureInfo, UmLanguagePack> umLanguages = UmLanguagePackUtils.BuildSupportedUmLanguages();
	}
}
