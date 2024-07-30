using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    private static DiscordSocketClient _client;
    private static InteractionService _commands;
    private static IServiceProvider _services;
    private static Dictionary<ulong, ulong> _ticketOwners = new Dictionary<ulong, ulong>();

    static async Task Main(string[] args)
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
        });

        _commands = new InteractionService(_client.Rest);

        _client.Log += Log;
        _client.Ready += OnReady;
        _client.InteractionCreated += OnInteraction;

        string token = "YOUR_TOKEN_HERE";

        await RegisterCommandsAsync();

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1); // Prevents the application from exiting immediately
    }

    private static Task Log(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }

    private static async Task OnReady()
    {
        Console.WriteLine("Logged in!");

        await _client.SetStatusAsync(UserStatus.Online);
        await _client.SetGameAsync("BETA版です");

        await RegisterCommandsAsync();
    }

    private static async Task RegisterCommandsAsync()
    {
        await _commands.AddModulesAsync(typeof(Program).Assembly, _services);
        await _commands.RegisterCommandsGloballyAsync();
    }

    private static async Task OnInteraction(SocketInteraction interaction)
    {
        var ctx = new SocketInteractionContext(_client, interaction);
        await _commands.ExecuteCommandAsync(ctx, _services);
    }
}

public class Commands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("call", "scpsl募集します指定のサーバーのみ機能します")]
    public async Task CallCommand()
    {
        await RespondAsync("<:mukou:1267467191142322216> エラー");
    }

    [SlashCommand("info", "BOTの情報を表示します")]
    public async Task InfoCommand()
    {
        await RespondAsync("開発者<:DEV:1267440009452064829><@1002132268736856136> 開発協力者<:ACCDEV:1267440500625903618> <@1032649313165258772> バージョン<:BE:1267439343882993734><:TA:1267439331069657119>v1", ephemeral: true);
    }

    [SlashCommand("emoji", "BOTに搭載されている絵文字をすべて表示します")]
    public async Task EmojiCommand()
    {
        await RespondAsync("搭載絵文字 <:partner:1267440895037542471> <:ACCDEV:1267440500625903618> <:DEV:1267440009452064829> <:BE:1267439343882993734> <:TA:1267439331069657119> <:B_:1267438603718627378> <:mukou:1267467191142322216>", ephemeral: true);
    }

    [SlashCommand("botid", "BOTのIDなどの情報を表示します")]
    public async Task BotIdCommand()
    {
        await RespondAsync("<:mukou:1267467191142322216>このコマンドを実行する権限がありません", ephemeral: true);
    }

    [SlashCommand("oplist", "サーバー運営情報を表示します")]
    public async Task OpListCommand()
    {
        await RespondAsync("<:mukou:1267467191142322216> NODATA", ephemeral: true);
    }

    [SlashCommand("ticket", "チケットを作成")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task CreateTicketCommand()
    {
        var channel = Context.Channel as ITextChannel;
        var embed = new EmbedBuilder()
            .WithTitle("")
            .WithColor(Color.Blue)
            .WithDescription("")
            .WithFooter("Made by Spicy │2024/07/30")
            .AddField("チケット", "お問い合わせはこちらからチケットを発行してください!");

        await channel.SendMessageAsync(embed: embed.Build());

        var componentBuilder = new ComponentBuilder()
            .WithButton("チケットを作成", "create_ticket", ButtonStyle.Primary);

        await channel.SendMessageAsync("", components: componentBuilder.Build());
    }

    [ComponentInteraction("create_ticket")]
    public async Task CreateTicket()
    {
        var guild = (SocketGuild)Context.Guild;
        var user = Context.User as SocketGuildUser;

        if (_ticketOwners.ContainsValue(user.Id))
        {
            await RespondAsync("既にチケットが存在します。", ephemeral: true);
            return;
        }

        var channelName = $"チケット-{user.Username}";
        var permissions = new OverwritePermissions(
            viewChannel: PermValue.Deny,
            sendMessages: PermValue.Deny,
            readMessageHistory: PermValue.Deny
        );
        var ticketChannel = await guild.CreateTextChannelAsync(channelName, properties: new ChannelCreationProperties
        {
            PermissionOverwrites = new[] {
                new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role, permissions),
                new Overwrite(Context.Guild.CurrentUser.Id, PermissionTarget.User, new OverwritePermissions(PermValue.Allow, PermValue.Allow, PermValue.Allow)),
                new Overwrite(user.Id, PermissionTarget.User, new OverwritePermissions(PermValue.Allow, PermValue.Allow, PermValue.Allow))
            }
        });

        _ticketOwners[ticketChannel.Id] = user.Id;

        await ticketChannel.SendMessageAsync($"{user.Mention} チケットが作成されました!");

        var embed = new EmbedBuilder()
            .WithTitle("")
            .WithColor(Color.Red)
            .AddField("チケット", "チケットが作成されました！\n" + ticketChannel.Mention);

        await RespondAsync(embed: embed.Build(), ephemeral: true);

        var componentBuilder = new ComponentBuilder()
            .WithButton("チケットを削除", "delete_ticket", ButtonStyle.Danger);

        await ticketChannel.SendMessageAsync("", components: componentBuilder.Build());
    }

    [ComponentInteraction("delete_ticket")]
    public async Task DeleteTicket()
    {
        var channel = Context.Channel as ITextChannel;

        await channel.DeleteAsync();

        if (_ticketOwners.ContainsKey(channel.Id))
        {
            _ticketOwners.Remove(channel.Id);
        }

        await RespondAsync("チケットが削除されました", ephemeral: true);
    }
}
