namespace TrailingSlash
{
    using Slashes;
    using System;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Hosting;

    /// <summary>
    /// Appends a trailing slash to the url and redirects the request.
    /// </summary>
    public class SlashAppender
    {
        /// <summary>
        /// Don't add a trailing slash when the requested url is empty.
        /// </summary>
        public bool ExceptEmpty { get; set; }

        /// <summary>
        /// Don't add a trailing slash when the requested url has an extension (such as resource).
        /// </summary>
        public bool ExceptWithExtension { get; set; }

        /// <summary>
        /// Don't add a trailing slash when the requested url starts with "api".
        /// </summary>
        public bool ExceptFromApi { get; set; }

        /// <summary>
        /// A custom function of yours to except urls using along the other flags.
        /// </summary>
        public Func<string, bool> Except { get; set; }

        /// <summary>
        /// Ctor.
        /// </summary>
        public SlashAppender()
        {
            ExceptEmpty = true;
            ExceptWithExtension = true;
            ExceptFromApi = true;
        }

        /// <summary>
        /// Adds a trailing slash to the current request url and redirects if necessary.
        /// </summary>
        public void Append()
        {
            var url = HttpContext.Current.Request.Url.AbsolutePath.Trim();
            if (NeedsAppend(url)) AppendAnyway();
        }

        /// <summary>
        /// Adds a trailing slash to the current request url and redirects even if it's not necessary.
        /// </summary>
        public void AppendAnyway()
        {
            var request = HttpContext.Current.Request;
            var response = HttpContext.Current.Response;

            var url = string.Format("/{0}/", Slash.Trim(request.Url.AbsolutePath));
            var query = request.Url.Query;
            var method = request.HttpMethod.ToUpper();

            response.Clear();
            response.Status = method == "GET" ? "301 Moved Permanently" : "307 Temporary Redirect";
            response.AddHeader("Location", url + query);
            response.End();
        }

        private bool NeedsAppend(string url)
        {
            // it doesn't need to append if doesn't
            return !(

                // already end with slash
                EndsWithSlash(url) ||

                // or the url is empty and we should except it
                (ExceptEmpty && Empty(url)) ||

                // or the url has extension and we should except it
                (ExceptWithExtension && HasExtension(url)) ||

                // or the url is from api and we should except it
                (ExceptFromApi && FromApi(url)) ||

                // or there is a exception function and it returned false (don't except it)
                (Except != null && Except(url))

            );
        }

        private static bool EndsWithSlash(string url)
        {
            return url.Last() == '/';
        }

        private static bool Empty(string url)
        {
            return string.IsNullOrEmpty(url);
        }

        private static bool HasExtension(string url)
        {
            return Path.HasExtension(url);
        }

        private static bool FromApi(string url)
        {
            var virtualPath = HostingEnvironment.ApplicationVirtualPath;
            var apiUrl = Slash.Trim(Slash.JoinTrimming(virtualPath, "api"));
            return Slash.TrimStart(url).StartsWith(apiUrl);
        }
    }
}
