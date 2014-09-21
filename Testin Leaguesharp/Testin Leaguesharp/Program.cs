using System;
using LeagueSharp;
using SharpDX;
using LeagueSharp.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testin_Leaguesharp
{
    class Program
    {
        static Obj_AI_Base Player = ObjectManager.Player;
        static Menu Halp;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        static void Game_OnGameLoad(EventArgs args)
        {
            Halp = new Menu("halp","halp" , true);
            Game.OnGameUpdate += Game_OnGameUpdate;
            Halp.AddItem(new MenuItem("print", "print info").SetValue(new KeyBind("P".ToCharArray()[0], KeyBindType.Press)));
            Halp.AddToMainMenu();
        }
        static void Game_OnGameUpdate(EventArgs args)
        {
            if (Halp.Item("print").GetValue<KeyBind>().Active)
            {
                Print();
            }
        }
        static void Print()
        {
            Game.PrintChat("Player.FlatCooldownMod");
            Game.PrintChat(Convert.ToString(Player.FlatCooldownMod)));
            Game.PrintChat("Player.Pet.Name");
            Game.PrintChat(Convert.ToString(Player.Pet.Name.ToString()));
            Game.PrintChat("Player.PercentCooldownMod");
            Game.PrintChat(Convert.ToString(Player.PercentCooldownMod.ToString()));
            Game.PrintChat("Player.PercentCCReduction");
            Game.PrintChat(Convert.ToString(Player.PercentCCReduction.ToString()));
            Game.PrintChat("Player.Spellbook.Spells[0].Name");
            Game.PrintChat(Player.Spellbook.Spells[0].Name);
            Game.PrintChat("Player.Spellbook.Spells[1].Name");
            Game.PrintChat(Player.Spellbook.Spells[1].Name);
            Game.PrintChat("Player.Spellbook.Spells[2].Name");
            Game.PrintChat(Player.Spellbook.Spells[2].Name);
            Game.PrintChat("Player.Spellbook.Spells[3].Name");
            Game.PrintChat(Player.Spellbook.Spells[3].Name);
            Game.PrintChat("Player.Spellbook.Spells[4].Name");
            Game.PrintChat(Player.Spellbook.Spells[4].Name);
        }
    }
}
