﻿using System;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;

namespace GiveMeMyCursorBack
{
	public class GiveMyMeCursorBackEntry : Mod
	{
		private byte _tickCounter;
		private Config _config;

		public override void Entry(IModHelper helper)
		{
			this._config = this.Helper.ReadConfig<Config>();

			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;

			Monitor.Log("Your cursor is now yours again. Rejoice!", LogLevel.Info);
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			try
			{
				var gmcm = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

				if (gmcm == null)
					return;

				gmcm.Register(
					mod: this.ModManifest,
					reset: () => this._config = new Config(),
					save: () => Helper.WriteConfig(_config)
				);

				gmcm.AddSectionTitle(
					mod: ModManifest,
					text: () => "Get your cursor back!");

				gmcm.AddParagraph(
					mod: ModManifest,
					text: () =>
						"This setting controls how many ticks the game will wait before checking if the game window has focus."
				);

				gmcm.AddNumberOption(
					mod: this.ModManifest,
					name: () => "Tick Threshold",
					tooltip: () => "How many frames the mod should wait before checking if the window is in focus.",
					getValue: () => _config.TickThreshold,
					setValue: value => _config.TickThreshold = value,
					max: 60,
					min: 0
				);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Caught exception trying to set up GMCM: {ex.Message}");
			}
		}

		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			_tickCounter++;

			if (_tickCounter < _config.TickThreshold)
				return;

			_tickCounter = 0;

			// TODO: Make this logic read more nicely.
			// TODO: Also, cache current location on location change to improve performance slightly.
			// If the player *is* in a festival...
			// or an event is active...
			// or is donating items to the museum...
			// or is choosing a building for their animal...
			// or is buying/modifying a building on their farm...
			// we want to simply invert Game1.displayHUD, because the HUD is disabled during festivals.
			if (Game1.isFestival() ||
				Game1.eventUp ||
				Game1.activeClickableMenu is MuseumMenu ||
				(Game1.activeClickableMenu is PurchaseAnimalsMenu || Game1.activeClickableMenu is CarpenterMenu) &&
				Game1.currentLocation is Farm)
			{
				Game1.options.hardwareCursor = (!Game1.game1.IsActive || Game1.displayHUD);
			}
			else
			{
				Game1.options.hardwareCursor = (!Game1.game1.IsActive || !Game1.displayHUD);
			}
		}
	}
}