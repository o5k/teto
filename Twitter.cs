using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Teto {
    /// <summary>
    /// A Twitter client.
    /// </summary>
    public class Twitter {
        /// <summary>
        /// The OAuth credentials this client is authorized to.
        /// </summary>
        private OAuth oAuth;

        /// <summary>
        /// Construct a Twitter client.
        /// </summary>
        /// <param name="oAuth">The OAuth credentials this client should be authorized to.</param>
        public Twitter(OAuth oAuth) {
            this.oAuth = oAuth;
        }

        /// <summary>
        /// Make a form-encoded request.
        /// </summary>
        /// <param name="method">The HTTP method to use.</param>
        /// <param name="url">The URL to request to.</param>
        /// <param name="parameters">The parameters to send.</param>
        /// <returns>The server's response.</returns>
        public Webbe.Response RequestForm(string method, string url, Webbe.Parameter[] parameters) {
            string oAuthHeader = oAuth.GenerateHeader(method, url, Webbe.Parameter.ToDictionary(parameters));

            Webbe w = new Webbe();
            w.Headers["Authorization"] = oAuthHeader;

            return w.UploadForm(method, url, parameters);
        }

        /// <summary>
        /// Make a multipart-encoded request.
        /// </summary>
        /// <param name="method">The HTTP method to use.</param>
        /// <param name="url">The URL to request to.</param>
        /// <param name="parameters">The parameters to send.</param>
        /// <returns>The server's response.</returns>
        public Webbe.Response RequestMultipart(string method, string url, Webbe.Parameter[] parameters) {
            string oAuthHeader = oAuth.GenerateHeader(method, url, new Dictionary<string, string>());

            Webbe w = new Webbe();
            w.Headers["Authorization"] = oAuthHeader;

            return w.UploadMultipart(method, url, parameters);
        }

        /// <summary>
        /// Make a text tweet.
        /// </summary>
        /// <param name="text">The text to tweet.</param>
        /// <returns>The server's response.</returns>
        public Webbe.Response Tweet(string text) {
            List<Webbe.Parameter> parameters = new List<Webbe.Parameter>();
            parameters.Add(new Webbe.FormParameter("status", text));

            return RequestForm("POST", "https://api.twitter.com/1.1/statuses/update.json", parameters.ToArray());
        }

        /// <summary>
        /// Make an image tweet.
        /// </summary>
        /// <param name="text">The text to tweet.</param>
        /// <param name="path">The path to the image to attach.</param>
        /// <returns>The server's response (specifically, the response to the tweet action).</returns>
        public Webbe.Response TweetImage(string text, string path) {
            JObject mediaObj = JObject.Parse(UploadFile(path).DataString);
            string mediaId = mediaObj["media_id"].ToObject<string>();

            List<Webbe.Parameter> parameters = new List<Webbe.Parameter>();
            parameters.Add(new Webbe.FormParameter("status", text));
            parameters.Add(new Webbe.FormParameter("media_ids", mediaId));

            return RequestForm("POST", "https://api.twitter.com/1.1/statuses/update.json", parameters.ToArray());
        }

        /// <summary>
        /// Upload a file to Twitter for later use in a tweet.
        /// </summary>
        /// <param name="path">The path to the image to upload.</param>
        /// <returns>The server's response.</returns>
        private Webbe.Response UploadFile(string path) {
            List<Webbe.Parameter> parameters = new List<Webbe.Parameter>();
            parameters.Add(new Webbe.FileParameter("media", Path.GetFileName(path), File.ReadAllBytes(path)));

            return RequestMultipart("POST", "https://upload.twitter.com/1.1/media/upload.json", parameters.ToArray());
        }
    }
}
