﻿using System;
using System.IO;
using System.Net;

namespace AiCodo.Services
{
    public static class WebService
    {
        public static string ServerUrl { get; set; } = "http://localhost:62126/";

        public static string Token { get; set; } = "";

        public static T Request<T>(string url, params object[] nameValues)
        {
            try
            {
                var data = new DynamicEntity(nameValues);
                var resultText = Post(url, data.ToJson());
                var result = resultText.ToJsonObject<ServiceResult<T>>();
                return result.Data;
            }
            catch (Exception ex)
            {
                ex.WriteErrorLog();
                throw;
            }
        }

        public static string Post(string url, string data)
        {
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
            }
            else
            {
                url = Path.Combine(WebService.ServerUrl, url);
            }
            return PostWithBasicToken(url, Token, data);
        }

        public static string PostWithBasicToken(string url, string token, string data,
            string tokenType = "Basic", string contentType = "application/json")
        {
            using (var client = new WebClient())
            {
                if (!string.IsNullOrEmpty(token))
                {
                    client.Headers.Add(HttpRequestHeader.Authorization, $"{tokenType} {token}");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(data);

                if (!string.IsNullOrEmpty(contentType))
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, contentType);
                }
                client.Headers.Add(HttpRequestHeader.ContentEncoding, "utf-8");
                var response = client.UploadData(url, bytes);
                var json = System.Text.Encoding.UTF8.GetString(response); ;
                return json;
            }
        }
    }
}