const { Client, GatewayIntentBits, ActivityType, REST, Routes } = require('discord.js');
const { token, clientId, guildId } = require('./config.json');
const rest = new REST({ version: '10' }).setToken(token);

const client = new Client({ intents: [GatewayIntentBits.Guilds] });

client.once('ready', async () => {
    console.log('ログインしました');

    client.user.setPresence({
        status: 'online',
        activities: [{ name: 'BETA版です', type: ActivityType.Playing }],
    });

    const commands = [
        {
            name: 'call',
            description: 'scpsl募集します指定のサーバーのみ機能します',
        },
        {
            name: 'info',
            description: 'BOTの情報を表示します',
        },
        {
            name: 'emoji',
            description: 'BOTに搭載されている絵文字をすべて表示します',
        },
        {
            name: 'botid',
            description: 'BOTのIDなどの情報を表示します',
        },
    ];

    try {
        console.log('Started refreshing application (/) commands.');
        await rest.put(Routes.applicationGuildCommands(clientId, guildId), {
            body: commands,
        });

        console.log('Successfully reloaded application (/) commands.');
    } catch (error) {
        console.error(error);
    }
});

client.on('interactionCreate', async interaction => {
    if (!interaction.isCommand()) return;

    const { commandName } = interaction;

    if (commandName === 'call') {
        await interaction.reply('<@&1238062718825791590>');
    } else if (commandName === 'info') {
        await interaction.reply({ content: '開発者<:DEV:1267440009452064829><@1002132268736856136> 開発協力者<:ACCDEV:1267440500625903618> <@1032649313165258772> バージョン<:BE:1267439343882993734><:TA:1267439331069657119>v1', ephemeral: true });
    } else if (commandName === 'emoji') {
        await interaction.reply({ content: '搭載絵文字 <:partner:1267440895037542471> <:ACCDEV:1267440500625903618> <:DEV:1267440009452064829> <:BE:1267439343882993734> <:TA:1267439331069657119> <:B_:1267438603718627378> <:mukou:1267467191142322216>', ephemeral: true });
    } else if (commandName === 'botid') {
        await interaction.reply({ content: '<:mukou:1267467191142322216>このコマンドを実行する権限がありません ', ephemeral: true });
    }
});

client.login(token);
