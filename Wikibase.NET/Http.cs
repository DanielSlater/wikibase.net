﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

namespace Wikibase
{
    /// <summary>
    /// Http related code
    /// </summary>
    class Http
    {
        /// <summary>
        /// The user agent
        /// </summary>
        public string UserAgent { get; set; }

        private CookieContainer cookies = new CookieContainer();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userAgent">The user agent</param>
        public Http(string userAgent)
        {
            this.UserAgent = userAgent;
        }

        /// <summary>
        /// Performs a http get request.
        /// </summary>
        /// <param name="url">The url</param>
        /// <returns>The response</returns>
        public string get(string url)
        {
            return this.post(url, null);
        }

        /// <summary>
        /// Performs a http post request.
        /// </summary>
        /// <param name="url">The url</param>
        /// <param name="postFields">The post fields</param>
        /// <returns>The response</returns>
        public string post(string url, Dictionary<string, string> postFields)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.UserAgent = this.UserAgent;
            request.ContentType = "application/x-www-form-urlencoded";

            if (this.cookies.Count == 0)
                request.CookieContainer = new CookieContainer();
            else
                request.CookieContainer = this.cookies;

            if (postFields != null)
            {
                request.Method = "POST";
                byte[] postBytes = Encoding.UTF8.GetBytes(this.buildQuery(postFields));
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            foreach (Cookie cookie in response.Cookies)
            {
                this.cookies.Add(cookie);
            }

            Stream respStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(respStream);
            string respStr = reader.ReadToEnd();
            reader.Close();
            response.Close();
            return respStr;
        }

        /// <summary>
        /// Builds a http query string.
        /// </summary>
        /// <param name="fields">The fields</param>
        /// <returns>The query string</returns>
        public string buildQuery(Dictionary<string, string> fields)
        {
            string query = "";
            foreach (KeyValuePair<string, string> field in fields)
            {
                query += HttpUtility.UrlEncode(field.Key) + "=" + HttpUtility.UrlEncode(field.Value) + "&";
            }
            if ( !String.IsNullOrEmpty(query) )
            {
                query = query.Remove(query.Length - 1);
            }
            return query;
        }
    }
}
