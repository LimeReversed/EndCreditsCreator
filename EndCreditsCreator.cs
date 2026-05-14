using Newtonsoft.Json;
using Streamer.bot.Plugin.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EndCredits
{
    public class EndCreditsStreamerbot
    {
        IInlineInvokeProxy CPH;
        public bool Execute()
        {

            var endCredits = new EndCreditsPlugin(CPH);
            return endCredits.Execute();
        }
    }

    public class EndCreditsPlugin
    {
        protected IInlineInvokeProxy _cph;
        public EndCreditsPlugin(IInlineInvokeProxy cph)
        {
            _cph = cph;
        }   

        public bool Execute()
        {
            bool success = _cph.TryGetArg("credits", out string creditsString);
            if (!success)
            {
                _cph.SetArgument("creditsText", "Not found");
                return true;
            }

            var credits = JsonConvert.DeserializeObject<CreditsData>(creditsString);
            var sectionCollector = new SectionCollector(credits);

            var sections = new List<Section>
            {
                sectionCollector.chatModerators, sectionCollector.chatUsers, sectionCollector.sponsors, sectionCollector.eventsRaids, sectionCollector.eventsRewardRedemptions, 
            };

            var endCreditsCreator = new EndCreditsCreator(sections);

            _cph.SetArgument("creditsText", endCreditsCreator.GetFullCredits());
            return true;
        }
    }

    public class EndCreditsCreator
    {
        public int LineCount { get; set; }
        public string CreditsString { get; set; } = "                         \n";
        protected HashSet<string> _namesToExclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Streamer.bot", "StreamElements", "Streamlabs", "Nightbot", "Moobot", "DeepBot", "WizeBot", "Fossabot", "PhantomBot", "StreamPuppy", "Muxy", "TwitchAlerts", "TwitchBot", "AnkhBot", "XanBot", "Sery_Bot"
        };

        private List<Section> _sections = new List<Section>();
        public EndCreditsCreator(List<Section> sections)
        {
            _sections = sections;
        }

        public void CreateCreditsFromSections(List<Section> sections)
        {
            if (sections?.Count > 0)
            {
                foreach (var section in sections)
                {
                    CreateCreditsFromSection(section);
                }
            }
        }

        public void CreateCreditsFromSection(Section section)
        {
            if (section?.Names?.Count > 0)
            {
                LineCount += section.Names.Count + 1;
                CreditsString += section.Names.CreateSection(section.Title, _namesToExclude);
            }
        }

        public string GetFullCredits()
        {
            CreateCreditsFromSections(_sections);

            int minimumLines = 15;
            int linesLeft = Math.Max((int)0, (int)minimumLines - (int)LineCount);

            for (int i = 0; i < linesLeft; i++)
            {
                CreditsString += "\n";
            }
            return CreditsString;
        }
    }

    public class Section
    {
        public string Title { get; set; } = "";
        public List<string> Names { get; set; } = new List<string>();

        public Section(string title, List<string> names)
        {
            Title = title;
            Names = names;
        }

        public Section() 
        {
            
        }
    }

    public class SectionCollector
    {
        public Section hypeTrainConductors = new Section();
        public Section hypeTrainContributors = new Section();
        public Section eventsCheerers = new Section();
        public Section eventsSubscribers = new Section();
        public Section eventsReSubscribers = new Section();
        public Section eventsGiftSubscribers = new Section();
        public Section eventsGiftBombers = new Section();
        public Section eventsRaids = new Section();
        public Section eventsRewardRedemptions = new Section();
        public Section eventsGoalContributions = new Section();
        public Section eventsGameUpdates = new Section();
        public Section eventsHypeTrains = new Section();
        public Section chatEditors = new Section();
        public Section chatModerators = new Section();
        public Section chatSubscribers = new Section();
        public Section chatVips = new Section();
        public Section chatUsers = new Section();
        public Section topAllTimeTopBitDonors = new Section();
        public Section topTopBitDonorsThisMonth = new Section();
        public Section topTopBitDonorsThisWeek = new Section();
        public Section topTopChannelRewardsRedeemers = new Section();
        public Section sponsors = new Section();

        public SectionCollector(CreditsData credits)
        {
            hypeTrainConductors = new Section("Conductors", credits.HypeTrain.Conductors ?? new List<string>());
            hypeTrainContributors = new Section("Contributors", credits.HypeTrain.Contributors ?? new List<string>());
            eventsCheerers = new Section("Cheerers", credits.Events.Cheers ?? new List<string>());
            eventsSubscribers = new Section("Subscribers", credits.Events.Subs ?? new List<string>());
            eventsReSubscribers = new Section("ReSubscribers", credits.Events.ReSubs ?? new List<string>());
            eventsGiftSubscribers = new Section("Gift Subscribers", credits.Events.GiftSubs ?? new List<string>());
            eventsGiftBombers = new Section("Gift Bombers", credits.Events.GiftBombs ?? new List<string>());
            eventsRaids = new Section("Raiders", credits.Events.Raided ?? new List<string>());
            eventsRewardRedemptions = new Section("Redeemers", credits.Events.RewardRedemptions ?? new List<string>());
            eventsGoalContributions = new Section("Goal Contributions", credits.Events.GoalContributions ?? new List<string>());
            eventsGameUpdates = new Section("Game Updates", credits.Events.GameUpdates ?? new List<string>());
            eventsHypeTrains = new Section("Hype Trains", credits.Events.HypeTrains ?? new List<string>());
            chatEditors = new Section("Editors", credits.Users.Editors ?? new List<string>());
            chatModerators = new Section("Moderators", credits.Users.Moderators ?? new List<string>());
            chatSubscribers = new Section("Subscribers", credits.Users.Subscribers ?? new List<string>());
            chatVips = new Section("VIPs", credits.Users.Vips ?? new List<string>());
            chatUsers = new Section("Users", credits.Users.Users ?? new List<string>());
            topAllTimeTopBitDonors = new Section("All Time Top Bit Donors", credits.Top.AllBits ?? new List<string>());
            topTopBitDonorsThisMonth = new Section("Top Bit Donors This Month", credits.Top.MonthBits ?? new List<string>());
            topTopBitDonorsThisWeek = new Section("Top Bit Donors This Week", credits.Top.WeekBits ?? new List<string>());
            topTopChannelRewardsRedeemers = new Section("Top Channel Rewards Redeemers", credits.Top.TopChannelRewards ?? new List<string>());
            var sponsorsSet = new HashSet<string>();

            sponsorsSet.UnionWith(hypeTrainConductors.Names);
            sponsorsSet.UnionWith(hypeTrainContributors.Names);
            sponsorsSet.UnionWith(eventsCheerers.Names);
            sponsorsSet.UnionWith(eventsSubscribers.Names);
            sponsorsSet.UnionWith(eventsReSubscribers.Names);
            sponsorsSet.UnionWith(eventsGiftSubscribers.Names);
            sponsorsSet.UnionWith(eventsGiftBombers.Names);
            sponsorsSet.UnionWith(eventsHypeTrains.Names);

            var sponsorsList = sponsorsSet.ToList();

            sponsors = new Section("Today's Sponsors", sponsorsList);
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
        public List<string> Follows { get; set; }
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
        public List<string> Subscribers { get; set; }
        public List<string> Vips { get; set; }
        public List<string> Users { get; set; }
    }

    public class TopBitsData
    {
        public List<string> AllBits { get; set; }
        public List<string> MonthBits { get; set; }
        public List<string> WeekBits { get; set; }
        public List<string> TopChannelRewards { get; set; }
    }

    public static class Helper
    {
        public static string ToCreditString(this List<string> list, HashSet<string> namesToExclude = null)
        {
            string result = null;
            foreach (string item in list)
            {
                bool shouldAdd = namesToExclude == null || (namesToExclude != null && !namesToExclude.Contains(item));
                if (shouldAdd)
                {
                    result += $"{item}\n";
                }
            }

            return result;
        }

        public static string CreateSection(this List<string> list, string title, HashSet<string> namesToExclude = null)
        {
            if (list.Count == 1 && namesToExclude != null && namesToExclude.Contains(list[0]))
            {
                return null;
            }

            string result = null;
            result = $"- {title} -\n";
            result += list.ToCreditString(namesToExclude);
            result += "\n";
            return result;
        }
    }
}