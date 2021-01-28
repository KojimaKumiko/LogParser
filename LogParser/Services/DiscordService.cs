using Database;
using Database.Models;
using Discord;
using Discord.Webhook;
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

        public async Task SendReports(IList<ParsedLogFile> logs)
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
                string success = log.Success ? "  -  :white_check_mark:" : "  -  :x:";
                string value = log.DpsReportLink ?? " - ";
                embedBuilder.AddField(log.BossName + success, value);
            }

            List<Embed> embeds = new List<Embed>
            {
                embedBuilder.Build(),
            };

            await webhookClient.SendMessageAsync(embeds: embeds, username: discordWebhookName).ConfigureAwait(false);
        }
    }
}
