using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot
{
    public class BotCommands
    {
        private readonly DiscordSocketClient _client;
        private readonly string _botToken;

        public BotCommands(string botToken)
        {
            _botToken = botToken;

            // Create a new configuration with the necessary intents
            var config = new DiscordSocketConfig
            {
                // Enable the necessary intents for the bot to work with guild members and message content
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.MessageContent
            };

            _client = new DiscordSocketClient(config); // Pass the config to the client
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Atempting to Login...");
            Console.ResetColor();
            _client.Ready += OnReady;
        }

        private Task Log(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        private async Task OnReady()
        {
            int n = 2;
            Console.Title = "Lotus - Nebula (Online)";
            Console.Beep();
            Console.Beep();
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("[+] Bot is online");
            Console.ResetColor();

            // List the guilds (servers) the bot is in
            var guilds = _client.Guilds.ToList();
            for (int i = 0; i < guilds.Count; i++)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\n{i + 1}. {guilds[i].Name} (ID: {guilds[i].Id})");
                Console.ResetColor();
            }

            Console.Write("\nSelect a server by number: ");
            int serverChoice;
            if (!int.TryParse(Console.ReadLine(), out serverChoice) || serverChoice < 1 || serverChoice > guilds.Count)
            {
                Console.WriteLine("Invalid choice!");
                return;
            }

            var selectedGuild = guilds[serverChoice - 1];

            // Ensure members are loaded
            await selectedGuild.DownloadUsersAsync();

            await ListMembersInGuild(selectedGuild);

            Console.WriteLine("\nDo you want to ban or kick a member? (Type 'ban' or 'kick')");
            string action = Console.ReadLine().ToLower();

            if (action == "kick")
            {
                await KickMember(selectedGuild);
            }
            else if (action == "ban")
            {
                await BanMember(selectedGuild);
            }
            else
            {
                Console.WriteLine("Invalid action. Please type 'ban' or 'kick'.");
            }
        }

        public async Task ListMembersInGuild(SocketGuild selectedGuild)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\nListing members for server: {selectedGuild.Name}");

            // Make sure to download all users in case of large guilds
            await selectedGuild.DownloadUsersAsync();

            // Get all the members of the selected guild
            var members = selectedGuild.Users.ToList();

            Console.WriteLine($"\nTotal members: {members.Count}\n");

            // List the members in the server with numbering
            for (int i = 0; i < members.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {members[i].Username} (ID: {members[i].Id})");
            }
            Console.ResetColor();
            Console.WriteLine("\nEnd of member list.");
        }

        private async Task KickMember(SocketGuild selectedGuild)
        {
            Console.Write("\nEnter the number of the user you want to kick: ");
            string userChoiceInput = Console.ReadLine();

            if (!int.TryParse(userChoiceInput, out int userChoice))
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            var members = selectedGuild.Users.ToList();
            if (userChoice < 1 || userChoice > members.Count)
            {
                Console.WriteLine("Invalid user number.");
                return;
            }

            var user = members[userChoice - 1]; // Adjust for zero-based index
            await user.KickAsync("Kicked by bot");
            Console.Clear();
            Console.WriteLine($"User {user.Username} has been kicked.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            await Task.CompletedTask;
        }

        private async Task BanMember(SocketGuild selectedGuild)
        {
            Console.Write("\nEnter the number of the user you want to ban: ");
            string userChoiceInput = Console.ReadLine();

            if (!int.TryParse(userChoiceInput, out int userChoice))
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            var members = selectedGuild.Users.ToList();
            if (userChoice < 1 || userChoice > members.Count)
            {
                Console.WriteLine("Invalid user number.");
                return;
            }

            var user = members[userChoice - 1]; // Adjust for zero-based index
            await selectedGuild.AddBanAsync(user, 7, "Banned by bot");
            Console.Clear();
            Console.WriteLine($"User {user.Username} has been banned.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            await Task.CompletedTask;
        }

        public async Task StartBotAsync()
        {
            try
            {
                await _client.LoginAsync(TokenType.Bot, _botToken);
                await _client.StartAsync();
                await Task.Delay(-1); // Keep the bot running indefinitely
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public class Program
    {
        public static async Task Main()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("\nMade by v1s0or, Kick and Ban Service For Lotus.\n");
            Console.ResetColor();

            Console.Title = "Lotus / Nebula";
            Console.WriteLine("Enter your bot token: ");
            string botToken = Console.ReadLine();

            if (botToken == string.Empty) {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Enter a Token!");
                Console.ResetColor();
                Console.ReadLine();
                Console.Clear();
                await Main();
            }

            if (botToken == "null") {
                Console.WriteLine("really.");
                Console.ReadKey();
                Console.Clear();
                await Main();
            }

            var botCommands = new BotCommands(botToken);
            await botCommands.StartBotAsync();
        }
    }
}