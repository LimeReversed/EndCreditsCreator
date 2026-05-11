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

            var endCredits = new EndCredits(CPH);
            return endCredits.Execute();
        }
    }

    public class EndCredits
    {
        protected IInlineInvokeProxy _cph;
        public EndCredits(IInlineInvokeProxy cph)
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
        protected HashSet<string> _namesToExclude = new HashSet<string>
        {
            "Streamer.bot", "StreamElements", "Streamlabs", "Nightbot", "Moobot", "DeepBot", "WizeBot", "Fossabot", "PhantomBot", "StreamPuppy", "Muxy", "TwitchAlerts", "TwitchBot", "AnkhBot", "XanBot"
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
        public string Title { get; set; }
        public List<string> Names { get; set; } = new List<string>();
    }

    public class SectionCollector
    {
        public Section hypeTrainConductors = null;
        public Section hypeTrainContributors = null;
        public Section eventsCheerers = null;
        public Section eventsSubscribers = null;
        public Section eventsReSubscribers = null;
        public Section eventsGiftSubscribers = null;
        public Section eventsGiftBombers = null;
        public Section eventsRaids = null;
        public Section eventsRewardRedemptions = null;
        public Section eventsGoalContributions = null;
        public Section eventsGameUpdates = null;
        public Section eventsHypeTrains = null;
        public Section chatEditors = null;
        public Section chatModerators = null;
        public Section chatSubscribers = null;
        public Section chatVips = null;
        public Section chatUsers = null;
        public Section topAllTimeTopBitDonors = null;
        public Section topTopBitDonorsThisMonth = null;
        public Section topTopBitDonorsThisWeek = null;
        public Section topTopChannelRewardsRedeemers = null;
        public Section sponsors = null;

        public SectionCollector(CreditsData credits)
        {
            hypeTrainConductors = new Section { Title = "Conductors", Names = credits.HypeTrain.Conductors };
            hypeTrainContributors = new Section { Title = "Contributors", Names = credits.HypeTrain.Contributors };
            eventsCheerers = new Section { Title = "Cheerers", Names = credits.Events.Cheers };
            eventsSubscribers = new Section { Title = "Subscribers", Names = credits.Events.Subs };
            eventsReSubscribers = new Section { Title = "ReSubscribers", Names = credits.Events.ReSubs };
            eventsGiftSubscribers = new Section { Title = "Gift Subscribers", Names = credits.Events.GiftSubs };
            eventsGiftBombers = new Section { Title = "Gift Bombers", Names = credits.Events.GiftBombs };
            eventsRaids = new Section { Title = "Raiders", Names = credits.Events.Raided };
            eventsRewardRedemptions = new Section { Title = "Redeemers", Names = credits.Events.RewardRedemptions };
            eventsGoalContributions = new Section { Title = "Goal Contributions", Names = credits.Events.GoalContributions };
            eventsGameUpdates = new Section { Title = "Game Updates", Names = credits.Events.GameUpdates };
            eventsHypeTrains = new Section { Title = "Hype Trains", Names = credits.Events.HypeTrains };
            chatEditors = new Section { Title = "Editors", Names = credits.Users.Editors };
            chatModerators = new Section { Title = "Moderators", Names = credits.Users.Moderators };
            chatSubscribers = new Section { Title = "Subscribers", Names = credits.Users.Subscribers };
            chatVips = new Section { Title = "VIPs", Names = credits.Users.Vips };
            chatUsers = new Section { Title = "Users", Names = credits.Users.Users };
            topAllTimeTopBitDonors = new Section { Title = "All Time Top Bit Donors", Names = credits.Top.AllBits };
            topTopBitDonorsThisMonth = new Section { Title = "Top Bit Donors This Month", Names = credits.Top.MonthBits };
            topTopBitDonorsThisWeek = new Section { Title = "Top Bit Donors This Week", Names = credits.Top.WeekBits };
            topTopChannelRewardsRedeemers = new Section { Title = "Top Channel Rewards Redeemers", Names = credits.Top.TopChannelRewards };
            var sponsorsSet = new HashSet<string>();

            sponsorsSet.UnionWith(hypeTrainConductors.Names);
            sponsorsSet.UnionWith(hypeTrainContributors.Names);
            sponsorsSet.UnionWith(eventsCheerers.Names);
            sponsorsSet.UnionWith(eventsSubscribers.Names);
            sponsorsSet.UnionWith(eventsReSubscribers.Names);
            sponsorsSet.UnionWith(eventsGiftSubscribers.Names);
            sponsorsSet.UnionWith(eventsGiftBombers.Names);
            sponsorsSet.UnionWith(eventsHypeTrains.Names);
            sponsorsSet.UnionWith(chatSubscribers.Names);
            sponsorsSet.UnionWith(topTopBitDonorsThisMonth.Names);

            var sponsorsList = sponsorsSet.ToList();

            sponsors = new Section { Title = "Sponsors", Names = sponsorsList };
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
}