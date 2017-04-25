using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisClient.Caching
{
    public static class Gzip
    {
        /// <summary>
        ///     Compresses JSON using GZIP
        /// </summary>
        /// <param name="json">JSON to compress</param>
        /// <returns>Compressed JSON</returns>
        public static string CompressJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return json;
            }
            var bytes = Encoding.Unicode.GetBytes(json);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        /// <summary>
        ///     Decompress a compressed JSON string
        /// </summary>
        /// <param name="compressedJson">The compressed JSON string</param>
        /// <returns>Decompressed JSON</returns>
        public static string DecompressJson(string compressedJson)
        {
            if (string.IsNullOrEmpty(compressedJson))
            {
                return compressedJson;
            }
            var bytes = Convert.FromBase64String(compressedJson);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.Unicode.GetString(mso.ToArray());
            }
        }
    }
}
