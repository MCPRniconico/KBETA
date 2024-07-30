from discord import app_commands, Interaction, Embed
import discord
from discord import app_commands

intents = discord.Intents.default()
client = discord.Client(intents=intents)
tree = app_commands.CommandTree(client)


@client.event
async def on_ready():
    print('ログインしました')

    await client.change_presence(status=discord.Status.online, activity=discord.CustomActivity(name='バグの調査中'))
    # スラッシュコマンドを同期
    await tree.sync()


@tree.command(name='call', description='scpsl募集します指定のサーバーのみ機能します')
async def test(interaction: discord.Interaction):
    await interaction.response.send_message('<:__:1267673604020109452>エラー')

@tree.command(name='info', description='BOTの情報を表示します')
async def test_command(interaction: discord.Interaction):
    await interaction.response.send_message('開発者<:DEV:1267440009452064829><@1002132268736856136> 開発協力者<:ACCDEV:1267440500625903618> <@1032649313165258772> バージョン<:BE:1267439343882993734><:TA:1267439331069657119>v1', ephemeral=True)

@tree.command(name='emoji', description='BOTに搭載されている絵文字をすべて表示します')
async def test_command(interaction: discord.Interaction):
    await interaction.response.send_message('搭載絵文字 <:partner:1267440895037542471> <:ACCDEV:1267440500625903618> <:DEV:1267440009452064829> <:BE:1267439343882993734> <:TA:1267439331069657119> <:B_:1267438603718627378> <:mukou:1267467191142322216>', ephemeral=True)

@tree.command(name='botid', description='BOTのIDなどの情報を表示します')
async def test_command(interaction: discord.Interaction):
    await interaction.response.send_message('<:mukou:1267467191142322216>このコマンドを実行する権限がありません ', ephemeral=True)

@tree.command(name='oplist', description='サーバー運営情報を表示します')
async def test_command(interaction: discord.Interaction):
    await interaction.response.send_message('<:mukou:1267467191142322216> NODATA', ephemeral=True)

@tree.command(name="ticket", description="チケットを作成")
@discord.app_commands.default_permissions(
    administrator=True
)
async def create_ticket(interaction: discord.Interaction):
    channel = client.get_channel(interaction.channel_id)
    embed = discord.Embed(
                title="",
                color=0x178CE6,
                description="",                
        )
    embed.set_footer(text="Made by Spicy │2024/07/30")
    embed.add_field(name="チケット",value="お問い合わせはこちらからチケットを発行してください！")
    await channel.send(embed=embed)
    # チケット作成ボタンの表示
    view = discord.ui.View()
    button = discord.ui.Button(style=discord.ButtonStyle.primary, label="チケットを作成", custom_id="create_ticket")
    view.add_item(button)
    await interaction.channel.send("", view=view)

ticket_owners = {}

@client.event
async def on_interaction(inter: discord.Interaction):
    try:
        custom_id = inter.data.get("custom_id")
        if custom_id == "create_ticket":
            # Handle ticket creation
            server = inter.guild

            # Check if the user already has a ticket
            if inter.user.id in ticket_owners.values():
                await inter.response.send_message("既にチケットが存在します。", ephemeral=True)
                return

            # Create a private channel
            overwrites = {
                server.default_role: discord.PermissionOverwrite(read_messages=False),
                server.me: discord.PermissionOverwrite(read_messages=True),
                inter.user: discord.PermissionOverwrite(read_messages=True)
            }
            channel_name = f"チケット-{inter.user.name}"
            channel = await server.create_text_channel(name=channel_name, overwrites=overwrites)

            ticket_owners[channel.id] = inter.user.id

            await channel.send(f"{inter.user.mention} チケットが作成されました!")

            ticket_message = f"チケットが作成されました！\n{channel.mention}"

            await inter.response.send_message(ticket_message, ephemeral=True)

            # Display delete button in the ticket channel
            view = discord.ui.View()
            button = discord.ui.Button(style=discord.ButtonStyle.danger, label="チケットを削除", custom_id="delete_ticket")
            view.add_item(button)
            await channel.send("", view=view)

        elif custom_id == "delete_ticket":
            # Handle ticket deletion
            channel = inter.channel

            # Delete the ticket channel
            await channel.delete()

            # Remove the ticket owner from the dictionary
            if channel.id in ticket_owners:
                del ticket_owners[channel.id]

    except Exception as e:
        print(f"An error occurred: {e}")

@tree.command(name='embed', description='運営パートナーを表示します')
async def embed_command(interaction: Interaction):
    embed = Embed(
        title="リスト",
        description="<:DEV:1267440009452064829>所有者<@1002132268736856136>　STEAMID:76561199555791158",
        color=0x00ff00  # 緑色
    )
    embed.add_field(name="EVENTManager", value="<@1189153844043722753> STEAMID 76561199525003326", inline=False)
    embed.add_field(name="EVENTManager", value="NotData", inline=True)
    embed.add_field(name="moderator", value="NotDATA。", inline=False)
    embed.add_field(name="<:1109542935038545960:1267745305214849035>Trusted", value="<@1135069041569054750> ", inline=True) 
    embed.add_field(name="<:1109542935038545960:1267745305214849035>Trusted", value="<@1104907457597296762> ", inline=False)  
    embed.add_field(name="<:1109542935038545960:1267745305214849035>Trusted", value="<@716212058445709362> ", inline=True)
    embed.add_field(name="<:1109542935038545960:1267745305214849035>Trusted", value="<@1234473338706067486>", inline=False)
    embed.add_field(name="<:1109542935038545960:1267745305214849035>Trusted", value="<@1085931921202229259> ", inline=True)
    embed.add_field(name="<:partner:1267440895037542471>YT", value="<@1101479370197061672>。", inline=False)
    embed.add_field(name="<:partner:1267440895037542471>YT", value="<@1020505405769666621>。", inline=True)
    embed.add_field(name="<:partner:1267440895037542471>YT", value="<@1039717417116512327>。", inline=False)
    embed.add_field(name="<:partner:1267440895037542471>YT", value="<@463867826676039690>。", inline=True)
    embed.add_field(name="<:partner:1267440895037542471>YT", value="<@1020505405769666621>。", inline=False)
    embed.add_field(name="<:partner:1267440895037542471>YT", value="<@297766775653924865>。", inline=True)
    embed.add_field(name="<:partner:1267440895037542471>YT", value="<@1002132268736856136>。", inline=False)
    embed.add_field(name="<:partner2:1267748273502818395>パートナー", value="<@1101479370197061672>。", inline=True)
    embed.add_field(name="DIscordr", value="Notdata。", inline=False)
    embed.set_footer(text="2024/7/30")
    embed.set_thumbnail(url="https://example.com/thumbnail.jpg")
    embed.set_image(url="https://example.com/image.jpg")

    await interaction.response.send_message(embed=embed, ephemeral=True)



client.run("ToKEN")
