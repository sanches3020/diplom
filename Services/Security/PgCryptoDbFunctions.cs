using Microsoft.EntityFrameworkCore;

namespace Sofia.Web.Services.Security
{
    public static class PgCryptoDbFunctions
    {
        [DbFunction("pgp_sym_encrypt", IsBuiltIn = true)]
        public static byte[] Encrypt(string data, string key)
            => throw new NotImplementedException();

        [DbFunction("pgp_sym_decrypt", IsBuiltIn = true)]
        public static string Decrypt(byte[] data, string key)
            => throw new NotImplementedException();
    }
}
