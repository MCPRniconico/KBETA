const { Client, GatewayIntentBits, Partials, EmbedBuilder, ButtonBuilder, ButtonStyle, ActionRowBuilder } = require('discord.js');
const { REST } = require('@discordjs/rest');
const { Routes } = require('discord-api-types/v9');
const token = 'YOUR_BOT_TOKEN';

const client = new Client({
    intents: [
        GatewayIntentBits.Guilds,
        GatewayIntentBits.GuildMessages,
        GatewayIntentBits.MessageContent,
        GatewayIntentBits.GuildMembers
    ],
    partials: [Partials.Channel]
});

client.once('ready', () => {
    console.log('Logged in!');
    client.user.setPresence({ activities: [{ name: 'Investigating bugs' }], status: 'online' });
    const rest = new REST({ version: '9' }).setToken(token);
    rest.put(
        Routes.applicationCommands(client.user.id),
        { body: commands }
    )
    .then(() => console.log('Successfully registered application commands.'))
    .catch(console.error);
});

const commands = [
    {
        name: 'call',
        description: 'Recruit for SCP:SL, only works on specified servers'
    },
    {
        name: 'info',
        description: 'Displays bot information'
    },
    {
        name: 'emoji',
        description: 'Displays all emojis installed in the bot'
    },
    {
        name: 'botid',
        description: 'Displays bot ID information'
    },
    {
        name: 'oplist',
        description: 'Displays server operation information'
    },
    {
        name: 'ticket',
        description: 'Create a ticket',
        default_permission: false
    },
    {
        name: 'embed',
        description: 'Displays operational partners'
    }
];

client.on('interactionCreate', async interaction => {
    if (!interaction.isCommand()) return;

    const { commandName } = interaction;

    if (commandName === 'call') {
        await interaction.reply('<:__:1267673604020109452>エラー');
    } else if (commandName === 'info') {
        await interaction.reply('開発者<:DEV:1267440009452064829><@1002132268736856136> 開発協力者<:ACCDEV:1267440500625903618> <@1032649313165258772> バージョン<:BE:1267439343882993734><:TA:1267439331069657119>v1', { ephemeral: true });
    } else if (commandName === 'emoji') {
        await interaction.reply('搭載絵文字 <:partner:1267440895037542471> <:ACCDEV:1267440500625903618> <:DEV:1267440009452064829> <:BE:1267439343882993734> <:TA:1267439331069657119> <:B_:1267438603718627378> <:mukou:1267467191142322216>', { ephemeral: true });
    } else if (commandName === 'botid') {
        await interaction.reply('<:mukou:1267467191142322216>このコマンドを実行する権限がありません ', { ephemeral: true });
    } else if (commandName === 'oplist') {
        await interaction.reply('<:mukou:1267467191142322216> NODATA', { ephemeral: true });
    } else if (commandName === 'ticket') {
        const channel = await interaction.guild.channels.cache.get(interaction.channelId);
        const embed = new EmbedBuilder()
            .setColor(0x178CE6)
            .setFooter({ text: 'Made by Spicy │2024/07/30' })
            .addFields({ name: 'チケット', value: 'お問い合わせはこちらからチケットを発行してください！' });
        await channel.send({ embeds: [embed] });

        const row = new ActionRowBuilder()
            .addComponents(
                new ButtonBuilder()
                    .setCustomId('create_ticket')
                    .setLabel('チケットを作成')
                    .setStyle(ButtonStyle.Primary),
            );

        await interaction.reply({ content: '', components: [row] });
    } else if (commandName === 'embed') {
        const embed = new EmbedBuilder()
            .setTitle('リスト')
            .setDescription('<:DEV:1267440009452064829>所有者<@1002132268736856136>　STEAMID:76561199555791158')
            .setColor(0x00ff00)
            .addFields(
                { name: 'EVENTManager', value: '<@1189153844043722753> STEAMID 76561199525003326', inline: false },
                { name: 'EVENTManager', value: 'NotData', inline: true },
                { name: 'moderator', value: 'NotDATA。', inline: false },
                { name: '<:1109542935038545960:1267745305214849035>Trusted', value: '<@1135069041569054750>', inline: true },
                { name: '<:1109542935038545960:1267745305214849035>Trusted', value: '<@1104907457597296762>', inline: false },
                { name: '<:1109542935038545960:1267745305214849035>Trusted', value: '<@716212058445709362>', inline: true },
                { name: '<:1109542935038545960:1267745305214849035>Trusted', value: '<@1234473338706067486>', inline: false },
                { name: '<:1109542935038545960:1267745305214849035>Trusted', value: '<@1085931921202229259>', inline: true },
                { name: '<:partner:1267440895037542471>YT', value: '<@1101479370197061672>。', inline: false },
                { name: '<:partner:1267440895037542471>YT', value: '<@1020505405769666621>。', inline: true },
                { name: '<:partner:1267440895037542471>YT', value: '<@1039717417116512327>。', inline: false },
                { name: '<:partner:1267440895037542471>YT', value: '<@463867826676039690>。', inline: true },
                { name: '<:partner:1267440895037542471>YT', value: '<@1020505405769666621>。', inline: false },
                { name: '<:partner:1267440895037542471>YT', value: '<@297766775653924865>。', inline: true },
                { name: '<:partner:1267440895037542471>YT', value: '<@1002132268736856136>。', inline: false },
                { name: '<:partner2:1267748273502818395>パートナー', value: '<@1101479370197061672>。', inline: true },
                { name: 'DIscordr', value: 'Notdata。', inline: false }
            )
            .setFooter({ text: '2024/7/30' })
            .setThumbnail('https://example.com/thumbnail.jpg')
            .setImage('https://example.com/image.jpg');

        await interaction.reply({ embeds: [embed], ephemeral: true });
    }
});

client.on('interactionCreate', async interaction => {
    if (!interaction.isButton()) return;

    const { customId } = interaction;

    if (customId === 'create_ticket') {
        const server = interaction.guild;

        if (Object.values(ticketOwners).includes(interaction.user.id)) {
            await interaction.reply({ content: '既にチケットが存在します。', ephemeral: true });
            return;
        }

        const overwrites = [
            {
                id: server.roles.everyone.id,
                deny: ['ViewChannel']
            },
            {
                id: client.user.id,
                allow: ['ViewChannel']
            },
            {
                id: interaction.user.id,
                allow: ['ViewChannel']
            }
        ];

        const channelName = `チケット-${interaction.user.username}`;
        const channel = await server.channels.create(channelName, {
            type: 0,
            permissionOverwrites: overwrites
        });

        ticketOwners[channel.id] = interaction.user.id;

        await channel.send(`${interaction.user} チケットが作成されました!`);

        const ticketMessage = `チケットが作成されました！\n${channel}`;

        await interaction.reply({ content: ticketMessage, ephemeral: true });

        const row = new ActionRowBuilder()
            .addComponents(
                new ButtonBuilder()
                    .setCustomId('delete_ticket')
                    .setLabel('チケットを削除')
                    .setStyle(ButtonStyle.Danger),
            );

        await channel.send({ content: '', components: [row] });
    } else if (customId === 'delete_ticket') {
        const channel = interaction.channel;

        await channel.delete();

        if (ticketOwners[channel.id]) {
            delete ticketOwners[channel.id];
        }
    }
});

client.login(token);

const ticketOwners = {};
