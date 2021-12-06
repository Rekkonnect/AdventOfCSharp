#nullable enable

using System.Net.Http;
using System.Text.RegularExpressions;

namespace AdventOfCSharp;

public static class WebsiteScraping
{
    private static readonly Regex puzzleAnswerPattern = new(@"Your puzzle answer was <code>(?'answer'.*?)</code>");

    public static string DownloadInput(int year, int day, bool outputLog = false)
    {
        var inputURI = GetProblemInputURI(year, day);
        return DownloadContent(inputURI, "input", outputLog);
    }

    public static ProblemOutput DownloadAnsweredCorrectOutputs(int year, int day, bool outputLog = false)
    {
        var inputURI = GetProblemURI(year, day);
        var content = DownloadContent(inputURI, "correct output", outputLog);
        return ParseAnsweredCorrectOutputs(content);
    }

    private static ProblemOutput ParseAnsweredCorrectOutputs(string siteContents)
    {
        var matches = puzzleAnswerPattern.Matches(siteContents);
        return ProblemOutput.Parse(matches.Select(match => match.Groups["answer"].Value).ToArray());
    }

    public static string GetProblemInputURI(int year, int day) => $"{GetProblemURI(year, day)}/input";
    public static string GetProblemURI(int year, int day) => $"https://adventofcode.com/{year}/day/{day}";

    private static string DownloadContent(string targetURI, string contentKind, bool outputLog)
    {
        if (SecretsStorage.Cookies is null)
            throw new InvalidOperationException("No cookie container class to use during input retrieval has been specified.");

        using var client = new HttpClient();
        SecretsStorage.Cookies.AddToDefaultRequestHeaders(client);

        while (true)
        {
            try
            {
                if (outputLog)
                    Console.WriteLine($"Downloading {contentKind} from the website...");

                var response = client.GetAsync(targetURI).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;

                if (outputLog)
                    Console.WriteLine($"Successfully downloaded {contentKind}\n");

                return responseString;
            }
            catch (HttpRequestException requestException)
            {
                if (outputLog)
                    ConsoleUtilities.WriteExceptionInfo(requestException);
            }
            // Other exceptions are not to be handled
        }
    }
}
