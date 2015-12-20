using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {

        // Add scheme (e.g. http) to input Web URL if it is missing
        public static string FixWebUrl(this string webUrl)
        {
            Uri fullWebUri = new UriBuilder(webUrl).Uri;
            return fullWebUri.AbsoluteUri;
        }

        // Hash the input string
        public static string GetHash(this string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

        // Remove HTML from input string
        public static string ScrubHtml(this string str)
        {
            var step1 = Regex.Replace(str, @"<[^>]+>|&nbsp;", "").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }

        // Convert input string to memory stream
        public static Stream ToStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
