using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using ArchiSteamFarm.Json;
using ArchiSteamFarm.Plugins;
using Newtonsoft.Json.Linq;
using SteamKit2;

namespace ArchiSteamFarm.Cobra.RenamePlugin {
	[Export(typeof(IPlugin))]
	internal sealed class RenamePlugin : IBotCommand {
		public string Name => nameof(RenamePlugin);
		public Version Version => typeof(RenamePlugin).Assembly.GetName().Version;
		public async Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args) {
			switch (args[0].ToUpperInvariant()) {
				case "RENAME" when bot.HasPermission(steamID, BotConfig.EPermission.Master):
					int args_length = args.Length;
					string response = "";
					bool rename = false;
					if(args_length < 2) {
						Random rnd_help = new Random();

						response = $"!rename New Bot Name\n\nWorks with spaces!\n\nVariables to use:\n\n%RANDOM1% to $RANDOM4% => generate a random number\n!rename Bot $RANDOM4% => Bot {rnd_help.Next(1000,9999)}\n\n%BOTNAME% => ASF internal bot name\n!rename Bot %BOTNAME% => Bot {bot.BotName}";
					} else {
						for(int i = 1;i < args_length;i++) {
							rename = true;

							if (args[i].Contains("%RANDOM1%")) {
								Random rnd = new Random();
								response = response + args[i].Replace("%RANDOM1%","") + rnd.Next(0,9) + " ";
							} else if (args[i].Contains("%RANDOM2%")) {
								Random rnd = new Random();
								response = response + args[i].Replace("%RANDOM2%", "") + rnd.Next(10, 99) + " ";
							} else if (args[i].Contains("%RANDOM3%")) {
								Random rnd = new Random();
								response = response + args[i].Replace("%RANDOM3%", "") + rnd.Next(100, 999) + " ";
							} else if (args[i].Contains("%RANDOM4%")) {
								Random rnd = new Random();
								response = response + args[i].Replace("%RANDOM4%", "") + rnd.Next(1000, 9999) + " ";
							} else if (args[i].Contains("%BOTNAME%")) {
								Random rnd = new Random();
								response = response + args[i].Replace("%BOTNAME%", "") + bot.BotName + " ";
							} else {
								response = response + args[i] + " ";
							}
						}
					}
					if(rename) {
						response = response.Remove(response.Length - 1);
						await bot.Commands.Response(steamID, $"nickname {bot.BotName} {response}").ConfigureAwait(false);
						return $"Changed my name to: {response}";
					} else {
						return response;
					}
				default:
					return null;
			}
		}
		public void OnLoaded() {
			ASF.ArchiLogger.LogGenericInfo("RenamePlugin by Cobra");
		}
	}
}
