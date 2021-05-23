using Database;
using Database.Models;
using Discord;
using Discord.Webhook;
using LogParser.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogParser.Services
{
    public class DiscordService : IDiscordService
    {
        private DatabaseContext dbContext;

        public DiscordService(DatabaseContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SendReports(IList<ParsedLogFileDto> logs)
        {
            if (logs == null)
            {
                throw new ArgumentNullException(nameof(logs));
            }

            string discordWebhookUrl = await SettingsManager.GetDiscordWebhookUrl(dbContext).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(discordWebhookUrl))
            {
                return;
            }

            string discordWebhookName = await SettingsManager.GetDiscordWebhookName(dbContext).ConfigureAwait(true);
            discordWebhookName = string.IsNullOrWhiteSpace(discordWebhookName) ? string.Empty : discordWebhookName;

            using var webhookClient = new DiscordWebhookClient(discordWebhookUrl);
            var embedBuilder = new EmbedBuilder
            {
                Color = Color.DarkBlue,
                Title = "Logs",
            };

            foreach (var log in logs)
            {
                var emote = GetBossEmote(log.TriggerID);
                string success = log.Success ? "  -  :white_check_mark:" : "  -  :x:";
                string value = log.DpsReportLink ?? " - ";
                string fieldName = emote != null ? emote + "  " : string.Empty;
                fieldName += log.BossName + success;
                embedBuilder.AddField(fieldName, value);
            }

            List<Embed> embeds = new List<Embed>
            {
                embedBuilder.Build(),
            };

            await webhookClient.SendMessageAsync(embeds: embeds, username: discordWebhookName).ConfigureAwait(false);
        }

        private Emote GetBossEmote(long bossId)
        {
            string emoteString = string.Empty;

            switch (bossId)
            {
                case 15438: // Value Guardian
                    emoteString = "<:vg:501749258320478228>";
                    break;
                case 15429: // Gorseval
                    emoteString = "<:gors:501748869017894912>";
                    break;
                case 15375: // Sabetha
                    emoteString = "<:sab:501749126061621260>";
                    break;
                case 16123: // Slothasor
                    emoteString = "<:sloth:501749219342811156>";
                    break;
                case 16088: // Berg
                case 16137: // Zane
                case 16125: // Narella
                    emoteString = "<:trio:576146907588460559>";
                    break;
                case 16115: // Matthias
                    emoteString = "<:matt:501748945060626444>";
                    break;
                case 16241: // McLeod / Escort
                    //emoteString = "<:gdn:484839339474288650>";
                    emoteString = string.Empty;
                    break;
                case 16235: // Keep Construct
                    emoteString = "<:kc:501748909010583552>";
                    break;
                case 16246: // Xera
                case 16286:
                    emoteString = "<:xera:501749293783318548>";
                    break;
                case 17194: // Cairn
                    emoteString = "<:cairn:501748574632280074>";
                    break;
                case 17172: // Mursaat Overseer (MO)
                    emoteString = "<:mo:501748997174591492>";
                    break;
                case 17188: // Samarog
                    emoteString = "<:sam:501749156394696704>";
                    break;
                case 17154: // Deimos
                    emoteString = "<:deimos:501749064346632213>";
                    break;
                case 19767: // Soulles Horror
                    emoteString = "<:horror:501748773752537088>";
                    break;
                case 19828: // River of Souls
                    //emoteString = "<:gdn:484839339474288650>";
                    emoteString = string.Empty;
                    break;
                case 19691: // Statues of Grenth
                case 19536:
                case 19651:
                case 19844:
                    //emoteString = "<:gors:501748869017894912>";
                    emoteString = string.Empty;
                    break;
                case 19450: // Dhuum
                    emoteString = "<:dhuum:501748828945383424>";
                    break;
                case 43974: // Conjured Amalgamate
                    emoteString = "<:ca:501871735503585300>";
                    break;
                case 21105: // Nikare (Twins)
                case 21089: // Kenut (Twins)
                    emoteString = "<:larg:501865906587041807>";
                    break;
                case 20934: // Qadim
                    emoteString = "<:qad:501865963143036939>";
                    break;
                case 21964: // Cardinal Sabir
                    emoteString = "<:sabi:591359598175191040>";
                    break;
                case 22006: // Cardinal Adina
                    emoteString = "<:adi:591359582740414500>";
                    break;
                case 22000: // Qadim the Peerless
                    emoteString = "<:qad2:591359616189988864>";
                    break;
            }

            _ = Emote.TryParse(emoteString, out var emote);
            return emote;
        }
    }
}
