using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Sofia.Web.Services.Security
{
    /// </summary>
    public class EncryptedStringService
    {
        private readonly IConfiguration _config;

        public EncryptedStringService(IConfiguration config)
        {
            _config = config;
        }

        private string Key => _config["EncryptionKey"]!;

        /// <summary>
        /// Используется ТОЛЬКО внутри LINQ (EF переведёт в SQL)
        /// </summary>
        public byte[] Encrypt(string value)
        {
            return PgCryptoDbFunctions.Encrypt(value, Key);
        }

        /// <summary>
        /// Используется ТОЛЬКО внутри LINQ
        /// </summary>
        public string Decrypt(byte[] value)
        {
            return PgCryptoDbFunctions.Decrypt(value, Key);
        }

        /// <summary>
        /// Helper для Select (чтобы код был чище)
        /// </summary>
        public IQueryable<string> DecryptSelect(IQueryable<byte[]> query)
        {
            return query.Select(v => PgCryptoDbFunctions.Decrypt(v, Key));
        }
    }
}