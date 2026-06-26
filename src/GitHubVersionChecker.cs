using CU_ModSettings.Model;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CU_ModSettings;

public class GitHubVersionChecker
{
    public static string GetGithubLinkFor(string author, string repository) => $"https://api.github.com/repos/{author}/{repository}/releases/latest";

    public static async Task<Version?> TryGetNewestVersionInformation(string author, string repository)
    {
        using HttpRequestMessage message = new(HttpMethod.Get, GetGithubLinkFor(author, repository));
        message.Headers.UserAgent.Add(new("ModSettings", Environment.Version.ToString()));

        using HttpClient client = new();
        using HttpResponseMessage response = await client.SendAsync(message).ConfigureAwait(false);

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (JsonConvert.DeserializeObject<GithubResponse>(content) is not GithubResponse githubResponse) return null;

        TimeSpan week = TimeSpan.FromDays(7);
        DateTime currentBuildCreationTime = File.GetCreationTimeUtc(Assembly.GetExecutingAssembly().Location);

        bool gotVersion = githubResponse.TryGetVersion(out Version? version);

        if (!gotVersion)
        {
            ModSettingsPlugin.LogDebug("Couldn't parse version from Github.");
            return null;
        }

        return version;
    }
}
