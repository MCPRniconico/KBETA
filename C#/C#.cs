using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;

class Program
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly CommandService _commandService;
    private readonly IServiceProvider _services;

    private Dictionary<ulong, ulong> ticketOwners = new Dictionary<ulong, ulong>();

    static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

    public Program()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
            GatewayIntents = GatewayIntents.AllUnprivileged
        });

        _commandService = new CommandService();
        _interactionService = new InteractionService(_client);

        _client.Log += Log;
        _client.Ready += OnReadyAsync;
        _client.InteractionCreated += HandleInteractionAsync;
    }

    private async Task RunBotAsync()
    {
        await _client.LoginAsync(TokenType.Bot, "YOUR_BOT_TOKEN");
        await _client.StartAsync();

        await _interactionService.AddModulesAsync(typeof(Program).Assembly, _services);
        await Task.Delay(-1);
    }

    private async Task OnReadyAsync()
    {
        Console.WriteLine("ログインしました");

        await _client.SetStatusAsync(UserStatus.Online);
        await _client.SetGameAsync("バグの調査中", null, ActivityType.CustomStatus);

        await _interactionService.RegisterCommandsGloballyAsync();
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private async Task HandleInteractionAsync(SocketInteraction interaction)
    {
        try
        {
            var customId = (interaction as SocketMessageComponent)?.Data.CustomId;
            if (customId == "create_ticket")
            {
                var user = interaction.User as SocketGuildUser;
                var server = user.Guild;

                if (ticketOwners.ContainsValue(user.Id))
                {
                    await interaction.RespondAsync("既にチケットが存在します。", ephemeral: true);
                    return;
                }

                var channel = await server.CreateTextChannelAsync($"チケット-{user.Username}", x =>
                {
                    x.PermissionOverwrites = new OverwritePermissions[]
                    {
                        new OverwritePermissions(user.Id, new OverwritePermissions(0, PermValue.Deny)),
                        new OverwritePermissions(_client.CurrentUser.Id, new OverwritePermissions(PermValue.Allow, PermValue.Allow)),
                        new OverwritePermissions(server.EveryoneRole.Id, new OverwritePermissions(0, PermValue.Deny))
                    };
                });

                ticketOwners[channel.Id] = user.Id;

                await channel.SendMessageAsync($"{user.Mention} チケットが作成されました!");

                var builder = new ComponentBuilder()
                    .WithButton("チケットを削除", "delete_ticket", ButtonStyle.Danger);
                await channel.SendMessageAsync("", components: builder.Build());
            }
            else if (customId == "delete_ticket")
            {
                var channel = interaction.Channel as SocketTextChannel;

                if (channel != null && ticketOwners.ContainsKey(channel.Id))
                {
                    await channel.DeleteAsync();
                    ticketOwners.Remove(channel.Id);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    [SlashCommand("call", "scpsl募集します指定のサーバーのみ機能します")]
    public async Task CallCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("<:__:1267673604020109452>エラー");
    }

    [SlashCommand("info", "BOTの情報を表示します")]
    public async Task InfoCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("開発者<:DEV:1267440009452064829><@1002132268736856136> 開発協力者<:ACCDEV:1267440500625903618> <@1032649313165258772> バージョン<:BE:1267439343882993734><:TA:1267439331069657119>v1", ephemeral: true);
    }

    [SlashCommand("emoji", "BOTに搭載されている絵文字をすべて表示します")]
    public async Task EmojiCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("搭載絵文字 <:partner:1267440895037542471> <:ACCDEV:1267440500625903618> <:DEV:1267440009452064829> <:BE:1267439343882993734> <:TA:1267439331069657119> <:B_:1267438603718627378> <:mukou:1267467191142322216>", ephemeral: true);
    }

    [SlashCommand("botid", "BOTのIDなどの情報を表示します")]
    public async Task BotIdCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("<:mukou:1267467191142322216>このコマンドを実行する権限がありません ", ephemeral: true);
    }

    [SlashCommand("oplist", "サーバー運営情報を表示します")]
    public async Task OpListCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("<:mukou:1267467191142322216> NODATA", ephemeral: true);
    }

    [SlashCommand("ticket", "チケットを作成")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task CreateTicketCommand(SocketSlashCommand command)
    {
        var builder = new ComponentBuilder()
            .WithButton("チケットを作成", "create_ticket", ButtonStyle.Primary);

        var embed = new EmbedBuilder()
            .WithColor(Color.Blue)
            .WithFooter(footer => footer.Text = "Made by Spicy │2024/07/30")
            .AddField("チケット", "お問い合わせはこちらからチケットを発行してください！");

        await command.Channel.SendMessageAsync(embed: embed.Build(), components: builder.Build());
    }

    [SlashCommand("embed", "運営パートナーを表示します")]
    public async Task EmbedCommand(SocketSlashCommand command)
    {
        var embed = new EmbedBuilder()
            .WithTitle("リスト")
            .WithDescription("<:DEV:1267440009452064829>所有者<@1002132268736856136>　STEAMID:76561199555791158")
            .WithColor(Color.Green)
            .AddField("EVENTManager", "<@1189153844043722753> STEAMID 76561199525003326", false)
            .AddField("EVENTManager", "NotData", true)
            .AddField("moderator", "NotDATA。", false)
            .AddField("<:1109542935038545960:1267745305214849035>Trusted", "<@1135069041569054750> ", true)
            .AddField("<:1109542935038545960:1267745305214849035>Trusted", "<@1104907457597296762> ", false)
            .AddField("<:1109542935038545960:1267745305214849035>Trusted", "<@716212058445709362> ", true)
            .AddField("<:1109542935038545960:1267745305214849035>Trusted", "<@1234473338706067486>", false)
            .AddField("<:1109542935038545960:1267745305214849035>Trusted", "<@1085931921202229259> ", true)
            .AddField("<:partner:1267440895037542471>YT", "<@1101479370197061672>。", false)
            .AddField("<:partner:1267440895037542471>YT", "<@1020505405769666621>。", true)
            .AddField("<:partner:1267440895037542471>YT", "<@1039717417116512327>。", false)
            .AddField("<:partner:1267440895037542471>YT", "<@463867826676039690>。", true)
            .AddField("<:partner:1267440895037542471>YT", "<@1020505405769666621>。", false)
            .AddField("<:partner:1267440895037542471>YT", "<@297766775653924865>。", true)
            .AddField("<:partner:1267440895037542471>YT", "<@1002132268736856136>。", false)
            .AddField("<:partner2:1267748273502818395>パートナー", "<@1101479370197061672>。", true)
            .AddField("DIscordr", "Notdata。", false)
            .WithFooter(footer => footer.Text = "2024/7/30")
            .WithThumbnailUrl("https://example.com/thumbnail.jpg")
            .WithImageUrl("https://example.com/image.jpg");

        await command.RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}
