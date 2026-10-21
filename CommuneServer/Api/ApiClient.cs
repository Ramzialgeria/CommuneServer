using CommuneServer.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CommuneServer.Api
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private string _token;

        private string _clientCode;
        private string _clientSecret;

        public ApiClient(string url)
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(url),
                Timeout = TimeSpan.FromSeconds(5)
            };
        }

        public void SetCredentials(
    string clientCode,
    string clientSecret)
        {
            _clientCode = clientCode;
            _clientSecret = clientSecret;
        }

        public async Task<bool> AuthenticateAsync(
            string code,
            string secret)
        {
            try
            {
                _clientCode = code;
                _clientSecret = secret;

                var request = new
                {
                    ClientCode = code,
                    ClientSecret = secret
                };

                var json =
                    JsonConvert.SerializeObject(request);

                using (var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"))
                {
                    var response =
                        await _http.PostAsync(
                            "api/auth/client",
                            content);

                    var body =
                        await response.Content
                            .ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine(
                        string.Format(
                            "AUTH: {0} {1}",
                            (int)response.StatusCode,
                            response.StatusCode));

                    System.Diagnostics.Debug.WriteLine(
                        "AUTH BODY: " + body);

                    if (!response.IsSuccessStatusCode)
                        return false;

                    var token =
                        JsonConvert
                            .DeserializeObject<TokenResponse>(
                                body);

                    if (token == null)
                        return false;

                    _token = token.AccessToken;

                    return !string.IsNullOrWhiteSpace(
                        _token);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "AUTH ERROR: " + ex);

                return false;
            }
        }

        public async Task<bool> HeartbeatAsync(
            HeartbeatRequest req)
        {
           
            if (string.IsNullOrWhiteSpace(_token))
            {
                // Startup authentication failed (central was down): try again now.
                if (!await AuthenticateAsync(_clientCode, _clientSecret))
                    return false;
            }
            try
            {
                _http.DefaultRequestHeaders.Authorization =
           new AuthenticationHeaderValue(
               "Bearer",
               _token);

                var json =
                    JsonConvert.SerializeObject(req);

                using (var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"))
                {
                    var response =
                        await _http.PostAsync(
                            "api/heartbeat",
                            content);

                    var body =
                        await response.Content
                            .ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine(
                        string.Format(
                            "HEARTBEAT: {0} {1}",
                            (int)response.StatusCode,
                            response.StatusCode));

                    System.Diagnostics.Debug.WriteLine(
                        "HEARTBEAT BODY: " + body);

                    if (response.StatusCode ==
                System.Net.HttpStatusCode.Unauthorized)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            "HEARTBEAT: JWT expired, re-authenticating...");

                        var authenticated =
                            await AuthenticateAsync(
                                _clientCode,
                                _clientSecret);

                        if (!authenticated)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "HEARTBEAT: Re-authentication failed");

                            return false;
                        }

                        // Retry heartbeat once with the new token
                        _http.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue(
                                "Bearer",
                                _token);

                        using (var retryContent =
                            new StringContent(
                                json,
                                Encoding.UTF8,
                                "application/json"))
                        {
                            var retryResponse =
                                await _http.PostAsync(
                                    "api/heartbeat",
                                    retryContent);

                            var retryBody =
                                await retryResponse.Content
                                    .ReadAsStringAsync();

                            System.Diagnostics.Debug.WriteLine(
                                string.Format(
                                    "HEARTBEAT RETRY: {0} {1}",
                                    (int)retryResponse.StatusCode,
                                    retryResponse.StatusCode));

                            System.Diagnostics.Debug.WriteLine(
                                "HEARTBEAT RETRY BODY: " +
                                retryBody);

                            return retryResponse.IsSuccessStatusCode;
                        }
                    }


                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "HEARTBEAT ERROR: " + ex);

                return false;
            }
        }

        public class TokenResponse
        {
            public string AccessToken { get; set; }
        }
    }
}