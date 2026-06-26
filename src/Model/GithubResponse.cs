using System.Diagnostics.CodeAnalysis;

namespace CU_ModSettings.Model;

public record GithubResponse
{
    private readonly static char[] digits = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

    [AllowNull] public string Tag_name { get; set; }
    [AllowNull] public string Name { get; set; }
    public bool Draft { get; set; }
    public bool Prerelease { get; set; }
    public DateTime Created_at { get; set; }
    public DateTime Updated_at { get; set; }
    public DateTime Published_at { get; set; }
    [AllowNull] public string Body { get; set; }

    public bool TryGetVersion([NotNullWhen(true)] out Version? version)
    {
        if (TryGetVersionFromString(Tag_name, out version)) return true;
        if (TryGetVersionFromString(Name, out version)) return true;

        version = null;
        return false;
    }

    private static bool TryGetVersionFromString(string str, [NotNullWhen(true)] out Version? version)
    {
        if (str.IndexOfAny(digits) is not (int start and >= 0))
        {
            version = null;
            return false;
        }

        int end = str.LastIndexOfAny(digits) - start + 1;
        if (end <= start)
        {
            version = null;
            return false;
        }

        return Version.TryParse(str[start..end], out version);
    }
}
