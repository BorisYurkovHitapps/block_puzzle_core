// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.IO;
using System.IO.Compression;
using CompressionLevel = System.IO.Compression.CompressionLevel;


namespace BlockPuzzle.Scripts.Runtime.extensions {
	public static class ByteArrayExtensions {
		public static byte[] Compressed (this byte[] self) {
			if (self == null)
				throw new NullReferenceException(nameof(self));

			using (MemoryStream output = new MemoryStream()) {
				using (GZipStream stream = new GZipStream(output, CompressionLevel.Optimal)) {
					stream.Write(self, 0, self.Length);
				}

				return output.ToArray();
			}
		}

		public static byte[] Decompressed (this byte[] self) {
			if (self == null)
				throw new NullReferenceException(nameof(self));

			if (self.IsCompressed() == false)
				throw new ArgumentException($"{nameof(self)} is not compressed.");

			using MemoryStream input  = new MemoryStream(self);
			using MemoryStream output = new MemoryStream();
			using GZipStream   stream = new GZipStream(input, CompressionMode.Decompress);

			stream.CopyTo(output);

			return output.ToArray();
		}

		public static bool IsCompressed (this byte[] self) {
			if (self == null)
				throw new NullReferenceException(nameof(self));

			bool hasGzipHeader = self.Length >= 2 && self[0] == 0x1f && self[1] == 0x8b;
			bool hasZipHeader  = self.Length >= 4 && self[0] == 0x50 && self[1] == 0x4b && self[2] == 0x03 && self[3] == 0x04;

			return hasGzipHeader || hasZipHeader;
		}
	}
}
