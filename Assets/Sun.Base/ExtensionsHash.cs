using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Assets.Sun.Base
{
	public static class ExtensionsHash
	{
		public static string HashMd5(string data)
		{
			var encoding = new UTF8Encoding();
			var bytes = encoding.GetBytes(data);

			var md5 = new MD5CryptoServiceProvider();
			var value = md5.ComputeHash(bytes);

			var builder = new StringBuilder();
			for(var index = 0; index < value.Length; index++)
			{
				builder.Append($"{value[index]:X2}");
			}

			return builder.ToString().PadLeft(32, '0');
		}

		public static string HashMd5(FileInfo fileName)
		{
			using var file = new FileStream(fileName.FullName, FileMode.Open);
			var md5 = new MD5CryptoServiceProvider();
			var value = md5.ComputeHash(file);
			file.Close();

			var builder = new StringBuilder();
			for(var index = 0; index < value.Length; index++)
			{
				builder.Append($"{value[index]:X2}");
			}

			return builder.ToString();
		}

		public static string HashMd5(byte[] bytes)
		{
			var md5 = new MD5CryptoServiceProvider();
			var value = md5.ComputeHash(bytes);

			var builder = new StringBuilder();
			for(var index = 0; index < value.Length; index++)
			{
				builder.Append($"{value[index]:X2}");
			}

			return builder.ToString();
		}

		public static unsafe int HashCombine(float left, float right)
		{
			var h1 = *(uint*)&left;
			var h2 = *(uint*)&right;
			var hash = h1 ^ (h2 >> 16 | h2 << 16);
			return *(int*)&hash;
		}

		public static unsafe int HashCombine(int left, int right)
		{
			var h1 = *(uint*)&left;
			var h2 = *(uint*)&right;
			var hash = h1 ^ (h2 >> 16 | h2 << 16);
			return *(int*)&hash;
		}

		public static int HashLy(this string source)
		{
			uint hash = 0;

			if(!string.IsNullOrEmpty(source))
			{
				unchecked
				{
					for(var index = 0; index < source.Length; index++)
					{
						var val = (uint)source[index];
						hash = hash * 1664525u + val % 256u + 1013904223u;
						hash = hash * 1664525u + val / 256u + 1013904223u;
					}
				}
			}

			return (int)hash;
		}

		public static int HashLy(this byte[] source)
		{
			uint hash = 0;

			if(source != null && source.Length != 0)
			{
				unchecked
				{
					for(var index = 0; index < source.Length; index++)
					{
						hash = hash * 1664525u + source[index] + 1013904223u;
					}
				}
			}

			return (int)hash;
		}

		public static unsafe int HashLy(this int size, byte* overPtr)
		{
			uint hash = 0;

			if(size != 0)
			{
				unchecked
				{
					for(var index = 0; index < size; index++)
					{
						hash = hash * 1664525u + overPtr[index] + 1013904223u;
					}
				}
			}

			return (int)hash;
		}
	}
}
