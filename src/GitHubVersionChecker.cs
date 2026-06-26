using CU_ModSettings.Model;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CU_ModSettings;

public class GitHubVersionChecker
{
    public static string GetGithubLinkFor(string author, string repository) => $"https://api.github.com/repos/{author}/{repository}/releases/latest";

    public static bool TryGetNewestVersionInformation(string author, string repository, [NotNullWhen(true)] out Version? version)
    {
        using HttpRequestMessage message = new(HttpMethod.Get, GetGithubLinkFor(author, repository));
        message.Headers.UserAgent.Add(new("ModSettings", Environment.Version.ToString()));

        using HttpClient client = new();

        HttpResponseMessage response = client.SendAsync(message).Result;

        if (JsonConvert.DeserializeObject<GithubResponse>(response.Content.ReadAsStringAsync().Result) is not GithubResponse githubResponse) 
        {
            version = null;
            return false;
        }

        TimeSpan week = TimeSpan.FromDays(7);
        DateTime currentBuildCreationTime = File.GetCreationTimeUtc(Assembly.GetExecutingAssembly().Location);

        bool gotVersion = githubResponse.TryGetVersion(out version);

        if (!gotVersion)
        {
            ModSettingsPlugin.LogDebug("Couldn't parse version from Github.");
            return false;
        }

        // Current version is younger.
        return Assembly.GetExecutingAssembly().GetName().Version < version;
    }
}
