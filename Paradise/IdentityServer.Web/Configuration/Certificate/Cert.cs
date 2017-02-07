using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace IdentityServer.Web.Configuration.Certificate
{
    public class Cert
    {
        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 Load()
        {
            var assembly = typeof(Cert).Assembly;
            using (
                var stream =
                    assembly.GetManifestResourceStream("IdentityServer.Web.Configuration.Certificate.idsrv3test.pfx"))
            {
                return new X509Certificate2(ReadStream(stream), "idsrv3test");
            }
        }

        /// <summary>
        /// Reads the stream.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private static byte[] ReadStream(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}