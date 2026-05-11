using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Streamer.bot.Plugin.Interface;

namespace EndCredits
{
    public class EndCreditsInStreamerbot
    {
        protected IInlineInvokeProxy _cph;

        public bool Execute()
        {
            bool success = _cph.TryGetArg("credits", out string creditsString);
            if (!success)
            {
                _cph.SetArgument("creditsText", "Not found");
                return true;
            }

            var credits = JsonConvert.DeserializeObject<CreditsData>(creditsString);
            var endCreditsCreator = new EndCreditsCreator(credits);


            _cph.SetArgument("creditsText", endCreditsCreator.GetFullCredits());
            return true;
        }
    }
}

public class EndCreditsCreator
{
    public int LineCount { get; set; }
    public string CreditsString { get; set; } = "                         \n";
    protected HashSet<string> _namesToExclude = new HashSet<string>
    {
        "Streamer.bot", "StreamElements", "Streamlabs", "Nightbot", "Moobot", "DeepBot", "WizeBot", "Fossabot", "PhantomBot", "StreamPuppy", "Muxy", "TwitchAlerts", "TwitchBot", "AnkhBot", "XanBot"
    };
    private CreditsData _creditsData;

    public EndCreditsCreator(CreditsData creditsData)
    {
        _creditsData = creditsData;
    }

    public void CreateCreditsFromType<T>(T type)
    {
        if (type != null)
        {
            List<string> mergedLists = new List<string>();
            foreach (var property in typeof(T).GetProperties())
            {
                var list = (List<string>)property.GetValue(type);
                if (list?.Count > 0)
                {
                    LineCount += list.Count + 1;
                    CreditsString += list.CreateSection(property.Name, _namesToExclude);
                }
            }
        }
    }

    public void CreateCreditsFromList(List<string> list)
    {
        if (list?.Count > 0)
        {
            LineCount += list.Count + 1;
            CreditsString += list.CreateSection("HypeTrainConductor", _namesToExclude);
        }
    }

    public string GetFullCredits()
    {
        CreateCreditsFromType(_creditsData.Users);
        CreateCreditsFromType(_creditsData.Events);

        CreateCreditsFromList(_creditsData.HypeTrain.Conductors);
        CreateCreditsFromList(_creditsData.HypeTrain.Contributors);

        CreateCreditsFromType(_creditsData.Top);

        int minimumLines = 15;
        int linesLeft = Math.Max((int)0, (int)minimumLines - (int)LineCount);

        for (int i = 0; i < linesLeft; i++)
        {
            CreditsString += "\n";
        }
        return CreditsString;
    }
}

public class CreditsData
{
    public EventsData Events { get; set; }
    public HypeTrainData HypeTrain { get; set; }
    public UserData Users { get; set; }
    public Dictionary<string, object> Custom { get; set; }
    public TopBitsData Top { get; set; }

}

public class HypeTrainData
{
    public List<string> Conductors { get; set; }
    public List<string> Contributors { get; set; }
}

public class EventsData
{
    // public List<string> Follows { get; set; }
    public List<string> Cheers { get; set; }
    public List<string> Subs { get; set; }
    public List<string> ReSubs { get; set; }
    public List<string> GiftSubs { get; set; }
    public List<string> GiftBombs { get; set; }
    public List<string> Raided { get; set; }
    public List<string> RewardRedemptions { get; set; }
    public List<string> GoalContributions { get; set; }
    public List<string> GameUpdates { get; set; }
    public List<string> Pyramids { get; set; }
    public List<string> HypeTrains { get; set; }
}

public class UserData
{
    public List<string> Editors { get; set; }
    public List<string> Moderators { get; set; }
    // public List<string> Subscribers { get; set; }
    public List<string> Vips { get; set; }
    public List<string> Users { get; set; }
}

public class TopBitsData
{
    // public List<string> AllBits { get; set; }
    // public List<string> MonthBits { get; set; }
    // public List<string> WeekBits { get; set; }
    // public List<string> TopChannelRewards { get; set; }
}

public static class Helper
{



    public static string ToCreditString(this List<string> list, HashSet<string> namesToExclude = null)
    {
        string result = null;
        foreach (string item in list)
        {
            if (namesToExclude == null || !namesToExclude.Contains(item))
            {
                result += $"{item}\n";
            }
        }

        return result;
    }

    public static string CreateSection(this List<string> list, string title, HashSet<string> namesToExclude = null)
    {
        string result = null;
        result = $"- {title} -\n";
        result += list.ToCreditString(namesToExclude);
        result += "\n";
        return result;
    }
}
