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
		private static readonly Random Random = new Random();
		internal static int RandomNext(int min, int max) {
			lock (Random) {
				return Random.Next(min,max);
			}
		}
		public string Name => nameof(RenamePlugin);
		public Version Version => typeof(RenamePlugin).Assembly.GetName().Version;
		public async Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args) {
			switch (args[0].ToUpperInvariant()) {
				case "RENAME" when bot.HasPermission(steamID, BotConfig.EPermission.Master):
					if (args.Length < 2) {
						return $"!rename New Bot Name\n\nWorks with spaces!\n\nVariables to use:\n\n%RANDOM1% to $RANDOM9% => generate a random number\n!rename Bot $RANDOM4% => Bot 7643\n\n%BOTNAME% => ASF internal bot name\n!rename Bot %BOTNAME% => Bot {bot.BotName}";
					}
					string user_arguments = Utilities.GetArgsAsText(message, 1);
					Regex regex_random = new Regex(@"%RANDOM(\d+)%");
					Match match = regex_random.Match(user_arguments);
					if (match.Success) {
						double maxrange_userinput = double.Parse(match.Groups[1].Value);
						if(maxrange_userinput > 9) {
							return "Sorry but you can't use a random number with more than 9 digits!";
						}
						int randomnumber = RandomNext(0, Convert.ToInt32(Math.Pow(10, maxrange_userinput) - 1));
						user_arguments = Regex.Replace(user_arguments, regex_random.ToString(), randomnumber.ToString($"D{maxrange_userinput}"));
					}
					if(new Regex("%BOTNAME%").Match(user_arguments).Success) { 
						user_arguments = Regex.Replace(user_arguments, @"%BOTNAME%", bot.BotName);
					}
					string response = await bot.Commands.Response(steamID, $"nickname {bot.BotName} {user_arguments}").ConfigureAwait(false);
					return response;
				default:
					return null;
			}
		}
		public void OnLoaded() {
			ASF.ArchiLogger.LogGenericInfo("RenamePlugin by Cobra");
		}
	}
}
