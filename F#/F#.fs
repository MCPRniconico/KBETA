open DSharpPlus
open DSharpPlus.EventArgs
open DSharpPlus.CommandsNext
open DSharpPlus.SlashCommands
open DSharpPlus.Interactivity
open System.Threading.Tasks

// Define your bot token here
let botToken = "YOUR_BOT_TOKEN_HERE"

// Create a new Discord client
let client = DiscordClient()

// Create a new SlashCommands extension
let slashCommands = SlashCommandsExtension()

// Event handler for when the bot is ready
let onReady (args: ReadyEventArgs) =
    printfn "Bot is online"
    args.Client.UpdateStatusAsync(ActivityType.Custom, "BETA版です") |> ignore
    Task.CompletedTask

// Event handler for interacting with commands
let onSlashCommandExecuted (args: SlashCommandExecutedEventArgs) =
    match args.CommandName with
    | "call" -> args.CommandContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, "エラー") |> ignore
    | "info" -> args.CommandContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, "開発者 <@1002132268736856136> 開発協力者 <@1032649313165258772> バージョン v1") |> ignore
    | "emoji" -> args.CommandContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, "搭載絵文字 <:partner:1267440895037542471> <:ACCDEV:1267440500625903618> <:DEV:1267440009452064829> <:BE:1267439343882993734> <:TA:1267439331069657119> <:B_:1267438603718627378> <:mukou:1267467191142322216>") |> ignore
    | "botid" -> args.CommandContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, "このコマンドを実行する権限がありません") |> ignore
    | "oplist" -> args.CommandContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, "NODATA") |> ignore
    | "ticket" -> 
        let channel = args.CommandContext.Channel
        let embed = DiscordEmbedBuilder()
        embed.Title <- ""
        embed.Color <- DiscordColor.Blue
        embed.Description <- "お問い合わせはこちらからチケットを発行してください！"
        embed.Footer <- EmbedFooter("Made by Spicy │2024/07/30", "")
        channel.SendMessageAsync(embed: embed) |> ignore
        Task.CompletedTask
    | _ -> Task.CompletedTask

// Configure the bot client
let configureClient () =
    client.UseSlashCommands()
    client.Ready.Add(onReady)
    client.SlashCommandExecuted.Add(onSlashCommandExecuted)

// Run the bot
[<EntryPoint>]
let main argv =
    configureClient()
    client.LoginAsync(TokenType.Bot, botToken) |> ignore
    client.StartAsync() |> ignore
    System.Console.ReadLine() |> ignore
    0
