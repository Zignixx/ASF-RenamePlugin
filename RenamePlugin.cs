using System;
using System.Collections.Generic;
using System.Composition;
using System.Text.RegularExpressions;
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
					if (args_length < 2) {
						return $"!rename New Bot Name\n\nWorks with spaces!\n\nVariables to use:\n\n%RANDOM1% to $RANDOM9% => generate a random number\n!rename Bot $RANDOM4% => Bot 7643\n\n%BOTNAME% => ASF internal bot name\n!rename Bot %BOTNAME% => Bot {bot.BotName}";
					}
					string user_arguments = Utilities.GetArgsAsText(args,1," ");
					Regex regex_random = new Regex(@"%RANDOM(\d+)%");
					Match match = regex_random.Match(user_arguments);
					if (match.Success) {
						int maxrange_userinput = int.Parse(match.Groups[1].Value);
						if(maxrange_userinput > 9) {
							return "Sorry but you can't use a random number with more than 9 digits!";
						}
						int maxrange = int.Parse(new string('9', maxrange_userinput));
						Random rnd = new Random();
						int randomnumber = rnd.Next(0, maxrange);
						user_arguments = Regex.Replace(user_arguments, @"%RANDOM(\d+)%", randomnumber.ToString($"D{maxrange_userinput}"));
					}
					if(new Regex("%BOTNAME%").Match(user_arguments).Success) { 
						user_arguments = Regex.Replace(user_arguments, @"%BOTNAME%", bot.BotName);
					}
					await bot.Commands.Response(steamID, $"nickname {bot.BotName} {user_arguments}").ConfigureAwait(false);
					return $"Changed my name to: {user_arguments}";
				default:
					return null;
			}
		}
		public void OnLoaded() {
			ASF.ArchiLogger.LogGenericInfo("RenamePlugin by Cobra");
		}
	}
}
