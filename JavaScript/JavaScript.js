const { Client, GatewayIntentBits, Events, REST, Routes, ActionRowBuilder, ButtonBuilder, ButtonStyle, EmbedBuilder, PermissionsBitField, InteractionType } = require('discord.js');
const { token } = require('./config.json'); // Tokenを含むconfig.jsonを用意してください

const client = new Client({ intents: [GatewayIntentBits.Guilds, GatewayIntentBits.GuildMessages, GatewayIntentBits.MessageContent, GatewayIntentBits.GuildMessageContent] });
const rest = new REST({ version: '10' }).setToken(token);

const commands = [
    {
        name: 'call',
        description: 'scpsl募集します指定のサーバーのみ機能します'
    },
    {
        name: 'info',
        description: 'BOTの情報を表示します'
    },
    {
        name: 'emoji',
        description: 'BOTに搭載されている絵文字をすべて表示します'
    },
    {
        name: 'botid',
        description: 'BOTのIDなどの情報を表示します'
    },
    {
        name: 'oplist',
        description: 'サーバー運営情報を表示します'
    },
    {
        name: 'ticket',
        description: 'チケットを作成',
        defaultPermission: false
    }
];

(async () => {
    try {
        console.log('Started refreshing application (/) commands.');

        await rest.put(Routes.applicationCommands(client.user.id), { body: commands });

        console.log('Successfully reloaded application (/) commands.');
    } catch (error) {
        console.error(error);
    }
})();

client.once(Events.ClientReady, () => {
    console.log('ログインしました');
    client.user.setPresence({
        activities: [{ name: 'BETA版です', type: 0 }], // 0 is PLAYING
        status: 'online'
    });
});

client.on(Events.InteractionCreate, async (interaction) => {
    if (interaction.type !== InteractionType.ApplicationCommand) return;

    const { commandName } = interaction;

    if (commandName === 'call') {
        await interaction.reply('<:mukou:1267467191142322216> エラー');
    } else if (commandName === 'info') {
        await interaction.reply({
            content: '開発者<:DEV:1267440009452064829><@1002132268736856136> 開発協力者<:ACCDEV:1267440500625903618> <@1032649313165258772> バージョン<:BE:1267439343882993734><:TA:1267439331069657119>v1',
            ephemeral: true
        });
    } else if (commandName === 'emoji') {
        await interaction.reply({
            content: '搭載絵文字 <:partner:1267440895037542471> <:ACCDEV:1267440500625903618> <:DEV:1267440009452064829> <:BE:1267439343882993734> <:TA:1267439331069657119> <:B_:1267438603718627378> <:mukou:1267467191142322216>',
            ephemeral: true
        });
    } else if (commandName === 'botid') {
        await interaction.reply({
            content: '<:mukou:1267467191142322216>このコマンドを実行する権限がありません',
            ephemeral: true
        });
    } else if (commandName === 'oplist') {
        await interaction.reply({
            content: '<:mukou:1267467191142322216> NODATA',
            ephemeral: true
        });
    } else if (commandName === 'ticket') {
        if (!interaction.memberPermissions.has(PermissionsBitField.Flags.Administrator)) {
            await interaction.reply({
                content: 'このコマンドを実行する権限がありません。',
                ephemeral: true
            });
            return;
        }

        const channel = interaction.channel;

        const embed = new EmbedBuilder()
            .setColor(0x178CE6)
            .setDescription("お問い合わせはこちらからチケットを発行してください！")
            .setFooter({ text: "Made by Spicy │2024/07/30" });

        await channel.send({ embeds: [embed] });

        const button = new ButtonBuilder()
            .setCustomId('create_ticket')
            .setLabel('チケットを作成')
            .setStyle(ButtonStyle.Primary);

        const row = new ActionRowBuilder().addComponents(button);

        await interaction.reply({ content: '', components: [row] });
    }
});

const ticketOwners = new Map();

client.on(Events.InteractionCreate, async (interaction) => {
    if (interaction.type !== InteractionType.MessageComponent) return;

    const customId = interaction.customId;

    if (customId === 'create_ticket') {
        const guild = interaction.guild;

        if (ticketOwners.has(interaction.user.id)) {
            await interaction.reply({ content: '既にチケットが存在します。', ephemeral: true });
            return;
        }

        const channelName = `チケット-${interaction.user.username}`;
        const overwrites = [
            {
                id: guild.id,
                deny: [PermissionsBitField.Flags.ViewChannel],
            },
            {
                id: interaction.guild.me.id,
                allow: [PermissionsBitField.Flags.ViewChannel],
            },
            {
                id: interaction.user.id,
                allow: [PermissionsBitField.Flags.ViewChannel],
            },
        ];

        const channel = await guild.channels.create({ name: channelName, type: 0, permissionOverwrites: overwrites });

        ticketOwners.set(interaction.user.id, channel.id);

        await channel.send(`チケットが作成されました！ ${interaction.user}`);
        const deleteButton = new ButtonBuilder()
            .setCustomId('delete_ticket')
            .setLabel('チケットを削除')
            .setStyle(ButtonStyle.Danger);

        const row = new ActionRowBuilder().addComponents(deleteButton);

        await channel.send({ components: [row] });

        await interaction.reply({ content: `チケットが作成されました！ ${channel}`, ephemeral: true });
    } else if (customId === 'delete_ticket') {
        const channel = interaction.channel;

        await channel.delete();
        ticketOwners.delete(interaction.user.id);
    }
});

client.login(token);
