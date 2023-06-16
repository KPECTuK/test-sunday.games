using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Sun.Base
{
	public static class ExtensionsStrings
	{
		/// <summary> spotted result: success </summary>
		public const string LOG_SPR_SUCCESS_S = "<color=lime>success</color>";
		/// <summary> spotted result: fail </summary>
		public const string LOG_SPR_FAIL_S = "<color=red>fail</color>";
		/// <summary> spotted result: fail </summary>
		public const string LOG_SPR_EMPTY_S = "<color=red>(null)</color>";

		private static int _indent;

		private class Indent : IDisposable
		{
			public Indent()
			{
				_indent += 2;
			}

			public void Dispose()
			{
				_indent -= 2;
			}
		}

		public static IDisposable WithGlobalIndent()
		{
			return new Indent();
		}

		private static int _prefixSize;

		private static string LogPrefix(string source)
		{
			var stack = new StackTrace();
			var frame = stack.GetFrame(2);
			var info = frame.GetMethod();
			var prefix = $"{info.DeclaringType?.NameNice() ?? "unknown"}.{info.Name}";
			_prefixSize = _prefixSize > prefix.Length ? _prefixSize : prefix.Length;
			var builder = new StringBuilder()
				.Append("<color=cyan>[")
				.AppendFormat($"{{0,{_prefixSize}}}", prefix)
				.Append("(..)]</color> ")
				.Append(new string(' ', _indent))
				.Append(source);
			return builder.ToString();
		}

		private static string LogWrap(string source)
		{
			var isDebug = source.Contains("[?]") || source.Contains("[DEBUG]");
			return isDebug ? $"<color=magenta>{source}</color>" : source;
		}

		private static string LogFilter(string source)
		{
			source ??= LOG_SPR_EMPTY_S;
			return source;
		}

		private static StringBuilder LogPrefix(StringBuilder source)
		{
			var stack = new StackTrace();
			var frame = stack.GetFrame(2);
			var info = frame.GetMethod();
			var prefix = $"{info.DeclaringType?.Name ?? "unknown"}.{info.Name}";
			_prefixSize = _prefixSize > prefix.Length ? _prefixSize : prefix.Length;
			var builder = new StringBuilder()
				.Append("<color=cyan>[")
				.AppendFormat($"{{0,{_prefixSize}}}", prefix)
				.Append("(..)]</color> ");
			return source.Insert(0, builder.ToString());
		}

		private static StringBuilder LogWrap(StringBuilder source)
		{
			const string CASE_01 = "[?]";
			var case01Num = 0;
			const string CASE_02 = "[DEBUG]";
			var case02Num = 0;

			for(var index = 0; index < source.Length; index++)
			{
				if(case01Num != CASE_01.Length && source[index] == CASE_01[case01Num])
				{
					case01Num++;
				}
				if(case02Num != CASE_02.Length && source[index] == CASE_02[case02Num])
				{
					case02Num++;
				}
			}

			var isDebug =
				case01Num == CASE_01.Length ||
				case02Num == CASE_02.Length;

			if(isDebug)
			{
				source.Insert(0, "<color=magenta>").Append("</color>");
			}

			return source;
		}

		private static StringBuilder LogFilter(StringBuilder source)
		{
			source ??= new StringBuilder(LOG_SPR_EMPTY_S);
			return source;
		}

		[Conditional("DEBUG")]
		public static void Log(this StringBuilder source)
		{
			source = LogFilter(source);
			source = LogPrefix(source);
			source = LogWrap(source);
			UnityEngine.Debug.Log(source.ToString());
		}

		[Conditional("DEBUG")]
		public static void Log(this string source)
		{
			source = LogFilter(source);
			source = LogPrefix(source);
			source = LogWrap(source);
			UnityEngine.Debug.Log(source);
		}

		[Conditional("DEBUG")]
		public static void LogWarning(this StringBuilder source)
		{
			source = LogFilter(source);
			source = LogPrefix(source);
			source = LogWrap(source);
			UnityEngine.Debug.LogWarning(source.ToString());
		}

		[Conditional("DEBUG")]
		public static void LogWarning(this string source)
		{
			source = LogFilter(source);
			source = LogPrefix(source);
			source = LogWrap(source);
			UnityEngine.Debug.LogWarning(source);
		}

		public static void LogError(this StringBuilder source)
		{
			source = LogFilter(source);
			source = LogPrefix(source);
			source = LogWrap(source);
			UnityEngine.Debug.LogError(source.ToString());
		}

		public static void LogError(this string source)
		{
			source = LogFilter(source);
			source = LogPrefix(source);
			source = LogWrap(source);
			UnityEngine.Debug.LogError(source);
		}

		public static string ToText<T>(this IEnumerable<T> source, string header = null, Func<T, string> renderer = null)
		{
			renderer ??= _ => _.ToString();

			var counterRowsTotal = 0;
			var counterRowsValue = 0;

			var builder = new StringBuilder();

			if(source != null)
			{
				if(source is string @string)
				{
					using var reader = new StringReader(@string);
					var lineNumber = -1;
					builder
						.AppendLine()
						.Append($"  {++lineNumber:D2}: ")
						.Append($"{new string(' ', 6)}10 |")
						.Append($"{new string(' ', 6)}20 |")
						.Append($"{new string(' ', 6)}30 |");

					while(true)
					{
						var buffer = reader.ReadLine();
						if(buffer == null)
						{
							break;
						}

						builder
							.AppendLine()
							.Append($"  {++lineNumber:D2}: ")
							.Append(buffer);
					}
				}
				else
				{
					foreach(var item in source)
					{
						counterRowsTotal++;
						builder.AppendLine();

						if(item == null)
						{
							builder.Append($"{counterRowsTotal:D3}: [null]");
						}
						else
						{
							builder.Append($"{counterRowsTotal:D3}: {renderer(item)}");
							counterRowsValue++;
						}
					}
				}
			}

			if(header == null)
			{
				return builder.Length == 0 ? "[null]" : builder.ToString();
			}

			builder.Insert(0, $"{header} [rows: {counterRowsTotal}/{counterRowsValue}]:");

			return builder.ToString();
		}

		public static string ToText(this IEnumerable source, string header = null, Func<object, string> renderer = null)
		{
			return ToText(source.Cast<object>(), header, renderer);
		}

		public static string ToText<T>(this T source) where T : Exception
		{
			if(ReferenceEquals(null, source))
			{
				return "<exception is null>";
			}

			var builder = new StringBuilder();

			void Splitter(string message, string indent)
			{
				var split = message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				for(var index = 0; index < split.Length; index++)
				{
					builder.Append(indent);
					builder.Append("  ");
					builder.Append(split[index].TrimStart(' '));
					builder.AppendLine();
				}
			}

			var current = source as Exception;
			var counter = 0;
			const int INDENT_STEP_I = 2;
			while(current != null)
			{
				builder.AppendLine($"-- .{++counter}");

				var indent = new string(' ', counter * INDENT_STEP_I);
				builder.Append(indent);
				builder.Append("Exception: ");
				builder.Append(current.GetType().Name);
				builder.Append(" :: ");
				builder.Append(current.GetType().Namespace);
				builder.AppendLine();

				if(!ReferenceEquals(null, current.Message))
				{
					builder.Append(indent);
					builder.AppendLine("Message:");
					Splitter(current.Message.TrimEnd('\n'), indent);
				}

				if(!ReferenceEquals(null, current.StackTrace))
				{
					builder.Append(indent);
					builder.AppendLine("Stack Trace:");
					Splitter(current.StackTrace.TrimEnd('\n'), indent);
				}

				current = current.InnerException;
			}

			builder.Append("-- .end exceptions trace");
			return $"Roll out totals: {counter}\n" + builder;
		}

		// ReSharper disable once StringLiteralTypo
		private static readonly char[] _base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

		public static string MakeUnique(this string source, int length = 6, int @base = 0)
		{
			@base = @base == 0 ? _base62Chars.Length : @base;
			var builder = new StringBuilder(source, source.Length + length);
			for(var ctr = 0; ctr < length; ctr++)
			{
				var index = Mathf.FloorToInt(UnityEngine.Random.value * @base) % _base62Chars.Length;
				builder.Append(index);
			}

			return builder.ToString();
		}

		private static readonly Regex _typeTemplate = new("(?'name'[^`]*)");

		public static string Dump(this Type source)
		{
			var builder = new StringBuilder();
			while(source != null)
			{
				builder
					.Append(source == typeof(object) ? "[O]" : source.NameNice())
					.Append(source == typeof(object) ? string.Empty : " > ");
				source = source.BaseType;
			}

			return builder.ToString();
		}

		private static string WriteArgs(Type type)
		{
			return
				type.IsGenericType
					? type.GetGenericArguments()
						.Aggregate(new StringBuilder(), (builder, _) => builder.Append(NameNice(_) + ", "))
						.ToString()
						.TrimEnd(", ".ToArray())
					: string.Empty;
		}

		public static string NameNice(this Type source)
		{
			if(source == null)
			{
				return "null";
			}

			return
				source.IsGenericType
					? $"{_typeTemplate.Match(source.Name).Groups["name"].Value}<{WriteArgs(source)}>"
					: source.Name;
		}

		private static readonly Regex _color = new("#?(?'r'[a-fA-F0-9]{2})(?'g'[a-fA-F0-9]{2})(?'b'[a-fA-F0-9]{2})(?'a'[a-fA-F0-9]{2})?");

		public static Color ToColor(this string source)
		{
			var match = _color.Match(source);

			if(match.Success)
			{
				var r = byte.Parse(match.Groups["r"].Value, NumberStyles.HexNumber) / 255f;
				var g = byte.Parse(match.Groups["g"].Value, NumberStyles.HexNumber) / 255f;
				var b = byte.Parse(match.Groups["b"].Value, NumberStyles.HexNumber) / 255f;
				var a = match.Groups["a"].Success ? byte.Parse(match.Groups["a"].Value, NumberStyles.HexNumber) / 255f : 1f;
				return new Color(r, g, b, a);
			}

			return Color.magenta;
		}

		private static readonly Regex _nameFilter = new(@"((\\s|_|\.)?\(Clone\))?", RegexOptions.Compiled);

		public static string CleanUpName(this string source)
		{
			return _nameFilter.Replace(source, string.Empty);
		}
	}
}
