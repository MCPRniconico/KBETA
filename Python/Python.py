import discord
from discord import app_commands

intents = discord.Intents.default()
client = discord.Client(intents=intents)
tree = app_commands.CommandTree(client)


@client.event
async def on_ready():
    print('ログインしました')

    await client.change_presence(status=discord.Status.online, activity=discord.CustomActivity(name='BETA版です'))
    # スラッシュコマンドを同期
    await tree.sync()


@tree.command(name='call', description='scpsl募集します指定のサーバーのみ機能します')
async def test(interaction: discord.Interaction):
    await interaction.response.send_message('<@&1238062718825791590>')

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
    await interaction.response.send_message()




client.run("TOKEN")
