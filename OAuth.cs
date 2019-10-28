using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Teto {
    /// <summary>
    /// An OAuth provider.
    /// </summary>
    public class OAuth {
        /// <summary>
        /// The instance's randomizer.
        /// </summary>
        private Random rng = new Random();
        /// <summary>
        /// The consumer key.
        /// </summary>
        private string consumerKey = null;
        /// <summary>
        /// The consumer secret.
        /// </summary>
        private string consumerSecret = null;
        /// <summary>
        /// The access token.
        /// </summary>
        private string accessToken = null;
        /// <summary>
        /// The access secret.
        /// </summary>
        private string accessSecret = null;

        /// <summary>
        /// Construct a new OAuth provider.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="accessSecret">The access secret.</param>
        public OAuth(string consumerKey, string consumerSecret, string accessToken, string accessSecret) {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.accessToken = accessToken;
            this.accessSecret = accessSecret;
        }

        /// <summary>
        /// Generates a 32-byte string nonce.
        /// </summary>
        /// <returns>The string nonce.</returns>
        string GenerateNonce() {
            byte[] bytes = new byte[32];
            rng.NextBytes(bytes);

            string unfiltered = Convert.ToBase64String(bytes);

            return new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled).Replace(unfiltered, "");
        }

        /// <summary>
        /// Generates an HTTP Authorization header using the OAuth info.
        /// </summary>
        /// <param name="method">The HTTP method to authorize.</param>
        /// <param name="url">The HTTP URL to authorize.</param>
        /// <param name="body">The body data to authorize.</param>
        /// <returns></returns>
        public string GenerateHeader(string method, string url, Dictionary<string, string> body) {
            // Gather the OAuth parameters
            string oauth_consumer_key = consumerKey;
            string oauth_nonce = GenerateNonce();
            string oauth_signature_method = "HMAC-SHA1";
            string oauth_timestamp = new UnixTimestamp(DateTime.UtcNow);
            string oauth_token = accessToken;
            string oauth_version = "1.0";

            // Create a sorted list of parameters to be signed
            SortedDictionary<string, string> percBody = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> bi in body) {
                percBody[Uri.EscapeDataString(bi.Key)] = Uri.EscapeDataString(bi.Value);
            }
            
            percBody["oauth_consumer_key"] = Uri.EscapeDataString(oauth_consumer_key);
            percBody["oauth_nonce"] = Uri.EscapeDataString(oauth_nonce);
            percBody["oauth_signature_method"] = Uri.EscapeDataString(oauth_signature_method);
            percBody["oauth_timestamp"] = Uri.EscapeDataString(oauth_timestamp);
            percBody["oauth_token"] = Uri.EscapeDataString(oauth_token);
            percBody["oauth_version"] = Uri.EscapeDataString(oauth_version);

            // Export the list as a string
            string parameterString = "";

            foreach (KeyValuePair<string, string> pbi in percBody) {
                parameterString += pbi.Key + "=" + pbi.Value + "&";
            }
            parameterString = parameterString.Substring(0, parameterString.Length - 1);

            // Assemble the parameters as well as URL and method into a string
            string signatureBaseString = "";

            signatureBaseString += method.ToUpper();
            signatureBaseString += "&";
            signatureBaseString += Uri.EscapeDataString(url);
            signatureBaseString += "&";
            signatureBaseString += Uri.EscapeDataString(parameterString);

            // Assemble a signing key
            string signingKey = Uri.EscapeDataString(consumerSecret) + "&" + Uri.EscapeDataString(accessSecret);

            // Sign the base string with the signing key
            HMACSHA1 hmac = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey));
            hmac.Initialize();

            byte[] buffer = Encoding.ASCII.GetBytes(signatureBaseString);
            string oauth_signature = Convert.ToBase64String(hmac.ComputeHash(buffer));

            // Generate an output header
            string outputHeader = "OAuth ";
            outputHeader += $"oauth_consumer_key=\"{ Uri.EscapeDataString(oauth_consumer_key) }\", ";
            outputHeader += $"oauth_nonce=\"{ Uri.EscapeDataString(oauth_nonce) }\", ";
            outputHeader += $"oauth_signature=\"{ Uri.EscapeDataString(oauth_signature) }\", ";
            outputHeader += $"oauth_signature_method=\"{ Uri.EscapeDataString(oauth_signature_method) }\", ";
            outputHeader += $"oauth_timestamp=\"{ Uri.EscapeDataString(oauth_timestamp) }\", ";
            outputHeader += $"oauth_token=\"{ Uri.EscapeDataString(oauth_token) }\", ";
            outputHeader += $"oauth_version=\"{ Uri.EscapeDataString(oauth_version) }\"";

            return outputHeader;
        }
    }
}
