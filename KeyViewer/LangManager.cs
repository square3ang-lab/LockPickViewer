using System;
using System.Collections.Generic;
using System.Linq;
using TinyJson;

namespace KeyViewer;

public class LangManager
{
	public Languages langs;

	private readonly Dictionary<string, Language[]> langDict;

	private Dictionary<string, string> globalDict;

	private Dictionary<string, Language> globalDict2;

	private Dictionary<string, string> curDict;

	private readonly Dictionary<string, string> engDict;

	public LangManager(string langJson)
	{
		langs = langJson.FromJson<Languages>();
		langDict = new Dictionary<string, Language[]>();
		langDict["Global"] = langs.Global;
		langDict["KeyViewer"] = langs.KeyViewer;
		globalDict = new Dictionary<string, string>();
		globalDict2 = langDict["Global"].ToDictionary((Language l) => l.Key);
		engDict = langDict["KeyViewer"].ToDictionary((Language l) => l.Key, (Language l) => l.English);
		curDict = new Dictionary<string, string>();
	}

	public string GetGlobal(string key)
	{
		return globalDict[key];
	}

	public string GetLanguageName(LanguageType lang)
	{
		return lang switch
		{
			LanguageType.English => globalDict2["LANGUAGE_NAME"].English, 
			LanguageType.French => globalDict2["LANGUAGE_NAME"].French, 
			LanguageType.Korean => globalDict2["LANGUAGE_NAME"].Korean, 
			LanguageType.Polish => globalDict2["LANGUAGE_NAME"].Polish, 
			LanguageType.Spanish => globalDict2["LANGUAGE_NAME"].Spanish, 
			LanguageType.Vietnamese => globalDict2["LANGUAGE_NAME"].Vietnamese, 
			LanguageType.SimplifiedChinese => globalDict2["LANGUAGE_NAME"].SimplifiedChinese, 
			_ => throw new InvalidOperationException("Invalid Language!"), 
		};
	}

	public void ChangeLanguage(LanguageType lang)
	{
		switch (lang)
		{
		case LanguageType.English:
			globalDict = langDict["Global"].ToDictionary((Language l) => l.Key, (Language l) => l.English);
			curDict = engDict;
			break;
		case LanguageType.French:
			globalDict = langDict["Global"].ToDictionary((Language l) => l.Key, (Language l) => l.French);
			curDict = langDict["KeyViewer"].ToDictionary((Language l) => l.Key, (Language l) => l.French);
			break;
		case LanguageType.Korean:
			globalDict = langDict["Global"].ToDictionary((Language l) => l.Key, (Language l) => l.Korean);
			curDict = langDict["KeyViewer"].ToDictionary((Language l) => l.Key, (Language l) => l.Korean);
			break;
		case LanguageType.Polish:
			globalDict = langDict["Global"].ToDictionary((Language l) => l.Key, (Language l) => l.Polish);
			curDict = langDict["KeyViewer"].ToDictionary((Language l) => l.Key, (Language l) => l.Polish);
			break;
		case LanguageType.Spanish:
			globalDict = langDict["Global"].ToDictionary((Language l) => l.Key, (Language l) => l.Spanish);
			curDict = langDict["KeyViewer"].ToDictionary((Language l) => l.Key, (Language l) => l.Spanish);
			break;
		case LanguageType.Vietnamese:
			globalDict = langDict["Global"].ToDictionary((Language l) => l.Key, (Language l) => l.Vietnamese);
			curDict = langDict["KeyViewer"].ToDictionary((Language l) => l.Key, (Language l) => l.Vietnamese);
			break;
		case LanguageType.SimplifiedChinese:
			globalDict = langDict["Global"].ToDictionary((Language l) => l.Key, (Language l) => l.SimplifiedChinese);
			curDict = langDict["KeyViewer"].ToDictionary((Language l) => l.Key, (Language l) => l.SimplifiedChinese);
			break;
		default:
			throw new InvalidOperationException("Invalid Language!");
		}
	}

	public string GetString(string key)
	{
		string text = curDict[key];
		return (text == "ul") ? engDict[key] : text;
	}
}
