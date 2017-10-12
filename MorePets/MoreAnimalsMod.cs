﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entoarox.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using xTile.ObjectModel;
using xTile.Tiles;

namespace Entoarox.MorePetsAndAnimals
{
    public class MoreAnimalsMod : Mod
    {
        private static bool replaceBus = true;
        private bool _TriggerAction = false;
        internal static Random random;
        internal static ModConfig Config;
        internal static IModHelper SHelper;
        internal static Dictionary<string, List<int>> Indexes = new Dictionary<string, List<int>>()
        {
            ["BabyBlue Chicken"] = new List<int>(),
            ["BabyBrown Chicken"] = new List<int>(),
            ["BabyBrown Cow"] = new List<int>(),
            ["BabyCow"] = new List<int>(),
            ["BabyGoat"] = new List<int>(),
            ["BabyPig"] = new List<int>(),
            ["BabyRabbit"] = new List<int>(),
            ["BabySheep"] = new List<int>(),
            ["BabyVoid Chicken"] = new List<int>(),
            ["BabyWhite Chicken"] = new List<int>(),
            ["BabyWhite Cow"] = new List<int>(),
            ["Blue Chicken"] = new List<int>(),
            ["Brown Chicken"] = new List<int>(),
            ["Brown Cow"] = new List<int>(),
            ["cat"] = new List<int>(),
            ["Cow"] = new List<int>(),
            ["Dinosaur"] = new List<int>(),
            ["dog"] = new List<int>(),
            ["Duck"] = new List<int>(),
            ["Goat"] = new List<int>(),
            // There can only be 1 horse in the vanilla game anyhow, and this mod currently does not provide a way to get more due to the functionality behind horses and potential issues with other mods
            //["horse"] = new List<int>(),
            ["Pig"] = new List<int>(),
            ["Rabbit"] = new List<int>(),
            ["ShearedSheep"] = new List<int>(),
            ["Sheep"] = new List<int>(),
            ["Void Chicken"] = new List<int>(),
            ["White Chicken"] = new List<int>(),
            ["White Cow"] = new List<int>(),
            // Special: MorePets separates baby ducks from baby white chickens
            // If a BabyDuck_0 is found, that is used instead of the `BabyWhite Chicken` for "vanilla skin" baby ducks
            ["BabyDuck"] = new List<int>(),
        };
        public override void Entry(IModHelper helper)
        {
            // init
            Config = helper.ReadConfig<ModConfig>();
            SHelper = helper;
            // load textures
            this.LoadPetSkins();
            List<string> partial = new List<string>()
            {
                "Statistics:", Environment.NewLine,
                "  Config:", Environment.NewLine,
                "    AdoptionPrice: "+Config.AdoptionPrice.ToString(), Environment.NewLine,
                "    RepeatedAdoptionPenality: "+Config.RepeatedAdoptionPenality.ToString(), Environment.NewLine,
                "    UseMaxAdoptionLimit: "+Config.UseMaxAdoptionLimit.ToString(), Environment.NewLine,
                "    MaxAdoptionLimit: "+Config.MaxAdoptionLimit.ToString(), Environment.NewLine,
                "    AnimalsOnly: "+Config.AnimalsOnly.ToString(), Environment.NewLine,
                "  Skins:"
            };
            foreach (KeyValuePair<string, List<int>> pair in Indexes)
                if(pair.Value.Count>0)
                    partial.Add($"{Environment.NewLine}    {pair.Key.PadRight(20)}: {pair.Value.Count} skins");
            Monitor.Log(string.Join("", partial), LogLevel.Trace);
            helper.ConsoleCommands.Add("kill_pets", "Kills all the pets you adopted using this mod, you monster", this.CommandFired_KillPets);
            if (Config.AnimalsOnly)
                replaceBus = false;
            if (replaceBus && !Indexes["dog"].Any() && !Indexes["cat"].Any())
            {
                replaceBus = false;
                Monitor.Log("The `AnimalsOnly` config option is set to `false`, yet no dog or cat skins have been found!", LogLevel.Error);
            }
            // hook events
            GameEvents.UpdateTick += GameEvents_UpdateTick;
            if(replaceBus)
            {
                ControlEvents.ControllerButtonPressed += ControlEvents_ControllerButtonPressed;
                ControlEvents.MouseChanged += ControlEvents_MouseChanged;
            }
            // check version
            Helper.RequestUpdateCheck("https://raw.githubusercontent.com/Entoarox/StardewMods/master/MorePets/update.json");
        }

        private void LoadPetSkins()
        {
            List<string> skins = new List<string>();
            foreach (FileInfo file in new DirectoryInfo(Path.Combine(Helper.DirectoryPath, "skins")).EnumerateFiles("*.xnb"))
            {
                if (file.Name.Contains("_"))
                {
                    string name = Path.GetFileNameWithoutExtension(file.Name);
                    string[] split = name.Split('_');
                    if (Indexes.ContainsKey(split[0]))
                    {
                        skins.Add(name);
                        Indexes[split[0]].Add(Convert.ToInt32(split[1]));
                    }
                    else
                        Monitor.Log("Found unexpected file `"+file.Name+"`, if this is meant to be a skin file it has incorrect naming", LogLevel.Warn);
                }
                else if(!file.Name.Equals("BabyDuck.xnb"))
                    Monitor.Log("Found file `" + file.Name + "`, if this is meant to be a skin file it has incorrect naming", LogLevel.Warn);
            }
            Monitor.Log("Skins found: " + string.Join(", ", skins), LogLevel.Trace);
        }
        private List<Pet> GetAllPets()
        {
            return Utility.getAllCharacters().Where(a => a is Pet).Cast<Pet>().ToList();
        }
        internal void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (replaceBus && Game1.getLocationFromName("BusStop") != null)
            {
                Monitor.Log("Patching bus stop...", LogLevel.Trace);
                GameLocation bus = Game1.getLocationFromName("BusStop");
                bus.map.AddTileSheet(new TileSheet("MorePetsTilesheet", bus.map, Helper.Content.GetActualAssetKey("box"), new xTile.Dimensions.Size(2, 2), new xTile.Dimensions.Size(16, 16)));
                bus.SetTile(1, 2, "Front", 0, "MorePetsTilesheet");
                bus.SetTile(2, 2, "Front", 1, "MorePetsTilesheet");
                bus.SetTile(1, 3, "Buildings", 2, "MorePetsTilesheet");
                bus.SetTile(2, 3, "Buildings", 3, "MorePetsTilesheet");
                bus.SetTileProperty(1, 3, "Buildings", "Action", "MorePetsAdoption");
                bus.SetTileProperty(2, 3, "Buildings", "Action", "MorePetsAdoption");
                replaceBus = false;
            }
            foreach (NPC npc in GetAllPets())
                if (npc.manners > 0 && npc.updatedDialogueYet == false)
                {
                    try
                    {
                        var type = npc is Dog ? "dog" : "cat";
                        npc.sprite = new AnimatedSprite(Helper.Content.Load<Texture2D>($"skins/{type}_{npc.manners}"), 0, 32, 32);
                    }
                    catch
                    {
                        Monitor.Log("Pet with unknown skin number found, using default: " + npc.manners.ToString(), LogLevel.Error);
                    }
                    npc.updatedDialogueYet = true;
                }
            foreach(FarmAnimal animal in Game1.getFarm().getAllFarmAnimals())
            {
                if (animal.meatIndex>999)
                {
                    string str = animal.type;
                    if (animal.age < animal.ageWhenMature)
                        str = "Baby" + animal.type;
                    else if (animal.showDifferentTextureWhenReadyForHarvest && animal.currentProduce <= 0)
                        str = "Sheared" + animal.type;
                    if(animal.meatIndex<999)
                        animal.meatIndex = Indexes[str][random.Next(0, Indexes[str].Count)]+999;
                    if (animal.meatIndex > 999)
                        try
                        {
                            animal.sprite = new AnimatedSprite(Helper.Content.Load<Texture2D>("skins/" + str + "_" + (animal.meatIndex-999).ToString()), 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                        }
                        catch
                        {
                            Monitor.Log("Animal with unknown skin number found, using default instead: " + (animal.meatIndex-999).ToString(), LogLevel.Error);
                            if (str=="BabyDuck")
                                str = "BabyWhite Chicken";
                            animal.sprite = new AnimatedSprite(Game1.content.Load<Texture2D>("Animals\\" + str), 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                        }
                }
                else if(animal.type == "Duck" && animal.age < animal.ageWhenMature)
                {
                    try
                    {
                        animal.sprite = new AnimatedSprite(Helper.Content.Load<Texture2D>("skins/BabyDuck"), 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                    }
                    catch
                    {
                        Monitor.Log("Encounted a issue trying to override the default texture for baby ducks with the custom one, using vanilla.", LogLevel.Error);
                        animal.sprite = new AnimatedSprite(Game1.content.Load<Texture2D>("Animals\\BabyWhite Chicken"), 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                    }
                }
            }
        }
        internal void CommandFired_KillPets(string name, string[] args)
        {
            GameLocation farm = Game1.getLocationFromName("Farm");
            GameLocation house = Game1.getLocationFromName("FarmHouse");
            foreach (Pet pet in GetAllPets())
                if (pet.age > 0)
                    if (farm.characters.Contains(pet))
                        farm.characters.Remove(pet);
                    else
                        house.characters.Remove(pet);
            Monitor.Log("You actually killed them.. you FAT monster!", LogLevel.Alert);
        }
        private void ControlEvents_ControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
        {
            if (e.ButtonPressed == Buttons.A)
                CheckForAction();
        }
        private void ControlEvents_ControllerButtonReleased(object sender, EventArgsControllerButtonReleased e)
        {
            if (_TriggerAction && e.ButtonReleased == Buttons.A)
            {
                DoAction();
                _TriggerAction = false;
            }
        }
        private void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (e.NewState.RightButton == ButtonState.Pressed && e.PriorState.RightButton != ButtonState.Pressed)
                CheckForAction();
            if (_TriggerAction && e.NewState.RightButton == ButtonState.Released)
            {
                DoAction();
                _TriggerAction = false;
            }
        }
        internal static List<string> seasons = new List<string>() { "spring", "summer", "fall", "winter" };
        private void CheckForAction()
        {
            if (!Game1.hasLoadedGame)
                return;
            if (!Game1.player.UsingTool && !Game1.pickingTool && !Game1.menuUp && (!Game1.eventUp || Game1.currentLocation.currentEvent.playerControlSequence) && !Game1.nameSelectUp && Game1.numberOfSelectedItems == -1 && !Game1.fadeToBlack)
            {
                Vector2 grabTile = new Vector2((Game1.getOldMouseX() + Game1.viewport.X), (Game1.getOldMouseY() + Game1.viewport.Y)) / Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                    grabTile = Game1.player.GetGrabTile();
                Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
                PropertyValue propertyValue = null;
                if (tile != null)
                    tile.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null && "MorePetsAdoption".Equals(propertyValue))
                {
                    _TriggerAction = true;
                }
            }
        }
        private void DoAction()
        {
            int seed = Game1.year * 1000 + seasons.IndexOf(Game1.currentSeason) * 100 + Game1.dayOfMonth;
            random = new Random(seed);
            List<Pet> list = GetAllPets();
            if ((Config.UseMaxAdoptionLimit && list.Count >= Config.MaxAdoptionLimit) || random.NextDouble() < Math.Max(0.1, Math.Min(0.9, list.Count * Config.RepeatedAdoptionPenality)) || list.FindIndex(a => a.age == seed) != -1)
                Game1.drawObjectDialogue("Just an empty box.");
            else
                AdoptQuestion.Show();
        }
    }
}
