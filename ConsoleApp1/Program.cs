using System;
using System.Net.Http;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace IdentityConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //discovery endpoint 
            var client = new HttpClient();
            var disco = client.GetDiscoveryDocumentAsync("http://localhost:5000").Result;
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            //request access token
            var tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = "api1"
            }).Result;
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }
            var token = tokenResponse.AccessToken;
            Console.WriteLine(token);
            //call identity resource api
            client.SetBearerToken(token);
            var userInfoResponse = client.GetAsync("http://localhost:5001/identity").Result;
            if (!userInfoResponse.IsSuccessStatusCode)
            {
                Console.WriteLine(userInfoResponse.StatusCode);
            }
            else
            {
                var content = userInfoResponse.Content.ReadAsStringAsync().Result;
                Console.WriteLine(JArray.Parse(content));
            }
            //pwd client
            var pwdClient = new HttpClient();
            var pwdTokenResponse = pwdClient.RequestPasswordTokenAsync(new PasswordTokenRequest()
            {
                Address = disco.TokenEndpoint,
                ClientId = "wpf client",
                ClientSecret = "wpf secret",
                Scope = "api1 openid profile",
                UserName="bob",
                Password= "Pass123$"
            }).Result;
            var pwdtoken = pwdTokenResponse.AccessToken;
            Console.WriteLine(pwdtoken);
            //call identity resource api
            pwdClient.SetBearerToken(pwdtoken);
            var userInfoPwdResponse = pwdClient.GetAsync(disco.UserInfoEndpoint).Result;
            var pwdContent = userInfoPwdResponse.Content.ReadAsStringAsync().Result;
            Console.WriteLine(pwdContent);
            
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
